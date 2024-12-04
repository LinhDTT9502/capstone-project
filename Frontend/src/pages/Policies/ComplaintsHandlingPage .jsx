import React from "react";
import { Typography } from "@material-tailwind/react";

function ComplaintsHandlingPage() {
  return (
    <div className="bg-zinc-100 min-h-screen px-10 py-20">
      <div className="max-w-3xl mx-auto">
        <Typography className="text-4xl font-alfa mb-6 text-center">
          Chính Sách Xử Lý Khiếu Nại
        </Typography>
        
        <Typography className="text-lg text-gray-700 mb-4">
          Tại 2Sport, chúng tôi cam kết mang đến dịch vụ chất lượng và sự hài lòng tối đa cho khách hàng. 
          Tuy nhiên, trong trường hợp có bất kỳ khiếu nại nào, chúng tôi đã xây dựng quy trình xử lý khiếu nại rõ ràng và minh bạch như sau:
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>Bước 1: Tiếp nhận khiếu nại</strong><br />
          Khách hàng có thể gửi khiếu nại qua email, điện thoại, hoặc trực tiếp tại cửa hàng. Mọi khiếu nại sẽ được ghi nhận và xử lý trong vòng 24 giờ làm việc.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>Bước 2: Xem xét khiếu nại</strong><br />
          Chúng tôi sẽ xem xét thông tin và chứng cứ liên quan đến khiếu nại, xác minh tính xác thực và xác định giải pháp phù hợp.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>Bước 3: Giải quyết khiếu nại</strong><br />
          Sau khi xác minh, chúng tôi sẽ đưa ra phương án giải quyết, có thể là đổi trả sản phẩm, hoàn tiền, hoặc các giải pháp khác tùy thuộc vào tính chất của khiếu nại.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>Bước 4: Thông báo kết quả</strong><br />
          Sau khi giải quyết khiếu nại, chúng tôi sẽ thông báo kết quả cho khách hàng qua các kênh đã liên hệ. Chúng tôi cam kết luôn thực hiện trách nhiệm của mình để bảo vệ quyền lợi của khách hàng.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          Chúng tôi luôn lắng nghe ý kiến của khách hàng và nỗ lực cải thiện chất lượng dịch vụ. Nếu bạn có bất kỳ thắc mắc nào, xin vui lòng liên hệ với chúng tôi qua các kênh sau:
        </Typography>

        <ul className="list-disc pl-5 text-lg text-gray-700">
          <li>Email: <a href="mailto:2sportteam@gmail.com" className="text-blue-500">2sportteam@gmail.com</a></li>
          <li>Hotline: <a href="tel:+84338581571" className="text-blue-500">+84 338-581-571</a></li>
        </ul>
      </div>
    </div>
  );
}

export default ComplaintsHandlingPage;
