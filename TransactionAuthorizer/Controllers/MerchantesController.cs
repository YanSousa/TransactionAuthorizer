using Microsoft.AspNetCore.Mvc;
using TransactionAuthorizer.Services;

namespace TransactionAuthorizer.Controllers
{
    [ApiController]
    [Route("merchantes")]
    public class MerchantesController: ControllerBase
    {
        private readonly ITransactionCategoryService _categoryService;

        public MerchantesController(ITransactionCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("merchants")]
        public IActionResult GetAllMerchants()
        {
            var merchants = _categoryService
                .GetAllMerchants()
                .Select(m => new { Merchant = m.Merchant, MCC = m.MCC });

            return Ok(merchants);
        }
    }
}
