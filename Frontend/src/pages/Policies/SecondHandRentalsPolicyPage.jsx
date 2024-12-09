import React from "react";
import PolicyLayout from "./PolicyLayout";

function SecondHandRentalsPolicyPage() {
  return (
    <PolicyLayout title="Chính Sách Khi Thuê Đồ 2hand Tại 2Sport">
      <div className="space-y-6 text-gray-700">
        <p className="text-lg">
          Tại 2Sport, chúng tôi cung cấp dịch vụ cho thuê đồ 2hand với các sản phẩm chất lượng và giá cả phải chăng. Chính sách dưới đây sẽ giúp bạn hiểu rõ hơn về quyền lợi, nghĩa vụ và các điều kiện khi thuê đồ 2hand từ chúng tôi.
        </p>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">1. Điều Kiện Thuê Đồ 2hand</h2>
          <ul className="list-disc pl-5 space-y-2 text-lg">
            <li>Độ tuổi tối thiểu là 18 tuổi.</li>
            <li>Cung cấp thông tin chính xác và đầy đủ khi thuê đồ.</li>
            <li>Đồng ý với các điều khoản và chính sách của 2Sport khi thuê đồ 2hand.</li>
          </ul>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">2. Quyền Lợi Khi Thuê Đồ 2hand</h2>
          <ul className="list-disc pl-5 space-y-2 text-lg">
            <li>Giá thuê hợp lý và tiết kiệm hơn so với việc mua đồ mới.</li>
            <li>Được phép kiểm tra và lựa chọn sản phẩm trước khi thuê.</li>
            <li>Các đồ thuê đều được kiểm tra kỹ lưỡng và vệ sinh sạch sẽ trước khi giao cho khách hàng.</li>
          </ul>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">3. Quy Trình Thuê Đồ 2hand</h2>
          <ol className="list-decimal pl-5 space-y-2 text-lg">
            <li>Chọn sản phẩm bạn muốn thuê từ danh sách đồ 2hand.</li>
            <li>Điền thông tin cá nhân và thông tin thuê đồ vào mẫu đăng ký.</li>
            <li>Thanh toán phí thuê theo thời gian thuê và giá trị sản phẩm.</li>
            <li>Nhận đồ thuê và kiểm tra tình trạng sản phẩm trước khi nhận.</li>
          </ol>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">4. Chính Sách Đổi Trả và Hoàn Tiền</h2>
          <ul className="list-disc pl-5 space-y-2 text-lg">
            <li>Sản phẩm có thể đổi trong vòng 3 ngày kể từ ngày nhận đồ, nếu sản phẩm bị lỗi hoặc không đúng với yêu cầu.</li>
            <li>Không áp dụng hoàn tiền cho các sản phẩm đã qua sử dụng hoặc bị hư hỏng bởi người thuê.</li>
            <li>Quá trình hoàn trả sản phẩm phải đảm bảo sản phẩm trong tình trạng nguyên vẹn, không bị hư hỏng hoặc thiếu các phụ kiện đi kèm.</li>
          </ul>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">5. Nghĩa Vụ Của Khách Hàng Khi Thuê Đồ 2hand</h2>
          <ul className="list-disc pl-5 space-y-2 text-lg">
            <li>Chăm sóc sản phẩm trong suốt thời gian thuê và không làm hỏng hoặc mất mát đồ thuê.</li>
            <li>Trả lại sản phẩm đúng hạn theo thỏa thuận trong hợp đồng thuê.</li>
            <li>Thông báo ngay lập tức cho chúng tôi nếu sản phẩm gặp sự cố trong quá trình sử dụng.</li>
          </ul>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">6. Chính Sách Về Phí Phạt</h2>
          <ul className="list-disc pl-5 space-y-2 text-lg">
            <li>Phí phạt trễ hạn: 10% của giá trị thuê mỗi ngày trễ.</li>
            <li>Phí phạt hư hỏng sản phẩm: Phụ thuộc vào mức độ hư hỏng, có thể lên đến 100% giá trị sản phẩm.</li>
          </ul>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">7. Liên Hệ</h2>
          <p>
            Nếu bạn có bất kỳ câu hỏi nào về chính sách thuê đồ 2hand hoặc cần hỗ trợ, vui lòng liên hệ với chúng tôi qua các kênh sau:
          </p>
          <ul className="list-disc pl-5 space-y-1 text-lg">
            <li>Email hỗ trợ: <a href="mailto:2sportteam@gmail.com" className="text-blue-500 hover:underline">2sportteam@gmail.com</a></li>
            <li>Hotline: <a href="tel:+84338581571" className="text-blue-500 hover:underline">+84 338-581-571</a></li>
          </ul>
        </section>
      </div>
    </PolicyLayout>
  );
}

export default SecondHandRentalsPolicyPage;
