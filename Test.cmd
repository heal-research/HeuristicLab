@ECHO OFF

SET /P BUILDBEFORETEST=Should the test project be rebuilt [n]:

SET TESTCATEGORY=%~1

IF "%TESTCATEGORY%"=="" SET TESTCATEGORY=Essential
SET /P USERCATEGORY=Which category do you want to run  [%TESTCATEGORY%]:
IF "%USERCATEGORY%" NEQ "" SET TESTCATEGORY=%USERCATEGORY%

set /P TESTPLATFORM=Which platform to run the tests [x64]:
IF "%TESTPLATFORM%"=="" SET TESTPLATFORM=x64

FOR /F "tokens=1,3 delims=	 " %%A IN ('REG QUERY "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MSBuild\ToolsVersions\4.0"') DO (
  IF "%%A"=="MSBuildToolsPath" SET MSBUILDPATH=%%B)

IF "%BUILDBEFORETEST%" NEQ "" (
  ECHO Building tests project ...
  %MSBUILDPATH%msbuild.exe "HeuristicLab 3.3 Tests.sln" /target:Rebuild /p:Configuration="Debug",Platform="%TESTPLATFORM%" /m:2 /nologo /verbosity:q /clp:ErrorsOnly
)

ECHO Test starting for category %TESTCATEGORY%...

FOR /F "tokens=1,2,* delims=	 " %%A IN ('REG QUERY "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\SxS\VS7"') DO (
  IF "%%A"=="14.0" (SET VSPATH=%%C) & GOTO :run_tests
  IF "%%A"=="12.0" (SET VSPATH=%%C) & GOTO :run_tests
)

:run_tests
"%VSPATH%Common7\IDE\CommonExtensions\Microsoft\TestWindow\VSTest.Console.exe" bin\HeuristicLab.Tests.dll /Framework:framework40 /Platform:%TESTPLATFORM% /TestCaseFilter:"TestCategory=%TESTCATEGORY%"

PAUSE
