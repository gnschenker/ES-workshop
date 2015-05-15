#General
This project uses [GetEventStore](https://geteventstore.com/) 3.0.3 for Windows and [MongoDB](https://www.mongodb.org/) 3.0.3 (Windows 64-bit 2009 R2+)

#Instructions


- Clone the Git repo to your local drive
- Open a Powershell command prompt and navigate to the folder containing the cloned repo
- Run MongoDB as a Windows service by using this command

```$path = Get-Location
echo "systemLog:" > .\MongoDb\mongod.cfg
echo "   destination: file" >> .\MongoDb\mongod.cfg
echo "   path: $path\logs\mongodb\mongod.log" >> .\MongoDb\mongod.cfg
echo "storage:" >> .\MongoDb\mongod.cfg
echo "   dbPath: C:\dev\ProjectHealth\Projects\data\mongodb" >> .\MongoDb\mongod.cfg
sc.exe create MongoDB binPath= "$path\mongodb\mongod.exe --service --config=$path\mongodb\mongod.cfg" DisplayName= "MongoDB" start= "auto"
Start-Service MongoDB```

- Run GetEventStore as a Windows service using this command

