import React from "react";
import PolicyLayout from "./PolicyLayout";

function ComplaintsHandlingPage() {
  return (
    <PolicyLayout title="Chính Sách Xử Lý Khiếu Nại">
      <div className="space-y-6 text-gray-700">
        <p className="text-lg">
          Tại 2Sport, chúng tôi cam kết mang đến dịch vụ chất lượng và sự hài lòng tối đa cho khách hàng. 
          Tuy nhiên, trong trường hợp có bất kỳ khiếu nại nào, chúng tôi đã xây dựng quy trình xử lý khiếu nại rõ ràng và minh bạch như sau:
        </p>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">Bước 1: Tiếp nhận khiếu nại</h2>
          <p>
            Khách hàng có thể gửi khiếu nại qua email, điện thoại, hoặc trực tiếp tại cửa hàng. Mọi khiếu nại sẽ được ghi nhận và xử lý trong vòng 24 giờ làm việc.
          </p>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">Bước 2: Xem xét khiếu nại</h2>
          <p>
            Chúng tôi sẽ xem xét thông tin và chứng cứ liên quan đến khiếu nại, xác minh tính xác thực và xác định giải pháp phù hợp.
          </p>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">Bước 3: Giải quyết khiếu nại</h2>
          <p>
            Sau khi xác minh, chúng tôi sẽ đưa ra phương án giải quyết, có thể là đổi trả sản phẩm, hoàn tiền, hoặc các giải pháp khác tùy thuộc vào tính chất của khiếu nại.
          </p>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">Bước 4: Thông báo kết quả</h2>
          <p>
            Sau khi giải quyết khiếu nại, chúng tôi sẽ thông báo kết quả cho khách hàng qua các kênh đã liên hệ. Chúng tôi cam kết luôn thực hiện trách nhiệm của mình để bảo vệ quyền lợi của khách hàng.
          </p>
        </section>

        <p className="text-lg">
          Chúng tôi luôn lắng nghe ý kiến của khách hàng và nỗ lực cải thiện chất lượng dịch vụ. Nếu bạn có bất kỳ thắc mắc nào, xin vui lòng liên hệ với chúng tôi qua các kênh sau:
        </p>

        <ul className="list-disc pl-5 space-y-1">
          <li>Email: <a href="mailto:2sportteam@gmail.com" className="text-blue-500 hover:underline">2sportteam@gmail.com</a></li>
          <li>Hotline: <a href="tel:+84338581571" className="text-blue-500 hover:underline">+84 338-581-571</a></li>
        </ul>
      </div>
    </PolicyLayout>
  );
}

export default ComplaintsHandlingPage;
