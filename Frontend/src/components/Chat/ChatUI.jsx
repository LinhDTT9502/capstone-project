const ChatUI = ({ onClose }) => {
  const [activeChat, setActiveChat] = useState(null);

  const conversations = [
    {
      id: 1,
      name: "cloudzy_official",
      avatar: "https://via.placeholder.com/40",
      lastMessage: "Rất nhiều sản phẩm...",
      date: "12/07/24",
    },
    {
      id: 2,
      name: "giadungbinh...",
      avatar: "https://via.placeholder.com/40",
      lastMessage: "Xin chào, cảm ơn bạn...",
      date: "16/06/24",
    },
    {
      id: 3,
      name: "cemmery",
      avatar: "https://via.placeholder.com/40",
      lastMessage: "[Voucher]",
      date: "07/07/24",
    },
  ];

 return (
   <div className="fixed top-20 left-10 z-50 bg-white border shadow-lg rounded-lg w-96 h-[500px] flex flex-col">
     {/* Bố cục chính */}
     <div className="flex-1 flex relative">
       {/* Sidebar danh sách hội thoại */}
       <div className="w-1/3 bg-gray-100 border-r flex flex-col">
         {/* Tiêu đề và tìm kiếm */}
         <div className="p-3 border-b">
           <h1 className="text-sm font-bold mb-2">Chat</h1>
           <input
             type="text"
             placeholder="Tìm theo tên..."
             className="w-full p-2 text-xs border rounded-lg"
           />
         </div>

         {/* Danh sách hội thoại */}
         <div className="flex-1 overflow-y-auto">
           {conversations.map((conversation) => (
             <div
               key={conversation.id}
               className={`flex items-center p-3 cursor-pointer hover:bg-gray-200 ${
                 activeChat === conversation.id ? "bg-gray-200" : ""
               }`}
               onClick={() => setActiveChat(conversation.id)}
             >
               <img
                 src={conversation.avatar}
                 alt={conversation.name}
                 className="w-10 h-10 rounded-full"
               />
               <div className="ml-3 flex-1">
                 <h2 className="text-sm font-bold">{conversation.name}</h2>
                 <p className="text-xs text-gray-500 truncate">
                   {conversation.lastMessage}
                 </p>
               </div>
               <span className="text-xs text-gray-400">
                 {conversation.date}
               </span>
             </div>
           ))}
         </div>
       </div>

       {/* Nội dung chat */}
       <div className="flex-1 flex flex-col">
         {activeChat ? (
           <>
             {/* Header của cuộc hội thoại */}
             <div className="p-3 border-b flex items-center">
               <h2 className="text-sm font-bold">
                 {
                   conversations.find(
                     (conversation) => conversation.id === activeChat
                   )?.name
                 }
               </h2>
               {/* Nút đóng */}
               <button
                 onClick={onClose}
                 className="absolute top-2 right-2 bg-red-500 text-white px-2 py-1 rounded-full text-xs hover:bg-red-600 transition duration-300"
               >
                X
               </button>
             </div>

             {/* Nội dung tin nhắn */}
             <div className="flex-1 overflow-y-auto p-3 bg-gray-50">
               <div className="text-center text-xs text-gray-500">
                 Đang hiển thị nội dung cuộc hội thoại...
               </div>
             </div>

             {/* Ô nhập tin nhắn */}
             <div className="p-3 border-t flex items-center">
               <input
                 type="text"
                 placeholder="Nhập tin nhắn..."
                 className="flex-1 p-2 border rounded-lg text-xs"
               />
               <button className="ml-2 bg-blue-500 text-white px-3 py-1 rounded-lg text-xs">
                 Gửi
               </button>
             </div>
           </>
         ) : (
           <div className="flex-1 flex flex-col items-center justify-center text-xs text-gray-500">
             <img
               src="https://via.placeholder.com/80"
               alt="Welcome"
               className="w-20 h-20"
             />
             <h2 className="text-sm font-bold mt-2">
               Chào mừng bạn đến với Shopee Chat
             </h2>
             <p>Bắt đầu trả lời người mua!</p>
           </div>
         )}
       </div>
     </div>
   </div>
 );
};
