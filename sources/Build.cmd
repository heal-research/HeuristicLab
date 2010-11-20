@ECHO OFF

SET /A COUNT=0
FOR /F "tokens=*" %%A IN ('dir /B *.sln') DO (
  CALL :forloopbody "%%A")

IF "%COUNT%"=="1" (
  SET SELECTED=%SOLUTIONS.1%
  ECHO Building %SOLUTIONS.1% as it is the only solution that was found ...
  GOTO :config_platform_selection)

ECHO Found the following solutions:
FOR /F "tokens=2* delims=.=" %%A IN ('SET SOLUTIONS.') DO ECHO %%A = %%B
ECHO.
SET /P SOLUTIONINDEX=Which solution to build (type the number): 

SET SELECTED=""
FOR /F "tokens=2* delims=.=" %%A IN ('SET SOLUTIONS.') DO (
IF "%%A"=="%SOLUTIONINDEX%" SET SELECTED=%%B)

IF %SELECTED%=="" GOTO :eof

:config_platform_selection
SET /P CONFIGURATION=Which configuration to build [Debug]: 
IF "%CONFIGURATION%"=="" SET CONFIGURATION=Debug
SET /P PLATFORM=Which platform to build [Any CPU]: 
IF "%PLATFORM%"=="" SET PLATFORM=Any CPU

REM First find the path to the msbuild.exe by performing a registry query
FOR /F "tokens=1,3 delims=	 " %%A IN ('REG QUERY "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MSBuild\ToolsVersions\4.0"') DO (
  IF "%%A"=="MSBuildToolsPath" SET MSBUILDPATH=%%B)

REM Then execute msbuild to clean and build the solution
REM Disable that msbuild creates a cache file of the solution
SET MSBuildUseNoSolutionCache=1
REM Run msbuild to clean and then build
%MSBUILDPATH%msbuild.exe %SELECTED% /target:Clean /p:Configuration="%CONFIGURATION%",Platform="%PLATFORM%" /nologo
%MSBUILDPATH%msbuild.exe %SELECTED% /target:Build /p:Configuration="%CONFIGURATION%",Platform="%PLATFORM%" /nologo

PAUSE

GOTO :eof

REM This workaround is necessary so that COUNT gets reevaluated
:forloopbody
SET /A COUNT+=1
SET SOLUTIONS.%COUNT%=%1
GOTO :eof