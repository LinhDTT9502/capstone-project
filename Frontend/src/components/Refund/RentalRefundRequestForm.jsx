import { useState } from "react";
import { Button, Input, Textarea, Checkbox } from "@material-tailwind/react";
import axios from "axios";
import { toast } from "react-toastify";

const RentalRefundRequestForm = ({ orderDetail }) => {
  const [isModalOpen, setIsModalOpen] = useState(false); 
  const [orderCode, setOrderCode] = useState("");
  const [reason, setReason] = useState("");
  const [notes, setNotes] = useState("");
  const [isAgreementAccepted, setIsAgreementAccepted] = useState(false);
  const [selected, setSelected] = useState(null);
  const [isSubmitting, setIsSubmitting] = useState(false); // Trạng thái gửi yêu cầu
  const [isOpen, setIsOpen] = useState(false);
  const [expandedItem, setExpandedItem] = useState(null);
  const openModal = () => setIsModalOpen(true);

  const closeModal = () => setIsModalOpen(false);

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!isAgreementAccepted) {
      toast.warning("Bạn phải đồng ý với chính sách hoàn tiền của hệ thống.");
      return;
    }

    
    const payload = {
      orderCode: orderDetail.rentalOrderCode,
      orderType: 2,
      reason,
      notes,
      isAgreementAccepted,
    };

    setIsSubmitting(true);
    try {
      const response = await axios.post(
        "https://twosport-api-offcial-685025377967.asia-southeast1.run.app//api/RefundRequest/create",
        payload,
        {
          headers: {
            accept: "*/*",
            "Content-Type": "application/json",
          },
        }
      );
      if (response.data.isSuccess) {
        toast.success("Yêu cầu hoàn tiền đã được gửi thành công!");
        setOrderCode("");
        setReason("");
        setNotes("");
        setIsAgreementAccepted(false);
        setIsModalOpen(false);
      } else {
        toast.warning(response.data.message);
      }
    } catch (error) {
      toast.error(
        error.response?.data?.message ??
          "Đã xảy ra lỗi. Vui lòng kiểm tra lại thông tin đơn hàng."
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleSelect = (item) => {
    setSelected(item);
    setReason(item.name); // Gán lý do vào state reason
    setIsOpen(false); // Đóng dropdown sau khi chọn lý do
  };

  const reasons = [
    { name: "Đơn đã hủy, cần hoàn tiền tiền cọc" },
    { name: "Khác" },
  ];

  return (
    <div>
      {/* Step 2: Nút mở pop-up */}
      <Button onClick={openModal} color="orange" className="w-full">
        Yêu cầu hoàn tiền
      </Button>

      {/* Step 3: Modal (Pop-up) */}
      {isModalOpen && (
        <div className="fixed inset-0 flex justify-center items-center bg-gray-800 bg-opacity-50 z-50">
          <div className="bg-white p-8 rounded-md shadow-lg w-full max-w-lg">
            <h2 className="text-2xl font-semibold text-gray-800 text-center mb-4">
              Biểu mẫu yêu cầu hoàn tiền
            </h2>
            <p className="text-sm text-gray-600 mb-6">
                 Vui lòng cung cấp thông tin đầy đủ và lý do chi tiết.
            </p>

            {/* Form yêu cầu hoàn tiền */}
            <form onSubmit={handleSubmit} className="space-y-4">
              {/* Mã đơn hàng */}
              <div>
                <label className="block text-sm font-medium text-gray-700 pb-2">
                  Mã đơn hàng
                </label>
                <Input
                  type="text"
                  value={orderDetail?.rentalOrderCode || ""}
                  readOnly
                  className="mt-1 w-full border border-gray-300 bg-gray-100 rounded-md cursor-not-allowed"
                  required
                />
              </div>
              {/* Lý do hoàn tiền */}
              <div className="relative w-full text-gray-700">
                <label className="block text-sm font-medium pb-2">Lý do</label>
                <button
                  type="button"
                  className="w-full text-left bg-white border border-gray-300 rounded-md py-2 px-3 shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                  onClick={() => setIsOpen((prev) => !prev)}
                >
                  {selected ? selected.name : "Chọn lý do hoàn tiền"}
                </button>
                {isOpen && (
                  <div className="absolute z-10 bg-white border border-gray-300 rounded-md mt-1 shadow-lg w-full">
                    <ul className="py-2">
                      {reasons.map((reason, idx) => (
                        <li key={idx} className="relative">
                          <button
                            type="button"
                            className={`w-full text-left px-4 py-2 text-gray-700 hover:bg-orange-200`}
                            onClick={() => handleSelect(reason)}
                          >
                            {reason.name}
                          </button>
                        </li>
                      ))}
                    </ul>
                  </div>
                )}
              </div>

              {/* Ghi chú bổ sung */}
              <div>
                <label className="block text-sm font-medium text-gray-700 pb-2">
                  Ghi chú bổ sung (không bắt buộc)
                </label>
                <Textarea
                  label="Chi tiết vấn đề bạn gặp phải (nếu có)"
                  value={notes}
                  onChange={(e) => setNotes(e.target.value)}
                  rows={3}
                  className="mt-1 w-full border border-gray-300 rounded-md text-gray-700"
                />
              </div>

              {/* Checkbox đồng ý */}
              <div className="flex items-center">
                <Checkbox
                  checked={isAgreementAccepted}
                  onChange={() => setIsAgreementAccepted(!isAgreementAccepted)}
                />
                <span className="text-sm text-gray-600 ml-2">
                  Tôi đồng ý với chính sách hoàn tiền của hệ thống.
                </span>
              </div>

              {/* Nút gửi yêu cầu hoàn tiền */}
              <div className="flex justify-end">
                <Button onClick={closeModal} color="gray" className="mr-2">
                  Đóng
                </Button>
                <Button
                  type="submit"
                  color="orange"
                  className="w-full mt-4 hover:bg-gray-500"
                  disabled={!isAgreementAccepted}
                >
                  {isSubmitting ? "Đang gửi..." : "Yêu cầu hoàn tiền"}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default RentalRefundRequestForm;
