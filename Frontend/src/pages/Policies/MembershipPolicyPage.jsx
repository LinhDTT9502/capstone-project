import React from "react";
import PolicyLayout from "./PolicyLayout";

function MembershipPolicyPage() {
  return (
    <PolicyLayout title="Chính Sách Dành Cho Membership">
      <div className="space-y-6 text-gray-700">
        <p className="text-lg">
          2Sport cung cấp chương trình Membership với nhiều quyền lợi và ưu đãi đặc biệt dành cho các thành viên. 
          Chính sách dưới đây sẽ giúp bạn hiểu rõ hơn về quyền lợi, nghĩa vụ và các điều kiện khi tham gia chương trình Membership của chúng tôi.
        </p>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">1. Điều Kiện Đăng Ký Membership</h2>
          <p>
            Để trở thành thành viên của chương trình Membership tại 2Sport, bạn cần đáp ứng các điều kiện sau:
          </p>
          <ul className="list-disc pl-5 text-lg text-gray-700 mb-4">
            <li>Độ tuổi tối thiểu là 18 tuổi.</li>
            <li>Cung cấp thông tin chính xác và đầy đủ khi đăng ký.</li>
            <li>Đồng ý với các điều khoản và chính sách của 2Sport.</li>
          </ul>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">2. Quyền Lợi Khi Làm Thành Viên</h2>
          <p>
            Khi tham gia chương trình Membership, bạn sẽ nhận được những quyền lợi sau:
          </p>
          <ul className="list-disc pl-5 text-lg text-gray-700 mb-4">
            <li>Giảm giá cho các sản phẩm và dịch vụ thuê đồ tại 2Sport.</li>
            <li>Ưu đãi đặc biệt trong các chương trình khuyến mãi và sự kiện.</li>
            <li>Quyền ưu tiên khi đặt đồ thuê hoặc tham gia các sự kiện đặc biệt.</li>
            <li>Các dịch vụ hỗ trợ khách hàng nhanh chóng và hiệu quả hơn.</li>
          </ul>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">3. Nghĩa Vụ Của Thành Viên</h2>
          <p>
            Để duy trì tư cách thành viên và hưởng các quyền lợi, bạn có trách nhiệm:
          </p>
          <ul className="list-disc pl-5 text-lg text-gray-700 mb-4">
            <li>Đảm bảo thông tin cá nhân luôn chính xác và cập nhật.</li>
            <li>Tuân thủ các quy định của 2Sport khi sử dụng dịch vụ và sản phẩm.</li>
            <li>Thanh toán đầy đủ các khoản phí theo yêu cầu của chương trình Membership.</li>
          </ul>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">4. Quy Định Về Đăng Ký Và Tham Gia</h2>
          <p>
            Sau khi đăng ký, bạn sẽ nhận được thẻ Membership hoặc mã số thành viên, giúp bạn xác nhận quyền lợi khi sử dụng dịch vụ tại 2Sport. 
            Chúng tôi khuyến khích bạn đăng ký trực tuyến để hưởng các ưu đãi sớm nhất.
          </p>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">5. Chấm Dứt Tư Cách Thành Viên</h2>
          <p>
            Bạn có thể chấm dứt tư cách thành viên bất cứ lúc nào bằng cách thông báo cho chúng tôi qua email hoặc hotline. 
            Chúng tôi cũng có quyền hủy bỏ tư cách thành viên nếu bạn vi phạm các quy định trong chính sách của chúng tôi.
          </p>
        </section>

        <section>
          <h2 className="text-xl font-semibold mb-2 text-gray-800">6. Điều Khoản Chấm Dứt Chương Trình Membership</h2>
          <p>
            Chúng tôi có quyền thay đổi hoặc chấm dứt chương trình Membership bất cứ lúc nào mà không cần thông báo trước. 
            Nếu chương trình bị chấm dứt, các quyền lợi của thành viên sẽ bị hủy bỏ và không có bồi thường.
          </p>
        </section>

        <section>
          <p className="text-lg">
            Nếu bạn có bất kỳ câu hỏi nào về chính sách Membership, vui lòng liên hệ với chúng tôi qua các kênh hỗ trợ sau:
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

export default MembershipPolicyPage;
