@ECHO OFF

SET HOST=
SET GENERATECONFIG=

ECHO.
ECHO *******************************************************************************************

SET /P HOST=Which host should be used? [services.heuristiclab.com]: 
IF "%HOST%"=="" SET HOST=services.heuristiclab.com

SET /P GENERATECONFIG=Would you like to generate the configuration file? [y]: 
IF "%GENERATECONFIG%"=="" SET GENERATECONFIG=y

SET ARGS=http://%HOST%/AccessService-3.3/AccessService.svc?wsdl ^
/out:AccessServiceClient ^
/namespace:*,HeuristicLab.Clients.Access ^
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
ECHO Generating AccessService client
ECHO.

SETLOCAL ENABLEDELAYEDEXPANSION
svcutil.exe %ARGS%
ENDLOCAL

ECHO.
ECHO ---------------------------------------------------------------------------------------
ECHO !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!!
ECHO.
ECHO Following modifications have to be done manually:
ECHO  * Remove method "protected void RaisePropertyChanged(string propertyName)" in generated AccessItem
ECHO  * Change endpoint identity in app.config from "<certificate encodedValue="..." />" to "<dns value="host" />", e.g. "<dns value="services.heuristiclab.com" />"
ECHO.
ECHO !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!!
ECHO ---------------------------------------------------------------------------------------
ECHO.
ECHO Generation of AccessService client finished.
ECHO *******************************************************************************************
ECHO.

PAUSE
