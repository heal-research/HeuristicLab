copy HeuristicLab.exe HeuristicLab.Console.exe

echo "Platform: %Platform%, architecture: %PROCESSOR_ARCHITECTURE%"
if "%Platform%" == "x86" (   
 @call "c:\Program Files\Microsoft Visual Studio 9.0\VC\vcvarsall.bat" x86
 @call "c:\Program Files\Microsoft Visual Studio 9.0\VC\bin\editbin.exe" /subsystem:console HeuristicLab.Console.exe
) else if "%Platform%" == "x64" ( 
  @call "c:\Program Files (x86)\Microsoft Visual Studio 9.0\VC\vcvarsall.bat" x64
  @call "c:\Program Files (x86)\Microsoft Visual Studio 9.0\VC\bin\editbin.exe" /subsystem:console HeuristicLab.Console.exe
) else if "%Platform%" == "AnyCPU" (
 if "%PROCESSOR_ARCHITECTURE%" == "x64" (
   @call "c:\Program Files (x86)\Microsoft Visual Studio 9.0\VC\vcvarsall.bat" x64
   @call "c:\Program Files (x86)\Microsoft Visual Studio 9.0\VC\bin\editbin.exe" /subsystem:console HeuristicLab.Console.exe
  ) else if "%PROCESSOR_ARCHITECTURE%" == "x86" (
   @call "c:\Program Files\Microsoft Visual Studio 9.0\VC\vcvarsall.bat" x86
   @call "c:\Program Files\Microsoft Visual Studio 9.0\VC\bin\editbin.exe" /subsystem:console HeuristicLab.Console.exe
  ) else (
      echo "ERROR: unknown architecture: "%PROCESSOR_ARCHITECTURE%"
  ) 
) else (
  echo "ERROR: unknown platform: %Platform%"
)

echo "Done creating console application"
