@ECHO OFF

ECHO Clean starting...

FOR /F "tokens=*" %%G IN ('DIR /AD /B') DO (
  IF EXIST "%%G\bin" (
    ECHO Cleaning "bin" in %%G ...
    RMDIR /S /Q "%%G\bin"
  )
  IF EXIST "%%G\obj" (
    ECHO Cleaning "obj" in %%G ...
    RMDIR /S /Q "%%G\obj"
  )
  FOR /F "tokens=*" %%T IN ('DIR /AD /B %%G') DO (
    IF EXIST "%%G\%%T\bin" (
      ECHO Cleaning "bin" in %%G in version %%T ...
      RMDIR /S /Q "%%G\%%T\bin"
    )
    IF EXIST "%%G\%%T\obj" (
      ECHO Cleaning "obj" in %%G in version %%T ...
      RMDIR /S /Q "%%G\%%T\obj"
    )
  )
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