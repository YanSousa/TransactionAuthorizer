using Microsoft.AspNetCore.Mvc;
using TransactionAuthorizer.Services;

namespace TransactionAuthorizer.Controllers
{
    [ApiController]
    [Route("merchantsManager")]
    public class MerchantsManagerController: ControllerBase
    {
        private readonly ITransactionCategoryService _categoryService;

        public MerchantsManagerController(ITransactionCategoryService categoryService)
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
