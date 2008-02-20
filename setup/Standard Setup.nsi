;NSIS Modern User Interface
;HeuristicLab Installation Script
;Written by Stefan Wagner

;--------------------------------
;Global

	!define PRODUCT "HeuristicLab"
	!define VERSION "3.0"
	!define CONFIGURATION "Release"
	!define ALL_USERS

;--------------------------------
;Include Modern UI

	!include "MUI2.nsh"

;--------------------------------
;General

	;Name and file
	Name "${PRODUCT} ${VERSION}"
	OutFile "${PRODUCT} ${VERSION} Setup.exe"

	;Default installation folder
	InstallDir "$PROGRAMFILES\${PRODUCT} ${VERSION}"

	;Get installation folder from registry if available
	InstallDirRegKey HKLM "Software\${PRODUCT} ${VERSION}" ""

	;Request application privileges for Windows Vista
	RequestExecutionLevel user

;--------------------------------
;Interface Settings

	!define MUI_ABORTWARNING

;--------------------------------
;Pages

	!insertmacro MUI_PAGE_WELCOME
	!insertmacro MUI_PAGE_LICENSE "Files\HeuristicLab License.txt"
	!insertmacro MUI_PAGE_COMPONENTS
	!insertmacro MUI_PAGE_DIRECTORY
	!insertmacro MUI_PAGE_INSTFILES
	!define MUI_FINISHPAGE_RUN "$INSTDIR\HeuristicLab.exe"
	!define MUI_FINISHPAGE_RUN_NOTCHECKED
	!define MUI_FINISHPAGE_LINK "http://www.heuristiclab.com"
	!define MUI_FINISHPAGE_LINK_LOCATION "http://www.heuristiclab.com"
	!insertmacro MUI_PAGE_FINISH

	!insertmacro MUI_UNPAGE_WELCOME
	!insertmacro MUI_UNPAGE_CONFIRM
	!insertmacro MUI_UNPAGE_INSTFILES
	!insertmacro MUI_UNPAGE_FINISH


;--------------------------------
;Languages

	!insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Installer Sections

Section "HeuristicLab (required)" SecHeuristicLab
	SectionIn RO

	;Get personal data folder
	ReadRegStr $9 HKCU "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders" Personal

	;Install files
	SetOutPath "$INSTDIR"
	CreateDirectory "$INSTDIR\plugins"
	CreateDirectory "$INSTDIR\plugins\backup"
	CreateDirectory "$INSTDIR\plugins\cache"
	CreateDirectory "$INSTDIR\plugins\temp"

	File "..\sources\HeuristicLab.PluginInfrastructure\bin\${CONFIGURATION}\HeuristicLab.PluginInfrastructure.dll"
	File "..\sources\HeuristicLab.PluginInfrastructure.GUI\bin\${CONFIGURATION}\HeuristicLab.PluginInfrastructure.GUI.dll"
	File "..\sources\HeuristicLab.PluginInfrastructure.GUI\ICSharpCode.SharpZipLib.dll"
	File "..\sources\HeuristicLab.PluginInfrastructure.GUI\ICSharpCode.SharpZipLib License.txt"
	File "..\sources\HeuristicLab\bin\${CONFIGURATION}\HeuristicLab.exe"
	File "..\sources\HeuristicLab\bin\${CONFIGURATION}\HeuristicLab.exe.config"
	File "Files\HeuristicLab.ico"
	File /a "Files\HeuristicLab License.txt"
	File "..\documentation\API Documentation\HeuristicLab API Documentation.chm"

	;Add install folder
	WriteRegStr HKLM "Software\${PRODUCT} ${VERSION}" "" $INSTDIR

	;Add uninstall information to "Add/Remove Programs"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT} ${VERSION}" "UninstallString" "$INSTDIR\Uninstall HeuristicLab.exe"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT} ${VERSION}" "InstallLocation" "$INSTDIR"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT} ${VERSION}" "DisplayName" "${PRODUCT} ${VERSION}"
	;WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT} ${VERSION}" "DisplayVersion" "${VERSION}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT} ${VERSION}" "DisplayIcon" "$INSTDIR\HeuristicLab.exe"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT} ${VERSION}" "Publisher" "HEAL"
	;WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT} ${VERSION}" "Contact" "HEAL"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT} ${VERSION}" "URLInfoAbout" "http://www.heuristiclab.com"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT} ${VERSION}" "HelpLink" "http://www.heuristiclab.com"
	;WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT} ${VERSION}" "Readme" "$INSTDIR\ReadMe.txt"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT} ${VERSION}" "Comments" "A paradigm-independent and extensible environment for heuristic optimization."
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT} ${VERSION}" "NoModify" 1
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT} ${VERSION}" "NoRepair" 1

	;Create uninstaller
	WriteUninstaller "$INSTDIR\Uninstall HeuristicLab.exe"
