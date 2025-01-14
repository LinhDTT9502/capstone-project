import React, { useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMessage } from "@fortawesome/free-solid-svg-icons";
import axios from "axios";

const ChatButton = () => {
  const [isOpen, setIsOpen] = useState(false);
  const [message, setMessage] = useState("");
  const [chatLogs, setChatLogs] = useState([]);

  const branchId = 3; 
  const senderRole = "Coordinator"; 
  const receiverRole = "a";

  const sendMessage = async () => {
    if (!message.trim()) return;

    try {
      // Gọi API để gửi tin nhắn
      const response = await axios.post(
        `https://twosport-api-offcial-685025377967.asia-southeast1.run.app/api/Chat/send-message`,
        { message },
        {
          params: { branchId, senderRole, receiverRole },
          headers: { "Content-Type": "application/json" },
        }
      );

      setChatLogs((prevLogs) => [...prevLogs, { sender: "You", message }]);
      setMessage("");
    } catch (error) {
      console.error("Failed to send message:", error);
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
              {chatLogs.map((log, index) => (
                <div
                  key={index}
                  className={`flex ${
                    log.sender === "You" ? "justify-end" : "justify-start"
                  } mb-2`}
                >
                  <div
                    className={`px-3 py-2 rounded-lg text-sm ${
                      log.sender === "You"
                        ? "bg-orange-500 text-white"
                        : "bg-gray-200 text-gray-800"
                    }`}
                  >
                    {log.message}
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
                  if (e.key === "Enter") sendMessage();
                }}
              />
              <button
                onClick={sendMessage}
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
