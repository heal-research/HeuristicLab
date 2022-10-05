for /f %%i in ('git rev-list --count HEAD') do set RESULT=%%i

IF EXIST "%ProjectDir%\Properties\AssemblyInfo.cs.frame" (	
	IF EXIST "%ProjectDir%\Properties\AssemblyInfo.cs" DEL /F "%ProjectDir%\Properties\AssemblyInfo.cs"	
	
	for /f "delims=" %%A in ('type "%ProjectDir%\Properties\AssemblyInfo.cs.frame"') do (
		
	    set "string=%%A"
		setlocal EnableExtensions EnableDelayedExpansion
	    set "modified=!string:$WCREV$=%RESULT%!"
		echo !modified!>>"%ProjectDir%\Properties\AssemblyInfo.cs"
		endlocal	   
	)
	
)
IF %ERRORLEVEL% NEQ 0 GOTO Error_Handling

IF EXIST "%ProjectDir%\Plugin.cs.frame" (
	IF EXIST "%ProjectDir%\Plugin.cs" DEL /F "%ProjectDir%\Plugin.cs"	

	for /f "delims=" %%A in ('type "%ProjectDir%\Plugin.cs.frame"') do (
		
	    set "string=%%A"
	    setlocal EnableExtensions EnableDelayedExpansion		
		set "modified=!string:$WCREV$=%RESULT%!"		
	    echo !modified!>>"%ProjectDir%\Plugin.cs"
		endlocal
	)
		
)
IF %ERRORLEVEL% NEQ 0 GOTO Error_Handling
GOTO Done

:Error_Handling
ECHO There was an error while running subwcrev. Please verify that the *.cs.frame files have been correctly converted to *.cs files, otherwise HeuristicLab won't build. 
exit 0

:Done