/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
 
; NSIS installer script for HeuristicLab Hive Slave
; NSIS version: 3.0b0

Name "HeuristicLab Hive Slave"
OutFile "HeuristicLab Hive Slave Installer.exe"

; Build configuration is either Debug or Release
!define BUILDCONFIGURATION "Debug"
!define SLAVEBUILDPATH "..\HeuristicLab.Clients.Hive.Slave.WindowsService\3.3\bin\${BUILDCONFIGURATION}"
!define VERSION "3.3.11"

InstallDir $PROGRAMFILES\HeuristicLabHiveSlave
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


	WriteRegStr HKLM SOFTWARE\HeuristicLabHiveSlave "Install_Dir" "$INSTDIR"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HeuristicLabHiveSlave" "DisplayName" "HeuristicLabHiveSlave"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HeuristicLabHiveSlave" "UninstallString" '"$INSTDIR\uninstall.exe"'
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HeuristicLabHiveSlave" "NoModify" 1
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HeuristicLabHiveSlave" "NoRepair" 1
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HeuristicLabHiveSlave" "DisplayVersion" "${VERSION}"
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
	Delete $INSTDIR\uninstall.exe

	RMDir "$INSTDIR"
SectionEnd


