import React from "react";
import PolicyLayout from "./PolicyLayout";

function ShippingPolicyPage() {
  return (
    <PolicyLayout title="Chính Sách Vận Chuyển">
      <div className="space-y-6 text-gray-700">
        <p className="text-lg">
          Tại 2Sport, chúng tôi cam kết mang đến dịch vụ vận chuyển nhanh chóng, an toàn và tiện lợi cho khách hàng. Dưới đây là chính sách vận chuyển của chúng tôi.
        </p>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">1. Phạm Vi Vận Chuyển</h2>
          <p>
            Chúng tôi cung cấp dịch vụ vận chuyển toàn quốc. Các đơn hàng sẽ được giao tận nơi tại tất cả các tỉnh thành trong Việt Nam. Tuy nhiên, một số khu vực xa xôi hoặc vùng sâu, vùng xa có thể bị giới hạn về thời gian giao hàng.
          </p>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">2. Thời Gian Vận Chuyển</h2>
          <p>
            Thời gian giao hàng thông thường là từ 2-5 ngày làm việc, tùy vào vị trí của khách hàng. Các đơn hàng được đặt vào cuối tuần hoặc ngày lễ sẽ được xử lý vào ngày làm việc tiếp theo.
          </p>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">3. Phí Vận Chuyển</h2>
          <p>
            Phí vận chuyển sẽ được tính dựa trên trọng lượng, kích thước của đơn hàng và địa điểm giao hàng. Bạn có thể xem phí vận chuyển khi thanh toán trong giỏ hàng.
          </p>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">4. Địa Chỉ Giao Hàng</h2>
          <p>
            Vui lòng cung cấp địa chỉ giao hàng chính xác và đầy đủ khi đặt hàng. Nếu thông tin giao hàng không chính xác hoặc thiếu sót, chúng tôi sẽ không chịu trách nhiệm về việc giao hàng không thành công.
          </p>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">5. Xác Nhận Đơn Hàng</h2>
          <p>
            Sau khi đặt hàng, bạn sẽ nhận được email hoặc tin nhắn xác nhận về đơn hàng của mình. Nếu có bất kỳ sự thay đổi nào về đơn hàng (sản phẩm hết hàng, thay đổi thời gian giao hàng,...), chúng tôi sẽ liên hệ với bạn để thông báo.
          </p>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">6. Kiểm Tra Đơn Hàng Khi Nhận</h2>
          <p>
            Khi nhận hàng, bạn vui lòng kiểm tra kỹ sản phẩm và tình trạng đóng gói. Nếu có bất kỳ vấn đề gì như sản phẩm bị hư hỏng, sai mẫu mã hoặc thiếu sót, vui lòng thông báo ngay cho chúng tôi để được hỗ trợ xử lý.
          </p>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">7. Chính Sách Về Giao Hàng Đảm Bảo</h2>
          <p>
            Chúng tôi cam kết vận chuyển an toàn, đảm bảo sản phẩm đến tay khách hàng trong tình trạng tốt nhất. Nếu có bất kỳ vấn đề nào trong quá trình vận chuyển, chúng tôi sẽ xử lý và đảm bảo quyền lợi cho khách hàng.
          </p>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">8. Liên Hệ</h2>
          <p>
            Nếu bạn có bất kỳ câu hỏi nào về chính sách vận chuyển, vui lòng liên hệ với chúng tôi qua:
          </p>
          <ul className="list-disc pl-5 space-y-1">
            <li>Email hỗ trợ: <a href="mailto:2sportteam@gmail.com" className="text-blue-500 hover:underline">2sportteam@gmail.com</a></li>
            <li>Hotline: <a href="tel:+84338581571" className="text-blue-500 hover:underline">+84 338-581-571</a></li>
          </ul>
        </section>
      </div>
    </PolicyLayout>
  );
}

export default ShippingPolicyPage;
