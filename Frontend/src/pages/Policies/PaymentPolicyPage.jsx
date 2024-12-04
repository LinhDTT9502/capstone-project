import React from "react";
import { Typography } from "@material-tailwind/react";

function PaymentPolicyPage() {
  return (
    <div className="bg-zinc-100 min-h-screen px-10 py-20">
      <div className="max-w-3xl mx-auto">
        <Typography className="text-4xl font-alfa mb-6 text-center">
          Chính Sách Thanh Toán
        </Typography>
        
        <Typography className="text-lg text-gray-700 mb-4">
          Tại 2Sport, chúng tôi cung cấp các phương thức thanh toán linh hoạt và tiện lợi nhất cho khách hàng. Dưới đây là các thông tin chi tiết về chính sách thanh toán của chúng tôi:
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>1. Các phương thức thanh toán</strong><br />
          Chúng tôi hỗ trợ nhiều phương thức thanh toán để thuận tiện cho khách hàng, bao gồm:
        </Typography>
        <ul className="list-disc pl-5 text-lg text-gray-700 mb-4">
          <li>Thanh toán qua thẻ tín dụng / thẻ ghi nợ (Visa, MasterCard, JCB, ...)</li>
          <li>Thanh toán qua chuyển khoản ngân hàng</li>
          <li>Thanh toán khi nhận hàng (COD - Cash on Delivery)</li>
          <li>Thanh toán qua các ví điện tử (Momo, ZaloPay, VNPay, ...)</li>
        </ul>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>2. Chính sách thanh toán</strong><br />
          - Khi thanh toán qua thẻ tín dụng hoặc thẻ ghi nợ, khách hàng sẽ phải cung cấp thông tin thẻ, bao gồm số thẻ, ngày hết hạn, và mã bảo mật. Thông tin thẻ sẽ được bảo mật và không được chia sẻ với bất kỳ bên thứ ba nào.<br />
          - Đối với phương thức thanh toán chuyển khoản ngân hàng, khách hàng vui lòng chuyển khoản vào tài khoản ngân hàng của chúng tôi và gửi thông báo thanh toán qua email hoặc các kênh hỗ trợ.<br />
          - Thanh toán khi nhận hàng (COD) chỉ áp dụng cho một số khu vực nhất định và khách hàng phải thanh toán bằng tiền mặt khi nhận sản phẩm.<br />
          - Các phương thức thanh toán qua ví điện tử sẽ được tích hợp trực tiếp trên nền tảng và khách hàng có thể lựa chọn phương thức thanh toán phù hợp.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>3. Xử lý đơn hàng</strong><br />
          Sau khi thanh toán thành công, chúng tôi sẽ xác nhận đơn hàng và bắt đầu xử lý giao hàng. Trong trường hợp có sự cố xảy ra trong quá trình thanh toán, chúng tôi sẽ liên hệ ngay với khách hàng để hỗ trợ giải quyết.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>4. Hoàn tiền</strong><br />
          Trong trường hợp yêu cầu hoàn tiền được chấp nhận, chúng tôi sẽ thực hiện hoàn tiền qua phương thức thanh toán ban đầu của khách hàng trong vòng 7 ngày làm việc. Phương thức hoàn tiền cụ thể sẽ phụ thuộc vào phương thức thanh toán ban đầu.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>5. Lưu ý</strong><br />
          - Để đảm bảo tính bảo mật, chúng tôi khuyến cáo khách hàng không chia sẻ thông tin thẻ hoặc tài khoản thanh toán cho bất kỳ ai.<br />
          - Nếu có bất kỳ sự cố nào trong quá trình thanh toán hoặc yêu cầu hỗ trợ thêm, khách hàng có thể liên hệ với chúng tôi qua các kênh hỗ trợ.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          Chúng tôi luôn cam kết bảo vệ quyền lợi của khách hàng và cung cấp một dịch vụ thanh toán an toàn, nhanh chóng và thuận tiện.
        </Typography>

        <ul className="list-disc pl-5 text-lg text-gray-700">
          <li>Email hỗ trợ: <a href="mailto:2sportteam@gmail.com" className="text-blue-500">2sportteam@gmail.com</a></li>
          <li>Hotline: <a href="tel:+84338581571" className="text-blue-500">+84 338-581-571</a></li>
        </ul>
      </div>
    </div>
  );
}

export default PaymentPolicyPage;
