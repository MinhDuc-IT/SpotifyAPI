using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Collections.Specialized;
using SpotifyAPI.DTOs;
using SpotifyAPI.Helpers;
using SpotifyAPI.Data;
using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Models;

//namespace SpotifyAPI.Services
//{
//    public class VnPayService
//    {
//        private readonly SpotifyDbContext _context;
//        private readonly IConfiguration _configuration;

//        public VnPayService(IConfiguration configuration, SpotifyDbContext context)
//        {
//            _configuration = configuration;
//            _context = context;
//        }

//        public string CreatePaymentUrl(HttpContext context, VnPaymentRequestDto model)
//        {
//            var tick = DateTime.Now.Ticks.ToString();

//            var vnpay = new VnPayLibrary();
//            vnpay.AddRequestData("vnp_Version", _configuration["VnPay:Version"]);
//            vnpay.AddRequestData("vnp_Command", _configuration["VnPay:Command"]);
//            vnpay.AddRequestData("vnp_TmnCode", _configuration["VnPay:TmnCode"]);
//            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());
//            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
//            vnpay.AddRequestData("vnp_CurrCode", _configuration["VnPay:CurrCode"]);
//            vnpay.AddRequestData("vnp_IpAddr", SpotifyAPI.Helpers.Utils.GetIpAddress(context));
//            vnpay.AddRequestData("vnp_Locale", _configuration["VnPay:Locale"]);
//            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toán gói premium cho User {model.UserId}");
//            vnpay.AddRequestData("vnp_OrderType", "other");
//            vnpay.AddRequestData("vnp_ReturnUrl", _configuration["VnPay:PaymentBackReturnUrl"]);
//            vnpay.AddRequestData("vnp_TxnRef", tick);
//            vnpay.AddRequestData("vnp_BankCode", "NCB");

//            var paymentUrl = vnpay.CreateRequestUrl(
//                _configuration["VnPay:BaseUrl"],
//                _configuration["VnPay:HashSecret"]);

//            return paymentUrl;
//        }

//        public VnPaymentResponseDto PaymentExecute(NameValueCollection collections)
//        {
//            var vnpay = new VnPayLibrary();
//            foreach (string key in collections.AllKeys)
//            {
//                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
//                {
//                    vnpay.AddResponseData(key, collections[key]);
//                }
//            }

//            var orderCode = vnpay.GetResponseData("vnp_TxnRef");
//            var amount = double.Parse(vnpay.GetResponseData("vnp_Amount")) / 100;
//            var transactionId = vnpay.GetResponseData("vnp_TransactionNo");
//            var secureHash = collections["vnp_SecureHash"];
//            var responseCode = vnpay.GetResponseData("vnp_ResponseCode");
//            var orderInfo = vnpay.GetResponseData("vnp_OrderInfo");
//            var payDate = vnpay.GetResponseData("vnp_PayDate");
//            var bankCode = vnpay.GetResponseData("vnp_BankCode");

//            bool isValidSignature = vnpay.ValidateSignature(secureHash, _configuration["VnPay:HashSecret"]);
//            if (!isValidSignature)
//            {
//                return new VnPaymentResponseDto { Success = false };
//            }

//            return new VnPaymentResponseDto
//            {
//                Success = true,
//                PaymentMethod = bankCode,
//                OrderDescription = orderInfo,
//                OrderCode = orderCode,
//                Amount = amount,
//                TransactionId = transactionId,
//                Token = secureHash,
//                VnPayResponseCode = responseCode,
//                TransactionDate = payDate
//            };
//        }
//    }
//}

public class VnPayService
{
    private readonly SpotifyDbContext _context;
    private readonly IConfiguration _configuration;

    public VnPayService(SpotifyDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<string> CreatePaymentUrl(HttpContext context, int userId, int planId, string clientIp)
    {
        var plan = await _context.Plans.FirstOrDefaultAsync(p => p.PlanId == planId);

        var tick = DateTime.UtcNow.Ticks.ToString();

        var vnpay = new VnPayLibrary();
        vnpay.AddRequestData("vnp_Version", _configuration["VnPay:Version"]);
        vnpay.AddRequestData("vnp_Command", _configuration["VnPay:Command"]);
        vnpay.AddRequestData("vnp_TmnCode", _configuration["VnPay:TmnCode"]);
        vnpay.AddRequestData("vnp_Amount", ((int)(plan.Price * 100)).ToString());
        vnpay.AddRequestData("vnp_CreateDate", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
        vnpay.AddRequestData("vnp_CurrCode", _configuration["VnPay:CurrCode"]);
        vnpay.AddRequestData("vnp_IpAddr", clientIp ?? "127.0.0.1");
        vnpay.AddRequestData("vnp_Locale", _configuration["VnPay:Locale"]);
        vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toán gói premium cho User {userId}");
        vnpay.AddRequestData("vnp_OrderType", "other");
        vnpay.AddRequestData("vnp_ReturnUrl", _configuration["VnPay:PaymentBackReturnUrl"]);
        vnpay.AddRequestData("vnp_TxnRef", tick);
        vnpay.AddRequestData("vnp_BankCode", "NCB");

        return vnpay.CreateRequestUrl(_configuration["VnPay:BaseUrl"], _configuration["VnPay:HashSecret"]);
    }

    public async Task<VnPaymentResponseDto> PaymentExecuteAsync(NameValueCollection collections)
    {
        var vnpay = new VnPayLibrary();
        foreach (string key in collections.AllKeys.Where(k => !string.IsNullOrEmpty(k) && k.StartsWith("vnp_")))
        {
            vnpay.AddResponseData(key, collections[key]);
        }

        var vnp_SecureHash = collections["vnp_SecureHash"];
        var validSignature = vnpay.ValidateSignature(vnp_SecureHash, _configuration["VnPay:HashSecret"]);
        if (!validSignature)
            return new VnPaymentResponseDto { Success = false };

        var orderCode = vnpay.GetResponseData("vnp_TxnRef");
        var userId = ExtractUserIdFromOrderInfo(vnpay.GetResponseData("vnp_OrderInfo")); // Tùy theo cách bạn encode UserId
        var plan = await _context.Plans.FirstOrDefaultAsync(p => p.Price * 100 == Convert.ToDecimal(vnpay.GetResponseData("vnp_Amount")));
        if (plan == null)
            return new VnPaymentResponseDto { Success = false };

        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserID == userId);
        if (user == null)
        {
            return new VnPaymentResponseDto { Success = false };
        }

        user.SubscriptionType = "Premium";

        var subscription = new UserSubscription
        {
            UserID = userId,
            PlanId = plan.PlanId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            PaymentStatus = "Success"
        };

        _context.UserSubscriptions.Add(subscription);
        await _context.SaveChangesAsync();

        return new VnPaymentResponseDto
        {
            Success = true,
            PaymentMethod = vnpay.GetResponseData("vnp_BankCode"),
            OrderDescription = vnpay.GetResponseData("vnp_OrderInfo"),
            OrderCode = orderCode,
            Amount = Convert.ToDouble(vnpay.GetResponseData("vnp_Amount")),
            TransactionId = vnpay.GetResponseData("vnp_TransactionNo"),
            Token = vnp_SecureHash,
            VnPayResponseCode = vnpay.GetResponseData("vnp_ResponseCode"),
            TransactionDate = vnpay.GetResponseData("vnp_PayDate")
        };
    }

    private int ExtractUserIdFromOrderInfo(string orderInfo)
    {
        var parts = orderInfo.Split(' ');
        if (parts.Length > 0 && int.TryParse(parts.Last(), out int userId))
            return userId;
        return 0;
    }
}
