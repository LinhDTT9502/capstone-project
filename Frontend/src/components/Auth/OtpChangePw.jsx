import React, { useState } from "react";
import { Button, Input } from "@material-tailwind/react";
import { toast } from "react-toastify";

export default function OTPPopup({ email, onSubmit }) {
  const [otp, setOtp] = useState("");

  const handleSubmit = async () => {
    if (!otp) {
      toast.error("Please enter the OTP.");
      return;
    }

    try {
      await onSubmit(otp);
    } catch (error) {
      toast.error("Error verifying OTP: " + error.message);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center">
      <div className="bg-white p-6 rounded-lg shadow-lg w-80">
        <h3 className="text-xl font-semibold mb-4">Enter OTP</h3>
        <Input
          type="text"
          value={otp}
          onChange={(e) => setOtp(e.target.value)}
          placeholder="Enter OTP"
          className="mb-4 w-full p-3 border border-gray-300 rounded-lg"
        />
        <Button
          onClick={handleSubmit}
          color="orange"
          className="w-full p-3 rounded-lg bg-orange-500 text-white hover:bg-orange-600"
        >
          Verify OTP
        </Button>
      </div>
    </div>
  );
}
