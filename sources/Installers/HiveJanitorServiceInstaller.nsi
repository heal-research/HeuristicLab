; NSIS installer script for HeuristicLab Hive Janitor Service
; NSIS version: v3.0b0

Name "HeuristicLab Hive Janitor Service"
OutFile "HeuristicLab Hive Janitor Service Installer.exe"

; Build configuration is either Debug or Release
!define BUILDCONFIGURATION "Debug"
!define JANITORBUILDPATH "..\HeuristicLab.Services.Hive.JanitorService\3.3\bin\${BUILDCONFIGURATION}"

InstallDir $PROGRAMFILES\HeuristicLabHiveJanitorService
InstallDirRegKey HKLM "Software\HeuristicLabHiveJanitorService" "Install_Dir"

RequestExecutionLevel admin

Page license
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

LicenseData "..\HeuristicLab\3.3\GNU General Public License.txt"
Icon "..\HeuristicLab\3.3\HeuristicLab.ico"


Section "HeuristicLabHiveJanitorService (required)"
	SetOutPath $INSTDIR

	File "${JANITORBUILDPATH}\GeoIP.dat"
	File "${JANITORBUILDPATH}\HeuristicLab.Common-3.3.dll"
	File "${JANITORBUILDPATH}\HeuristicLab.Persistence-3.3.dll"
	File "${JANITORBUILDPATH}\HeuristicLab.PluginInfrastructure-3.3.dll"
	File "${JANITORBUILDPATH}\HeuristicLab.Services.Access.dll"
	File "${JANITORBUILDPATH}\HeuristicLab.Services.Hive.DataAccess-3.3.dll"
	File "${JANITORBUILDPATH}\HeuristicLab.Services.Hive.JanitorService-3.3.exe"
	File "${JANITORBUILDPATH}\HeuristicLab.Services.Hive.JanitorService-3.3.exe.config"
	File "${JANITORBUILDPATH}\HeuristicLab.Services.Hive-3.3.dll"
	File "${JANITORBUILDPATH}\HeuristicLab.Tracing-3.3.dll"
	File "${JANITORBUILDPATH}\ICSharpCode.SharpZipLib License.txt"
	File "${JANITORBUILDPATH}\ICSharpCode.SharpZipLib.dll"

	WriteRegStr HKLM SOFTWARE\HeuristicLabHiveJanitorService "Install_Dir" "$INSTDIR"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HeuristicLabHiveJanitorService" "DisplayName" "HeuristicLabHiveJanitorService"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HeuristicLabHiveJanitorService" "UninstallString" '"$INSTDIR\uninstall.exe"'
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HeuristicLabHiveJanitorService" "NoModify" 1
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HeuristicLabHiveJanitorService" "NoRepair" 1
	WriteUninstaller "uninstall.exe"

	nsExec::ExecToLog '"$INSTDIR\HeuristicLab.Services.Hive.JanitorService-3.3.exe" --install'

SectionEnd


Section "un.Uninstall"  
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HeuristicLabHiveJanitorService"
	DeleteRegKey HKLM SOFTWARE\HeuristicLabHiveJanitorService
	  
	nsExec::ExecToLog '"$INSTDIR\HeuristicLab.Services.Hive.JanitorService-3.3.exe" --uninstall'
	  
	Delete "$INSTDIR\GeoIP.dat"
	Delete "$INSTDIR\HeuristicLab.Common-3.3.dll"
	Delete "$INSTDIR\HeuristicLab.Persistence-3.3.dll"
	Delete "$INSTDIR\HeuristicLab.PluginInfrastructure-3.3.dll"
	Delete "$INSTDIR\HeuristicLab.Services.Access.dll"
	Delete "$INSTDIR\HeuristicLab.Services.Hive.DataAccess-3.3.dll"
	Delete "$INSTDIR\HeuristicLab.Services.Hive.JanitorService-3.3.exe"
	Delete "$INSTDIR\HeuristicLab.Services.Hive.JanitorService-3.3.exe.config"
	Delete "$INSTDIR\HeuristicLab.Services.Hive-3.3.dll"
	Delete "$INSTDIR\HeuristicLab.Tracing-3.3.dll"
	Delete "$INSTDIR\ICSharpCode.SharpZipLib License.txt"
	Delete "$INSTDIR\ICSharpCode.SharpZipLib.dll"    
	Delete "$INSTDIR\uninstall.exe"

	RMDir "$INSTDIR"

SectionEnd


