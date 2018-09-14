@ECHO OFF

SET HOST=
SET GENERATECONFIG=

ECHO.
ECHO *******************************************************************************************

SET /P HOST=Which host should be used? [services.heuristiclab.com]: 
IF "%HOST%"=="" SET HOST=services.heuristiclab.com

SET /P GENERATECONFIG=Would you like to generate the configuration file? [y]: 
IF "%GENERATECONFIG%"=="" SET GENERATECONFIG=y

SET ARGS=http://%HOST%/Hive-3.3/HiveService.svc?wsdl ^
/out:HiveServiceClient ^
/namespace:*,HeuristicLab.Clients.Hive ^
/collectionType:System.Collections.Generic.List`1 ^
/targetClientVersion:Version35 ^
/serializable ^
/enableDataBinding ^
/syncOnly

IF "%GENERATECONFIG%"=="y" (
  SET ARGS=%ARGS% /config:..\app.config /mergeConfig
) ELSE (
  SET ARGS=%ARGS% /noConfig
)

ECHO.
ECHO Generating HiveService client
ECHO.

SETLOCAL ENABLEDELAYEDEXPANSION
svcutil.exe %ARGS%
ENDLOCAL

ECHO.
ECHO ---------------------------------------------------------------------------------------
ECHO !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!!
ECHO.
ECHO Following modifications have to be done manually:
ECHO  * Remove method "protected void RaisePropertyChanged(string propertyName)" in generated HiveItem
ECHO  * Change endpoint identity in app.config from "<certificate encodedValue="..." />" to "<dns value="host" />", e.g. "<dns value="services.heuristiclab.com" />"
ECHO.
ECHO !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!!
ECHO ---------------------------------------------------------------------------------------
ECHO.
ECHO Generation of HiveService client finished.
ECHO *******************************************************************************************
ECHO.

PAUSE
