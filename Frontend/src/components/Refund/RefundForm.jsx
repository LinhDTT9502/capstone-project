import React, { useState } from "react";
import axios from "axios";
import { Button, Input, Textarea, Checkbox } from "@material-tailwind/react";

const RefundForm = () => {
  const [orderCode, setOrderCode] = useState("");
  const [orderType, setOrderType] = useState(1);
  const [reason, setReason] = useState("");
  const [notes, setNotes] = useState("");
  const [isAgreementAccepted, setIsAgreementAccepted] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!isAgreementAccepted) {
      alert("You must agree to the policy before submitting.");
      return;
    }

    const payload = {
      orderCode,
      orderType,
      reason,
      notes,
      isAgreementAccepted,
    };

    setIsSubmitting(true);

    try {
      const response = await axios.post(
        "https://capstone-project-703387227873.asia-southeast1.run.app/api/RefundRequest/create",
        payload,
        {
          headers: {
            accept: "*/*",
            "Content-Type": "application/json",
          },
        }
      );
      alert("Yêu cầu hoàn tiền đã được gửi thành công!");
      setOrderCode("");
      setOrderType(1);
      setReason("");
      setNotes("");
      setIsAgreementAccepted(false);

      setIsSubmitting(false);
    } catch (error) {
      console.error("Error submitting refund request:", error);
      alert("Đã xảy ra lỗi. Vui lòng kiểm tra lại thông tin đơn hàng.");
      setIsSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100 py-10">
      <div className="max-w-lg bg-white p-8 rounded-md shadow-md">
        <h2 className="text-2xl font-semibold text-gray-800 text-center mb-4">
          Biểu mẫu yêu cầu hoàn tiền
        </h2>
        <p className="text-sm text-gray-600 mb-6">
          Nếu bạn không hài lòng với đơn hàng của mình, bạn có thể yêu cầu hoàn
          tiền. Vui lòng cung cấp thông tin đầy đủ và lý do chi tiết.
        </p>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700">
              Mã đơn hàng
            </label>
            <Input
              type="text"
              placeholder="VD: 2411041041"
              value={orderCode}
              onChange={(e) => setOrderCode(e.target.value)}
              required
              className="mt-1 w-full"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700">
              Loại đơn hàng
            </label>
            <select
              value={orderType}
              onChange={(e) => setOrderType(Number(e.target.value))}
              className="mt-1 w-full p-2 border border-gray-300 rounded-md"
              required
            >
              <option value={1}>Đơn Mua</option>
              <option value={2}>Đơn Thuê</option>
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700">
              Lý do hoàn tiền
            </label>
            <Textarea
              placeholder="Nhập lý do tại sao bạn muốn hoàn tiền"
              value={reason}
              onChange={(e) => setReason(e.target.value)}
              required
              rows={4}
              className="mt-1 w-full border border-gray-300 rounded-md"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700">
              Ghi chú bổ sung (không bắt buộc)
            </label>
            <Textarea
              placeholder="Nhập thêm thông tin (nếu có)"
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
              rows={3}
              className="mt-1 w-full border border-gray-300 rounded-md"
            />
          </div>

          <div className="flex items-center">
            <Checkbox
              checked={isAgreementAccepted}
              onChange={() => setIsAgreementAccepted(!isAgreementAccepted)}
            />
            <span className="text-sm text-gray-600 ml-2">
              Tôi đồng ý với chính sách hoàn tiền của hệ thống.
            </span>
          </div>

          <Button
            type="submit"
            color="blue"
            className="w-full mt-4"
            disabled={
              isSubmitting || !orderCode || !reason || !isAgreementAccepted
            }
          >
            {isSubmitting ? "Đang gửi..." : "Gửi yêu cầu hoàn tiền"}
          </Button>
        </form>
      </div>
    </div>
  );
};

export default RefundForm;