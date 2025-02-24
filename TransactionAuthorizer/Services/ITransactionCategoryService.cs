namespace TransactionAuthorizer.Services
{
    public interface ITransactionCategoryService
    {
        string GetCategory(string mcc);
        string GetCorrectedMCC(string mcc, string merchantName);
        List<(string Merchant, string MCC)> GetAllMerchants();
    }
}
