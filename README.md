# mkConnectNAS

mkConnectNAS is a litte windows form application that connects your fileshares to different drive letter on startup via a configuration file.
It's in a working state and I'm currently not planning to polish this any further.

## Download
Get the application here: https://github.com/Derevar/mkConnectNAS/releases

## Features
* Configure the shares via the config.xml file

## Configuration
```xml
<?xml version="1.0" encoding="UTF-8"?>
<config>
    <credentials>
        <username>YOURUSERNAMEHERE</username>
        <password>YOURPASSWORDHERE</password>
    </credentials>
    <drives>
        <drive letter="X" path="\\path\to\your\fileshare1" />
        <drive letter="Y" path="\\path\to\your\fileshare2" />
        <drive letter="Z" path="\\path\to\your\fileshare3" />
    </drives>
</config>
```

![Example](https://github.com/Derevar/mkConnectNAS/raw/master/Example.png?raw=true)