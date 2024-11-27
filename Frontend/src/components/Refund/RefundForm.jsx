import React, { useState } from 'react';
import axios from 'axios';
import { Button, Input, Textarea, Checkbox } from '@material-tailwind/react';

const RefundForm = () => {
  const [orderCode, setOrderCode] = useState('');
  const [orderType, setOrderType] = useState(1); // default: 1 (order)
  const [reason, setReason] = useState('');
  const [notes, setNotes] = useState('');
  const [isAgreementAccepted, setIsAgreementAccepted] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!isAgreementAccepted) {
      alert('You must agree to the policy before submitting.');
      return;
    }

    const payload = {
      orderCode,
      orderType,
      reason,
      notes,
      isAgreementAccepted
    };

    setIsSubmitting(true);

    try {
      const response = await axios.post(
        'https://capstone-project-703387227873.asia-southeast1.run.app/api/RefundRequest/create',
        payload,
        {
          headers: {
            accept: '*/*',
            'Content-Type': 'application/json',
          },
        }
      );
      alert('Refund request submitted successfully');
      setIsSubmitting(false);
    } catch (error) {
      console.error('Error submitting refund request:', error);
      alert('Failed to submit refund request');
      setIsSubmitting(false);
    }
  };

  return (
    <div className="max-w-md mx-auto bg-white p-6 rounded-lg shadow-lg">
      <h2 className="text-2xl font-semibold text-center mb-6">Biểu mẫu yêu cầu hoàn tiền</h2>
      <form onSubmit={handleSubmit} className="space-y-4">
        <div className='space-y-4'>
          <label className="block text-sm font-medium">Mã đơn hàng</label>
          <Input
            type="text"
            placeholder="Nhập mã đơn hàng"
            label='ví dụ 2411041041'
            value={orderCode}
            onChange={(e) => setOrderCode(e.target.value)}
            required
            className="mt-1 w-full"
          />
        </div>

        <div>
          <label className="block text-sm font-medium">Loại đơn hàng</label>
          <select
            value={orderType}
            onChange={(e) => setOrderType(Number(e.target.value))}
            className="mt-1 w-full p-2 border rounded-md"
            required
          >
            <option value={1}>Đơn Mua</option>
            <option value={2}>Đơn Thuê</option>
          </select>
        </div>

        <div>
          <label className="block text-sm font-medium">Lý do</label>
          <Textarea
            placeholder="Nhập lý do yêu cầu hoàn tiền"
            value={reason}
            onChange={(e) => setReason(e.target.value)}
            required
            rows={3}
            className="mt-1 w-full"
          />
        </div>

        <div>
          <label className="block text-sm font-medium">Ghi chú</label>
          <Textarea
            placeholder="Enter any additional notes"
            value={notes}
            onChange={(e) => setNotes(e.target.value)}
            rows={3}
            className="mt-1 w-full"
          />
        </div>

        <div className="flex items-center space-x-2">
          <Checkbox
            checked={isAgreementAccepted}
            onChange={() => setIsAgreementAccepted(!isAgreementAccepted)}
          />
          <span className="text-sm">I agree to the refund policy</span>
        </div>

        <Button
          type="submit"
          color="blue"
          className="w-full mt-4"
          disabled={isSubmitting || !orderCode || !reason || !isAgreementAccepted}
        >
          {isSubmitting ? 'Submitting...' : 'Submit Refund Request'}
        </Button>
      </form>
    </div>
  );
};

export default RefundForm;
