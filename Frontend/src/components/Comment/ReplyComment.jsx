import React, { useState } from 'react';
import axios from 'axios';

const ReplyComment = ({ productId, parentCommentId, onReplySuccess }) => {
  const [content, setContent] = useState('');

  const handleReply = () => {
    if (!content.trim()) {
      alert('Vui lòng nhập nội dung phản hồi.');
      return;
    }

    const token = localStorage.getItem('token');
    if (!token) {
      alert('Vui lòng đăng nhập để trả lời!');
      return;
    }

    axios
      .post(
        `https://capstone-project-703387227873.asia-southeast1.run.app/api/Comment/reply-comment/${productId}?parentCommentId=${parentCommentId}`,
        { content },
        {
          headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
          },
        }
      )
      .then(() => {
        setContent('');
        onReplySuccess(); // Notify the parent to refresh the comments
      })
      .catch((error) => {
        console.error('Error replying to comment:', error);
        alert('Đã xảy ra lỗi khi phản hồi. Vui lòng thử lại sau.');
      });
  };

  return (
    <div className="mt-2">
      <textarea
        className="w-full p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring focus:border-blue-500"
        rows="2"
        placeholder="Nhập nội dung phản hồi..."
        value={content}
        onChange={(e) => setContent(e.target.value)}
      ></textarea>
      <button
        className="mt-2 px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600"
        onClick={handleReply}
      >
        Gửi phản hồi
      </button>
    </div>
  );
};

export default ReplyComment;
