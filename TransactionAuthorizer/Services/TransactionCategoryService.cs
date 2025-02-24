using TransactionAuthorizer.Utils;

namespace TransactionAuthorizer.Services
{
    public class TransactionCategoryService : ITransactionCategoryService
    {
        private static readonly Dictionary<string, string> MerchantMCCMap = new(StringComparer.OrdinalIgnoreCase)
        {
            // Transport
            { "UBER TRIP           SAO PAULO BR",      "4121" },
            { "99POP               RIO DE JANEIRO BR", "4121" },
            { "CABIFY              BELO HORIZONTE BR", "4121" },

            // MEAL
            { "UBER EATS           SAO PAULO BR",     "5812" },
            { "IFOOD               RECIFE BR",        "5812" },
            { "RESTAURANTE BOM SABOR BRASILIA BR",    "5811" },
            { "PADARIA DO ZE           SAO PAULO BR", "5811" },
            { "HABIBS              CURITIBA BR",      "5812" },
            { "MC DONALDS          PORTO ALEGRE BR",  "5812" },

            // Food
            { "CARREFOUR           SALVADOR BR",      "5411" },
            { "PÃO DE AÇÚCAR       FORTALEZA BR",     "5411" },
            { "EXTRA               CAMPINAS BR",      "5411" },
            { "ASSAÍ               GOIANIA BR",       "5411" },
            { "ATACADÃO            NATAL BR",         "5411" },
    
            // Financial services
            { "PAG*JoseDaSilva     RIO DE JANEIRO BR", "6012" },
            { "PICPAY             SAO PAULO BR",      "6012" },
            { "MERCADO PAGO       BELO HORIZONTE BR", "6012" },
            { "PAYPAL             CURITIBA BR",       "6012" },

            // Public transport
            { "PICPAY*BILHETEUNICO GOIANIA BR",       "4111" },
            { "RECARGA BILHETE ÚNICO SAO PAULO BR",   "4111" },
            { "SPTRANS            CAMPINAS BR",       "4111" },
            { "METRÔ SP           SAO PAULO BR",      "4111" },
            { "CPTM               SAO PAULO BR",      "4111" },
    
            // General purchases
            { "AMAZON             FORTALEZA BR",      "5999" },
            { "MAGAZINE LUIZA     RIO DE JANEIRO BR", "5732" },
            { "CASAS BAHIA        RECIFE BR",         "5732" },
            { "LOJAS AMERICANAS   PORTO ALEGRE BR",   "5999" },
            { "SUBMARINO          BRASILIA BR",       "5732" }
        };

        public string GetCategory(string mcc) => mcc switch
        {
            TypesOfEstablishments.Food1 or TypesOfEstablishments.Food2 => TransactionCategories.Food,
            TypesOfEstablishments.Meal1 or TypesOfEstablishments.Meal2 => TransactionCategories.Meal,
            _ => TransactionCategories.Cash
        };

        public string GetCorrectedMCC(string merchant, string originalMcc)
        {
            string normalizedMerchant = NormalizeText(merchant);

            foreach (var kvp in MerchantMCCMap)
            {
                string normalizedKey = NormalizeText(kvp.Key);

                if (normalizedMerchant.Contains(normalizedKey))
                    return kvp.Value;
            }

            return originalMcc;
        }

        private string NormalizeText(string text)
        {
            return string.Join(" ", text.ToUpper().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        public List<(string Merchant, string MCC)> GetAllMerchants()
        {
            return MerchantMCCMap.Select(m => (m.Key.Trim(), m.Value)).ToList();
        }
    }
}


