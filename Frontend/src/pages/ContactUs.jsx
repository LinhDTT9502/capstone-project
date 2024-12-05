import React, { useState } from "react";
import { Button } from "@material-tailwind/react";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css"; // Import toastify styles
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
        toast.success("Thông tin của bạn của bạn đã được gửi thành công!");
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
    <div className="relative mb-[4%]">
      <div className="flex justify-center relative">
        <img
          src="/assets/images/ContactUs.jpg"
          alt="contactUs"
          className="mx-auto h-[350px] w-[90%]"
        />
        <h1 className="text-[60px] py-20 text-white font-alfa absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2">
          Liên hệ
        </h1>
      </div>

      <div className="mt-10 mx-[10%] flex">
        <div className="w-1/3 justify-center flex flex-col ml-20 mr-20">
          <h2 className="text-[20px] font-bold text-[#524FF5]">
            Chào mừng bạn
          </h2>
          <h1 className="text-[35px] font-bold text-black">
            Hãy kết nối với chúng tôi
          </h1>
          <p className="text-[15px] text-[#6A6A6A] mt-3 text-wrap">
            Gửi phản hồi hoặc thắc mắc của bạn, chúng tôi sẽ phản hồi sớm nhất
            có thể.
          </p>
          <h2 className="text-[20px] font-bold text-black mt-3">
            Giờ làm việc
          </h2>
          <div className="mt-3">
            <p className="text-[15px] text-[#6A6A6A] mt-2">
              Thứ 2 - Thứ 6: 9:00 - 18:00
            </p>
            <p className="text-[15px] text-[#6A6A6A] mt-2">
              Thứ 7: 10:00 - 15:00
            </p>
            <p className="text-[15px] text-[#6A6A6A] mt-2">Chủ nhật: Nghỉ</p>
          </div>
        </div>

        <div className="w-2/3 bg-[#EEEEEE] mx-16">
          <h1 className="text-[35px] font-bold text-black mt-10 ml-[14%]">
            Gửi tin nhắn
          </h1>
          <div className="flex flex-col mt-7 items-center border-black">
            <input
              type="text"
              placeholder="Họ và tên"
              value={fullName}
              onChange={(e) => setFullName(e.target.value)}
              className="bg-white h-12 w-[70%] text-black pl-5 rounded-lg"
            />
            <input
              type="email"
              placeholder="Email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="bg-white mt-5 h-12 w-[70%] text-black pl-5 rounded-lg"
            />
            <textarea
              placeholder="Nội dung"
              value={content}
              onChange={(e) => setContent(e.target.value)}
              className="bg-white mt-5 h-24 w-[70%] text-black pl-5 rounded-lg resize-none"
            />
          </div>

          <Button
            className="mt-10 ml-[15%] mb-10"
            onClick={handleSubmit}
            disabled={isSubmitting}
          >
            {isSubmitting ? "Đang gửi..." : "Gửi ngay"}
          </Button>
        </div>
      </div>
    </div>
  );
};

export default ContactUs;
