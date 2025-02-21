using Microsoft.AspNetCore.Mvc;
using TransactionAuthorizer.Models;
using TransactionAuthorizer.Services;

namespace TransactionAuthorizer.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly IBalanceService _balanceService;
        private readonly ITransactionCategoryService _categoryService;

        public TransactionController(IBalanceService balanceService, ITransactionCategoryService categoryService)
        {
            _balanceService = balanceService;
            _categoryService = categoryService;
        }

        [HttpPost]
        public IActionResult ProcessTransaction([FromBody] Transaction transaction)
        {
            if (transaction == null || transaction.Amount <= 0)
                return Ok(new { code = "07" });

            string correctedMCC = _categoryService.GetCorrectedMCC(transaction.MCC, transaction.Merchant);
            string category = _categoryService.GetCategory(correctedMCC);

            if (TryProcessTransaction(category, transaction.Amount) || TryProcessTransaction("CASH", transaction.Amount))
            {
                return Ok(new { code = "00" }); 
            }

            return Ok(new { code = "51" }); 
        }

        private bool TryProcessTransaction(string category, decimal amount)
        {
            if (_balanceService.HasSufficientBalance(category, amount))
            {
                _balanceService.DeductBalance(category, amount);
                return true;
            }
            return false;
        }
    }
}
