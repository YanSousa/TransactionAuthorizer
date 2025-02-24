namespace TransactionAuthorizer.Dtos;

public record TransactionResult(string Code, string Message)
{
    public static TransactionResult Success()
    {
        return new TransactionResult("00", "Transaction approved.");
    }

    public static TransactionResult InsufficientFound()
    {
        return new TransactionResult("51", "Insufficient balance.");
    }

    public static TransactionResult Blocked()
    {
        return new TransactionResult("07", "Transaction blocked.");
    }

    public static TransactionResult UserNotFound()
    {
        return new TransactionResult("14", "User not found.");
    }
}
