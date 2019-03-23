# ToDo-ASP.NETCore-React
A simple ToDo list app using ASP.NET Core, React and Bootstrap. Created to demonstrate my abilities with ASP.NET and React.

### Technologies
This app uses ASP.NET for the backend, with Entity Framework Core used for database interaction, which is a SQLite database (purely for ease of portability, this shouldn't be used in production).

Because this uses Entity Framework Core, no SQL is involved, so equivalent queries are provided as comments alongside database interactions.

### Dependencies
- .NET Core SDK (version 2.2.105 tested)
- npm

### Running
Simply run `dotnet run` in `./sample-app`. The first run may take a while to start due to downloading node packages.

The server can be accessed at the address printed to console (usually localhost:5000 for Linux).

Tested on Linux, should run on Windows and MacOS, but this hasn't been tested fully.

### Authentication
Authentication is provided using JWT tokens. To obtain a token, `POST /api/auth/login` with `email` and `password`. This token should be provided as a bearer in the Authorization HTTP header of any requests made. These tokens are valid for 24 hours (configurable).

The database is pre-populated with two accounts: test@test.com and admin@test.com, the latter of which is designated as an admin user. The password for both accounts is "password".

### Back-End Configuration
A full CRUD interface exists for managing data in the system, but there is no user interface for this. Only users designated as admin with a boolean flag can access it. A JWT token with this flag set is required. The available endpoints are as follows:
#### Create
- URL: `POST /api/admin/createUser`  
Parameters: `String email, String password, Boolean admin`
- URL: `POST /api/admin/createList`  
Parameters: `Int userId,  String listName`
- URL: `POST /api/admin/createItem`  
Parameters: `Int listId, String text, Boolean complete`

#### Read
- URL: `GET /api/admin/getUsers`  
Parameters: `(none)`
- URL: `GET /api/admin/getLists/{userId}`  
Parameters: `Int id (user ID)`
- URL: `GET /api/admin/getItems/{listId}`  
Parameters: `Int id (list ID)`

#### Update
- URL: `POST /api/admin/updateUser`  
Parameters: `Int userId, String email, String password, Boolean admin`
- URL: `POST /api/admin/updateList`  
Parameters: `Int listId, String listName`
- URL: `POST /api/admin/updateItem`  
Parameters: `Int itemId, String text, Boolean complete`

#### Delete
- URL: `POST /api/admin/deleteUser/{userId}`  
Parameters: `Int id (user ID)`
- URL: `POST /api/admin/deleteList/{listId}`  
Parameters: `Int id (list ID)`
- URL: `POST /api/admin/deleteItem/{itemId}`  
Parameters: `Int id (item ID)`

All endpoints return JSON with a `success` flag and any other requested data (if applicable).
