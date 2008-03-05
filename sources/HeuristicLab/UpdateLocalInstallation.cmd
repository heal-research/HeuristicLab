set target=C:\Program Files\HeuristicLab 3.0

copy "HeuristicLab.exe" "%target%"
copy "HeuristicLab.exe.config" "%target%"
copy "HeuristicLab.PluginInfrastructure.dll" "%target%"
copy "HeuristicLab.PluginInfrastructure.GUI.dll" "%target%"
copy "ICSharpCode.SharpZipLib.dll" "%target%"
copy "ICSharpCode.SharpZipLib License.txt" "%target%"
copy "plugins\*.*" "%target%\plugins"
