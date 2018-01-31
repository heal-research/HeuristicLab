IF EXIST "%ProjectDir%\Properties\AssemblyInfo.cs.frame" SubWCRev "%ProjectDir%\" "%ProjectDir%\Properties\AssemblyInfo.cs.frame" "%ProjectDir%\Properties\AssemblyInfo.cs"
IF %ERRORLEVEL% NEQ 0 GOTO Error_Handling
IF EXIST "%ProjectDir%\Plugin.cs.frame" SubWCRev "%ProjectDir%\" "%ProjectDir%\Plugin.cs.frame" "%ProjectDir%\Plugin.cs"
IF %ERRORLEVEL% NEQ 0 GOTO Error_Handling
GOTO Done

:Error_Handling
ECHO There was an error while running subwcrev. Please verify that the *.cs.frame files have been correctly converted to *.cs files, otherwise HeuristicLab won't build. 
exit 0

:Done