import React from "react";
import PolicyLayout from "./PolicyLayout";

function PaymentPolicyPage() {
  return (
    <PolicyLayout title="Chính Sách Thanh Toán">
      <div className="space-y-6 text-gray-700">
        <p className="text-lg">
          Tại 2Sport, chúng tôi cung cấp các phương thức thanh toán linh hoạt và tiện lợi nhất cho khách hàng. 
          Dưới đây là các thông tin chi tiết về chính sách thanh toán của chúng tôi:
        </p>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">1. Các Phương Thức Thanh Toán</h2>
          <p>
            Chúng tôi hỗ trợ nhiều phương thức thanh toán để thuận tiện cho khách hàng, bao gồm:
          </p>
          <ul className="list-disc pl-5 text-lg text-gray-700 mb-4">
            <li>Thanh toán qua thẻ tín dụng / thẻ ghi nợ (Visa, MasterCard, JCB, ...)</li>
            <li>Thanh toán qua chuyển khoản ngân hàng</li>
            <li>Thanh toán khi nhận hàng (COD - Cash on Delivery)</li>
            <li>Thanh toán qua các ví điện tử (Momo, ZaloPay, VNPay, ...)</li>
          </ul>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">2. Chính Sách Thanh Toán</h2>
          <p>
            - Khi thanh toán qua thẻ tín dụng hoặc thẻ ghi nợ, khách hàng sẽ phải cung cấp thông tin thẻ, bao gồm số thẻ, ngày hết hạn, và mã bảo mật. Thông tin thẻ sẽ được bảo mật và không được chia sẻ với bất kỳ bên thứ ba nào.<br />
            - Đối với phương thức thanh toán chuyển khoản ngân hàng, khách hàng vui lòng chuyển khoản vào tài khoản ngân hàng của chúng tôi và gửi thông báo thanh toán qua email hoặc các kênh hỗ trợ.<br />
            - Thanh toán khi nhận hàng (COD) chỉ áp dụng cho một số khu vực nhất định và khách hàng phải thanh toán bằng tiền mặt khi nhận sản phẩm.<br />
            - Các phương thức thanh toán qua ví điện tử sẽ được tích hợp trực tiếp trên nền tảng và khách hàng có thể lựa chọn phương thức thanh toán phù hợp.
          </p>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">3. Xử Lý Đơn Hàng</h2>
          <p>
            Sau khi thanh toán thành công, chúng tôi sẽ xác nhận đơn hàng và bắt đầu xử lý giao hàng. 
            Trong trường hợp có sự cố xảy ra trong quá trình thanh toán, chúng tôi sẽ liên hệ ngay với khách hàng để hỗ trợ giải quyết.
          </p>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">4. Hoàn Tiền</h2>
          <p>
            Trong trường hợp yêu cầu hoàn tiền được chấp nhận, chúng tôi sẽ thực hiện hoàn tiền qua phương thức thanh toán ban đầu của khách hàng trong vòng 7 ngày làm việc. 
            Phương thức hoàn tiền cụ thể sẽ phụ thuộc vào phương thức thanh toán ban đầu.
          </p>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">5. Lưu Ý</h2>
          <p>
            - Để đảm bảo tính bảo mật, chúng tôi khuyến cáo khách hàng không chia sẻ thông tin thẻ hoặc tài khoản thanh toán cho bất kỳ ai.<br />
            - Nếu có bất kỳ sự cố nào trong quá trình thanh toán hoặc yêu cầu hỗ trợ thêm, khách hàng có thể liên hệ với chúng tôi qua các kênh hỗ trợ.
          </p>
        </section>

        <section>
          <p className="text-lg">
            Chúng tôi luôn cam kết bảo vệ quyền lợi của khách hàng và cung cấp một dịch vụ thanh toán an toàn, nhanh chóng và thuận tiện.
          </p>

          <ul className="list-disc pl-5 text-lg text-gray-700 mb-4">
            <li>Email hỗ trợ: <a href="mailto:2sportteam@gmail.com" className="text-blue-500 hover:underline">2sportteam@gmail.com</a></li>
            <li>Hotline: <a href="tel:+84338581571" className="text-blue-500 hover:underline">+84 338-581-571</a></li>
          </ul>
        </section>
      </div>
    </PolicyLayout>
  );
}

export default PaymentPolicyPage;
