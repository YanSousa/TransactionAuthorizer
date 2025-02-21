namespace TransactionAuthorizer.Services
{
    public interface ITransactionCategoryService
    {
        string GetCategory(string mcc);
        string GetCorrectedMCC(string mcc, string merchantName);
    }
}
