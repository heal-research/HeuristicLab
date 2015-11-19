echo off

echo.
echo *******************************************************************************************
echo Generating DeploymentService clients
echo.

svcutil.exe ^
  http://services.heuristiclab.com/Deployment-3.3/UpdateService.svc/mex ^
  http://services.heuristiclab.com/Deployment-3.3/AdminService.svc/mex ^
  /out:ServiceClients ^
  /namespace:*,HeuristicLab.PluginInfrastructure.Advanced.DeploymentService ^
  /targetClientVersion:Version35 ^
  /mergeConfig ^
  /config:..\..\app.config

echo.
echo Generation of DeploymentService clients finished.
echo *******************************************************************************************
echo.

pause