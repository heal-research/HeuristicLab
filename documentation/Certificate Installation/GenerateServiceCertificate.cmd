@echo off
REM Use this tool to create a testcertificate on the server:
REM ========================================================
REM Options:
REM -sr	 specifies the Store Location -> must be the same as used in WcfSettings.cs!
REM -ss  specifies the Store Sub Location -> must be the same as used in WcfSettings.cs!
REM -n   certificate name, don't change!
REM -sk  specifies the primary use
REM ========================================================
cls
echo This batch will create and install a new service certificate
echo for the HIVE Server!
echo You must have administrator rights in order to access the 
echo local machine certification store, otherwise you will get
echo an error message. In that case try to change the store location
echo to CurrentUser and also change the store location in WcfSettings to CurrentUser.
pause
makecert -sk HTTPS-Key -ss My -n CN=HIVE-Server -sky exhange
pause