copy %1\HeuristicLab.PluginInfrastructure.GUI\ICSharpCode.SharpZipLib License.txt .\

rmdir plugins /s /q 
mkdir plugins
mkdir plugins\cache
mkdir plugins\temp
mkdir plugins\backup
copy %1\HeuristicLab.AdvancedOptimizationFrontend\%2\HeuristicLab.AdvancedOptimizationFrontend.dll .\plugins
copy %1\HeuristicLab.AdvancedOptimizationFrontend\WeifenLuo.WinFormsUI.Docking.dll .\plugins
copy %1\HeuristicLab.AdvancedOptimizationFrontend\"WeifenLuo.WinFormsUI.Docking License.txt" .\plugins
copy %1\HeuristicLab.BitVector\%2\HeuristicLab.BitVector.dll .\plugins
copy %1\HeuristicLab.CEDMA.Charting\%2\HeuristicLab.CEDMA.Charting.dll .\plugins
copy %1\HeuristicLab.CEDMA.Core\%2\HeuristicLab.CEDMA.Core.dll .\plugins
copy %1\HeuristicLab.CEDMA.DB\%2\HeuristicLab.CEDMA.DB.dll .\plugins
copy %1\HeuristicLab.CEDMA.DB\%2\SemWeb.SqliteStore.dll .\plugins
copy %1\HeuristicLab.CEDMA.DB\%2\SemWeb.dll .\plugins
copy %1\HeuristicLab.CEDMA.DB.Interfaces\%2\HeuristicLab.CEDMA.DB.Interfaces.dll .\plugins
copy %1\HeuristicLab.CEDMA.Operators\%2\HeuristicLab.CEDMA.Operators.dll .\plugins
copy %1\HeuristicLab.CEDMA.Server\%2\HeuristicLab.CEDMA.Server.dll .\plugins
copy %1\HeuristicLab.Charting\%2\HeuristicLab.Charting.dll .\plugins
copy %1\HeuristicLab.Charting.Data\%2\HeuristicLab.Charting.Data.dll .\plugins
copy %1\HeuristicLab.Core\%2\HeuristicLab.Core.dll .\plugins
copy %1\HeuristicLab.Constraints\%2\HeuristicLab.Constraints.dll .\plugins
copy %1\HeuristicLab.Data\%2\HeuristicLab.Data.dll .\plugins
copy %1\HeuristicLab.DataAnalysis\%2\HeuristicLab.DataAnalysis.dll .\plugins
copy %1\HeuristicLab.DistributedEngine\%2\HeuristicLab.DistributedEngine.dll .\plugins
copy %1\HeuristicLab.Evolutionary\%2\HeuristicLab.Evolutionary.dll .\plugins
copy %1\HeuristicLab.Functions\%2\HeuristicLab.Functions.dll .\plugins
copy %1\HeuristicLab.Grid\%2\HeuristicLab.Grid.dll .\plugins
copy %1\HeuristicLab.Logging\%2\HeuristicLab.Logging.dll .\plugins
copy %1\HeuristicLab.Operators\%2\HeuristicLab.Operators.dll .\plugins
copy %1\HeuristicLab.Operators.Metaprogramming\%2\HeuristicLab.Operators.Metaprogramming.dll .\plugins
copy %1\HeuristicLab.Operators.Programmable\%2\HeuristicLab.Operators.Programmable.dll .\plugins
copy %1\HeuristicLab.Operators.Stopwatch\%2\HeuristicLab.Operators.Stopwatch.dll .\plugins
copy %1\HeuristicLab.OptimizationFrontend\%2\HeuristicLab.OptimizationFrontend.dll .\plugins
copy %1\HeuristicLab.Permutation\%2\HeuristicLab.Permutation.dll .\plugins
copy %1\HeuristicLab.Random\%2\HeuristicLab.Random.dll .\plugins
copy %1\HeuristicLab.RealVector\%2\HeuristicLab.RealVector.dll .\plugins
copy %1\HeuristicLab.Routing.TSP\%2\HeuristicLab.Routing.TSP.dll .\plugins
copy %1\HeuristicLab.Scheduling.JSSP\%2\HeuristicLab.Scheduling.JSSP.dll .\plugins
copy %1\HeuristicLab.Selection\%2\HeuristicLab.Selection.dll .\plugins
copy %1\HeuristicLab.Selection.OffspringSelection\%2\HeuristicLab.Selection.OffspringSelection.dll .\plugins
copy %1\HeuristicLab.SequentialEngine\%2\HeuristicLab.SequentialEngine.dll .\plugins
copy %1\HeuristicLab.SGA\%2\HeuristicLab.SGA.dll .\plugins
copy %1\HeuristicLab.SQLite\%2\HeuristicLab.SQLite.dll .\plugins
copy %1\HeuristicLab.SQLite\System.Data.SQLite.dll .\plugins
copy %1\HeuristicLab.SQLite\"SQLite License.txt" .\plugins
copy %1\HeuristicLab.SQLite\SQLite.NET.chm .\plugins
copy %1\HeuristicLab.StructureIdentification\%2\HeuristicLab.StructureIdentification.dll .\plugins
copy %1\HeuristicLab.TestFunctions\%2\HeuristicLab.TestFunctions.dll .\plugins
copy %1\HeuristicLab.ThreadParallelEngine\%2\HeuristicLab.ThreadParallelEngine.dll .\plugins
copy %1\HeuristicLab.ES\%2\HeuristicLab.ES.dll .\plugins