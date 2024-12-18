import React, { useState } from "react";
import { useSelector } from "react-redux";
import { Button, Input } from "@material-tailwind/react";
import { updatePassword } from "../../api/apiUser";
import { toast, ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { selectUser } from "../../redux/slices/authSlice"; 

const UserChangePassword = () => {
  const user = useSelector(selectUser);
  const userId = user.UserId;

  const [formData, setFormData] = useState({
    oldPassword: "",
    newPassword: "",
    confirmPassword: "",
  });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async () => {
    if (formData.newPassword !== formData.confirmPassword) {
      toast.error("Mật khẩu xác nhận không khớp!");
      return;
    }
  
    if (formData.newPassword === formData.oldPassword) {
      toast.error("Mật khẩu mới không được giống mật khẩu cũ!");
      return;
    }
  
    if (formData.newPassword.length < 6 || formData.newPassword.length > 16) {
      toast.error("Mật khẩu mới phải từ 6 đến 16 ký tự!");
      return;
    }
  
    if (!window.confirm("Bạn có chắc chắn muốn đổi mật khẩu?")) {
      return;
    }
  
    try {
      await updatePassword(userId, formData.oldPassword, formData.newPassword);
      toast.success("Đổi mật khẩu thành công!");
      setFormData({ oldPassword: "", newPassword: "", confirmPassword: "" });
    } catch (error) {
      toast.error("Đổi mật khẩu thất bại!");
    }
  };
  

  return (
    <>
      <ToastContainer />
      <div className="container mx-auto pt-2 rounded-lg max-w-4xl">
        <div className="flex items-center justify-between mb-6">
          <h2 className="font-bold text-2xl text-orange-500">Đổi mật khẩu</h2>
        </div>
        <div className="space-y-4">
          <Input
            label="Mật khẩu cũ"
            type="password"
            name="oldPassword"
            value={formData.oldPassword}
            onChange={handleChange}
            required
          />
          <Input
            label="Mật khẩu mới"
            type="password"
            name="newPassword"
            value={formData.newPassword}
            onChange={handleChange}
            required
          />
          <Input
            label="Xác nhận mật khẩu mới"
            type="password"
            name="confirmPassword"
            value={formData.confirmPassword}
            onChange={handleChange}
            required
          />
          <div className="flex justify-end mt-4 space-x-2">
            <Button
              color="gray"
              variant="outlined"
              onClick={() =>
                setFormData({ oldPassword: "", newPassword: "", confirmPassword: "" })
              }
            >
              Hủy
            </Button>
            <Button color="orange" variant="filled" onClick={handleSubmit}>
              Lưu thay đổi
            </Button>
          </div>
        </div>
      </div>
    </>
  );
};

export default UserChangePassword;