# How to run this portion of the application
Run the buildapi.sh script from the base directory. If you are on windows and can't access bash, use wsl or git bash. You need docker installed
The api will be accessible on port 42069 (we are very mature, I know), and you can use [wumpapi.http](wumpapi.http) to make sample requests.
# About
This is the backend of the game, and contains almost every endpoint that will be in the final project. It is basically complete
The entrypoint is in [startup](api/Startup.cs)
## Features currently not implemented
 - Winning the game - game has a win condition, no game starts
 - Updating statistics (Endpoint not implemented yet, and no statistics are updated, but the infrastructure to manage statistics exists)
 - Fees
## Features currently implemented on the backend
 - Creating accounts
 - General persistence via database
 - Authentication
 - Items
 - Buying/selling items
 - Power generation
 - Game starting/persistence
 - Alliance Management
 - Item usage
 - Item building
 - Giving items
 - Graph data
 - Everything in between to make all of these features work
