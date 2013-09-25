echo off

echo.
echo *******************************************************************************************
echo Generating AccessService client
echo.

svcutil.exe ^
  http://localhost:81/AccessService?wsdl ^
  /out:AccessServiceClient ^
  /namespace:*,HeuristicLab.Clients.Access ^
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
echo  * Remove method "protected void RaisePropertyChanged(string propertyName)" in AccessServiceItem
echo.
echo !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!! ATTENTION !!!
echo ---------------------------------------------------------------------------------------
echo.
echo Generation of AccessService client finished.
echo *******************************************************************************************

