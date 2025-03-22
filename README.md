# RealTimeChatApp

Fast and robust real time chat application using .NET back-end and SignalR for chat capabilities paired with a React front-end.

Architecture:
[React App (TS)]
    │
    ├── Auth API --> [Login/Register]
    ├── REST API --> [User Info, Messages]
    └── SignalR --> [ws://localhost:5258/hub]
                       │
                 [.NET SignalR Hub]
                       │
             [Controllers | Auth | DB]
                       │
                 [SQL Server / SQLite]
                 
