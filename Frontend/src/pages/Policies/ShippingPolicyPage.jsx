import React from "react";
import { Typography } from "@material-tailwind/react";

function ShippingPolicyPage() {
  return (
    <div className="bg-zinc-100 min-h-screen px-10 py-20">
      <div className="max-w-3xl mx-auto">
        <Typography className="text-4xl font-alfa mb-6 text-center">
          Chính Sách Vận Chuyển
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          Tại 2Sport, chúng tôi cam kết mang đến dịch vụ vận chuyển nhanh chóng, an toàn và tiện lợi cho khách hàng. Dưới đây là chính sách vận chuyển của chúng tôi.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>1. Phạm Vi Vận Chuyển</strong><br />
          Chúng tôi cung cấp dịch vụ vận chuyển toàn quốc. Các đơn hàng sẽ được giao tận nơi tại tất cả các tỉnh thành trong Việt Nam. Tuy nhiên, một số khu vực xa xôi hoặc vùng sâu, vùng xa có thể bị giới hạn về thời gian giao hàng.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>2. Thời Gian Vận Chuyển</strong><br />
          Thời gian giao hàng thông thường là từ 2-5 ngày làm việc, tùy vào vị trí của khách hàng. Các đơn hàng được đặt vào cuối tuần hoặc ngày lễ sẽ được xử lý vào ngày làm việc tiếp theo.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>3. Phí Vận Chuyển</strong><br />
          Phí vận chuyển sẽ được tính dựa trên trọng lượng, kích thước của đơn hàng và địa điểm giao hàng. Bạn có thể xem phí vận chuyển khi thanh toán trong giỏ hàng.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>4. Địa Chỉ Giao Hàng</strong><br />
          Vui lòng cung cấp địa chỉ giao hàng chính xác và đầy đủ khi đặt hàng. Nếu thông tin giao hàng không chính xác hoặc thiếu sót, chúng tôi sẽ không chịu trách nhiệm về việc giao hàng không thành công.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>5. Xác Nhận Đơn Hàng</strong><br />
          Sau khi đặt hàng, bạn sẽ nhận được email hoặc tin nhắn xác nhận về đơn hàng của mình. Nếu có bất kỳ sự thay đổi nào về đơn hàng (sản phẩm hết hàng, thay đổi thời gian giao hàng,...) chúng tôi sẽ liên hệ với bạn để thông báo.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>6. Kiểm Tra Đơn Hàng Khi Nhận</strong><br />
          Khi nhận hàng, bạn vui lòng kiểm tra kỹ sản phẩm và tình trạng đóng gói. Nếu có bất kỳ vấn đề gì như sản phẩm bị hư hỏng, sai mẫu mã hoặc thiếu sót, vui lòng thông báo ngay cho chúng tôi để được hỗ trợ xử lý.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>7. Chính Sách Về Giao Hàng Đảm Bảo</strong><br />
          Chúng tôi cam kết vận chuyển an toàn, đảm bảo sản phẩm đến tay khách hàng trong tình trạng tốt nhất. Nếu có bất kỳ vấn đề nào trong quá trình vận chuyển, chúng tôi sẽ xử lý và đảm bảo quyền lợi cho khách hàng.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>8. Liên Hệ</strong><br />
          Nếu bạn có bất kỳ câu hỏi nào về chính sách vận chuyển, vui lòng liên hệ với chúng tôi qua:
        </Typography>

        <ul className="list-disc pl-5 text-lg text-gray-700 mb-4">
          <li>Email hỗ trợ: <a href="mailto:2sportteam@gmail.com" className="text-blue-500">2sportteam@gmail.com</a></li>
          <li>Hotline: <a href="tel:+84338581571" className="text-blue-500">+84 338-581-571</a></li>
        </ul>
      </div>
    </div>
  );
}

export default ShippingPolicyPage;
