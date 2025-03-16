import * as signalR from "@microsoft/signalr";

// Set up SignalR to manage the connection
export const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5258/hub")   // .NET mapping
    .withAutomaticReconnect([0, 2000, 5000, 10000])
    .configureLogging(signalR.LogLevel.Information)
    .build();

// Establise connection before sending messages
export async function ensureConnected(): Promise<void> {
    console.log("Checking SignalR State:", connection.state);

    // Wait for full disconnection before attempting to start
    while (connection.state === signalR.HubConnectionState.Disconnecting) {
        console.log("Waiting for disconnection...");
        await new Promise((resolve) => setTimeout(resolve, 500)); // Small delay
    }

    // Ensure disconnected state
    if (connection.state === signalR.HubConnectionState.Disconnected) {
        try {
            await connection.start();
            console.log("SignalR Connected!");
        } catch (err: any) {
            console.error("SignalR Connection Error:", err);
            setTimeout(ensureConnected, 5000); // Retry after 5s if it fails
        }
    } else {
        console.warn("SignalR is already connecting or connected.");
    }
};