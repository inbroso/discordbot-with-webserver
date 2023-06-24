# Discord Bot with a Webserver
With this Discord Bot you can send HTTP Requests to the Discord Bot, so you can for example send a HTTP Request from another service (e.g. Control Panel for Game Servers) to the Discord Bot to get or set Discord Roles.

## Webserver functionality
The webserver commands are in the CommandsController.cs in the discordbotServer project. There you can also add more.
e.g. ( curl -X POST "https://localhost:50001/commands/giverole?userId=HEREID&roleId=HEREROLEID" ) is the HTTP Request to give a specific role to an user.
