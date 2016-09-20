# What's this app

On ms Windows, to transfer messages of log files to syslog server.
This app runs like following command image for *nix.

$ tail -f *logfile* | nc -u *syslog server* 514



![](https://github.com/nori-nori/log2syslog/blob/master/image.png)




# Spec
support follows.

##multi files
access.log, error.log, apps.log, etc


##file moving(rotation)
When file rotated(cf. apache, mysql, etc),
it reopen new logfile. not need to restart app.

##non stop
When editing a target, it keeps monitoring other targets.

this is for Managed Service.


# techinical info
This app depend on **FileSystemWatcher** API.

It's like **inotify** on linux.

Features and limits are the same as the API.


# Todo
- convert Win app to service.
- support Event log.
- support SNMP
- etc...

