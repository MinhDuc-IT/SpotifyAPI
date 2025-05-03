using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Data;
using SpotifyAPI.DTOs;
using SpotifyAPI.Helpers;
using SpotifyAPI.Services;
using System.Collections.Specialized;
using System.Security.Claims;

namespace SpotifyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly VnPayService _vnPayService;
        private readonly IUserService _userService;
        private readonly SpotifyDbContext _context;

        public PaymentController(VnPayService vnPayService, IUserService userService, SpotifyDbContext context)
        {
            _vnPayService = vnPayService;
            _userService = userService;
            _context = context;
        }

        /// <summary>
        /// Gửi yêu cầu tạo link thanh toán
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePaymentUrl([FromBody] VnPaymentRequestDto dto)
        {
            var firebaseId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userService.GetUserByFirebaseUidAsync(firebaseId);
            if (user == null)
                return Unauthorized("Người dùng không hợp lệ.");

            var plan = await _context.Plans.FindAsync(dto.PlanId);
            if (plan == null)
                return BadRequest("Gói dịch vụ không tồn tại.");
            var url = await _vnPayService.CreatePaymentUrl(HttpContext, user.UserID, dto.PlanId, dto.ClientIp);
            return Ok(new { paymentUrl = url });
        }

        /// <summary>
        /// VNPay callback trả về kết quả thanh toán
        /// </summary>
        //[HttpGet("callback")]
        //public async Task<IActionResult> Callback()
        //{
        //    var collection = Request.Query;
        //    NameValueCollection queryCollection = new NameValueCollection();
        //    foreach (var key in collection.Keys)
        //    {
        //        queryCollection.Add(key.ToString(), collection[key.ToString()]);
        //    }

        //    var response = await _vnPayService.PaymentExecuteAsync(queryCollection);

        //    //if (!response.Success)
        //    //    return BadRequest("Xác minh chữ ký thất bại hoặc dữ liệu không hợp lệ.");

        //    //// Tùy frontend dùng webview hay deeplink có thể trả view hoặc JSON
        //    //return Ok(response);

        //    var deepLinkUrl = response.Success
        //    ? "spotifyminiapp://payment-success"
        //    : "spotifyminiapp://payment-failure";

        //    return Redirect(deepLinkUrl);
        //}

        [HttpGet("callback")]
        public async Task<IActionResult> Callback()
        {
            var collection = Request.Query;
            NameValueCollection queryCollection = new NameValueCollection();

            foreach (var key in collection.Keys)
            {
                queryCollection.Add(key.ToString(), collection[key.ToString()]);
            }

            var response = await _vnPayService.PaymentExecuteAsync(queryCollection);

            // Lấy message từ mã lỗi
            var message = VNPayError.GetMessage(response.VnPayResponseCode);

            var queryParams = new Dictionary<string, string>
            {
                { "transactionId", response.TransactionId },
                { "amount", response.Amount.ToString() },
                { "orderCode", response.OrderCode },
                { "paymentMethod", response.PaymentMethod },
                { "orderDescription", response.OrderDescription },
                { "transactionDate", response.TransactionDate },
                { "message", message }
            };

            var queryString = string.Join("&", queryParams
                .Where(kvp => !string.IsNullOrEmpty(kvp.Value))
                .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

            var deepLinkUrl = response.Success
                ? $"spotifyminiapp://payment-success?{queryString}"
                : $"spotifyminiapp://payment-failure?{queryString}";

            return Redirect(deepLinkUrl);
        }

        [HttpGet("start")]
        public IActionResult StartPayment()
        {
            return RedirectToAction("GetPaymentResult");
        }

        [HttpGet("result")]
        public IActionResult GetPaymentResult()
        {
            var rnd = new Random().Next(0, 2); // 0 hoặc 1
            if (rnd == 1) return Redirect("spotifyminiapp://payment-success");
            return Redirect("spotifyminiapp://payment-failure");
        }
    }

}
