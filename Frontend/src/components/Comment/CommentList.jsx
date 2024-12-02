// export default CommentList = ({ comments }) => {
//     return (
//       <div>
//         {comments.map((comment) => (
//           <div key={comment.id} className="mb-4">
//             <div className="border border-gray-300 p-4 rounded-lg">
//               <p><strong>{comment.username}</strong>: {comment.content}</p>
//               <p className="text-sm text-gray-500">Đăng lúc: {new Date(comment.createdAt).toLocaleString()}</p>
//             </div>
//             <div className="ml-8">
//               {comment.replies.map(reply => (
//                 <div key={reply.id} className="border-l border-gray-300 pl-4 mt-2">
//                   <p><strong>{reply.username}</strong>: {reply.content}</p>
//                   <p className="text-sm text-gray-500">Trả lời lúc: {new Date(reply.createdAt).toLocaleString()}</p>
//                 </div>
//               ))}
//             </div>
//           </div>
//         ))}
//       </div>
//     );
//   };
  