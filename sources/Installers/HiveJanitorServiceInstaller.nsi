/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
 
; NSIS installer script for HeuristicLab Hive Janitor Service
; NSIS version: 3.0b0

Name "HeuristicLab Hive Janitor Service"
OutFile "HeuristicLab Hive Janitor Service Installer.exe"

; Build configuration is either Debug or Release
!define BUILDCONFIGURATION "Debug"
!define JANITORBUILDPATH "..\HeuristicLab.Services.Hive.JanitorService\3.3\bin\${BUILDCONFIGURATION}"
!define VERSION "3.3.14"

InstallDir $PROGRAMFILES\HeuristicLabHiveJanitorService
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
	File "${JANITORBUILDPATH}\HeuristicLab.Services.Access.DataAccess.dll"
	File "${JANITORBUILDPATH}\HeuristicLab.Services.Hive.DataAccess-3.3.dll"
	File "${JANITORBUILDPATH}\HeuristicLab.Services.Hive.JanitorService-3.3.exe"
	File "${JANITORBUILDPATH}\HeuristicLab.Services.Hive.JanitorService-3.3.exe.config"
	File "${JANITORBUILDPATH}\HeuristicLab.Services.Hive-3.3.dll"
	File "${JANITORBUILDPATH}\HeuristicLab.Tracing-3.3.dll"
	
	WriteRegStr HKLM SOFTWARE\HeuristicLabHiveJanitorService "Install_Dir" "$INSTDIR"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HeuristicLabHiveJanitorService" "DisplayName" "HeuristicLabHiveJanitorService"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HeuristicLabHiveJanitorService" "UninstallString" '"$INSTDIR\uninstall.exe"'
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HeuristicLabHiveJanitorService" "NoModify" 1
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HeuristicLabHiveJanitorService" "NoRepair" 1
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\HeuristicLabHiveJanitorService" "DisplayVersion" "${VERSION}"
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
	Delete "$INSTDIR\uninstall.exe"

	RMDir "$INSTDIR"
SectionEnd


