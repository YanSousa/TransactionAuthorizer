using System;
using System.Collections.Generic;

namespace TransactionAuthorizer.Services
{
    public class TransactionCategoryService : ITransactionCategoryService
    {
        private static readonly Dictionary<string, string> MerchantMCCMap = new()
        {
            { "UBER TRIP", "4121" },
            { "UBER EATS", "5812" },
            { "PAG*JoseDaSilva", "6012" },
            { "PICPAY*BILHETEUNICO", "4111" }
        };

        public string GetCategory(string mcc) => mcc switch
        {
            "5411" or "5412" => "FOOD",
            "5811" or "5812" => "MEAL",
            _ => "CASH"
        };
        public string GetCorrectedMCC(string mcc, string merchantName)
        {
            foreach (var entry in MerchantMCCMap)
            {
                if (merchantName.StartsWith(entry.Key, StringComparison.OrdinalIgnoreCase))
                {
                    return entry.Value;
                }
            }
            return mcc;
        }
    }
}
