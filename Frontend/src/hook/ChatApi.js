import { useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";

const ChatHub = (
  onNotificationReceived,
  hubUrl = "https://twosport-api-offcial-685025377967.asia-southeast1.run.app/notificationHub"
) => {
  const connectionRef = useRef(null);

  useEffect(() => {
    // Create the SignalR connection
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        accessTokenFactory: () => localStorage.getItem("token"), // Fetch token dynamically
        transport: signalR.HttpTransportType.All, // Auto-select transport
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000]) // Reconnect intervals
      .build();

    connectionRef.current = connection;

    const startConnection = async () => {
      try {
        await connection.start();
        console.log("SignalR Connected");

        // Register event listeners
        connection.on("ReceiveMessage", (message) => {
          console.log("ReceiveMessage received:", message);
          if (onNotificationReceived) {
            onNotificationReceived(message);
          }
        });

        connection.on("ReceiveNotification", (notification) => {
          console.log("Notification received:", notification);
          if (onNotificationReceived) {
            onNotificationReceived(notification);
          }
        });
      } catch (error) {
        console.error("Error connecting to SignalR:", error);
      }
    };

    // Start the connection
    startConnection();

    // Handle connection lifecycle events
    connection.onclose(() => {
      console.error("SignalR connection closed.");
      // Handle reconnection attempts if needed
    });

    connection.onreconnecting(() => {
      console.log("SignalR reconnecting...");
      // Handle reconnection progress if needed
    });

    connection.onreconnected(() => {
      console.log("SignalR reconnected.");
      // Handle post-reconnection actions if needed
    });

    // Cleanup the connection on unmount
    return () => {
      if (connectionRef.current) {
        connectionRef.current.stop();
        console.log("SignalR connection stopped.");
      }
    };
  }, [hubUrl, onNotificationReceived]); // Dependencies: hubUrl and callback

  return connectionRef.current;
};

export default ChatHub;
