#General
This is `Iteration Zero` for projects implemented as Micro Services using CQRS, DDD and Event Sourcing.

This project uses

- ASP.NET Web API using attribute routing
- [GetEventStore](https://geteventstore.com/) 3.0.3 for Windows 
- [MongoDB](https://www.mongodb.org/) 3.0.3 (Windows 64-bit 2009 R2+)
- StructureMap for DI
- PostSharp for AOP to provide things like logging
- Psake to build and test the solution
- NUnit and Rhino Mocks for Unit and Integration tests
- OctoPack to generate Nuget as artifacts for deployment

#Prerequisites
You need to have the following installed on your system 

- Visual Studio 2013 Professional or higher
- [PostSharp](https://visualstudiogallery.msdn.microsoft.com/a058d5d3-e654-43f8-a308-c3bdfdd0be4a)

#Instructions

- Clone the Git repository to your local drive
- Open a PowerShell command prompt and navigate to the folder containing the cloned repository
- Build the solution by invoking the following command

```.\psake\psake.ps1 .\build.ps1 Test```

- Run MongoDB as a Windows service by using this command

    $path = Get-Location
    echo "systemLog:" > .\MongoDb\mongod.cfg
    echo "   destination: file" >> .\MongoDb\mongod.cfg
    echo "   path: $path\logs\mongodb\mongod.log" >> .\MongoDb\mongod.cfg
    echo "storage:" >> .\MongoDb\mongod.cfg
    echo "   dbPath: C:\dev\ProjectHealth\Projects\data\mongodb" >> .\MongoDb\mongod.cfg
    sc.exe create MongoDB binPath= "$path\mongodb\mongod.exe --service --config=$path\mongodb\mongod.cfg"  DisplayName= "MongoDB" start= "auto" 
    Start-Service MongoDB

- Execute the batch file `RunGetEventStore.cmd` in the root folder of the repository to run GetEventStore. This will start GES with the data directory `..\Data\EventStore\Projects` listening at the default tcp-ip port 1113 and http port 2113. The default username is equal to `admin` and the default password is `changeit`.

#Admin GES
Open a browser and navigate to `localhost:2113/web/index.html`. Enter the credentials when asked (`admin`/`changeit`). Navigate to the `Stream Browser` tab. You should see a list of streams. Click on the one whose events you want to see, e.g. `SampleAggregate-<ID>` where `<ID>` is a Guid representing the ID of the aggregate instance. The list of events in the stream will be displayed starting with the most recent event.

#How to use
Run the application. You can use the Postman REST client for Google Chrome to test the application. Iteration zero implements a `Samples` controller with multiple endpoints
##GET requests

##POST requests

1 ```localhost:3030/api/samples/start```

2 ```localhost:3030/api/samples/<sampleId>/step1```

3 ```localhost:3030/api/samples/<sampleId>/approve```

4 ```localhost:3030/api/samples/<sampleId>/cancel```

the POST requests to the first endpoint expects a body

```{ name: "Some sample name" }```

whilst the POST requests to the second endpoint expects a body like this

```{ quantity: 12, dueDate: "2015-05-20" }```

the POST requests to the third and fourth endpoints expect no body 