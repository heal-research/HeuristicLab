copy "%SolutionDir%\HeuristicLab.PluginInfrastructure.GUI\ICSharpCode.SharpZipLib License.txt" .\

rmdir plugins /s /q 
mkdir plugins
mkdir plugins\cache
mkdir plugins\temp
mkdir plugins\backup

copy "%SolutionDir%\HeuristicLab.AdvancedOptimizationFrontend\%Outdir%\HeuristicLab.AdvancedOptimizationFrontend-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.AdvancedOptimizationFrontend\WeifenLuo.WinFormsUI.Docking.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.AdvancedOptimizationFrontend\WeifenLuo.WinFormsUI.Docking License.txt" .\plugins
copy "%SolutionDir%\HeuristicLab.BitVector\%Outdir%\HeuristicLab.BitVector-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.CEDMA.Charting\%Outdir%\HeuristicLab.CEDMA.Charting-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.CEDMA.Core\%Outdir%\HeuristicLab.CEDMA.Core-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.CEDMA.DB\%Outdir%\HeuristicLab.CEDMA.DB-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.CEDMA.DB\%Outdir%\SemWeb.SqliteStore.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.CEDMA.DB\%Outdir%\SemWeb.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.CEDMA.DB.Interfaces\%Outdir%\HeuristicLab.CEDMA.DB.Interfaces-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.CEDMA.Operators\%Outdir%\HeuristicLab.CEDMA.Operators-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.CEDMA.Server\%Outdir%\HeuristicLab.CEDMA.Server-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Charting\%Outdir%\HeuristicLab.Charting-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Charting.Data\%Outdir%\HeuristicLab.Charting.Data-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Communication.Data\%Outdir%\HeuristicLab.Communication.Data-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Communication.Operators\%Outdir%\HeuristicLab.Communication.Operators-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Core\%Outdir%\HeuristicLab.Core-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Constraints\%Outdir%\HeuristicLab.Constraints-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Data\%Outdir%\HeuristicLab.Data-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.DataAnalysis\%Outdir%\HeuristicLab.DataAnalysis-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.DistributedEngine\%Outdir%\HeuristicLab.DistributedEngine-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Evolutionary\%Outdir%\HeuristicLab.Evolutionary-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.GP\%Outdir%\HeuristicLab.GP-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.GP.Boolean\%Outdir%\HeuristicLab.GP.Boolean-1.0.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.GP.SantaFe\%Outdir%\HeuristicLab.GP.SantaFe-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.GP.StructureIdentification\%Outdir%\HeuristicLab.GP.StructureIdentification-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.GP.StructureIdentification.Classification\%Outdir%\HeuristicLab.GP.StructureIdentification.Classification-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.GP.StructureIdentification.TimeSeries\%Outdir%\HeuristicLab.GP.StructureIdentification.TimeSeries-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Grid\%Outdir%\HeuristicLab.Grid-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Logging\%Outdir%\HeuristicLab.Logging-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Operators\%Outdir%\HeuristicLab.Operators-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Operators.Metaprogramming\%Outdir%\HeuristicLab.Operators.Metaprogramming-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Operators.Programmable\%Outdir%\HeuristicLab.Operators.Programmable-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Operators.Stopwatch\%Outdir%\HeuristicLab.Operators.Stopwatch-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.OptimizationFrontend\%Outdir%\HeuristicLab.OptimizationFrontend-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Permutation\%Outdir%\HeuristicLab.Permutation-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Random\%Outdir%\HeuristicLab.Random-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.RealVector\%Outdir%\HeuristicLab.RealVector-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Routing.TSP\%Outdir%\HeuristicLab.Routing.TSP-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Scheduling.JSSP\%Outdir%\HeuristicLab.Scheduling.JSSP-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Selection\%Outdir%\HeuristicLab.Selection-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Selection.OffspringSelection\%Outdir%\HeuristicLab.Selection.OffspringSelection-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.SequentialEngine\%Outdir%\HeuristicLab.SequentialEngine-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.SGA\%Outdir%\HeuristicLab.SGA-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.SimOpt\%Outdir%\HeuristicLab.SimOpt-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.SQLite\%Outdir%\HeuristicLab.SQLite-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.SQLite\System.Data.SQLite.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.SQLite\SQLite License.txt" .\plugins
copy "%SolutionDir%\HeuristicLab.SQLite\SQLite.NET.chm" .\plugins
copy "%SolutionDir%\HeuristicLab.TestFunctions\%Outdir%\HeuristicLab.TestFunctions-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.ThreadParallelEngine\%Outdir%\HeuristicLab.ThreadParallelEngine-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.ES\%Outdir%\HeuristicLab.ES-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Visualization\%Outdir%\HeuristicLab.Visualization-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Visualization.Test\%Outdir%\HeuristicLab.Visualization.Test-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Server\%Outdir%\HeuristicLab.Hive.Server-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Server.ADODataAccess\%Outdir%\HeuristicLab.Hive.Server.ADODataAccess-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Server.Core\%Outdir%\HeuristicLab.Hive.Server.Core-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Server.Core\%Outdir%\HeuristicLab.Hive.Contracts-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Server.Scheduler\%Outdir%\HeuristicLab.Hive.Server.Scheduler-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Client.Core\%Outdir%\HeuristicLab.Hive.Client.Core-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Client.Common\%Outdir%\HeuristicLab.Hive.Client.Common-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Client.Console\%Outdir%\HeuristicLab.Hive.Client.Console-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Client.Console\%Outdir%\ZedGraph.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Client.ExecutionEngine\%Outdir%\HeuristicLab.Hive.Client.ExecutionEngine-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Client.Communication\%Outdir%\HeuristicLab.Hive.Client.Communication-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Client.ExecutionEngine\%Outdir%\HeuristicLab.Hive.Client.ExecutionEngine-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Server.Console\%Outdir%\HeuristicLab.Hive.Server.Console-3.2.dll" .\plugins