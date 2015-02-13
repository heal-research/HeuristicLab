echo off

echo.
echo *******************************************************************************************
echo Generating HiveService client
echo.

svcutil.exe ^
  http://localhost/Hive-3.3/HiveService.svc?wsdl ^
  /out:HiveServiceClient ^
  /namespace:*,HeuristicLab.Clients.Hive ^
  /collectionType:System.Collections.Generic.List`1 ^
  /targetClientVersion:Version35 ^
  /serializable ^
  /enableDataBinding ^
  /config:..\app.config 

echo.
echo ---------------------------------------------------------------------------------------
echo !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!!
echo.
echo Following modifications have to be done manually in generated data contracts:
echo  * Remove method "protected void RaisePropertyChanged(string propertyName)" in HiveItem
echo.
echo !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!!
echo ---------------------------------------------------------------------------------------
echo.
echo Generation of HiveService client finished.
echo *******************************************************************************************

