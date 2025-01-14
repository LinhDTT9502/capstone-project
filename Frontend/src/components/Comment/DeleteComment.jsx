import React from "react";
import axios from "axios";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faTrash } from "@fortawesome/free-solid-svg-icons";

const DeleteComment = ({ commentId, onDeleteSuccess }) => {
  const handleDelete = () => {
    const token = localStorage.getItem("token");
    if (!token) {
      alert("Vui lòng đăng nhập để xóa bình luận!");
      return;
    }

    const confirmDelete = window.confirm(
      "Bạn có chắc chắn muốn xóa bình luận này?"
    );
    if (!confirmDelete) return;

    axios
      .delete(
        `https://twosport-api-offcial-685025377967.asia-southeast1.run.app/api/Comment/remove-comment/${commentId}`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      )
      .then(() => {
        alert("Xóa bình luận thành công!");
        onDeleteSuccess(); // Notify the parent to refresh the comments
      })
      .catch((error) => {
        console.error("Error deleting comment:", error);
        alert("Đã xảy ra lỗi khi xóa bình luận. Vui lòng thử lại sau.");
      });
  };

  return (
    <button
      onClick={handleDelete}
      className="text-red-500 hover:text-red-700 hover:bg-red-100 p-2 rounded-md flex items-center"
    >
      <FontAwesomeIcon icon={faTrash} className="w-4 h-4 mr-1" />
      Xóa
    </button>
  );
};

export default DeleteComment;
