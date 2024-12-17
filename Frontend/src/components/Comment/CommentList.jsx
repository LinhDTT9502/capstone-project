import React, { useEffect, useState } from 'react';
import { getComment } from '../../services/Comment/CommentService';
import PostComment from './PostComment';
import ReplyComment from './ReplyComment';
import DeleteComment from './DeleteComment';

const CommentList = ({ productCode }) => {
  const [comments, setComments] = useState([]);
  const [replyingTo, setReplyingTo] = useState(null);
  const [token, setToken] = useState(null);
  const [userId, setUserId] = useState(null);

  const fetchComments = async () => {
    try {
      const commentData = await getComment(productCode);
      const structuredComments = buildCommentTree(commentData);
      setComments(structuredComments);
    } catch (error) {
      console.error('Error fetching comments:', error);
    }
  };

  useEffect(() => {
    const fetchToken = () => {
      const storedToken = localStorage.getItem('token'); // Dùng localStorage thay vì AsyncStorage
      if (storedToken) {
        setToken(storedToken);
        const decodedToken = JSON.parse(atob(storedToken.split('.')[1])); // Giải mã JWT (nếu dùng JWT)
        setUserId(decodedToken.userId); // Giả sử bạn lưu userId trong token
      } else {
        setToken(null);
        setUserId(null);
      }
    };

    fetchToken();
    fetchComments();
    
  }, [productCode]);

  const buildCommentTree = (comments) => {
    const commentMap = {};
    const roots = [];

    comments.forEach((comment) => {
      commentMap[comment.id] = { ...comment, replies: [] };
    });

    comments.forEach((comment) => {
      if (comment.parentCommentId === 0) {
        roots.push(commentMap[comment.id]);
      } else if (commentMap[comment.parentCommentId]) {
        commentMap[comment.parentCommentId].replies.push(commentMap[comment.id]);
      }
    });

    return roots;
  };

  const renderComments = (comments, level = 0) => {
    return comments.map((comment) => (
      <div key={comment.id} className={`pl-${level * 4} mt-6`}>
        <div className="bg-white rounded-lg shadow-sm p-4 transition duration-300 ease-in-out hover:shadow-md">
          <div className="flex space-x-4 items-start">
            <div className="flex-shrink-0">
              <div className="w-10 h-10 rounded-full bg-gray-200 flex items-center justify-center text-gray-500 font-semibold">
                {comment.fullName.charAt(0).toUpperCase()}
              </div>
            </div>
            <div className="flex-1">
              <div className="flex justify-between items-center">
                <h3 className="font-medium text-gray-800">{comment.fullName}</h3>
                <span className="text-xs text-gray-500">
                  {new Date(comment.createdAt).toLocaleString()}
                </span>
              </div>
              <p className="text-gray-700 mt-2">{comment.content}</p>
            </div>
          </div>
          <div className="mt-3 flex items-center space-x-4">
            {/* {comment.parentCommentId === 0 && (
              <button
                className="text-blue-500 hover:text-blue-600 text-sm font-medium transition duration-300 ease-in-out"
                onClick={() => setReplyingTo(replyingTo === comment.id ? null : comment.id)}
              >
                Trả lời
              </button>
            )} */}

            {/* Hiển thị nút xóa chỉ khi người dùng đã đăng nhập và là chủ của bình luận */}
            {token && userId === comment.userId && (
              <DeleteComment
                commentId={comment.id}
                onDeleteSuccess={() => fetchComments()}
              />
            )}
          </div>
          {/* {replyingTo === comment.id && comment.parentCommentId === 0 && (
            <div className="mt-4">
              <ReplyComment
                productCode={productCode}
                parentCommentId={comment.id}
                onReplySuccess={() => {
                  setReplyingTo(null);
                  fetchComments();
                }}
              />
            </div>
          )} */}
        </div>
        {comment.replies?.length > 0 && (
          <div className="ml-6 mt-4 border-l-2 border-gray-200 pl-4">
            {renderComments(comment.replies, level + 1)}
          </div>
        )}
      </div>
    ));
  };

  return (
    <div className="container mx-auto p-4">
      <div className="bg-white shadow-lg rounded-lg p-6">
        <h2 className="text-2xl font-semibold mb-6 text-gray-800">Bình luận</h2>
        {comments.length > 0 ? (
          <div className="space-y-6">{renderComments(comments)}</div>
        ) : (
          <p className="text-gray-500 italic">Chưa có bình luận nào.</p>
        )}
        <div className="mt-8">
          <PostComment productCode={productCode} onCommentPosted={fetchComments} />
        </div>
      </div>
    </div>
  );
};

export default CommentList;
