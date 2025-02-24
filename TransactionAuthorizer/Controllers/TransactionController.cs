using Microsoft.AspNetCore.Mvc;
using TransactionAuthorizer.Dtos;
using TransactionAuthorizer.Models;
using TransactionAuthorizer.Services;
using TransactionAuthorizer.Utils;

namespace TransactionAuthorizer.Controllers
{
    [ApiController]
    [Route("transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly IBalanceService _balanceService;
        private readonly ITransactionCategoryService _categoryService;
        private readonly IUserRepositoryService _userRepository;

        public TransactionController(
            IBalanceService balanceService, 
            ITransactionCategoryService categoryService,
            IUserRepositoryService userRepository)
        {
            _balanceService = balanceService;
            _categoryService = categoryService;
            _userRepository = userRepository;
        }

        [HttpPost("process")]
        public IActionResult ProcessTransaction([FromBody] Transaction transaction)
        {
            var user = _userRepository.GetUser(transaction.Account);
            if (user == null)
                return Ok(TransactionResult.UserNotFound());

            if (transaction == null || transaction.Amount <= 0)
                return Ok(TransactionResult.Blocked());

            string correctedMCC = _categoryService.GetCorrectedMCC(transaction.Merchant, transaction.MCC);
            string category = _categoryService.GetCategory(correctedMCC);

            if (_balanceService.DeductBalance(transaction.Account, category, transaction.Amount))
                return Ok(TransactionResult.Success());

            if (category != TransactionCategories.Cash &&
                _balanceService.DeductBalance(transaction.Account, TransactionCategories.Cash, transaction.Amount))
                return Ok(TransactionResult.Success());

            return Ok(TransactionResult.InsufficientFound());
        }
    }
}