# How to run this portion of the application
## Docker
Within the same folder as this README run ```docker build .``` and note the hash

Then run ```docker run RESULTING_HASH_HERE```

You can then make requests to the api. It defaults to port 8080
## From an IDE
I don't know for certain if this works in Visual Studio, but if your using rider you should be able to open the solution file

After you have the project open you should be able to see the various run configurations, and if you run ~~either~~(jk just use the http one, we don't have a certificate) of the .NET launch configurations it should start, and run in a developer environment

The api will be accessible on port 5034, and you can use [wumpapi.http]() to make sample requests. In the future I'll probably setup more .http files for unit testing
