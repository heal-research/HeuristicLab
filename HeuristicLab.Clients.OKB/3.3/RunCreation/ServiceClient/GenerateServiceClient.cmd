echo off

echo.
echo *******************************************************************************************
echo Generating OKB run creation service client
echo.

REM If app.config should be generated, use option "/config:..\..\app.config" and optionally "/mergeConfig" instead of "/noConfig".

svcutil.exe ^
  http://localhost:8732/Design_Time_Addresses/OKB-3.3/RunCreationService/mex ^
  /out:RunCreationServiceClient ^
  /namespace:*,HeuristicLab.Clients.OKB.RunCreation ^
  /collectionType:System.Collections.Generic.List`1 ^
  /targetClientVersion:Version35 ^
  /enableDataBinding ^
  /noConfig

echo.
echo Generation of OKB run creation service client finished.
echo *******************************************************************************************
echo.

pause
