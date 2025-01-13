import React, { useState } from "react";

import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMessage } from "@fortawesome/free-solid-svg-icons";

const ChatButton = () => {
  const [isChatOpen, setIsChatOpen] = useState(false);

  const toggleChat = () => setIsChatOpen((prev) => !prev);

 const [isOpen, setIsOpen] = useState(false);

 return (
   <div className="fixed bottom-2 right-4">
     {/* Button to toggle chat box */}
     {isOpen === false && (
       <div className="">

         <button
           onClick={() => setIsOpen(!isOpen)}
           className="relative bg-white text-orange-500 px-4 border-2 border-orange-500 py-2 rounded-md shadow-lg hover:bg-orange-200 focus:outline-none"
         >
           <FontAwesomeIcon icon={faMessage} color="orange" />
           {/* Ping animation */}
           <span className="absolute top-0 right-0 flex h-3 w-3">
             <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-sky-400 opacity-75"></span>
             <span className="relative inline-flex rounded-full h-3 w-3 bg-red-500"></span>
           </span>
           {/* Button Text */}
           {" "}<span className="relative z-10">Chat</span>
         </button>
       </div>
     )}

     {/* Chat box container */}
     {isOpen && (
       <div className="pr-16">
         <div className="mt-4 w-80 bg-white shadow-lg rounded-lg border border-gray-300">
           {/* Chat header */}
           <div className="bg-orange-600 text-white px-4 py-2 flex justify-between items-center rounded-t-lg">
             <h3 className="text-lg font-semibold">Hỗ trợ trực tuyến</h3>
             <button
               onClick={() => setIsOpen(false)}
               className="text-white hover:text-white focus:outline-none"
             >
               ✖
             </button>
           </div>

           {/* Chat body */}
           <div className="p-4 h-64 overflow-y-auto">
             <div className="text-gray-500 text-sm">
               <p>Xin chào! Tôi có thể giúp gì cho bạn?</p>
             </div>
             {/* Example chat messages */}
             <div className="mt-4">
               <div className="flex items-start mb-2">
                 <div className="bg-gray-200 text-gray-800 px-3 py-2 rounded-lg text-sm">
                   Xin chào! Tôi cần hỗ trợ.
                 </div>
               </div>
               <div className="flex items-start justify-end mb-2">
                 <div className="bg-orange-500 text-white px-3 py-2 rounded-lg text-sm">
                   Chào bạn, tôi có thể giúp gì?
                 </div>
               </div>
             </div>
           </div>

           {/* Chat input */}
           <div className="border-t border-gray-300 p-2">
             <input
               type="text"
               placeholder="Nhập tin nhắn..."
               className="w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-orange-500"
             />
           </div>
         </div>
       </div>
     )}
   </div>
 );
};


export default ChatButton;
