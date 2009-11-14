copy "%SolutionDir%\HeuristicLab.PluginInfrastructure.GUI\ICSharpCode.SharpZipLib License.txt" .\

rmdir plugins /s /q
mkdir plugins
mkdir plugins\cache
mkdir plugins\temp
mkdir plugins\backup

copy "%SolutionDir%\HeuristicLab.AdvancedOptimizationFrontend\3.3\%Outdir%\HeuristicLab.AdvancedOptimizationFrontend-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Charting\3.3\%Outdir%\HeuristicLab.Charting-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Charting.Data\3.3\%Outdir%\HeuristicLab.Charting.Data-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Common\3.2\%Outdir%\HeuristicLab.Common-3.2.dll" .\plugins 
copy "%SolutionDir%\HeuristicLab.Common.Resources\3.2\%Outdir%\HeuristicLab.Common.Resources-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Core\3.3\%Outdir%\HeuristicLab.Core-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Constraints\3.3\%Outdir%\HeuristicLab.Constraints-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Data\3.3\%Outdir%\HeuristicLab.Data-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.DataAnalysis\3.3\%Outdir%\HeuristicLab.DataAnalysis-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Evolutionary\3.3\%Outdir%\HeuristicLab.Evolutionary-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.GP\3.4\%Outdir%\HeuristicLab.GP-3.4.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.GP.StructureIdentification\3.4\%Outdir%\HeuristicLab.GP.StructureIdentification-3.4.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Logging\3.3\%Outdir%\HeuristicLab.Logging-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.MainForm\3.2\%Outdir%\HeuristicLab.MainForm-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.MainForm.Test\3.2\%Outdir%\HeuristicLab.MainForm.Test-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.MainForm.WindowsForms\3.2\%Outdir%\HeuristicLab.MainForm.WindowsForms-3.2.dll" .\plugins
copy "%SolutionDir%\WinFormsUI\%Outdir%\WeifenLuo.WinFormsUI.Docking.dll" .\plugins
copy "%SolutionDir%\WinFormsUI\%Outdir%\WeifenLuo.WinFormsUI.Docking License.txt" .\plugins
copy "%SolutionDir%\HeuristicLab.Modeling\3.3\%Outdir%\HeuristicLab.Modeling-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Operators\3.3\%Outdir%\HeuristicLab.Operators-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Operators.Programmable\3.3\%Outdir%\HeuristicLab.Operators.Programmable-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.OptimizationFrontend\3.3\%Outdir%\HeuristicLab.OptimizationFrontend-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Permutation\3.3\%Outdir%\HeuristicLab.Permutation-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Persistence\3.3\%Outdir%\HeuristicLab.Persistence-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Persistence.GUI\3.3\%Outdir%\HeuristicLab.Persistence.GUI-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Random\3.3\%Outdir%\HeuristicLab.Random-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Routing.TSP\3.3\%Outdir%\HeuristicLab.Routing.TSP-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Selection\3.3\%Outdir%\HeuristicLab.Selection-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Selection.OffspringSelection\3.3\%Outdir%\HeuristicLab.Selection.OffspringSelection-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.SequentialEngine\3.3\%Outdir%\HeuristicLab.SequentialEngine-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.SGA\3.3\%Outdir%\HeuristicLab.SGA-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.TestFunctions\3.3\%Outdir%\HeuristicLab.TestFunctions-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.ThreadParallelEngine\3.3\%Outdir%\HeuristicLab.ThreadParallelEngine-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Tracing\3.2\%Outdir%\HeuristicLab.Tracing-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Tracing\3.2\%Outdir%\log4net.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Tracing\3.2\log4net licence.txt" .\plugins
copy "%SolutionDir%\HeuristicLab.Tracing\3.2\HeuristicLab.log4net.xml" .\plugins
