import React from "react";
import { Typography } from "@material-tailwind/react";

function ReturnsRefundsPage() {
  return (
    <div className="bg-zinc-100 min-h-screen px-10 py-20">
      <div className="max-w-3xl mx-auto">
        <Typography className="text-4xl font-alfa mb-6 text-center">
          Chính Sách Đổi Trả và Hoàn Tiền
        </Typography>
        
        <Typography className="text-lg text-gray-700 mb-4">
          Tại 2Sport, chúng tôi hiểu rằng sự hài lòng của khách hàng là ưu tiên hàng đầu. Nếu bạn không hài lòng với sản phẩm hoặc dịch vụ của chúng tôi, bạn có thể yêu cầu đổi trả hoặc hoàn tiền theo các chính sách sau:
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>Bước 1: Điều kiện đổi trả</strong><br />
          Sản phẩm phải được đổi trả trong vòng 7 ngày kể từ ngày mua hàng. Để đủ điều kiện, sản phẩm phải còn nguyên vẹn, chưa qua sử dụng, và có đầy đủ bao bì cùng hóa đơn mua hàng.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>Bước 2: Quá trình đổi trả</strong><br />
          Bạn có thể mang sản phẩm đến cửa hàng hoặc gửi qua dịch vụ chuyển phát. Trong trường hợp bạn gửi sản phẩm qua dịch vụ chuyển phát, vui lòng giữ lại biên lai gửi hàng để theo dõi.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>Bước 3: Xem xét và xử lý yêu cầu</strong><br />
          Chúng tôi sẽ kiểm tra tình trạng sản phẩm sau khi nhận được yêu cầu đổi trả. Nếu sản phẩm đủ điều kiện, chúng tôi sẽ tiến hành hoàn tiền hoặc đổi sản phẩm mới.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>Bước 4: Hoàn tiền</strong><br />
          Hoàn tiền sẽ được thực hiện qua phương thức thanh toán ban đầu của bạn trong vòng 7 ngày làm việc kể từ khi chúng tôi nhận được sản phẩm. Trong trường hợp không thể hoàn tiền theo phương thức ban đầu, chúng tôi sẽ trao đổi với bạn về các phương án khác.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>Lưu ý:</strong> Các sản phẩm giảm giá, sản phẩm đặc biệt, hoặc các dịch vụ cho thuê không được áp dụng chính sách đổi trả.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          Chúng tôi cam kết luôn hỗ trợ khách hàng tốt nhất có thể và tạo ra những trải nghiệm mua sắm thoải mái. Nếu bạn có bất kỳ câu hỏi nào hoặc cần hỗ trợ thêm, đừng ngần ngại liên hệ với chúng tôi qua các kênh dưới đây:
        </Typography>

        <ul className="list-disc pl-5 text-lg text-gray-700">
          <li>Email: <a href="mailto:2sportteam@gmail.com" className="text-blue-500">2sportteam@gmail.com</a></li>
          <li>Hotline: <a href="tel:+84338581571" className="text-blue-500">+84 338-581-571</a></li>
        </ul>
      </div>
    </div>
  );
}

export default ReturnsRefundsPage;
