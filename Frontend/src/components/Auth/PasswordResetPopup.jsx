import React, { useState } from "react";
import { Button, Input } from "@material-tailwind/react";
import { toast } from "react-toastify";

export default function PasswordResetPopup({ onSubmit }) {
  const [oldPassword, setOldPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmNewPassword, setConfirmNewPassword] = useState("");

  const handleSubmit = async () => {
    if (newPassword !== confirmNewPassword) {
      toast.error("New passwords do not match.");
      return;
    }

    if (!oldPassword || !newPassword || !confirmNewPassword) {
      toast.error("Please fill in all fields.");
      return;
    }

    try {
      await onSubmit(oldPassword, newPassword);
    } catch (error) {
      toast.error("Error resetting password: " + error.message);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center">
      <div className="bg-white p-6 rounded-lg shadow-lg w-80">
        <h3 className="text-xl font-semibold mb-4">Reset Password</h3>
        <Input
          type="password"
          value={oldPassword}
          onChange={(e) => setOldPassword(e.target.value)}
          placeholder="Old Password"
          className="mb-4 w-full p-3 border border-gray-300 rounded-lg"
        />
        <Input
          type="password"
          value={newPassword}
          onChange={(e) => setNewPassword(e.target.value)}
          placeholder="New Password"
          className="mb-4 w-full p-3 border border-gray-300 rounded-lg"
        />
        <Input
          type="password"
          value={confirmNewPassword}
          onChange={(e) => setConfirmNewPassword(e.target.value)}
          placeholder="Confirm New Password"
          className="mb-4 w-full p-3 border border-gray-300 rounded-lg"
        />
        <Button
          onClick={handleSubmit}
          color="orange"
          className="w-full p-3 rounded-lg bg-orange-500 text-white hover:bg-orange-600"
        >
          Reset Password
        </Button>
      </div>
    </div>
  );
}
