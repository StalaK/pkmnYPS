# pkmnYPS

## Requirements
- .NET 8
- Visual Studio
- Node
- MySQL

## Setup

### API

1. Open `pkmnYPS.sln` in Visual Studio
2. Ensure the MySQL server is running
3. Create a new MySQL user with the ability to create new database schemas.
4. Open the file filebehind `appsettings.Develop.json` in the `pkmnYPS` project.
5. Update the `pkmnYPS` connection string so the host and port are configured to point at the MySql server. Optionally, update the database name, username and password as required.
6. In the Package Manager Console enter the following command to run the Entity Framework migration to the configured database.
   ```
   Update-Database
   ```
8. Run the `pkmnYPS` project in IIS Express

### UI

1. In a terminal, navigate to `pkmnYPS\pkmnYPS.UI\`
2. Enter the following command to retrieve the project dependencies
   ```
   npm install
   ```
3. Open the `.env` file or create a new `.env.local` file and set the varaible `VITE_API_URL` to the base URL of the API. The default is `https://localhost:44356`.
4. Enter the command to compile for development
   ```
    npm run dev
   ```
5. Navigate to the URL to view the website
