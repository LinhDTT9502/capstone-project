import React, { useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMessage } from "@fortawesome/free-solid-svg-icons";
import axios from "axios";
import { fetchCustomerChat } from "../../services/ChatService";
import { useEffect } from "react";
import { useSelector } from 'react-redux';
import { selectUser } from "../../redux/slices/authSlice";

const ChatButton = () => {
  const [isOpen, setIsOpen] = useState(false);
  const [message, setMessage] = useState("");
  const [chatLogs, setChatLogs] = useState([]);
  const [chatSessionId, setChatSessionId] = useState(null)
  const user = useSelector(selectUser);
  const [imageFile, setImageFile] = useState(null)

  const fetchChats = async () => {
    try {
      const response = await fetchCustomerChat();
      console.log(response.messageVMs.$values);
      setChatSessionId(response.chatSessionId)
      setChatLogs(response.messageVMs.$values)
      // // If there are chat sessions, set the first one as the selected chat
      // if (response && response.length > 0) {
      //   setSelectedChat(response[0]);
      //   fetchChatContent(response[0].chatSessionId); // Fetch chat content for the first session
      // }
    } catch (error) {
      console.error("Error fetching chat sessions:", error);
    }
  };
  useEffect(() => {

    fetchChats();
  }, []);

  const handleSendMessage = async () => {
    if (!message.trim()) return;

    // Prepare the request data
    const token = localStorage.getItem("token");
    const senderRole = "Customer";
    const receiverRole = "Coordinator";
    const messageContent = message.trim();

    const formData = new FormData();
    formData.append("message", messageContent);
    formData.append("chatSessionId", chatSessionId);
    formData.append("senderRole", senderRole);
    formData.append("receiverRole", receiverRole);

    if (imageFile) {
      formData.append("imageFile", imageFile);
    }

    try {
      const response = await axios.post(
        `https://twosport-api-offcial-685025377967.asia-southeast1.run.app/api/Chat/send-message`,
        formData,
        {
          headers: {
            "Authorization": `Bearer ${token}`,
            "accept": "*/*",
          },
          params: {
            chatSessionId,
            senderRole,
            receiverRole,
            message: messageContent,
          },
        }
      );

      if (response) {
        fetchChats(); // Fetch updated chat content after sending the message
        setMessage(""); // Clear the message input
        setImageFile(null); // Clear the selected image
      } else {
        console.error("Failed to send message:", response.status);
      }
    } catch (error) {
      console.error("Error sending message:", error);
    }
  };

  return (
    <div className="fixed bottom-2 right-4">
      {/* Nút mở chat */}
      {!isOpen && (
        <button
          onClick={() => setIsOpen(true)}
          className="relative bg-white text-orange-500 px-4 border-2 border-orange-500 py-2 rounded-md shadow-lg hover:bg-orange-200 focus:outline-none"
        >
          <FontAwesomeIcon icon={faMessage} color="orange" />
          <span className="relative z-10"> Chat</span>
        </button>
      )}

      {/* Chat Box */}
      {isOpen && (
        <div className="pr-16">
          <div className="mt-4 w-80 bg-white shadow-lg rounded-lg border border-gray-300">
            {/* Header */}
            <div className="bg-orange-600 text-white px-4 py-2 flex justify-between items-center rounded-t-lg">
              <h3 className="text-lg font-semibold">Hỗ trợ trực tuyến</h3>
              <button
                onClick={() => setIsOpen(false)}
                className="text-white hover:text-white focus:outline-none"
              >
                ✖
              </button>
            </div>

            {/* Chat logs */}
            <div className="p-4 h-64 overflow-y-auto">
              {chatLogs.map((chat, index) => (
                <div
                  key={index}
                  className={`mb-2 flex ${chat.senderId == user.UserId ? "justify-end" : "justify-start"
                    }`}
                >
                  <div
                    className={`p-3 rounded-lg max-w-3/4 ${chat.senderId == user.UserId
                      ? "bg-blue-500 text-white"
                      : "bg-gray-300 text-black"
                      }`}
                  >
                    <p>{chat.content}</p>
                    {/* {chat.imageUrl&&<img src={chat.imageUrl}/> } */}

                  </div>
                </div>
              ))}

            </div>

            {/* Input */}
            <div className="border-t border-gray-300 p-2">
              <input
                type="text"
                value={message}
                onChange={(e) => setMessage(e.target.value)}
                placeholder="Nhập tin nhắn..."
                className="w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-orange-500"
                onKeyDown={(e) => {
                  if (e.key === "Enter") handleSendMessage();
                }}
              />
              <button
                onClick={handleSendMessage}
                className="mt-2 bg-orange-500 text-white px-4 py-2 rounded-md hover:bg-orange-600"
              >
                Gửi
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default ChatButton;
