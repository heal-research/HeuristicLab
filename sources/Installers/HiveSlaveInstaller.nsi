; NSIS installer script for HeuristicLab Hive Slave
; NSIS version: v3.0b0

Name "HeuristicLab Hive Slave"
OutFile "HeuristicLab Hive Slave Installer.exe"

; Build configuration is either Debug or Release
!define BUILDCONFIGURATION "Debug"
!define SLAVEBUILDPATH "..\HeuristicLab.Clients.Hive.Slave.WindowsService\3.3\bin\${BUILDCONFIGURATION}"

InstallDir $PROGRAMFILES\HeuristicLabHiveSlave
InstallDirRegKey HKLM "Software\HeuristicLabHiveSlave" "Install_Dir"

RequestExecutionLevel admin

Page license
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

LicenseData "..\HeuristicLab\3.3\GNU General Public License.txt"
Icon "..\HeuristicLab\3.3\HeuristicLab.ico"


Section "HeuristicLabHiveSlave (required)"
	SetOutPath $INSTDIR

	File "${SLAVEBUILDPATH}\HeuristicLab.Clients.Hive.Slave.WindowsService.exe"
	File "${SLAVEBUILDPATH}\HeuristicLab.Clients.Common-3.3.dll"
	File "${SLAVEBUILDPATH}\HeuristicLab.Clients.Hive.Slave.WindowsService.exe.config"
	File "${SLAVEBUILDPATH}\HeuristicLab.Clients.Hive.SlaveCore-3.3.dll"
	File "${SLAVEBUILDPATH}\HeuristicLab.Clients.Hive-3.3.dll"
	File "${SLAVEBUILDPATH}\HeuristicLab.Collections-3.3.dll"
	File "${SLAVEBUILDPATH}\HeuristicLab.Common.Resources-3.3.dll"
	File "${SLAVEBUILDPATH}\HeuristicLab.Common-3.3.dll"
	File "${SLAVEBUILDPATH}\HeuristicLab.Core-3.3.dll"
	File "${SLAVEBUILDPATH}\HeuristicLab.Data-3.3.dll"
	File "${SLAVEBUILDPATH}\HeuristicLab.Hive-3.3.dll"
	File "${SLAVEBUILDPATH}\HeuristicLab.MainForm-3.3.dll"
	File "${SLAVEBUILDPATH}\HeuristicLab.Optimization-3.3.dll"
	File "${SLAVEBUILDPATH}\HeuristicLab.Parameters-3.3.dll"
	File "${SLAVEBUILDPATH}\HeuristicLab.Persistence-3.3.dll"
	File "${SLAVEBUILDPATH}\HeuristicLab.PluginInfrastructure-3.3.dll"
	File "${SLAVEBUILDPATH}\HeuristicLab.Tracing-3.3.dll"
	File "${SLAVEBUILDPATH}\ICSharpCode.SharpZipLib License.txt"
	File "${SLAVEBUILDPATH}\ICSharpCode.SharpZipLib.dll"


	WriteRegStr HKLM SOFTWARE\HeuristicLabHiveSlave "Install_Dir" "$INSTDIR"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HeuristicLabHiveSlave" "DisplayName" "HeuristicLabHiveSlave"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HeuristicLabHiveSlave" "UninstallString" '"$INSTDIR\uninstall.exe"'
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HeuristicLabHiveSlave" "NoModify" 1
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HeuristicLabHiveSlave" "NoRepair" 1
	WriteUninstaller "uninstall.exe"

	nsExec::ExecToLog '"$INSTDIR\HeuristicLab.Clients.Hive.Slave.WindowsService.exe" --install'

SectionEnd


Section "un.Uninstall"  
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HeuristicLabHiveSlave"
	DeleteRegKey HKLM SOFTWARE\HeuristicLabHiveSlave

	nsExec::ExecToLog '"$INSTDIR\HeuristicLab.Clients.Hive.Slave.WindowsService.exe" --uninstall'
	  
	Delete $INSTDIR\HeuristicLab.Clients.Hive.Slave.WindowsService.exe
	Delete $INSTDIR\HeuristicLab.Clients.Common-3.3.dll
	Delete $INSTDIR\HeuristicLab.Clients.Hive.Slave.WindowsService.exe.config
	Delete $INSTDIR\HeuristicLab.Clients.Hive.SlaveCore-3.3.dll
	Delete $INSTDIR\HeuristicLab.Clients.Hive-3.3.dll
	Delete $INSTDIR\HeuristicLab.Collections-3.3.dll
	Delete $INSTDIR\HeuristicLab.Common.Resources-3.3.dll
	Delete $INSTDIR\HeuristicLab.Common-3.3.dll
	Delete $INSTDIR\HeuristicLab.Core-3.3.dll
	Delete $INSTDIR\HeuristicLab.Data-3.3.dll
	Delete $INSTDIR\HeuristicLab.Hive-3.3.dll
	Delete $INSTDIR\HeuristicLab.MainForm-3.3.dll
	Delete $INSTDIR\HeuristicLab.Optimization-3.3.dll
	Delete $INSTDIR\HeuristicLab.Parameters-3.3.dll
	Delete $INSTDIR\HeuristicLab.Persistence-3.3.dll
	Delete $INSTDIR\HeuristicLab.PluginInfrastructure-3.3.dll
	Delete $INSTDIR\HeuristicLab.Tracing-3.3.dll
	Delete "$INSTDIR\ICSharpCode.SharpZipLib License.txt"
	Delete $INSTDIR\ICSharpCode.SharpZipLib.dll
	Delete $INSTDIR\uninstall.exe

	RMDir "$INSTDIR"

SectionEnd


