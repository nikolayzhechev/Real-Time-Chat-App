# RealTimeChatApp

Fast and robust real time chat application using .NET back-end and SignalR for chat capabilities paired with a React front-end.

### Architecture:

	React App: Handles UI, routes, auth, and connects to SignalR  
	SignalR Hub: Real-time messaging layer  
	Controller API: Handles login, registration, and protected endpoints  
	UserService / ChatService: Business logic layer  
	Database: Persistent storage (PostgreSQL)  
	Redis: Real-time caching or pub-sub for scalability  

	Back-end:
		RealTimeChatApp/
		├── Controllers/         → API endpoints (AuthController, MessageController)
		├── Hubs/                → SignalR Hubs (ChatHub.cs)
		├── Services/            → Interfaces + implementations (IUserService, IChatService)
		├── Models/              → DTOs and EF Entities
		├── Data/                → DbContext, database migrations
		└── Program.cs           → Service configuration, middleware
	
	Design patterns:
		- Repository Pattern → abstracts data access
		- Service Layer → handles business logic
		- DTOs → For requests/responses, doesn't expose EF entities
		- Command/Handler	→ future extensibility (MediatR for CQRS)
		- Singleton Pattern → for in-memory chat service or caching
		- Factory Pattern	→ token creation or message formatting
	
	Front-end:
		client/
		├── src/
		│   ├── components/       → Reusable UI elements (ChatBox, MessageItem)
		│   ├── pages/            → Route-level views (LoginPage, ChatPage)
		│   ├── services/         → Auth, SignalR connection, API helpers
		│   ├── context/          → AuthContext, SignalRContext, UserContext (via useContext)
		│   ├── hooks/            → useAuth, useSignalR (abstractions)
		│   ├── App.tsx
		│   └── index.tsx
	