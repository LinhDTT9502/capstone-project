import React, { useState } from "react";
import { Button } from "@material-tailwind/react";
import { toast, ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { sendFeedback } from "../services/feedbackService";

const ContactUs = () => {
  const [fullName, setFullName] = useState("");
  const [email, setEmail] = useState("");
  const [content, setContent] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const validateEmail = (email) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  };

  const handleSubmit = async () => {
    // Validate inputs
    if (!fullName.trim()) {
      toast.error("Vui lòng nhập họ và tên.");
      return;
    }
    if (!email.trim() || !validateEmail(email)) {
      toast.error("Vui lòng nhập email hợp lệ.");
      return;
    }
    if (!content.trim()) {
      toast.error("Vui lòng nhập nội dung.");
      return;
    }

    setIsSubmitting(true);

    const feedbackData = {
      fullName,
      email,
      content,
    };

    try {
      const response = await sendFeedback(feedbackData);
      if (response.status === 200) {
        toast.success("Thông tin của bạn đã được gửi thành công!");
        setFullName("");
        setEmail("");
        setContent("");
      } else {
        toast.error("Đã xảy ra lỗi khi gửi thông tin. Vui lòng thử lại.");
      }
    } catch (error) {
      toast.error("Đã xảy ra lỗi. Vui lòng thử lại sau.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <>
      <ToastContainer />
      <div className="relative mb-16">
        <div className="relative h-[400px] overflow-hidden">
          <img
            src="/assets/images/Contact_Header.jpg"
            alt="contactUs"
            className="w-full h-full object-cover"
          />
          <div className="absolute inset-0 bg-black bg-opacity-50 flex items-center justify-center">
          <h1 className="text-[40px] py-10 text-white font-alfa absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 text-wrap text-center transition duration-300 ease-in-out hover:scale-110">
              Liên hệ
            </h1>
          </div>
        </div>

        <div className="container mx-auto px-[15%] py-16">
          <div className="flex flex-col lg:flex-row gap-12">
            <div className="lg:w-1/3 space-y-8">
              <div>
                <h2 className="text-xl font-semibold text-blue-600 mb-2">
                  Chào mừng bạn
                </h2>
                <h1 className="text-3xl font-bold text-gray-800 mb-4">
                  Hãy kết nối với chúng tôi
                </h1>
                <p className="text-gray-600">
                  Gửi phản hồi hoặc thắc mắc của bạn, chúng tôi sẽ phản hồi sớm nhất
                  có thể.
                </p>
              </div>
              <div>
                <h2 className="text-2xl font-bold text-gray-800 mb-4">
                  Giờ làm việc
                </h2>
                <ul className="space-y-2 text-gray-600">
                  <li>Thứ 2 - Thứ 6: 9:00 - 18:00</li>
                  <li>Thứ 7: 10:00 - 15:00</li>
                  <li>Chủ nhật: Nghỉ</li>
                </ul>
              </div>
            </div>

            <div className="lg:w-2/3 bg-gray-100 rounded-lg shadow-lg p-8">
              <h1 className="text-3xl font-bold text-gray-800 mb-6">
                Gửi tin nhắn
              </h1>
              <div className="space-y-4">
                <input
                  type="text"
                  placeholder="Họ và tên"
                  value={fullName}
                  onChange={(e) => setFullName(e.target.value)}
                  className="w-full px-4 py-2 rounded-lg border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-500 transition-all duration-300"
                />
                <input
                  type="email"
                  placeholder="Email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  className="w-full px-4 py-2 rounded-lg border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-500 transition-all duration-300"
                />
                <textarea
                  placeholder="Nội dung"
                  value={content}
                  onChange={(e) => setContent(e.target.value)}
                  className="w-full px-4 py-2 rounded-lg border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-500 transition-all duration-300 resize-none h-32"
                />
              </div>

              <Button
                className="mt-6 bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-6 rounded-lg transition-all duration-300"
                onClick={handleSubmit}
                disabled={isSubmitting}
              >
                {isSubmitting ? "Đang gửi..." : "Gửi ngay"}
              </Button>
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

export default ContactUs;

