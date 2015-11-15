# About xUnit2NUnit
This is a simple RESTful web service that converts xUnit result files to NUnit.

Converting one XML file into another XML file can be done in a few lines of code, depending on the chosen programming language. So this web service is merely a show case of various technologies and a playground to test latest technologies.

# Technology and architecture

## ASP.NET 5
The code has been written with C# for ASP.NET 5, targeting the .NET 4.5.1 environment.

## Hosting
The service can be used in a self-hosting environment (just run `dnx web`), or also in IIS or notably an Azure Web App.

## POST/GET calls
The service offers a POST call to request a transition of the provided xUnit XML file into an NUnit XML file. This call returns almost immediately with a Location response header. This allows the client to query the status of the transition through a simple GET call to the location's URL. The idea is, that POST calls can lead to long processing times, depending on what the service has to do. Assuming it would be a long-running call, it'd be a convenient way for a client to come back at a random point later instead of keeping a connection alive until all processing has been completed. The conciseness of handling the POST request can be found in [ConversionRequestController.cs](src/Service/Controllers/ConversionRequestController.cs).

## Task Parallel Library - Data Flow
The request is processed through various data flow blocks. This is mostly to show case how easy it is to setup a chain of tasks (well, functions), and hide complexity of each of the steps within those functions. As mentioned, this is definitely overhead for such a simple project, but I hope this provides some ideas. The details can be found in [XUnit2NUnitConverter.cs](src/Service/Flow/XUnit2NUnitConverter.cs).

# Testing
The service is covered with service level tests that run as part of the build. The .NET Execution Environment (DNX) makes it easy to start a web service as a standalone application and run a set of tests against the API. The details of how to run the tests can be found in the [build script](build.ps1). 
