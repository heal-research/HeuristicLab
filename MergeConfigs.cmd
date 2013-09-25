@echo off

IF "%Outdir%"=="" (
  SET Outdir=bin\
  SET INTERACTIVE=1
)

echo Recreating HeuristicLab 3.3.exe.config...
copy /Y "%Outdir%app.config" "%Outdir%HeuristicLab 3.3.exe.config"

echo Merging...
FOR /F "tokens=*" %%A IN ('dir /B "%Outdir%*.dll.config"') DO (
  ConfigMerger "%Outdir%%%A" "%Outdir%HeuristicLab 3.3.exe.config"
)

IF "%INTERACTIVE%"=="1" (
  pause
)