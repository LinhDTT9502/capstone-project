import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";

const useOrderNotification = (onNotificationReceived) => {
  const [connection, setConnection] = useState(null);

  useEffect(() => {
    const token = localStorage.getItem("token");

    const newConnection = new signalR.HubConnectionBuilder()
      //   .configureLogging(signalR.LogLevel.Debug)
      .withUrl(`https://localhost:7276/notificationHub`, {
        accessTokenFactory: () => `${token}`,
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
        withCredentials: true
      })
      .withAutomaticReconnect() // Tự động kết nối lại
      .build();

    setConnection(newConnection);

    // Cleanup function
    return () => {
      if (
        connection &&
        connection.state !== signalR.HubConnectionState.Disconnected
      ) {
        connection.stop();
        console.log("SignalR connection stopped.");
      }
    };
  }, []); // Only run once

  useEffect(() => {
    if (connection) {
      const startConnection = async () => {
        if (connection.state === signalR.HubConnectionState.Connected) {
          console.log("SignalR is already connected.");
          return;
        }

        if (
          connection.state === signalR.HubConnectionState.Connecting ||
          connection.state === signalR.HubConnectionState.Reconnecting
        ) {
          console.log("SignalR is already connecting or reconnecting.");
          return;
        }

        try {
          await connection.start();
          console.log("SignalR Connected");

          //Group
          connection.on("ReceiveMessage", (message) => {
            console.log("ReceiveMessage received:", message);
            onNotificationReceived(message);
          });

          //User
          connection.on("ReceiveNotification", (message) => {
            console.log("Notification received:", message);
            onNotificationReceived(message);
          });
        } catch (error) {
          console.error("Error connecting to SignalR:", error);
        }
      };

      if (connection) {
        startConnection();

        return () => {
          if (connection) {
            connection.off("ReceiveNotification");
            connection.stop();
          }
        };
      }
    }
  }, [connection, onNotificationReceived]);
};

export default useOrderNotification;

// import { HubConnectionBuilder } from '@microsoft/signalr';

// class useOrderNotification {
//     constructor() {
//         this.connection = new HubConnectionBuilder()
//             .withUrl('https://capstone-project-703387227873.asia-southeast1.run.app/notificationHub', {
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
