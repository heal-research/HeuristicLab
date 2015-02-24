README HeuristicLab Hive Slave Installation
===========================================

The HeuristicLab Hive Slave runs as a windows service and calculates jobs that it receives from the Hive server. 
Every slave sends periodically heartbeats to the server and asks for a new job. If the server has new jobs, the slave fetches the jobs and calculates them. The installer can be generated using NSIS (Nullsoft Scriptable Install System, http://nsis.sourceforge.net/): 

* Download and install NSIS
* Build the ExtLibs, HeuristicLab and Slave solutions
* Go to the Installers folder
* Right-click on the file HiveSlaveInstaller.nsi, select "Compile NSIS Script" 

You should now have a file "HeuristicLab Hive Slave Installer.exe" which installs the Hive Slave service and starts it. 
The default installation path is C:\Program Files\HeuristicLabHiveSlave.
The installed windows services is called HeuristicLab.Clients.Hive.Slave and can be stopped or started from the 
Windows Services console.
There will also be a Event source created which is called "HLHive" and a Log called "HiveSlave" where 
severe errors are logged. The logs can be viewed with the Windows Event Viewer. 
The windows service will be started automatically after the installation is completed and will also 
automatically start after boot. 
