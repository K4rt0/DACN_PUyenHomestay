using Backend.Extensions.VnPay;
using Backend.Models.Payment;
using Backend.Services;
using DotEnv.Core;

namespace Backend.Repositories
{
    public class VnPayRepository : IVnPayService
    {
        public string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model)
        {
            var tick = DateTime.Now.Ticks.ToString();

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", EnvReader.Instance["VNP_VERSION"]);
            vnpay.AddRequestData("vnp_Command", EnvReader.Instance["VNP_COMMAND"]);
            vnpay.AddRequestData("vnp_TmnCode", EnvReader.Instance["VNP_TMNCODE"]);
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());

            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", EnvReader.Instance["VNP_CURRCODE"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", EnvReader.Instance["VNP_LOCALE"]);

            vnpay.AddRequestData("vnp_OrderInfo", model.OrderId!);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", EnvReader.Instance["VNP_RETURNURL"]);

            vnpay.AddRequestData("vnp_TxnRef", tick); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            var paymentUrl = vnpay.CreateRequestUrl(EnvReader.Instance["VNP_BASEURL"], EnvReader.Instance["VNP_HASHSECRET"]);

            return paymentUrl;
        }

        public VnPay PaymentExecute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, EnvReader.Instance["VNP_HASHSECRET"]);
            if (!checkSignature)
            {
                return new VnPay
                {
                    Success = false
                };
            }

            return new VnPay
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OrderInfo,
                OrderId = vnp_orderId.ToString(),
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode
            };
        }
    }
}