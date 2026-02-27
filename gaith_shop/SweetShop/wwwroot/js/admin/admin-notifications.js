"use strict";

// 1. Establish connection to the Hub URL defined in Program.cs
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .withAutomaticReconnect() // Automatically reconnect if the connection drops
    .configureLogging(signalR.LogLevel.Information)
    .build();

// 2. Listen for the "ReceiveOrderNotification" event broadcasted by the server
connection.on("ReceiveOrderNotification", function (orderData) {
    console.log("New order received:", orderData);

    // Create a toast/notification element dynamically
    const notificationHtml = `
        <div class="alert alert-success alert-dismissible fade show" role="alert" style="box-shadow: 0 4px 6px rgba(0,0,0,0.1); border-left: 5px solid #28a745;">
            <strong>🛒 طلبية جديدة!</strong>
            <br/>
            العميل: ${orderData.customerName} <br/>
            رقم الطلب: #${orderData.orderId} <br/>
            الإجمالي: ${orderData.total} د.أ <br/>
            <small class="text-muted">${orderData.date}</small>
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;

    // Append it to a notification container in the Admin Dashboard UI
    const container = document.getElementById("admin-notifications-container");
    if (container) {
        container.insertAdjacentHTML('afterbegin', notificationHtml);
        
        // Optional: Play a sound
        // const audio = new Audio('/sounds/notification.mp3');
        // audio.play().catch(e => console.log("Audio play blocked by browser"));
    }
});

// 3. Start the connection
async function startSignalR() {
    try {
        await connection.start();
        console.log("SignalR Connected. Listening for Admin notifications...");
    } catch (err) {
        console.error("Error connecting to SignalR Hub:", err);
        // Retry connection after 5 seconds if initial connection fails
        setTimeout(startSignalR, 5000);
    }
}

// Initialize connection when document is ready
document.addEventListener("DOMContentLoaded", () => {
    startSignalR();
});
