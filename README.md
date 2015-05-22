#General
This project uses [GetEventStore](https://geteventstore.com/) 3.0.3 for Windows and [MongoDB](https://www.mongodb.org/) 3.0.3 (Windows 64-bit 2009 R2+)

#Instructions


- Clone the Git repo to your local drive
- Open a Powershell command prompt and navigate to the folder containing the cloned repo
- Build the solution by invoking the following command: `invoke-psake .\build.ps1 Test`

- Run MongoDB as a Windows service by using this command

    $path = Get-Location
    echo "systemLog:" > .\MongoDb\mongod.cfg
    echo "   destination: file" >> .\MongoDb\mongod.cfg
    echo "   path: $path\logs\mongodb\mongod.log" >> .\MongoDb\mongod.cfg
    echo "storage:" >> .\MongoDb\mongod.cfg
    echo "   dbPath: C:\dev\ProjectHealth\Projects\data\mongodb" >> .\MongoDb\mongod.cfg
    sc.exe create MongoDB binPath= "$path\mongodb\mongod.exe --service --config=$path\mongodb\mongod.cfg"  DisplayName= "MongoDB" start= "auto" 
    Start-Service MongoDB

#Run GetEventStore (GES)
To run GES open a command prompt as Administrator and navigate to the EventStore subfolder of the project. Start GES using this command

```EventStore.ClusterNode.exe --db ..\Data\EventStore```

This will start GES with the data directory `..\Data\EventStore` listening at the default tcp-ip port 1113 and http port 2113. The default username is equal to `admin` and the default password is `changeit`.

#Admin GES
Open a browser and navigate to `localhost:2113/web/index.html`. Enter the credentials when asked (admin/changeit). Navigate to the `Stream Browser` tab. You should see a list of streams. Click on the one whose events you want to see, e.g. `SampleAggregate-1`. The list of events in the stream will be displayed starting with the most recent event.

#How to use
Run the application. You can use the Postman REST client for Google Chrome to test the application. Iteration zero implements a `Samples` controller with two endpoints

```localhost:3030/api/samples/start```

```localhost:3030/api/samples/<sampleId>/step1```

The first endpoint expects a body

```{ name: "Some sample name" }```

whilst the second endpoint expects a body like

```{ quantity: 12, dueDate: "2015-05-20" }```