@ECHO OFF

SET Outdir=bin
SET FXCOPCMD=C:\Program Files (x86)\Microsoft Visual Studio 10.0\Team Tools\Static Analysis Tools\FxCop\FxCopCmd.exe

ECHO. > FxCopResults.txt

FOR /F "tokens=*" %%G IN ('DIR /B %Outdir%\HeuristicLab.*.dll') DO (
  ECHO Performing Code Analysis on %Outdir%\%%G
  "%FXCOPCMD%" /file:%Outdir%\%%G /rule:+HeuristicLab.FxCop.dll /directory:%Outdir% /console /quiet >> FxCopResults.txt
)

PAUSE