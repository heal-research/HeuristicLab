set target=C:\Program Files\HeuristicLab 3.3.10

FOR /F "skip=1 tokens=1 delims= usebackq" %%G IN ("%ProjectDir%\Files.txt") DO copy "%Outdir%\%%G" "%target%" >nul

echo "Platform: %Platform%, architecture: %PROCESSOR_ARCHITECTURE%"
if "%Platform%" == "x86" (   
  FOR /F "skip=1 tokens=* usebackq" %%G IN ("%ProjectDir%\Files.x86.txt") DO copy "%Outdir%\%%G" "%target%" >nul
) else if "%Platform%" == "x64" ( 
  FOR /F "skip=1 tokens=* usebackq" %%G IN ("%ProjectDir%\Files.x64.txt") DO copy "%Outdir%\%%G" "%target%" >nul
) else if "%Platform%" == "AnyCPU" (
  if "%PROCESSOR_ARCHITECTURE%" == "x64" (
  FOR /F "skip=1 tokens=* usebackq" %%G IN ("%ProjectDir%\Files.x64.txt") DO copy "%Outdir%\%%G" "%target%" >nul
  ) else if "%PROCESSOR_ARCHITECTURE%" == "x86" (
  FOR /F "skip=1 tokens=* usebackq" %%G IN ("%ProjectDir%\Files.x86.txt") DO copy "%Outdir%\%%G" "%target%" >nul
  ) else (
    echo "ERROR: unknown architecture: "%PROCESSOR_ARCHITECTURE%"
  ) 
) else (
  echo "ERROR: unknown platform: %Platform%"
)

echo "CustomPostBuild done"