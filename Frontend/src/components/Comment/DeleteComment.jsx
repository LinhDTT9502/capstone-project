import React from 'react';
import axios from 'axios';

const DeleteComment = ({ commentId, onDeleteSuccess }) => {
  const handleDelete = () => {
    const token = localStorage.getItem('token');
    if (!token) {
      alert('Vui lòng đăng nhập để xóa bình luận!');
      return;
    }

    const confirmDelete = window.confirm('Bạn có chắc chắn muốn xóa bình luận này?');
    if (!confirmDelete) return;

    axios
      .delete(
        `https://capstone-project-703387227873.asia-southeast1.run.app/api/Comment/remove-comment/${commentId}`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      )
      .then(() => {
        alert('Xóa bình luận thành công!');
        onDeleteSuccess(); // Notify the parent to refresh the comments
      })
      .catch((error) => {
        console.error('Error deleting comment:', error);
        alert('Đã xảy ra lỗi khi xóa bình luận. Vui lòng thử lại sau.');
      });
  };

  return (
    <button
      className="ml-4 text-red-500 hover:underline text-sm"
      onClick={handleDelete}
    >
      Xóa
    </button>
  );
};

export default DeleteComment;
