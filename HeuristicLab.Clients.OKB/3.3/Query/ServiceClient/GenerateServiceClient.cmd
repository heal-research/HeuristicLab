echo off

echo.
echo *******************************************************************************************
echo Generating OKB query service client
echo.

REM If app.config should be generated, use option "/config:..\..\app.config" and optionally "/mergeConfig" instead of "/noConfig".

svcutil.exe ^
  http://localhost:8732/Design_Time_Addresses/OKB-3.3/QueryService/mex ^
  /out:QueryServiceClient ^
  /namespace:*,HeuristicLab.Clients.OKB.Query ^
  /collectionType:System.Collections.Generic.List`1 ^
  /targetClientVersion:Version35 ^
  /enableDataBinding ^
  /noConfig

echo.
echo Generation of OKB query service client finished.
echo *******************************************************************************************
echo.

pause
