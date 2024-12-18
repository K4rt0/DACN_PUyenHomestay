using Backend.Models;

namespace Backend.Extensions
{
    public class SendEmail
    {
        public static string? GetVerificationEmailTemplate(string full_name, string verificationLink)
        {
            // Đọc nội dung file HTML
            var current = Directory.GetCurrentDirectory();
            var templatePath = Path.Combine(current, "Templates", "account_active.html");
            var emailBody = File.ReadAllText(templatePath);

            // Chèn link xác thực vào template
            emailBody = emailBody.Replace("{{full_name}}", full_name ?? "bạn");
            emailBody = emailBody.Replace("{{verification_link}}", verificationLink);

            return emailBody;
        }
        public static string? NotifyBookingTemplate(bool success, Reservation reservation)
        {
            var current = Directory.GetCurrentDirectory();
            var templatePath = Path.Combine(current, "Templates", "order_message.html");
            var emailBody = File.ReadAllText(templatePath);
            string header = success ?
                $"Chúng tôi vui mừng thông báo rằng đơn hàng <a href='http://localhost:5173/booking/{reservation.booking_id}' target='_blank'>#{reservation.booking_id}</a> của bạn đã được xác nhận thành công!" :
                $"Rất tiếc, đơn hàng <a href='http://localhost:5173/booking/{reservation.booking_id}' target='_blank'>#{reservation.booking_id}</a> của bạn đã bị hủy.";
            string footer = success ?
                "Cảm ơn bạn đã đặt phòng tại hệ thống của chúng tôi. Chúng tôi hy vọng bạn sẽ có một kỳ nghỉ tuyệt vời!" :
                "Chúng tôi rất tiếc vì sự bất tiện này. Nếu bạn cần hỗ trợ, vui lòng liên hệ với chúng tôi.";

            emailBody = emailBody.Replace("{{Color}}", success ? "header__success" : "header__cancel");
            emailBody = emailBody.Replace("{{Title}}", success ? "Đặt phòng thành công !" : "Đặt phòng thất bại !");
            emailBody = emailBody.Replace("{{CustomerName}}", reservation.user!.full_name);
            emailBody = emailBody.Replace("{{Header}}", header);
            emailBody = emailBody.Replace("{{ImageLink}}", "https://i.imgur.com/" + reservation.room?.room_images?.FirstOrDefault()?.url + reservation.room?.room_images?.FirstOrDefault()?.type ?? "");
            emailBody = emailBody.Replace("{{HotelName}}", reservation.room?.branch?.name);
            emailBody = emailBody.Replace("{{RoomType}}", reservation.room?.room_name);
            emailBody = emailBody.Replace("{{CheckInDate}}", reservation.check_in.ToString());
            emailBody = emailBody.Replace("{{CheckOutDate}}", reservation.check_out.ToString());
            emailBody = emailBody.Replace("{{TotalAmount}}", reservation.total_price.ToString());
            emailBody = emailBody.Replace("{{Footer}}", footer);
            return emailBody;
        }
    }
}