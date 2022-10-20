@ECHO OFF

SET HOST=
SET GENERATECONFIG=

ECHO.
ECHO *******************************************************************************************

SET /P HOST=Which host should be used? [services.heuristiclab.com]: 
IF "%HOST%"=="" SET HOST=services.heuristiclab.com

SET /P GENERATECONFIG=Would you like to generate the configuration file? [y]: 
IF "%GENERATECONFIG%"=="" SET GENERATECONFIG=y

SET ARGS=http://%HOST%/Deployment-3.3/UpdateService.svc?wsdl http://%HOST%/Deployment-3.3/AdminService.svc?wsdl ^
/out:ServiceClients ^
/namespace:*,HeuristicLab.PluginInfrastructure.Advanced.DeploymentService ^
/targetClientVersion:Version35 ^
/syncOnly

IF "%GENERATECONFIG%"=="y" (
  SET ARGS=%ARGS% /config:..\..\app.config /mergeConfig
) ELSE (
  SET ARGS=%ARGS% /noConfig
)

ECHO.
ECHO Generating UpdateService and AdminService clients
ECHO.

SETLOCAL ENABLEDELAYEDEXPANSION
svcutil.exe %ARGS%
ENDLOCAL

ECHO.
ECHO ---------------------------------------------------------------------------------------
ECHO !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!!
ECHO.
ECHO Following modifications have to be done manually:
ECHO  * Change endpoint identity in app.config from "<certificate encodedValue="..." />" to "<dns value="host" />", e.g. "<dns value="services.heuristiclab.com" />"
ECHO.
ECHO !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!!
ECHO ---------------------------------------------------------------------------------------
ECHO.
ECHO Generation of UpdateService and AdminService clients finished.
ECHO *******************************************************************************************
ECHO.

PAUSE
