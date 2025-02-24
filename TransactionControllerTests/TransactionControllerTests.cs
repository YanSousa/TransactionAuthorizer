using Microsoft.AspNetCore.Mvc;
using Moq;
using TransactionAuthorizer.Controllers;
using TransactionAuthorizer.Dtos;
using TransactionAuthorizer.Models;
using TransactionAuthorizer.Services;
using TransactionAuthorizer.Utils;

public class TransactionControllerTests
{
    private readonly Mock<IBalanceService> _mockBalanceService;
    private readonly Mock<ITransactionCategoryService> _mockCategoryService;
    private readonly Mock<IUserRepositoryService> _mockUserRepository;
    private readonly TransactionController _controller;

    public TransactionControllerTests()
    {
        _mockBalanceService = new Mock<IBalanceService>();
        _mockCategoryService = new Mock<ITransactionCategoryService>();
        _mockUserRepository = new Mock<IUserRepositoryService>();

        _controller = new TransactionController(
            _mockBalanceService.Object,
            _mockCategoryService.Object,
            _mockUserRepository.Object
        );
    }

    // Tests whether the GetUser method returns NotFound when the user does not exist
    [Fact]
    public void GetUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        string account = "invalid_user";
        _mockUserRepository.Setup(repo => repo.GetUser(account)).Returns((UserAccount?)null);

