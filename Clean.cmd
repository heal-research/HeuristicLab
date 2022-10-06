@ECHO OFF

ECHO Clean starting...

FOR /F "tokens=*" %%G IN ('DIR /b /a:d /s bin 2^>nul') DO (
  ECHO Cleaning %%G 
  RMDIR /S /Q %%G
)

FOR /F "tokens=*" %%G IN ('DIR /b /a:d /s obj 2^>nul') DO (
  ECHO Cleaning %%G
  RMDIR /S /Q %%G
)

IF EXIST bin (
  ECHO Cleaning "bin" in current directory ...
  RMDIR /S /Q bin
)

IF EXIST TestResults (
  ECHO Cleaning "TestResults" in current directory ...
  RMDIR /S /Q TestResults
)

ECHO Clean finished.

PAUSE
PAUSE
