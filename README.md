# Authentication and Authorization without Identity Framework

This project is a demonstration of authentication and authorization in ASP.NET Core MVC without using the ASP.NET Identity Framework or Entity Framework. The goal is to provide a custom implementation for handling user roles, claims, and permissions.

# File/Folder Descriptions

## Account
- **ClaimManager**: Handles user claim operations.
- **MailingServices**: Contains services for sending emails.
- **RoleManager**: Manages roles and related operations.
- **UserManager**: Manages user operations and data.

## Controllers
- **AccountController.cs**: Manages user account actions such as login, registration, and password management.
- **AdminTicketController.cs**: Manages admin-specific ticket operations.
- **ErrorController.cs**: Handles error pages and responses.
- **HomeController.cs**: Handles requests for the home page.
- **ProductItemController.cs**: Handles CRUD operations for product items.
- **TicketController.cs**: Manages user-side ticket operations.

## Models
- **ErrorViewModel.cs**: A model for handling and displaying errors.
- **Product.cs**: Represents the product data structure.
- **ReplyModel.cs**: Represents the reply data structure for tickets.
- **ChatViewModel.cs**: Represents the chat data structure.

## Services
- **BidirectionalChat.cs**: Manages chat operations.
- **ChatHub.cs**: SignalR hub for handling chat connections.
- **ServicesModel.cs**: A collection of services used across the application.
- **TicketManagementServices.cs**: Manages ticket-related business logic.

## Views

### Account
- **ChangePassword.cshtml**: View for changing user passwords.
- **Login.cshtml**: Login page view.
- **Register.cshtml**: Registration page view.
- *...other account-related views.*

### Home
- **AdminPanel.cshtml**: Admin dashboard view.
- **Index.cshtml**: Home page view.
- **Cart.cshtml**: User cart view.

### Shared
- **_Layout.cshtml**: Shared layout view for consistent UI across pages.
- **Error.cshtml**: Error page view.
- **_ValidationScriptsPartial.cshtml**: Partial view for validation scripts.
- **_ViewImports.cshtml**: Shared imports for views.
- **_ViewStart.cshtml**: Configuration view that runs before other views.
- **NotFound.cshtml**: 404 error page view.

### Ticket
- **Chat.cshtml**: Chat page view.
- **IssueTicket.cshtml**: View for issuing a new ticket.
- **TicketDetail.cshtml**: View for displaying ticket details.
- **Index.cshtml**: Index view for listing tickets.

## Dependencies
This project relies on the following dependencies:

- **ASP.NET Core MVC**
- **SignalR for real-time chat functionality**

Make sure to restore the NuGet packages before building and running the application.

## Configuration
- **appsettings.json**: Contains configuration settings for the application, such as connection strings and logging.
- **appsettings.Development.json**: Development-specific settings.


## Project Structure
```
.
├── Account
│   ├── ClaimManager
│   │   ├── ClaimAction.cs
│   │   ├── IClaimAction.cs
│   ├── MailingServices
│   │   ├── EmailService.cs
│   ├── RoleManager
│   │   ├── IRoleAction.cs
│   │   ├── RoleAction.cs
│   │   ├── RoleModel.cs
│   ├── UserManager
│   │   ├── IUserAction.cs
│   │   ├── UserAction.cs
│   │   ├── UserModel.cs
├── Controllers
│   ├── AccountController.cs
│   ├── AdminTicketController.cs
│   ├── ErrorController.cs
│   ├── HomeController.cs
│   ├── ProductItemController.cs
│   ├── TicketController.cs
├── Models
│   ├── ErrorViewModel.cs
│   ├── Products
│   │   ├── IProductServices.cs
│   │   ├── Product.cs
│   │   ├── ProductServices.cs
│   ├── Ticket
│   │   ├── ChatViewModel.cs
│   │   ├── Modules.cs
│   │   ├── ReplyModel.cs
│   │   ├── Tickets.cs
│   │   ├── TicketsViewModelAdmin.cs
│   │   ├── ViewModelToCreateTicket.cs
├── Services
│   ├── BidirectionalChat
│   │   ├── BidirectionalChat.cs
│   │   ├── AdminSideNotification.cs
│   │   ├── ChatHub.cs
│   │   ├── ClientSideNotification.cs
│   ├── TicketManagement
│   │   ├── TicketManagementServices.cs
├── Views
│   ├── Account
│   │   ├── ChangePassword.cshtml
│   │   ├── DeleteAccount.cshtml
│   │   ├── EditData.cshtml
│   │   ├── EmailConfirm.cshtml
│   │   ├── EmailVerified.cshtml
│   │   ├── Login.cshtml
│   │   ├── Manage.cshtml
│   │   ├── Register.cshtml
│   │   ├── RegisterSuccessfully.cshtml
│   ├── AdminTicket
│   │   ├── AdminChat.cshtml
│   │   ├── AdminListTickets.cshtml
│   ├── Home
│   │   ├── AdminPanel.cshtml
│   │   ├── Cart.cshtml
│   │   ├── Index.cshtml
│   │   ├── UserCart.cshtml
│   │   ├── UserData.cshtml
│   ├── ProductItem
│   │   ├── ProductItem.cshtml
│   ├── Shared
│   │   ├── _Layout.cshtml
│   │   ├── _ValidationScriptsPartial.cshtml
│   │   ├── Error.cshtml
│   │   ├── _ViewImports.cshtml
│   │   ├── _ViewStart.cshtml
│   │   ├── NotFound.cshtml
│   ├── Ticket
│   │   ├── Chat.cshtml
│   │   ├── Index.cshtml
│   │   ├── IssueTicket.cshtml
│   │   ├── TicketDetail.cshtml
│   │   ├── ViewImports.cshtml
│   │   ├── ViewStart.cshtml
├── wwwroot
│   ├── ChatImages
│   ├── css
│   ├── js
│   ├── lib
│   ├── favicon.ico
├── .gitignore
├── appsettings.json
├── appsettings.Development.json
├── Program.cs
└── README.md

```

## Features

- Custom user management including registration, login, and data editing.
- Role-based access control (RBAC) for securing different parts of the application.
- Custom claims management to handle user-specific permissions.
- Simple error handling.

## Getting Started

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or any preferred C# IDE

### Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/Hrsh-Sidhpuria/Authorication-and-Authentication-without-Identity-Framework.git
    ```
2. Navigate to the project directory:
    ```sh
    cd Authorication-and-Authentication-without-Identity-Framework
    ```
3. Restore the project dependencies:
    ```sh
    dotnet restore
    ```

### Running the Application

1. Build the project:
    ```sh
    dotnet build
    ```
2. Run the project:
    ```sh
    dotnet run
    ```
3. Open your browser and navigate to `https://localhost:5001` (or the URL specified in the console output).

## Usage

### User Registration

- Navigate to `/Account/Register` to register a new user.
- Provide the required details and submit the form.

### User Login

- Navigate to `/Account/Login` to log in.
- Enter your credentials and submit the form.

### Managing Roles and Claims

- Implemented custom role and claim management can be accessed and modified through the application.

## Contributing

1. Fork the repository.
2. Create your feature branch (`git checkout -b feature/YourFeature`).
3. Commit your changes (`git commit -m 'Add some feature'`).
4. Push to the branch (`git push origin feature/YourFeature`).
5. Open a pull request.

## Acknowledgements

- ASP.NET Core Documentation
- Community contributions and tutorials
