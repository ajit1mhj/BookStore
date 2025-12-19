using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Authorize]
    public class UserOrderController : Controller
    {
        private readonly IUserOrderRepository _userOrderRepo;

        public UserOrderController(IUserOrderRepository userOrderRepo) 
        {
            _userOrderRepo = userOrderRepo;
        }
        public async Task<IActionResult> UserOrder()
        {
            var orders = await _userOrderRepo.UserOrders();
            return View(orders);
        }
    }
}
