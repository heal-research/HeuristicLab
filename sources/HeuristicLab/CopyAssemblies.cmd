copy %1\HeuristicLab.PluginInfrastructure.GUI\ICSharpCode.SharpZipLib License.txt .\

rmdir plugins /s /q 
mkdir plugins
mkdir plugins\cache
mkdir plugins\temp
mkdir plugins\backup
copy %1\HeuristicLab.AdvancedOptimizationFrontend\%2\HeuristicLab.AdvancedOptimizationFrontend-3.2.dll .\plugins
copy %1\HeuristicLab.AdvancedOptimizationFrontend\WeifenLuo.WinFormsUI.Docking.dll .\plugins
copy %1\HeuristicLab.AdvancedOptimizationFrontend\"WeifenLuo.WinFormsUI.Docking License.txt" .\plugins
copy %1\HeuristicLab.BitVector\%2\HeuristicLab.BitVector-3.2.dll .\plugins
copy %1\HeuristicLab.CEDMA.Charting\%2\HeuristicLab.CEDMA.Charting-3.2.dll .\plugins
copy %1\HeuristicLab.CEDMA.Core\%2\HeuristicLab.CEDMA.Core-3.2.dll .\plugins
copy %1\HeuristicLab.CEDMA.DB\%2\HeuristicLab.CEDMA.DB-3.2.dll .\plugins
copy %1\HeuristicLab.CEDMA.DB\%2\SemWeb.SqliteStore.dll .\plugins
copy %1\HeuristicLab.CEDMA.DB\%2\SemWeb.dll .\plugins
copy %1\HeuristicLab.CEDMA.DB.Interfaces\%2\HeuristicLab.CEDMA.DB.Interfaces-3.2.dll .\plugins
copy %1\HeuristicLab.CEDMA.Operators\%2\HeuristicLab.CEDMA.Operators-3.2.dll .\plugins
copy %1\HeuristicLab.CEDMA.Server\%2\HeuristicLab.CEDMA.Server-3.2.dll .\plugins
copy %1\HeuristicLab.Charting\%2\HeuristicLab.Charting-3.2.dll .\plugins
copy %1\HeuristicLab.Charting.Data\%2\HeuristicLab.Charting.Data-3.2.dll .\plugins
copy %1\HeuristicLab.Communication.Data\%2\HeuristicLab.Communication.Data-3.2.dll .\plugins
copy %1\HeuristicLab.Communication.Operators\%2\HeuristicLab.Communication.Operators-3.2.dll .\plugins
copy %1\HeuristicLab.Core\%2\HeuristicLab.Core-3.2.dll .\plugins
copy %1\HeuristicLab.Constraints\%2\HeuristicLab.Constraints-3.2.dll .\plugins
copy %1\HeuristicLab.Data\%2\HeuristicLab.Data-3.2.dll .\plugins
copy %1\HeuristicLab.DataAnalysis\%2\HeuristicLab.DataAnalysis-3.2.dll .\plugins
copy %1\HeuristicLab.DistributedEngine\%2\HeuristicLab.DistributedEngine-3.2.dll .\plugins
copy %1\HeuristicLab.Evolutionary\%2\HeuristicLab.Evolutionary-3.2.dll .\plugins
copy %1\HeuristicLab.GP\%2\HeuristicLab.GP-3.2.dll .\plugins
copy %1\HeuristicLab.GP.Boolean\%2\HeuristicLab.GP.Boolean-1.0.dll .\plugins
copy %1\HeuristicLab.GP.SantaFe\%2\HeuristicLab.GP.SantaFe-3.2.dll .\plugins
copy %1\HeuristicLab.GP.StructureIdentification\%2\HeuristicLab.GP.StructureIdentification-3.2.dll .\plugins
copy %1\HeuristicLab.GP.StructureIdentification.Classification\%2\HeuristicLab.GP.StructureIdentification.Classification-3.2.dll .\plugins
copy %1\HeuristicLab.GP.StructureIdentification.TimeSeries\%2\HeuristicLab.GP.StructureIdentification.TimeSeries-3.2.dll .\plugins
copy %1\HeuristicLab.Grid\%2\HeuristicLab.Grid-3.2.dll .\plugins
copy %1\HeuristicLab.Logging\%2\HeuristicLab.Logging-3.2.dll .\plugins
copy %1\HeuristicLab.Operators\%2\HeuristicLab.Operators-3.2.dll .\plugins
copy %1\HeuristicLab.Operators.Metaprogramming\%2\HeuristicLab.Operators.Metaprogramming-3.2.dll .\plugins
copy %1\HeuristicLab.Operators.Programmable\%2\HeuristicLab.Operators.Programmable-3.2.dll .\plugins
copy %1\HeuristicLab.Operators.Stopwatch\%2\HeuristicLab.Operators.Stopwatch-3.2.dll .\plugins
copy %1\HeuristicLab.OptimizationFrontend\%2\HeuristicLab.OptimizationFrontend-3.2.dll .\plugins
copy %1\HeuristicLab.Permutation\%2\HeuristicLab.Permutation-3.2.dll .\plugins
copy %1\HeuristicLab.Random\%2\HeuristicLab.Random-3.2.dll .\plugins
copy %1\HeuristicLab.RealVector\%2\HeuristicLab.RealVector-3.2.dll .\plugins
copy %1\HeuristicLab.Routing.TSP\%2\HeuristicLab.Routing.TSP-3.2.dll .\plugins
copy %1\HeuristicLab.Scheduling.JSSP\%2\HeuristicLab.Scheduling.JSSP-3.2.dll .\plugins
copy %1\HeuristicLab.Selection\%2\HeuristicLab.Selection-3.2.dll .\plugins
copy %1\HeuristicLab.Selection.OffspringSelection\%2\HeuristicLab.Selection.OffspringSelection-3.2.dll .\plugins
copy %1\HeuristicLab.SequentialEngine\%2\HeuristicLab.SequentialEngine-3.2.dll .\plugins
copy %1\HeuristicLab.Settings\%2\HeuristicLab.Settings-3.2.dll .\plugins
copy %1\HeuristicLab.SGA\%2\HeuristicLab.SGA-3.2.dll .\plugins
copy %1\HeuristicLab.SimOpt\%2\HeuristicLab.SimOpt-3.2.dll .\plugins
copy %1\HeuristicLab.SQLite\%2\HeuristicLab.SQLite-3.2.dll .\plugins
copy %1\HeuristicLab.SQLite\System.Data.SQLite.dll .\plugins
copy %1\HeuristicLab.SQLite\"SQLite License.txt" .\plugins
copy %1\HeuristicLab.SQLite\SQLite.NET.chm .\plugins
copy %1\HeuristicLab.TestFunctions\%2\HeuristicLab.TestFunctions-3.2.dll .\plugins
copy %1\HeuristicLab.ThreadParallelEngine\%2\HeuristicLab.ThreadParallelEngine-3.2.dll .\plugins
copy %1\HeuristicLab.ES\%2\HeuristicLab.ES-3.2.dll .\plugins
copy %1\HeuristicLab.Visualization\%2\HeuristicLab.Visualization-3.2.dll .\plugins
copy %1\HeuristicLab.Visualization.Test\%2\HeuristicLab.Visualization.Test-3.2.dll .\plugins
copy %1\HeuristicLab.Hive.Server\%2\HeuristicLab.Hive.Server-3.2.dll .\plugins
copy %1\HeuristicLab.Hive.Server.ADODataAccess\%2\HeuristicLab.Hive.Server.ADODataAccess-3.2.dll .\plugins
copy %1\HeuristicLab.Hive.Server.Core\%2\HeuristicLab.Hive.Server.Core-3.2.dll .\plugins
copy %1\HeuristicLab.Hive.Server.Core\%2\HeuristicLab.Hive.Contracts-3.2.dll .\plugins
copy %1\HeuristicLab.Hive.Server.Scheduler\%2\HeuristicLab.Hive.Server.Scheduler-3.2.dll .\plugins
copy %1\HeuristicLab.Hive.Client.Core\%2\HeuristicLab.Hive.Client.Core-3.2.dll .\plugins
copy %1\HeuristicLab.Hive.Client.Common\%2\HeuristicLab.Hive.Client.Common-3.2.dll .\plugins
copy %1\HeuristicLab.Hive.Client.Console\%2\HeuristicLab.Hive.Client.Console-3.2.dll .\plugins
copy %1\HeuristicLab.Hive.Client.ExecutionEngine\%2\HeuristicLab.Hive.Client.ExecutionEngine-3.2.dll .\plugins