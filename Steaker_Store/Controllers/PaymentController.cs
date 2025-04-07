using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Steaker_Store.Areas.Admin.Models;
using Steaker_Store.Models;
using Steaker_Store.Models.Vnpay;
using Steaker_Store.Services.Momo;
using Steaker_Store.Services.VnPay;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Steaker_Store.Controllers
{
    [Authorize(Roles = SD.Role_Customer)]
    public class PaymentController : Controller
    {
        private IMomoService _momoService;
        private readonly IVnPayService _vnPayService;
        private readonly ApplicationDbContext _context;
        public PaymentController(IMomoService momoService, IVnPayService vnPayService, ApplicationDbContext context)
        {
            _momoService = momoService;
            _vnPayService = vnPayService;
            _context = context;
        }

        [HttpPost]
        [Route("CreatePaymentUrl")]
        public async Task<IActionResult> CreatePaymentUrl(OrderInfoModel model)
        {
            var response = await _momoService.CreatePaymentAsync(model);
            return Redirect(response.PayUrl);
        }

        [HttpGet]
        public IActionResult PaymentCallBack()
        {
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            return View(response);
        }

        [HttpPost]
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model, string orderCode)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderCode == orderCode);
            if (order == null)
            {
                Console.WriteLine("Không tìm thấy đơn hàng với OrderCode: " + orderCode);
                return NotFound("Đơn hàng không tồn tại.");
            }
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext, order.OrderCode);

            return Redirect(url);
        }
       

    }
}

