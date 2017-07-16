@ECHO OFF

SET HOST=
SET GENERATECONFIG=

ECHO.
ECHO *******************************************************************************************

SET /P HOST=Which host should be used? [services.heuristiclab.com]: 
IF "%HOST%"=="" SET HOST=services.heuristiclab.com

SET /P GENERATECONFIG=Would you like to generate the configuration file? [y]: 
IF "%GENERATECONFIG%"=="" SET GENERATECONFIG=y

SET ARGS=http://%HOST%/OKB-3.3/AdministrationService.svc?wsdl ^
/out:AdministrationServiceClient ^
/namespace:*,HeuristicLab.Clients.OKB.Administration ^
/collectionType:System.Collections.Generic.List`1 ^
/targetClientVersion:Version35 ^
/enableDataBinding ^
/syncOnly

IF "%GENERATECONFIG%"=="y" (
  SET ARGS=%ARGS% /config:..\..\app.config /mergeConfig
) ELSE (
  SET ARGS=%ARGS% /noConfig
)

ECHO.
ECHO Generating AdministrationService client
ECHO.

SETLOCAL ENABLEDELAYEDEXPANSION
svcutil.exe %ARGS%
ENDLOCAL

ECHO.
ECHO ---------------------------------------------------------------------------------------
ECHO !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!!
ECHO.
ECHO Following modifications have to be done manually in generated data contracts:
ECHO  * Remove method "protected void RaisePropertyChanged(string propertyName)" in generated OKBItem
ECHO.
ECHO !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!!
ECHO ---------------------------------------------------------------------------------------
ECHO.
ECHO Generation of AdministrationService client finished.
ECHO *******************************************************************************************
ECHO.

PAUSE
