copy "%SolutionDir%\HeuristicLab.PluginInfrastructure.GUI\ICSharpCode.SharpZipLib License.txt" .\

rmdir plugins /s /q
mkdir plugins
mkdir plugins\cache
mkdir plugins\temp
mkdir plugins\backup

copy "%SolutionDir%\ALGLIB\%Outdir%\ALGLIB.dll" .\plugins
copy "%SolutionDir%\ALGLIB\%Outdir%\ALGLIB License.txt" .\plugins
copy "%SolutionDir%\HeuristicLab.AdvancedOptimizationFrontend\3.2\%Outdir%\HeuristicLab.AdvancedOptimizationFrontend-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.AdvancedOptimizationFrontend\3.3\%Outdir%\HeuristicLab.AdvancedOptimizationFrontend-3.3.dll" .\plugins
copy "%SolutionDir%\WinFormsUI\%Outdir%\WeifenLuo.WinFormsUI.Docking.dll" .\plugins
copy "%SolutionDir%\WinFormsUI\%Outdir%\WeifenLuo.WinFormsUI.Docking License.txt" .\plugins
copy "%SolutionDir%\HeuristicLab.Assignment.QAP\3.2\%Outdir%\HeuristicLab.Assignment.QAP-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.BitVector\3.2\%Outdir%\HeuristicLab.BitVector-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.BackgroundProcessing\3.2\%Outdir%\HeuristicLab.BackgroundProcessing-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.CEDMA.Server\3.3\%Outdir%\HeuristicLab.CEDMA.Server-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Charting\3.2\%Outdir%\HeuristicLab.Charting-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Charting\3.3\%Outdir%\HeuristicLab.Charting-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Charting.Data\3.2\%Outdir%\HeuristicLab.Charting.Data-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Charting.Data\3.3\%Outdir%\HeuristicLab.Charting.Data-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Common\3.2\%Outdir%\HeuristicLab.Common-3.2.dll" .\plugins 
copy "%SolutionDir%\HeuristicLab.Common.Resources\3.2\%Outdir%\HeuristicLab.Common.Resources-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Communication.Data\3.2\%Outdir%\HeuristicLab.Communication.Data-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Communication.Operators\3.2\%Outdir%\HeuristicLab.Communication.Operators-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Core\3.2\%Outdir%\HeuristicLab.Core-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Core\3.3\%Outdir%\HeuristicLab.Core-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Constraints\3.2\%Outdir%\HeuristicLab.Constraints-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Constraints\3.3\%Outdir%\HeuristicLab.Constraints-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Data\3.2\%Outdir%\HeuristicLab.Data-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Data\3.3\%Outdir%\HeuristicLab.Data-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.DataAccess\3.2\%Outdir%\HeuristicLab.DataAccess-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.DataAccess.ADOHelper\3.2\%Outdir%\HeuristicLab.DataAccess.ADOHelper-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.DataAnalysis\3.2\%Outdir%\HeuristicLab.DataAnalysis-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.DataAnalysis\3.3\%Outdir%\HeuristicLab.DataAnalysis-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.DistributedEngine\3.2\%Outdir%\HeuristicLab.DistributedEngine-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.ES\3.2\%Outdir%\HeuristicLab.ES-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Evolutionary\3.2\%Outdir%\HeuristicLab.Evolutionary-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Evolutionary\3.3\%Outdir%\HeuristicLab.Evolutionary-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.GP\3.3\%Outdir%\HeuristicLab.GP-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.GP\3.4\%Outdir%\HeuristicLab.GP-3.4.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.GP.Algorithms\3.2\%Outdir%\HeuristicLab.GP.Algorithms-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.GP.Interfaces\3.3\%Outdir%\HeuristicLab.GP.Interfaces-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.GP.Operators\3.3\%Outdir%\HeuristicLab.GP.Operators-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.GP.Boolean\3.3\%Outdir%\HeuristicLab.GP.Boolean-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.GP.SantaFe\3.3\%Outdir%\HeuristicLab.GP.SantaFe-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.GP.StructureIdentification\3.3\%Outdir%\HeuristicLab.GP.StructureIdentification-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.GP.StructureIdentification\3.4\%Outdir%\HeuristicLab.GP.StructureIdentification-3.4.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.GP.StructureIdentification.Classification\3.3\%Outdir%\HeuristicLab.GP.StructureIdentification.Classification-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.GP.StructureIdentification.ConditionalEvaluation\3.3\%Outdir%\HeuristicLab.GP.StructureIdentification.ConditionalEvaluation-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.GP.StructureIdentification.TimeSeries\3.3\%Outdir%\HeuristicLab.GP.StructureIdentification.TimeSeries-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Grid\3.2\%Outdir%\HeuristicLab.Grid-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Grid.HiveBridge\3.2\%Outdir%\HeuristicLab.Grid.HiveBridge-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Client.Common\3.2\%Outdir%\HeuristicLab.Hive.Client.Common-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Client.Communication\3.2\%Outdir%\HeuristicLab.Hive.Client.Communication-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Client.Console\3.2\%Outdir%\HeuristicLab.Hive.Client.Console-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Client.Console\3.2\%Outdir%\ZedGraph.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Client.Console\3.2\%Outdir%\Calendar.DayView.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Client.Core\3.2\%Outdir%\HeuristicLab.Hive.Client.Core-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Client.ExecutionEngine\3.2\%Outdir%\HeuristicLab.Hive.Client.ExecutionEngine-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Engine\3.2\%Outdir%\HeuristicLab.Hive.Engine-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.JobBase\3.2\%Outdir%\HeuristicLab.Hive.JobBase-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Server\3.2\%Outdir%\HeuristicLab.Hive.Server-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Server.ADODataAccess\3.2\%Outdir%\HeuristicLab.Hive.Server.ADODataAccess-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Server.Console\3.2\%Outdir%\HeuristicLab.Hive.Server.Console-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Server.Core\3.2\%Outdir%\HeuristicLab.Hive.Server.Core-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Server.Core\3.2\%Outdir%\HeuristicLab.Hive.Contracts-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Server.Core\3.2\%Outdir%\Authorization\*.xml" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Server.DataAccess\3.2\%Outdir%\HeuristicLab.Hive.Server.DataAccess-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Hive.Server.Scheduler\3.2\%Outdir%\HeuristicLab.Hive.Server.Scheduler-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.IntVector\3.2\%Outdir%\HeuristicLab.IntVector-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.LinearRegression\3.2\%Outdir%\HeuristicLab.LinearRegression-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.LinearRegression\3.2\%Outdir%\ALGLIB License.txt" .\plugins
copy "%SolutionDir%\HeuristicLab.Logging\3.2\%Outdir%\HeuristicLab.Logging-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Logging\3.3\%Outdir%\HeuristicLab.Logging-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.MainForm\3.2\%Outdir%\HeuristicLab.MainForm-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.MainForm.Test\3.2\%Outdir%\HeuristicLab.MainForm.Test-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Modeling\3.2\%Outdir%\HeuristicLab.Modeling-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Modeling\3.3\%Outdir%\HeuristicLab.Modeling-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Modeling.Database\3.2\%Outdir%\HeuristicLab.Modeling.Database-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Modeling.Database.SQLServerCompact\3.2\%Outdir%\HeuristicLab.Modeling.Database.SQLServerCompact-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Operators\3.2\%Outdir%\HeuristicLab.Operators-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Operators\3.3\%Outdir%\HeuristicLab.Operators-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Operators.Metaprogramming\3.2\%Outdir%\HeuristicLab.Operators.Metaprogramming-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Operators.Programmable\3.2\%Outdir%\HeuristicLab.Operators.Programmable-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Operators.Programmable\3.3\%Outdir%\HeuristicLab.Operators.Programmable-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Operators.Stopwatch\3.2\%Outdir%\HeuristicLab.Operators.Stopwatch-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.OptimizationFrontend\3.2\%Outdir%\HeuristicLab.OptimizationFrontend-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.OptimizationFrontend\3.3\%Outdir%\HeuristicLab.OptimizationFrontend-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Permutation\3.2\%Outdir%\HeuristicLab.Permutation-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Permutation\3.3\%Outdir%\HeuristicLab.Permutation-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Persistence\3.3\%Outdir%\HeuristicLab.Persistence-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Persistence.GUI\3.3\%Outdir%\HeuristicLab.Persistence.GUI-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Random\3.2\%Outdir%\HeuristicLab.Random-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Random\3.3\%Outdir%\HeuristicLab.Random-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.RealVector\3.2\%Outdir%\HeuristicLab.RealVector-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Routing.TSP\3.2\%Outdir%\HeuristicLab.Routing.TSP-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Routing.TSP\3.3\%Outdir%\HeuristicLab.Routing.TSP-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.SA\3.2\%Outdir%\HeuristicLab.SA-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Scheduling.JSSP\3.2\%Outdir%\HeuristicLab.Scheduling.JSSP-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Selection\3.2\%Outdir%\HeuristicLab.Selection-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Selection\3.3\%Outdir%\HeuristicLab.Selection-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Selection.OffspringSelection\3.2\%Outdir%\HeuristicLab.Selection.OffspringSelection-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Selection.OffspringSelection\3.3\%Outdir%\HeuristicLab.Selection.OffspringSelection-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Selection.Uncertainty\3.2\%Outdir%\HeuristicLab.Selection.Uncertainty-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.SequentialEngine\3.2\%Outdir%\HeuristicLab.SequentialEngine-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.SequentialEngine\3.3\%Outdir%\HeuristicLab.SequentialEngine-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.SGA\3.2\%Outdir%\HeuristicLab.SGA-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.SGA\3.3\%Outdir%\HeuristicLab.SGA-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.FixedOperators\3.2\%Outdir%\HeuristicLab.FixedOperators-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.SimOpt\3.2\%Outdir%\HeuristicLab.SimOpt-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.SQLite\3.2\%Outdir%\HeuristicLab.SQLite-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.SQLite\3.2\SQLite License.txt" .\plugins
copy "%SolutionDir%\HeuristicLab.SQLite\3.2\SQLite.NET.chm" .\plugins
copy "%SolutionDir%\HeuristicLAb.SQLServerCompact\3.2\%Outdir%\HeuristicLab.SQLServerCompact-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLAb.SQLServerCompact\3.2\%Outdir%\System.Data.SqlServerCe.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.StatisticalAnalysis\3.2\%Outdir%\HeuristicLab.StatisticalAnalysis-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.SupportVectorMachines\3.2\%Outdir%\HeuristicLab.SupportVectorMachines-3.2.dll" .\plugins
copy "%SolutionDir%\LibSVM\%Outdir%\LibSVM.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.TestFunctions\3.2\%Outdir%\HeuristicLab.TestFunctions-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.TestFunctions\3.3\%Outdir%\HeuristicLab.TestFunctions-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.ThreadParallelEngine\3.2\%Outdir%\HeuristicLab.ThreadParallelEngine-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.ThreadParallelEngine\3.3\%Outdir%\HeuristicLab.ThreadParallelEngine-3.3.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Tracing\3.2\%Outdir%\HeuristicLab.Tracing-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Tracing\3.2\%Outdir%\log4net.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Tracing\3.2\log4net licence.txt" .\plugins
copy "%SolutionDir%\HeuristicLab.Tracing\3.2\HeuristicLab.log4net.xml" .\plugins
copy "%SolutionDir%\HeuristicLab.Tracing\3.2\HeuristicLab.Hive.log4net.xml" .\plugins
copy "%SolutionDir%\HeuristicLab.Visualization\3.2\%Outdir%\HeuristicLab.Visualization-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Visualization.Test\3.2\%Outdir%\HeuristicLab.Visualization.Test-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Security.Contracts\3.2\%Outdir%\HeuristicLab.Security.Contracts-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Security.Core\3.2\%Outdir%\HeuristicLab.Security.Core-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Security.DataAccess\3.2\%Outdir%\HeuristicLab.Security.DataAccess-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Security.ADODataAccess\3.2\%Outdir%\HeuristicLab.Security.ADODataAccess-3.2.dll" .\plugins
copy "%SolutionDir%\HeuristicLab.Security.Server\3.2\%Outdir%\HeuristicLab.Security.Server-3.2.dll" .\plugins
echo "Platform: %Platform%, architecture: %PROCESSOR_ARCHITECTURE%"
if "%Platform%" == "x86" (   
  copy /B /Y "%SolutionDir%\HeuristicLab.SQLite\3.2\System.Data.SQLite.dll" .\plugins
  xcopy "%SolutionDir%\HeuristicLab.SQLServerCompact\3.2\x86\*" .\plugins /e /i
) else if "%Platform%" == "x64" ( 
  copy /B /Y "%SolutionDir%\HeuristicLab.SQLite\3.2\System.Data.SQLite.x64.dll" .\plugins\System.Data.SQLite.dll
  xcopy "%SolutionDir%\HeuristicLab.SQLServerCompact\3.2\x64\*" .\plugins /e /i
) else if "%Platform%" == "AnyCPU" (
  if "%PROCESSOR_ARCHITECTURE%" == "x64" (
    copy /B /Y "%SolutionDir%\HeuristicLab.SQLite\3.2\System.Data.SQLite.x64.dll" .\plugins\System.Data.SQLite.dll
     xcopy "%SolutionDir%\HeuristicLab.SQLServerCompact\3.2\x64\*" .\plugins /e /i
  ) else if "%PROCESSOR_ARCHITECTURE%" == "x86" (
    copy /B /Y "%SolutionDir%\HeuristicLab.SQLite\3.2\System.Data.SQLite.dll" .\plugins
    xcopy "%SolutionDir%\HeuristicLab.SQLServerCompact\3.2\x86\*" .\plugins /e /i
  ) else (
    echo "ERROR: unknown architecture: "%PROCESSOR_ARCHITECTURE%"
  ) 
) else (
  echo "ERROR: unknown platform: %Platform%"
)
