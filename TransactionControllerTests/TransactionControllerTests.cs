using Microsoft.AspNetCore.Mvc;
using Moq;
using TransactionAuthorizer.Controllers;
using TransactionAuthorizer.Models;
using TransactionAuthorizer.Services;

public class TransactionControllerTests
{
    [Fact]
    public void ProcessTransaction_ShouldReturn07_WhenTransactionIsInvalid()
    {
        // Tests if the method returns code "07" when the transaction is null.

        // Arrange
        var mockBalanceService = new Mock<IBalanceService>();
        var mockCategoryService = new Mock<ITransactionCategoryService>();

        var controller = new TransactionController(mockBalanceService.Object, mockCategoryService.Object);

        // Act: Call the ProcessTransaction method with a null transaction.
        var result = controller.ProcessTransaction(null) as OkObjectResult;

        // Assert: Verify that the result is not null, has status 200, and returns code "07".
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal("07", result.Value.GetType().GetProperty("code")?.GetValue(result.Value)?.ToString());
    }

    [Fact]
    public void ProcessTransaction_ShouldReturn00_WhenSufficientBalance()
    {
        // Tests if the method returns code "00" when there is sufficient balance for the transaction.

        // Arrange
        var mockBalanceService = new Mock<IBalanceService>();
        var mockCategoryService = new Mock<ITransactionCategoryService>();

        // Define that MCC "5411" belongs to the "FOOD" category.
        mockCategoryService.Setup(s => s.GetCategory("5411")).Returns("FOOD");

        // Configure the balance service to indicate that there is enough balance and that the deduction is successful.
        mockBalanceService.Setup(s => s.HasSufficientBalance(It.IsAny<string>(), 100)).Returns(true);
        mockBalanceService.Setup(s => s.DeductBalance(It.IsAny<string>(), 100)).Returns(true);

        var controller = new TransactionController(mockBalanceService.Object, mockCategoryService.Object);
        var transaction = new Transaction { Amount = 100, MCC = "5411", Merchant = "SUPERMARKET" };

        // Act: Call the ProcessTransaction method with a valid transaction.
        var result = controller.ProcessTransaction(transaction) as OkObjectResult;

        // Assert: Verify that the result is not null, has status 200, and returns code "00".
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal("00", result.Value.GetType().GetProperty("code")?.GetValue(result.Value)?.ToString());
    }

    [Fact]
    public void ProcessTransaction_ShouldReturn51_WhenInsufficientBalance()
    {
        // Tests if the method returns code "51" when there is insufficient balance.

        // Arrange
        var mockBalanceService = new Mock<IBalanceService>();
        var mockCategoryService = new Mock<ITransactionCategoryService>();

        // Configure the balance service to always return insufficient balance.
        mockBalanceService.Setup(s => s.HasSufficientBalance(It.IsAny<string>(), It.IsAny<decimal>())).Returns(false);

        var controller = new TransactionController(mockBalanceService.Object, mockCategoryService.Object);
        var transaction = new Transaction { Amount = 1000, MCC = "5411", Merchant = "SUPERMARKET" };

        // Act: Call the ProcessTransaction method with a transaction that should fail due to insufficient balance.
        var result = controller.ProcessTransaction(transaction) as OkObjectResult;

        // Assert: Verify that the result is not null, has status 200, and returns code "51".
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal("51", result.Value.GetType().GetProperty("code")?.GetValue(result.Value)?.ToString());
    }

}

public class BalanceServiceTests
{
    [Fact]
    public void HasSufficientBalance_ShouldReturnTrue_WhenBalanceIsEnough()
    {
        // Tests whether the method returns "true" when there is sufficient balance.

        // Arrange
        var balanceService = new BalanceService();

        // Act
        bool result = balanceService.HasSufficientBalance("FOOD", 100);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasSufficientBalance_ShouldReturnFalse_WhenBalanceIsNotEnough()
    {
        // Tests whether the method returns "false" when there is not enough balance.

        // Arrange
        var balanceService = new BalanceService();

        // Act
        bool result = balanceService.HasSufficientBalance("FOOD", 1000);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void DeductBalance_ShouldReturnTrue_WhenBalanceIsEnough()
    {
        // Tests whether the method returns "true" when the balance deduction is successful
        // and verifies that the balance was updated correctly.

        // Arrange
        var balanceService = new BalanceService();

        // Act
        bool result = balanceService.DeductBalance("FOOD", 100);

        // Assert
        Assert.True(result);
        Assert.Equal(400, balanceService.GetBalance("FOOD")); // 500 - 100
    }

    [Fact]
    public void DeductBalance_ShouldReturnFalse_WhenBalanceIsNotEnough()
    {
        // Tests whether the method returns "false" when there is not enough balance to deduct
        // and ensures that the balance remains unchanged.

        // Arrange
        var balanceService = new BalanceService();

        // Act
        bool result = balanceService.DeductBalance("FOOD", 1000);

        // Assert
        Assert.False(result);
        Assert.Equal(500, balanceService.GetBalance("FOOD")); // Balance should not change
    }
}
