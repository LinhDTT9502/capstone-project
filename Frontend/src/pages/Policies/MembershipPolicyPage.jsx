import React from "react";
import { Typography } from "@material-tailwind/react";

function MembershipPolicyPage() {
  
  return (
    <div className="bg-zinc-100 min-h-screen px-10 py-20">
      <div className="max-w-3xl mx-auto">
        <Typography className="text-4xl font-alfa mb-6 text-center">
          Chính Sách Dành Cho Membership
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          2Sport cung cấp chương trình Membership với nhiều quyền lợi và ưu đãi đặc biệt dành cho các thành viên. Chính sách dưới đây sẽ giúp bạn hiểu rõ hơn về quyền lợi, nghĩa vụ và các điều kiện khi tham gia chương trình Membership của chúng tôi.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>1. Điều Kiện Đăng Ký Membership</strong><br />
          Để trở thành thành viên của chương trình Membership tại 2Sport, bạn cần đáp ứng các điều kiện sau:
        </Typography>
        <ul className="list-disc pl-5 text-lg text-gray-700 mb-4">
          <li>Độ tuổi tối thiểu là 18 tuổi.</li>
          <li>Cung cấp thông tin chính xác và đầy đủ khi đăng ký.</li>
          <li>Đồng ý với các điều khoản và chính sách của 2Sport.</li>
        </ul>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>2. Quyền Lợi Khi Làm Thành Viên</strong><br />
          Khi tham gia chương trình Membership, bạn sẽ nhận được những quyền lợi sau:
        </Typography>
        <ul className="list-disc pl-5 text-lg text-gray-700 mb-4">
          <li>Giảm giá cho các sản phẩm và dịch vụ thuê đồ tại 2Sport.</li>
          <li>Ưu đãi đặc biệt trong các chương trình khuyến mãi và sự kiện.</li>
          <li>Quyền ưu tiên khi đặt đồ thuê hoặc tham gia các sự kiện đặc biệt.</li>
          <li>Các dịch vụ hỗ trợ khách hàng nhanh chóng và hiệu quả hơn.</li>
        </ul>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>3. Nghĩa Vụ Của Thành Viên</strong><br />
          Để duy trì tư cách thành viên và hưởng các quyền lợi, bạn có trách nhiệm:
        </Typography>
        <ul className="list-disc pl-5 text-lg text-gray-700 mb-4">
          <li>Đảm bảo thông tin cá nhân luôn chính xác và cập nhật.</li>
          <li>Tuân thủ các quy định của 2Sport khi sử dụng dịch vụ và sản phẩm.</li>
          <li>Thanh toán đầy đủ các khoản phí theo yêu cầu của chương trình Membership.</li>
        </ul>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>4. Quy Định Về Đăng Ký Và Tham Gia</strong><br />
          Sau khi đăng ký, bạn sẽ nhận được thẻ Membership hoặc mã số thành viên, giúp bạn xác nhận quyền lợi khi sử dụng dịch vụ tại 2Sport. Chúng tôi khuyến khích bạn đăng ký trực tuyến để hưởng các ưu đãi sớm nhất.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>5. Chấm Dứt Tư Cách Thành Viên</strong><br />
          Bạn có thể chấm dứt tư cách thành viên bất cứ lúc nào bằng cách thông báo cho chúng tôi qua email hoặc hotline. Chúng tôi cũng có quyền hủy bỏ tư cách thành viên nếu bạn vi phạm các quy định trong chính sách của chúng tôi.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          <strong>6. Điều Khoản Chấm Dứt Chương Trình Membership</strong><br />
          Chúng tôi có quyền thay đổi hoặc chấm dứt chương trình Membership bất cứ lúc nào mà không cần thông báo trước. Nếu chương trình bị chấm dứt, các quyền lợi của thành viên sẽ bị hủy bỏ và không có bồi thường.
        </Typography>

        <Typography className="text-lg text-gray-700 mb-4">
          Nếu bạn có bất kỳ câu hỏi nào về chính sách Membership, vui lòng liên hệ với chúng tôi qua các kênh hỗ trợ sau:
        </Typography>

        <ul className="list-disc pl-5 text-lg text-gray-700 mb-4">
          <li>Email hỗ trợ: <a href="mailto:2sportteam@gmail.com" className="text-blue-500">2sportteam@gmail.com</a></li>
          <li>Hotline: <a href="tel:+84338581571" className="text-blue-500">+84 338-581-571</a></li>
        </ul>
      </div>
    </div>
  );
}

export default MembershipPolicyPage;
