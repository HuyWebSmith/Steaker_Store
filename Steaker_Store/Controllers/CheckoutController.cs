using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Steaker_Store.Areas.Admin.Models;
using Steaker_Store.Models;
using Steaker_Store.Models.Momo;
using Steaker_Store.Services.Momo;
using Steaker_Store.Services.VnPay;
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

        return Json(response);
    }
}
