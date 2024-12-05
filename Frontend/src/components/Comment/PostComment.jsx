import React, { useState } from 'react';
import axios from 'axios';

const PostComment = ({ productId, onCommentPosted }) => {
  const [content, setContent] = useState('');
  const [error, setError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handlePostComment = async () => {
    const token = localStorage.getItem('token'); // Get token from localStorage
    if (!token) {
      alert('Vui lòng đăng nhập để bình luận!');
      return;
    }

    if (!content.trim()) {
      setError('Vui lòng nhập nội dung bình luận.');
      return;
    }

    setIsSubmitting(true);
    try {
      await axios.post(
        `https://capstone-project-703387227873.asia-southeast1.run.app/api/Comment/comment/${productId}`,
        { content },
        {
          headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
          },
        }
      );
      setContent(''); // Clear the comment field
      setError('');
      if (onCommentPosted) onCommentPosted(); // Trigger a callback to refresh comments
    } catch (error) {
      console.error('Error posting comment:', error);
      alert('Đã xảy ra lỗi khi đăng bình luận. Vui lòng thử lại sau.');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="bg-white shadow-md rounded-lg p-6 mt-4">
      <h2 className="text-lg font-semibold mb-4">Viết bình luận:</h2>
      <textarea
        className="w-full p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring focus:border-blue-500"
        rows="4"
        placeholder="Nhập nội dung bình luận..."
        value={content}
        onChange={(e) => setContent(e.target.value)}
        disabled={isSubmitting}
      ></textarea>
      {error && <p className="text-red-500 text-sm mt-2">{error}</p>}
      <button
        onClick={handlePostComment}
        className={`mt-4 px-6 py-2 font-medium text-white rounded-lg ${
          isSubmitting
            ? 'bg-gray-400 cursor-not-allowed'
            : 'bg-blue-500 hover:bg-blue-600'
        }`}
        disabled={isSubmitting}
      >
        {isSubmitting ? 'Đang gửi...' : 'Gửi bình luận'}
      </button>
    </div>
  );
};

export default PostComment;
