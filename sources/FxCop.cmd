@ECHO OFF

SET Outdir=bin
SET FXCOPCMD=

ECHO. > FxCopResults.txt

IF "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
  SET FXCOPCMD="C:\Program Files (x86)\Microsoft Visual Studio 10.0\Team Tools\Static Analysis Tools\FxCop\FxCopCmd.exe"
) ELSE (
  IF "%PROCESSOR_ARCHITECTURE%"=="x86" (
    REM If the PROCESSOR_ARCHITECTURE indicates x86, the OS need not necessarily by 32bit.
    REM For WOW64 processes, a special environment variable will indicate the real architecture.
    IF "%PROCESSOR_ARCHITEW6432%"=="AMD64" (
      SET FXCOPCMD="C:\Program Files (x86)\Microsoft Visual Studio 10.0\Team Tools\Static Analysis Tools\FxCop\FxCopCmd.exe"
    ) ELSE (
      SET FXCOPCMD="C:\Program Files\Microsoft Visual Studio 10.0\Team Tools\Static Analysis Tools\FxCop\FxCopCmd.exe"
    )
  ) ELSE (
    ECHO Unknown architecture: %PROCESSOR_ARCHITECTURE%
    GOTO :end
  )
)

IF NOT EXIST %FXCOPCMD% (
  ECHO FxCopCmd.exe could not be found.
  GOTO :end
)

FOR /F "tokens=*" %%G IN ('DIR /B %Outdir%\HeuristicLab.*.dll') DO (
  ECHO Performing Code Analysis on %Outdir%\%%G
  %FXCOPCMD% /file:%Outdir%\%%G /rule:+HeuristicLab.FxCop.dll /directory:%Outdir% /console /quiet >> FxCopResults.txt
)

:end

PAUSE