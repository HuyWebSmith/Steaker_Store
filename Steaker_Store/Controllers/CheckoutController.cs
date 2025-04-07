using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Steaker_Store.Areas.Admin.Models;
using Steaker_Store.Models;
using Steaker_Store.Models.Momo;
using Steaker_Store.Services.Momo;
using Steaker_Store.Services.VnPay;
using System;
using System.Threading.Tasks;

[Authorize(Roles = SD.Role_Customer)]
public class CheckoutController : Controller
{
    private readonly IMomoService _momoService;
    private readonly IVnPayService _vnPayService;

    public CheckoutController(IMomoService momoService, IVnPayService vnPayService)
    {
        _momoService = momoService;
        _vnPayService = vnPayService;
    }


    [HttpGet]
    public IActionResult PaymentCallBack()
    {
        var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
        return View(response);
    }

    [HttpPost]
    public IActionResult MomoNotify([FromBody] MomoExecuteResponseModel model)
    {
        if (model == null || string.IsNullOrEmpty(model.OrderId))
        {
            return BadRequest("Dữ liệu phản hồi không hợp lệ");
        }

        if (model.Amount != null)
        {
            Console.WriteLine($"Đơn hàng {model.OrderId} đã được thanh toán thành công với số tiền {model.Amount}.");
        }
        else
        {
            Console.WriteLine($"Thanh toán đơn hàng {model.OrderId} thất bại.");
        }

        return Ok(new { message = "Received" });
    }

    [HttpGet]
    public IActionResult PaymentCallbackVnpay()
    {
        var response = _vnPayService.PaymentExecute(Request.Query);
        // Giả sử bạn lưu OrderId vào trong model khi thanh toán
        if (response != null && !string.IsNullOrEmpty(response.OrderId))
        {

            // Tìm đơn hàng trong DB và cập nhật trạng thái
            using (var scope = HttpContext.RequestServices.CreateScope())
            {
                var orderId = response.OrderId;

                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var order = db.Orders.FirstOrDefault(o => o.OrderCode == orderId);



                if (order != null)
                {
                    order.Status = response.Success
                        ? Steaker_Store.Enums.PaymentStatusEnum.DaThanhToan
                        : Steaker_Store.Enums.PaymentStatusEnum.ThanhToanThatBai;

                    db.SaveChanges();
                }
            }
        }
        return View("PaymentResult", response);
    }
}