        // Act
        var result = _controller.GetUser(account);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(TransactionResult.UserNotFound(), notFoundResult.Value);
    }

    // Tests whether the GetUser method returns user data when it exists
    [Fact]
    public void GetUser_ReturnsUser_WhenUserExists()
    {
        // Arrange
        string account = "user_001";
        var user = new UserAccount { Account = account, FoodBalance = 500, MealBalance = 300, CashBalance = 1000 };
        _mockUserRepository.Setup(repo => repo.GetUser(account)).Returns(user);

        // Act
        var result = _controller.GetUser(account);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<UserAccount>(okResult.Value);
        Assert.Equal(account, returnedUser.Account);
    }

    // Tests whether the GetUsers method returns the list of all registered users
    [Fact]
    public void GetUsers_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<UserAccount>
        {
            new() { Account = "user_001", FoodBalance = 500, MealBalance = 300, CashBalance = 1000 },
            new() { Account = "user_002", FoodBalance = 400, MealBalance = 250, CashBalance = 1200 }
        };

        _mockUserRepository.Setup(repo => repo.GetAllUsers()).Returns(users);

        // Act
        var result = _controller.GetUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUsers = Assert.IsType<List<UserAccount>>(okResult.Value);
        Assert.Equal(2, returnedUsers.Count);
    }

    // Tests whether a transaction is blocked when the transaction amount is zero or negative
    [Fact]
    public void ProcessTransaction_ReturnsBlocked_WhenAmountIsZeroOrNegative()
    {
        // Arrange
        var transaction = new Transaction { Account = "user_001", Amount = 0 };
        var user = new UserAccount { Account = "user_001", FoodBalance = 500, MealBalance = 300, CashBalance = 1000 };

        _mockUserRepository.Setup(repo => repo.GetUser(transaction.Account)).Returns(user);

        // Act
        var result = _controller.ProcessTransaction(transaction);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var transactionResult = Assert.IsType<TransactionResult>(okResult.Value);
        Assert.Equal(TransactionResult.Blocked().Code, transactionResult.Code);
    }

    // Tests whether a transaction is processed successfully when there is sufficient balance
    [Fact]
    public void ProcessTransaction_ReturnsSuccess_WhenBalanceIsDeducted()
    {
        // Arrange
        var transaction = new Transaction { Account = "user_001", Amount = 100, Merchant = "Shop", MCC = "1234" };
        var user = new UserAccount { Account = "user_001", FoodBalance = 500, MealBalance = 300, CashBalance = 1000 };

        _mockUserRepository.Setup(repo => repo.GetUser(transaction.Account)).Returns(user);
        _mockCategoryService.Setup(service => service.GetCorrectedMCC(transaction.Merchant, transaction.MCC)).Returns("1234");
        _mockBalanceService.Setup(service => service.DeductBalance(transaction.Account, It.IsAny<string>(), transaction.Amount)).Returns(true);

        // Act
        var result = _controller.ProcessTransaction(transaction);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var transactionResult = Assert.IsType<TransactionResult>(okResult.Value);
        Assert.Equal(TransactionResult.Success().Code, transactionResult.Code);
    }

    // // This test checks if the transaction returns "Insufficient Funds"
    [Fact]
    public void ProcessTransaction_ReturnsInsufficientFunds_WhenNoBalanceAvailable()
    {
        // Arrange
        var transaction = new Transaction { Account = "user_001", Amount = 100, Merchant = "Shop", MCC = "1234" };
        var user = new UserAccount { Account = "user_001", FoodBalance = 500, MealBalance = 300, CashBalance = 1000 };

        _mockUserRepository.Setup(repo => repo.GetUser(transaction.Account)).Returns(user);
        _mockCategoryService.Setup(service => service.GetCorrectedMCC(transaction.Merchant, transaction.MCC)).Returns("1234");
        _mockBalanceService.Setup(service => service.DeductBalance(transaction.Account, It.IsAny<string>(), transaction.Amount)).Returns(false);

        // Act
        var result = _controller.ProcessTransaction(transaction);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var transactionResult = Assert.IsType<TransactionResult>(okResult.Value);
        Assert.Equal(TransactionResult.InsufficientFound().Code, transactionResult.Code);
    }

    // Test return when user does not exist
    [Fact]
    public void ProcessTransaction_ReturnsUserNotFound_WhenUserDoesNotExist1()
    {
        // Arrange
        var transaction = new Transaction { Account = "12345", Amount = 100, MCC = "1234", Merchant = "Loja X" };
        _mockUserRepository.Setup(repo => repo.GetUser("12345")).Returns((UserAccount)null);

        // Act
        var result = _controller.ProcessTransaction(transaction) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var transactionResult = Assert.IsType<TransactionResult>(result.Value);
        Assert.Equal("14", transactionResult.Code);
    }

    // Tests return when transaction is blocked (amount <= 0)
    [Fact]
    public void ProcessTransaction_ReturnsBlocked_WhenAmountIsInvalid()
    {
        // Arrange
        var transaction = new Transaction { Account = "12345", Amount = 0, MCC = "1234", Merchant = "Loja X" };
        _mockUserRepository.Setup(repo => repo.GetUser("12345")).Returns(new UserAccount());

        // Act
        var result = _controller.ProcessTransaction(transaction) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var transactionResult = Assert.IsType<TransactionResult>(result.Value);
        Assert.Equal("07", transactionResult.Code);
    }

    // Test return when balance is sufficient for the category
    [Fact]
    public void ProcessTransaction_ReturnsSuccess_WhenBalanceIsSufficient()
    {
        // Arrange
        var transaction = new Transaction
        {
            Account = "12345",
            Amount = 50,
            MCC = "5412",
            Merchant = "Loja X"
        };

        var userAccount = new UserAccount
        {
            Account = "12345",
            CashBalance = 1314,
            FoodBalance = 2000,
            MealBalance = 1000
        };

        _mockUserRepository.Setup(repo => repo.GetUser("12345"))
            .Returns(userAccount);

        _mockCategoryService.Setup(cs => cs.GetCorrectedMCC(transaction.Merchant, transaction.MCC))
            .Returns("5412");

        _mockCategoryService.Setup(cs => cs.GetCategory("5412"))
            .Returns(TransactionCategories.Food);

        _mockBalanceService.Setup(bs => bs.DeductBalance("12345", TransactionCategories.Food, 50))
            .Callback<string, string, decimal>((account, category, amount) =>
            {
                Console.WriteLine($"DeductBalance chamado com: Account={account}, Category={category}, Amount={amount}");
            })
            .Returns(true);

        // Act
        var result = _controller.ProcessTransaction(transaction) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var transactionResult = Assert.IsType<TransactionResult>(result.Value);
        Assert.Equal("00", transactionResult.Code); 

        _mockBalanceService.Verify(bs => bs.DeductBalance("12345", TransactionCategories.Food, 50), Times.Once);
    }



    // Test fallback to CASH when category balance is not sufficient
    [Fact]
    public void ProcessTransaction_UsesCashBalance_WhenCategoryBalanceIsInsufficient()
    {
        // Arrange
        var transaction = new Transaction { Account = "12345", Amount = 100, MCC = "1234", Merchant = "Loja X" };
        _mockUserRepository.Setup(repo => repo.GetUser("12345")).Returns(new UserAccount());
        _mockCategoryService.Setup(cs => cs.GetCorrectedMCC("Loja X", "1234")).Returns("1234");
        _mockCategoryService.Setup(cs => cs.GetCategory("1234")).Returns("Transport");
        _mockBalanceService.Setup(bs => bs.DeductBalance("12345", "Transport", 100)).Returns(false);
        _mockBalanceService.Setup(bs => bs.DeductBalance("12345", TransactionCategories.Cash, 100)).Returns(true);

        // Act
        var result = _controller.ProcessTransaction(transaction) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var transactionResult = Assert.IsType<TransactionResult>(result.Value);
        Assert.Equal("00", transactionResult.Code);
    }

    // Tests insufficient balance return when there is no balance in the category or in CASH
    [Fact]
    public void ProcessTransaction_ReturnsInsufficientFunds_WhenNoBalanceAvailableCash()
    {
        // Arrange
        var transaction = new Transaction { Account = "12345", Amount = 200, MCC = "1234", Merchant = "Loja X" };
        _mockUserRepository.Setup(repo => repo.GetUser("12345")).Returns(new UserAccount());
        _mockCategoryService.Setup(cs => cs.GetCorrectedMCC("Loja X", "1234")).Returns("1234");
        _mockCategoryService.Setup(cs => cs.GetCategory("1234")).Returns("Health");
        _mockBalanceService.Setup(bs => bs.DeductBalance("12345", "Health", 200)).Returns(false);
        _mockBalanceService.Setup(bs => bs.DeductBalance("12345", TransactionCategories.Cash, 200)).Returns(false);

        // Act
        var result = _controller.ProcessTransaction(transaction) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var transactionResult = Assert.IsType<TransactionResult>(result.Value);
        Assert.Equal("51", transactionResult.Code);
    }

    // Test if MCC is corrected by merchant name
    [Fact]
    public void ProcessTransaction_CorrectsMCC_BasedOnMerchantName()
    {
        // Arrange
        var transaction = new Transaction
        {
            Account = "12345",
            Amount = 75,
            MCC = "9999",
            Merchant = "UBER EATS           SAO PAULO BR"
        };

        var userAccount = new UserAccount { Account = "12345" };
        _mockUserRepository.Setup(repo => repo.GetUser("12345")).Returns(userAccount);

        _mockCategoryService.Setup(cs => cs.GetCorrectedMCC(transaction.Merchant, transaction.MCC))
            .Returns("5411");

        _mockCategoryService.Setup(cs => cs.GetCategory("5411"))
            .Returns(TransactionCategories.Food);

        _mockBalanceService.Setup(bs => bs.DeductBalance("12345", TransactionCategories.Food, 75))
            .Returns(true);

        // Act
        var result = _controller.ProcessTransaction(transaction) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var transactionResult = Assert.IsType<TransactionResult>(result.Value);
        Assert.Equal("00", transactionResult.Code);
    }
}
