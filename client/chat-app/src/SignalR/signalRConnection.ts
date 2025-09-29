import * as signalR from "@microsoft/signalr";

let connection: signalR.HubConnection | null = null;

// Set up SignalR to manage the connection
function createConnection(token: string):signalR.HubConnection {
     return new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5258/hub", {
            accessTokenFactory: () => {
                return localStorage.getItem("token") || "";
            }
        })   // .NET mapping
        .withAutomaticReconnect([0, 2000, 5000, 10000])
        .configureLogging(signalR.LogLevel.Information)
        .build();
}

// Establise connection before sending messages
export async function ensureConnected(token: string): Promise<void> {
    if (!connection || connection.state === signalR.HubConnectionState.Disconnected) {
        connection = createConnection(token);
    }

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

// Export connection
export function getConnection(): signalR.HubConnection | null {
    return connection;
}

// On logout
export async function stopConnection(): Promise<void> {
    if (connection) {
        try {
            await connection.stop();
            console.log("SignalR connection stopped.");
        } catch (err) {
            console.error("Error stopping SignalR connection:", err);
        } finally {
            connection = null;
        }
    }
};