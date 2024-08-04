# Authentication and Authorization without Identity Framework

This project is a demonstration of authentication and authorization in ASP.NET Core MVC without using the ASP.NET Identity Framework or Entity Framework. The goal is to provide a custom implementation for handling user roles, claims, and permissions.

## Project Structure
```
.
├── Account
│   ├── ClaimManager
│   │   ├── ClaimAction.cs
│   │   ├── IClaimAction.cs
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
│   ├── ErrorController.cs
│   ├── HomeController.cs
├── Models
│   ├── ErrorViewModel.cs
├── Views
│   ├── Account
│   │   ├── ChangePassword.cshtml
│   │   ├── DeleteAccount.cshtml
│   │   ├── EditData.cshtml
│   │   ├── Login.cshtml
│   │   ├── Register.cshtml
│   │   ├── ViewData.cshtml
│   ├── Home
│   │   ├── AdminPanel.cshtml
│   │   ├── Cart.cshtml
│   │   ├── Index.cshtml
│   │   ├── UserCart.cshtml
│   │   ├── UserData.cshtml
│   ├── Shared
│   │   ├── _Layout.cshtml
│   │   ├── _ValidationScriptsPartial.cshtml
│   │   ├── Error.cshtml
│   │   ├── _ViewImports.cshtml
│   │   ├── _ViewStart.cshtml
│   │   ├── NotFound.cshtml
├── .gitignore
├── appsettings.json
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
