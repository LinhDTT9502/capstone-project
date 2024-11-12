import { useEffect } from "react";
import * as signalR from "@microsoft/signalr";
const useOrderNotification = (onNotificationReceived) => {
    useEffect(() => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("https://twosportapi-295683427295.asia-southeast2.run.app/notificationHub")
            .withAutomaticReconnect()
            .build();

        connection.start()
            .then(() => console.log("SignalR Connected"))
            .catch(err => console.log("Error connecting to SignalR", err));
        connection.on("ReceiveOrderNotification", (orderCode) => {
            onNotificationReceived(orderCode);
        });
        return () => {
            connection.stop();
        };
    }, [onNotificationReceived]);
};
export default useOrderNotification;