copy HeuristicLab.exe HeuristicLab.Console.exe
@call "c:\Program Files\Microsoft Visual Studio 9.0\VC\vcvarsall.bat" x86
@editbin.exe /subsystem:console HeuristicLab.Console.exe

echo "Done creating console application"