SectionEnd

SubSection "File Types" SubSecFileTypes
	Section "HeuristicLab Files (.hl)" SecHeuristicLabFiles
		;Backup old extension association if any exists
		ReadRegStr $R0 HKCR ".hl" ""
		StrCmp $R0 "" skipBackup
			StrCmp $R0 "HeuristicLab.File" skipBackup
				WriteRegStr HKCR ".hl" "backup_value" $R0

		skipBackup:
		;Write extension association
		WriteRegStr HKCR ".hl" "" "HeuristicLab.File"

		;Delete file type if any exists
		ReadRegStr $R0 HKCR "HeuristicLab.File" ""
		StrCmp $R0 "" skipDelete
			DeleteRegKey HKCR "HeuristicLab.File"

		skipDelete:
		;Write file type
		WriteRegStr HKCR "HeuristicLab.File" "" "HeuristicLab File"
		WriteRegStr HKCR "HeuristicLab.File\DefaultIcon" "" $INSTDIR\HeuristicLab.ico
		WriteRegStr HKCR "HeuristicLab.File\shell" "" "open"
		WriteRegStr HKCR "HeuristicLab.File\shell\open\command" "" '$INSTDIR\HeuristicLab.exe "%1"'
	SectionEnd
SubSectionEnd

SubSection "Shortcuts" SubSecShortcuts
	Section "Create Start Menu Shortcuts" SecStartMenuShortcuts
		CreateDirectory "$SMPROGRAMS\${PRODUCT} ${VERSION}"
		CreateShortCut "$SMPROGRAMS\${PRODUCT} ${VERSION}\HeuristicLab.lnk" "$INSTDIR\HeuristicLab.exe"
		CreateShortCut "$SMPROGRAMS\${PRODUCT} ${VERSION}\Uninstall HeuristicLab.lnk" "$INSTDIR\Uninstall HeuristicLab.exe"
		CreateShortCut "$SMPROGRAMS\${PRODUCT} ${VERSION}\HeuristicLab API Documentation.lnk" "$INSTDIR\HeuristicLab API Documentation.chm"
	SectionEnd

	Section "Create Desktop Shortcuts" SecDesktopShortcuts
		CreateShortCut "$DESKTOP\HeuristicLab ${VERSION}.lnk" "$INSTDIR\HeuristicLab.exe"
	SectionEnd

	Section "Create Quick Launch Shortcuts" SecQuickLaunchShortcuts
		StrCmp $QUICKLAUNCH $TEMP skip
			CreateShortCut "$QUICKLAUNCH\HeuristicLab ${VERSION}.lnk" "$INSTDIR\HeuristicLab.exe"
		skip:
	SectionEnd
SubSectionEnd

