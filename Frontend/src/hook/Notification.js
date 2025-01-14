import { useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";

const useOrderNotification = (
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

export default useOrderNotification;

// import { HubConnectionBuilder } from '@microsoft/signalr';

// class useOrderNotification {
//     constructor() {
//         this.connection = new HubConnectionBuilder()
//             .withUrl('https://twosport-api-offcial-685025377967.asia-southeast1.run.app//notificationHub', {
//                 accessTokenFactory: () => localStorage.getItem('token') // Assume JWT token is stored in localStorage
//             })
//             .build();
//     }

//     // Initialize the connection
//     async startConnection() {
//         try {
//             await this.connection.start();
//             console.log("SignalR connection established.");
//             this.setUpListeners();
//         } catch (err) {
//             console.error("Error while starting connection: ", err);
//         }
//     }

//     // Set up listeners for notifications
//     setUpListeners() {
//         this.connection.on("ReceiveOrderCreated", (message) => {
//             console.log("New Order Created: ", message);
//             // Show the notification to the admin, for example using a toast or alert
//         });

//         this.connection.on("ReceiveOrderRejected", (message) => {
//             console.log("Order Rejected: ", message);
//             // Show the notification to the admin
//         });

//         this.connection.on("ReceiveNotification", (message) => {
//             console.log("Notification: ", message);
//             // Show the notification to the user, e.g., rental expiration
//         });
//     }

//     // Stop the connection (if needed)
//     stopConnection() {
//         this.connection.stop();
//     }
// }

// export default new useOrderNotification();
