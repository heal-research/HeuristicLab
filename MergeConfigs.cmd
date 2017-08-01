@echo off

IF "%TargetDir%"=="" (
  SET TargetDir=.\bin\
  SET INTERACTIVE=1
)

echo Recreating HeuristicLab 3.3.exe.config...
copy /Y "%SolutionDir%\HeuristicLab\3.3\app.config" "%TargetDir%HeuristicLab 3.3.exe.config"

echo Merging...
FOR /F "tokens=*" %%A IN ('dir /B "%TargetDir%*.dll.config"') DO (
  "%SolutionDir%ConfigMerger.exe" "%TargetDir%%%A" "%TargetDir%HeuristicLab 3.3.exe.config"
)

IF "%INTERACTIVE%"=="1" (
  pause
)