import * as signalR from "@microsoft/signalr";

// Set up SignalR to manage the connection
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7250/hub")
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();

export default connection;