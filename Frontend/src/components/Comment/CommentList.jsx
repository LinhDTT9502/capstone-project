import React, { useEffect, useState } from 'react';
import { getComment } from '../../services/Comment/CommentService';
import PostComment from './PostComment';
import ReplyComment from './ReplyComment';
import DeleteComment from './DeleteComment';

const CommentList = ({ productId }) => {
  const [comments, setComments] = useState([]);
  const [replyingTo, setReplyingTo] = useState(null);

  const fetchComments = async () => {
    try {
      const commentData = await getComment(productId);
      const structuredComments = buildCommentTree(commentData);
      setComments(structuredComments);
    } catch (error) {
      console.error('Error fetching comments:', error);
    }
  };

  useEffect(() => {
    fetchComments();
  }, [productId]);

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
      <div key={comment.id} className={`pl-${level * 4} mt-4`}>
        <div className="flex space-x-4 items-start">
          <div className="flex-1">
            <div className="flex justify-between items-center">
              <h3 className="font-medium text-gray-800">{comment.username}</h3>
              <span className="text-sm text-gray-500">
                {new Date(comment.createdAt).toLocaleString()}
              </span>
            </div>
            <p className="text-gray-700 mt-1">{comment.content}</p>
          </div>
        </div>
        <div className="mt-2 flex items-center">
          <button
            className="text-blue-500 hover:underline text-sm"
            onClick={() => setReplyingTo(replyingTo === comment.id ? null : comment.id)}
          >
            Trả lời
          </button>
          <DeleteComment
            commentId={comment.id}
            onDeleteSuccess={() => fetchComments()}
          />
        </div>
        {replyingTo === comment.id && (
          <ReplyComment
            productId={productId}
            parentCommentId={comment.id}
            onReplySuccess={() => {
              setReplyingTo(null);
              fetchComments();
            }}
          />
        )}
        {comment.replies?.length > 0 && (
          <div className="ml-6 border-l-2 border-gray-300">
            {renderComments(comment.replies, level + 1)}
          </div>
        )}
      </div>
    ));
  };

  return (
    <div className="container mx-auto p-4">
      <div className="bg-white shadow-md rounded-lg p-6">
        <h2 className="text-xl font-semibold mb-4">Bình luận:</h2>
        {comments.length > 0 ? (
          <div>{renderComments(comments)}</div>
        ) : (
          <p className="text-gray-500">Chưa có bình luận nào.</p>
        )}
        <PostComment productId={productId} onCommentPosted={fetchComments} />
      </div>
    </div>
  );
};

export default CommentList;
