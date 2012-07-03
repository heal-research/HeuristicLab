echo off

echo.
echo *******************************************************************************************
echo Generating OKB administration service client
echo.

REM If app.config should be generated, use option "/config:..\..\app.config" and optionally "/mergeConfig" instead of "/noConfig".

svcutil.exe ^
  http://localhost:8732/Design_Time_Addresses/OKB-3.3/AdministrationService/mex ^
  /out:AdministrationServiceClient ^
  /namespace:*,HeuristicLab.Clients.OKB.Administration ^
  /collectionType:System.Collections.Generic.List`1 ^
  /targetClientVersion:Version35 ^
  /enableDataBinding ^
  /noConfig

echo.
echo ---------------------------------------------------------------------------------------
echo !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!!
echo.
echo Following modifications have to be done manually in generated data contracts:
echo  * Remove method "protected void RaisePropertyChanged(string propertyName)" in OKBItem
echo.
echo !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!!
echo ---------------------------------------------------------------------------------------
echo.
echo Generation of OKB administration service client finished.
echo *******************************************************************************************
echo.

pause
