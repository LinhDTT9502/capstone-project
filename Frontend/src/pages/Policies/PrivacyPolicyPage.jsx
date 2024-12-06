import React from "react";
import { Typography } from "@material-tailwind/react";

function PrivacyPolicyPage() {
  return (
    <div className="bg-zinc-100 min-h-screen px-10 py-20">
      <div className="max-w-3xl mx-auto">
        <Typography className="text-4xl font-alfa mb-6 text-center">
          Chính Sách Bảo Mật Thông Tin
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          Chúng tôi cam kết bảo vệ quyền riêng tư của khách hàng và bảo mật thông tin cá nhân mà khách hàng cung cấp trong quá trình sử dụng dịch vụ của 2Sport. Chính sách bảo mật này giải thích về cách chúng tôi thu thập, sử dụng và bảo vệ thông tin của khách hàng.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>1. Thông Tin Chúng Tôi Thu Thập</strong><br />
          Khi khách hàng truy cập trang web hoặc sử dụng dịch vụ của 2Sport, chúng tôi có thể thu thập các thông tin sau:
        </Typography>
        <ul className="list-disc pl-5 text-lg text-gray-700 mb-4">
          <li>Thông tin cá nhân (tên, địa chỉ email, số điện thoại, địa chỉ giao hàng, v.v.)</li>
          <li>Thông tin thanh toán (chi tiết thẻ tín dụng, thông tin giao dịch)</li>
          <li>Thông tin sử dụng dịch vụ (lịch sử mua hàng, các sản phẩm đã xem, thông tin đăng nhập)</li>
          <li>Thông tin từ các cuộc khảo sát hoặc phản hồi từ khách hàng</li>
        </ul>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>2. Cách Thức Sử Dụng Thông Tin</strong><br />
          Chúng tôi sử dụng thông tin thu thập được để:
        </Typography>
        <ul className="list-disc pl-5 text-lg text-gray-700 mb-4">
          <li>Cung cấp và xử lý đơn hàng của khách hàng</li>
          <li>Cải thiện dịch vụ và trải nghiệm người dùng trên trang web của chúng tôi</li>
          <li>Gửi các thông báo liên quan đến đơn hàng hoặc dịch vụ</li>
          <li>Gửi email marketing về các chương trình khuyến mãi, sản phẩm mới hoặc thông tin hữu ích (nếu khách hàng đã đồng ý nhận)</li>
        </ul>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>3. Bảo Mật Thông Tin</strong><br />
          Chúng tôi áp dụng các biện pháp bảo mật tiên tiến để bảo vệ thông tin cá nhân của khách hàng khỏi việc truy cập, thay đổi, tiết lộ hoặc xóa trái phép. Các biện pháp bảo mật này bao gồm:
        </Typography>
        <ul className="list-disc pl-5 text-lg text-gray-700 mb-4">
          <li>Sử dụng mã hóa SSL (Secure Socket Layer) để bảo vệ các giao dịch thanh toán</li>
          <li>Hạn chế quyền truy cập thông tin cá nhân đối với những người không có thẩm quyền</li>
          <li>Sử dụng hệ thống giám sát và các biện pháp bảo mật khác để ngăn ngừa việc truy cập trái phép vào dữ liệu</li>
        </ul>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>4. Chia Sẻ Thông Tin</strong><br />
          Chúng tôi không chia sẻ thông tin cá nhân của khách hàng với bất kỳ bên thứ ba nào ngoài các trường hợp sau:
        </Typography>
        <ul className="list-disc pl-5 text-lg text-gray-700 mb-4">
          <li>Chia sẻ với các đối tác dịch vụ đáng tin cậy để thực hiện các chức năng hỗ trợ, chẳng hạn như thanh toán, giao hàng, hoặc phân tích dữ liệu. Những đối tác này chỉ được phép sử dụng thông tin của khách hàng cho mục đích thực hiện dịch vụ mà họ cung cấp.</li>
          <li>Khi yêu cầu của pháp luật hoặc yêu cầu của cơ quan nhà nước có thẩm quyền.</li>
        </ul>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>5. Quyền Của Khách Hàng</strong><br />
          Khách hàng có quyền yêu cầu chúng tôi:
        </Typography>
        <ul className="list-disc pl-5 text-lg text-gray-700 mb-4">
          <li>Truy cập vào thông tin cá nhân mà chúng tôi đã thu thập về bạn</li>
          <li>Cập nhật, chỉnh sửa hoặc yêu cầu xóa thông tin cá nhân của bạn</li>
          <li>Chấm dứt việc sử dụng thông tin cá nhân của bạn cho các mục đích marketing (nếu có)</li>
        </ul>

        <Typography className="text-lg text-gray-700 mb-4">
          Để thực hiện các quyền này, khách hàng có thể liên hệ với chúng tôi qua các kênh hỗ trợ.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>6. Sự Thay Đổi Chính Sách Bảo Mật</strong><br />
          Chúng tôi có thể cập nhật chính sách bảo mật này theo thời gian. Mọi thay đổi sẽ được thông báo trên trang web và có hiệu lực ngay khi được đăng tải. Khách hàng nên kiểm tra định kỳ để nắm được thông tin mới nhất.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          Nếu bạn có bất kỳ câu hỏi nào về chính sách bảo mật, vui lòng liên hệ với chúng tôi:
        </Typography>

        <ul className="list-disc pl-5 text-lg text-gray-700 mb-4">
          <li>Email hỗ trợ: <a href="mailto:2sportteam@gmail.com" className="text-blue-500">2sportteam@gmail.com</a></li>
          <li>Hotline: <a href="tel:+84338581571" className="text-blue-500">+84 338-581-571</a></li>
        </ul>
      </div>
    </div>
  );
}

export default PrivacyPolicyPage;
