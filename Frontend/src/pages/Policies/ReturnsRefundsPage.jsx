import React from "react";
import PolicyLayout from "./PolicyLayout";

function ReturnsRefundsPage() {
  return (
    <PolicyLayout title="Chính Sách Đổi Trả và Hoàn Tiền">
      <div className="space-y-6 text-gray-700">
        <p className="text-lg">
          Tại 2Sport, chúng tôi hiểu rằng sự hài lòng của khách hàng là ưu tiên hàng đầu. Nếu bạn không hài lòng với sản phẩm hoặc dịch vụ của chúng tôi, bạn có thể yêu cầu đổi trả hoặc hoàn tiền theo các chính sách sau:
        </p>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">Bước 1: Điều kiện đổi trả</h2>
          <p>
            Sản phẩm phải được đổi trả trong vòng 7 ngày kể từ ngày mua hàng. Để đủ điều kiện, sản phẩm phải còn nguyên vẹn, chưa qua sử dụng, và có đầy đủ bao bì cùng hóa đơn mua hàng.
          </p>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">Bước 2: Quá trình đổi trả</h2>
          <p>
            Bạn có thể mang sản phẩm đến cửa hàng hoặc gửi qua dịch vụ chuyển phát. Trong trường hợp bạn gửi sản phẩm qua dịch vụ chuyển phát, vui lòng giữ lại biên lai gửi hàng để theo dõi.
          </p>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">Bước 3: Xem xét và xử lý yêu cầu</h2>
          <p>
            Chúng tôi sẽ kiểm tra tình trạng sản phẩm sau khi nhận được yêu cầu đổi trả. Nếu sản phẩm đủ điều kiện, chúng tôi sẽ tiến hành hoàn tiền hoặc đổi sản phẩm mới.
          </p>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">Bước 4: Hoàn tiền</h2>
          <p>
            Hoàn tiền sẽ được thực hiện qua phương thức thanh toán ban đầu của bạn trong vòng 7 ngày làm việc kể từ khi chúng tôi nhận được sản phẩm. Trong trường hợp không thể hoàn tiền theo phương thức ban đầu, chúng tôi sẽ trao đổi với bạn về các phương án khác.
          </p>
        </section>

        <p className="text-lg font-semibold">
          Lưu ý: Các sản phẩm giảm giá, sản phẩm đặc biệt, hoặc các dịch vụ cho thuê không được áp dụng chính sách đổi trả.
        </p>

        <p className="text-lg">
          Chúng tôi cam kết luôn hỗ trợ khách hàng tốt nhất có thể và tạo ra những trải nghiệm mua sắm thoải mái. Nếu bạn có bất kỳ câu hỏi nào hoặc cần hỗ trợ thêm, đừng ngần ngại liên hệ với chúng tôi qua các kênh dưới đây:
        </p>

        <ul className="list-disc pl-5 space-y-1">
          <li>Email: <a href="mailto:2sportteam@gmail.com" className="text-blue-500 hover:underline">2sportteam@gmail.com</a></li>
          <li>Hotline: <a href="tel:+84338581571" className="text-blue-500 hover:underline">+84 338-581-571</a></li>
        </ul>
      </div>
    </PolicyLayout>
  );
}

export default ReturnsRefundsPage;
