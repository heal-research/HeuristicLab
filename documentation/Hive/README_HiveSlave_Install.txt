README HeuristicLab Hive Slave Installation
===========================================

The HeuristicLab Hive Slave runs as a windows service and calculates jobs that it receives from the Hive server. 
Every Slave sends periodically heartbeats to the server and asks for a new job. 
If the server has new jobs, the Slave fetches the jobs and calculates them. 

There are two installers for the Hive Slave:

HeuristicLab.Clients.Hive.Slave.WindowsServiceSetup.msi: 
Installs the Slave windows service. The default installation path is C:\Program Files\HEAL\HeuristicLab Hive Slave.
The installed windows services is called HeuristicLab.Clients.Hive.Slave and can be stopped or started from the 
Windows Services console.
There will also be a Event source created which is called "HLHive" and a Log called "HiveSlave" where 
severe errors are logged. The logs can be viewed with the Windows Event Viewer. 
The windows service will be started automatically after the installation is completed and will also 
automatically start after boot. 

HeuristicLab.Clients.Hive.Slave.TrayIconInstaller.msi: 
Installs the graphical frontend for the windows service. This program should be installed for all users 
(checkbox in installation dialog) so that all users can use the application. 
The Hive Slave TrayIcon is started when a user logs in and sits in the windows taskbar.
If opened, it displays information about the current state of the windows service and lets the user
pause the slave. 
The default installation path is C:\Program Files\HEAL\HeuristicLab Hive Slave TrayIcon.
