# RealTimeChatApp

Fast and robust real time chat application using .NET back-end and SignalR for chat capabilities paired with a React front-end.

Architecture:
- **React Front-End (TypeScript):** Handles UI, user input, and connects via SignalR.
- **SignalR Client:** Manages real-time messaging over WebSocket.
- **.NET Back-End API:**
  - Auth Controller: Issues JWT tokens.
  - SignalR Hub (`ChatHub.cs`): Real-time message broadcast.
  - Other Controllers: User profile, etc.
- **Auth Service:** Validates credentials, issues JWT.
- **Entity Framework:** Handles DB access.
- **SQL Database:** Stores users and messages.
- **Redis (Optional):** Used for caching, presence, and scaling SignalR (with backplane).