;--------------------------------
;Descriptions

	;Language strings
	LangString DESC_SecHeuristicLab ${LANG_ENGLISH} "Installs HeuristicLab and the core components."

	LangString DESC_SubSecFileTypes ${LANG_ENGLISH} "Registers HeuristicLab specific file types."
	LangString DESC_SecHeuristicLabFiles ${LANG_ENGLISH} "Associates HeuristicLab files (.hl) with HeuristicLab."

	LangString DESC_SubSecShortcuts ${LANG_ENGLISH} "Adds shortcuts to the start menu, the desktop, etc. for easy and quick access."
	LangString DESC_SecStartMenuShortcuts ${LANG_ENGLISH} "Adds shortcuts to the start menu of the current user."
	LangString DESC_SecDesktopShortcuts ${LANG_ENGLISH} "Adds shortcuts to the desktop of the current user."
	LangString DESC_SecQuickLaunchShortcuts ${LANG_ENGLISH} "Adds shortcuts to the quick launch bar of the current user."

	;Assign language strings to sections
	!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
		!insertmacro MUI_DESCRIPTION_TEXT ${SecHeuristicLab} $(DESC_SecHeuristicLab)
		!insertmacro MUI_DESCRIPTION_TEXT ${SubSecFileTypes} $(DESC_SubSecFileTypes)
		!insertmacro MUI_DESCRIPTION_TEXT ${SecHeuristicLabFiles} $(DESC_SecHeuristicLabFiles)
		!insertmacro MUI_DESCRIPTION_TEXT ${SubSecShortcuts} $(DESC_SubSecShortcuts)
		!insertmacro MUI_DESCRIPTION_TEXT ${SecStartMenuShortcuts} $(DESC_SecStartMenuShortcuts)
		!insertmacro MUI_DESCRIPTION_TEXT ${SecDesktopShortcuts} $(DESC_SecDesktopShortcuts)
		!insertmacro MUI_DESCRIPTION_TEXT ${SecQuickLaunchShortcuts} $(DESC_SecQuickLaunchShortcuts)
	!insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------
;Uninstaller Section

Section "Uninstall"
	;Delete HeuristicLab
	Delete "$INSTDIR\HeuristicLab.PluginInfrastructure.dll"
	Delete "$INSTDIR\HeuristicLab.PluginInfrastructure.GUI.dll"
	Delete "$INSTDIR\ICSharpCode.SharpZipLib.dll"
	Delete "$INSTDIR\ICSharpCode.SharpZipLib License.txt"
	Delete "$INSTDIR\HeuristicLab.exe"
	Delete "$INSTDIR\HeuristicLab.exe.config"
	Delete "$INSTDIR\Uninstall HeuristicLab.exe"
	Delete "$INSTDIR\HeuristicLab.ico"
	Delete "$INSTDIR\HeuristicLab License.txt"
	Delete "$INSTDIR\HeuristicLab API Documentation.chm"

	;Delete shortcuts
	Delete "$DESKTOP\HeuristicLab ${VERSION}.lnk"
	Delete "$SMPROGRAMS\${PRODUCT} ${VERSION}\HeuristicLab.lnk"
	Delete "$SMPROGRAMS\${PRODUCT} ${VERSION}\Uninstall HeuristicLab.lnk"
	Delete "$SMPROGRAMS\${PRODUCT} ${VERSION}\HeuristicLab API Documentation.lnk"
	RMDir "$SMPROGRAMS\${PRODUCT} ${VERSION}"
	StrCmp $QUICKLAUNCH $TEMP skip
		Delete "$QUICKLAUNCH\HeuristicLab ${VERSION}.lnk"
	skip:

	;Delete folders
	RMDir "$INSTDIR\plugins"
	RMDir /r "$INSTDIR"

	;Remove install folder
	DeleteRegKey /ifempty HKLM "Software\${PRODUCT} ${VERSION}"

	;Remove uninstall information from "Add/Remove Programs"
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT} ${VERSION}"

	;Remove extension associations
	;HeuristicLab File (hl)
	ReadRegStr $R0 HKCR ".hl" ""
	StrCmp $R0 "HeuristicLab.File" 0 hlFinished
		ReadRegStr $R0 HKCR ".hl" "backup_value"
		StrCmp $R0 "" 0 restoreHl
			DeleteRegKey HKCR ".hl"
			Goto hlFinished
		restoreHl:
			WriteRegStr HKCR ".hl" "" $R0
			DeleteRegValue HKCR ".hl" "backup_value"
	HlFinished:

	;Remove file types
	DeleteRegKey HKCR "HeuristicLab.File"
SectionEnd
