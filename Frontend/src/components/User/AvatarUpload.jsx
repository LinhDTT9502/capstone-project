import React, { useState } from "react";
import { uploadAvatar } from "../../services/ManageUserService";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCamera, faSpinner } from "@fortawesome/free-solid-svg-icons";
import { toast } from "react-toastify";

const AvatarUpload = ({ userId, onAvatarChange, imgAvatarPath, setAvatar, fetchUserData }) => {
  const [isUploading, setIsUploading] = useState(false);

  const handleFileChange = async (e) => {
    const file = e.target.files[0];
    if (!file) return;

    setIsUploading(true);
    try {
      const response = await uploadAvatar(userId, file);
      const newAvatarPath = response.imgAvatarPath;
      onAvatarChange(newAvatarPath);
      await fetchUserData(); // Refresh user data to get the latest avatar
      toast.success("Thay ảnh đại diện thành công!");
    } catch (error) {
      console.error("Error uploading avatar:", error);
      toast.error("Failed to upload avatar. Please try again.");
    } finally {
      setIsUploading(false);
    }
  };

  return (
    <div className="relative">
      <label
        htmlFor="avatar-upload"
        className="cursor-pointer bg-orange-500 hover:bg-orange-600 text-white rounded-full p-2 transition duration-300 ease-in-out"
      >
        {isUploading ? (
          <FontAwesomeIcon icon={faSpinner} spin />
        ) : (
          <FontAwesomeIcon icon={faCamera} />
        )}
      </label>
      <input
        id="avatar-upload"
        type="file"
        onChange={handleFileChange}
        className="hidden"
        accept="image/*"
      />
    </div>
  );
};

export default AvatarUpload;

