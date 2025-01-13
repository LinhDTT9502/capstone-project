import React, { useState } from 'react';
import axios from 'axios';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPaperPlane } from '@fortawesome/free-solid-svg-icons';

const PostComment = ({ productCode, onCommentPosted }) => {
  const [content, setContent] = useState('');
  const [error, setError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handlePostComment = async () => {
    const token = localStorage.getItem('token');
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
        `https://twosport-api-offcial-685025377967.asia-southeast1.run.app/api/Comment/comment/${productCode}`,
        { content },
        {
          headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
          },
        }
      );
      setContent('');
      setError('');
      if (onCommentPosted) onCommentPosted();
    } catch (error) {
      console.error('Error posting comment:', error);
      setError('Đã xảy ra lỗi khi đăng bình luận. Vui lòng thử lại sau.');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="bg-white shadow-lg rounded-lg p-6 mt-8">
      <h2 className="text-xl font-semibold mb-4 text-gray-800">
        Viết bình luận
      </h2>
      <div className="relative">
        <textarea
          className="w-full p-4 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition duration-200 ease-in-out resize-none"
          rows="4"
          placeholder="Nhập nội dung bình luận..."
          value={content}
          onChange={(e) => setContent(e.target.value)}
          disabled={isSubmitting}
        ></textarea>
        <div className="absolute bottom-3 right-3 text-gray-400 text-sm">
          {content.length}/1000
        </div>
      </div>
      {error && (
        <p className="text-red-500 text-sm mt-2 flex items-center">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            className="h-4 w-4 mr-1"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"
            />
          </svg>
          {error}
        </p>
      )}
      <div className="mt-4 flex justify-end">
        <button
          onClick={handlePostComment}
          className={`px-6 py-2 font-medium text-white rounded-lg transition duration-200 ease-in-out ${
            isSubmitting
              ? "bg-gray-400 cursor-not-allowed"
              : "bg-blue-500 hover:bg-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-opacity-50"
          }`}
          disabled={isSubmitting}
        >
          <FontAwesomeIcon icon={faPaperPlane} style={{ color: "#ffffff" }} />{" "}
          {isSubmitting ? (
            <span className="flex items-center">
              <svg
                className="animate-spin -ml-1 mr-3 h-5 w-5 text-white"
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 24 24"
              >
                <circle
                  className="opacity-25"
                  cx="12"
                  cy="12"
                  r="10"
                  stroke="currentColor"
                  strokeWidth="4"
                ></circle>
                <path
                  className="opacity-75"
                  fill="currentColor"
                  d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                ></path>
              </svg>
              Đang gửi...
            </span>
          ) : (
            "Gửi bình luận"
          )}
        </button>
      </div>
    </div>
  );
};

export default PostComment;

