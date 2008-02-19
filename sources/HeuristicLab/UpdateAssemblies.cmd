rmdir plugins /s /q 
mkdir plugins
mkdir plugins\cache
mkdir plugins\temp
mkdir plugins\backup
copy ..\..\..\HeuristicLab.AdvancedOptimizationFrontend\bin\%1\HeuristicLab.AdvancedOptimizationFrontend.dll .\plugins
copy ..\..\..\HeuristicLab.AdvancedOptimizationFrontend\bin\%1\WeifenLuo.WinFormsUI.Docking.dll .\plugins
copy ..\..\..\HeuristicLab.BitVector\bin\%1\HeuristicLab.BitVector.dll .\plugins
copy ..\..\..\HeuristicLab.Charting\bin\%1\HeuristicLab.Charting.dll .\plugins
copy ..\..\..\HeuristicLab.Charting.Data\bin\%1\HeuristicLab.Charting.Data.dll .\plugins
copy ..\..\..\HeuristicLab.Charting.Gantt\bin\%1\HeuristicLab.Charting.Gantt.dll .\plugins
copy ..\..\..\HeuristicLab.Charting.Grid\bin\%1\HeuristicLab.Charting.Grid.dll .\plugins
copy ..\..\..\HeuristicLab.Core\bin\%1\HeuristicLab.Core.dll .\plugins
copy ..\..\..\HeuristicLab.Constraints\bin\%1\HeuristicLab.Constraints.dll .\plugins
copy ..\..\..\HeuristicLab.Data\bin\%1\HeuristicLab.Data.dll .\plugins
copy ..\..\..\HeuristicLab.DataAnalysis\bin\%1\HeuristicLab.DataAnalysis.dll .\plugins
copy ..\..\..\HeuristicLab.DistributedEngine\bin\%1\HeuristicLab.DistributedEngine.dll .\plugins
copy ..\..\..\HeuristicLab.Evolutionary\bin\%1\HeuristicLab.Evolutionary.dll .\plugins
copy ..\..\..\HeuristicLab.Functions\bin\%1\HeuristicLab.Functions.dll .\plugins
copy ..\..\..\HeuristicLab.Grid\bin\%1\HeuristicLab.Grid.dll .\plugins
copy ..\..\..\HeuristicLab.Logging\bin\%1\HeuristicLab.Logging.dll .\plugins
copy ..\..\..\HeuristicLab.Operators\bin\%1\HeuristicLab.Operators.dll .\plugins
copy ..\..\..\HeuristicLab.Operators.Programmable\bin\%1\HeuristicLab.Operators.Programmable.dll .\plugins
copy ..\..\..\HeuristicLab.OptimizationFrontend\bin\%1\HeuristicLab.OptimizationFrontend.dll .\plugins
copy ..\..\..\HeuristicLab.Permutation\bin\%1\HeuristicLab.Permutation.dll .\plugins
copy ..\..\..\HeuristicLab.Random\bin\%1\HeuristicLab.Random.dll .\plugins
copy ..\..\..\HeuristicLab.RealVector\bin\%1\HeuristicLab.RealVector.dll .\plugins
copy ..\..\..\HeuristicLab.Routing.TSP\bin\%1\HeuristicLab.Routing.TSP.dll .\plugins
copy ..\..\..\HeuristicLab.Scheduling.JSSP\bin\%1\HeuristicLab.Scheduling.JSSP.dll .\plugins
copy ..\..\..\HeuristicLab.Selection\bin\%1\HeuristicLab.Selection.dll .\plugins
copy ..\..\..\HeuristicLab.Selection.OffspringSelection\bin\%1\HeuristicLab.Selection.OffspringSelection.dll .\plugins
copy ..\..\..\HeuristicLab.SequentialEngine\bin\%1\HeuristicLab.SequentialEngine.dll .\plugins
copy ..\..\..\HeuristicLab.SGA\bin\%1\HeuristicLab.SGA.dll .\plugins
copy ..\..\..\HeuristicLab.StructureIdentification\bin\%1\HeuristicLab.StructureIdentification.dll .\plugins
copy ..\..\..\HeuristicLab.TestFunctions\bin\%1\HeuristicLab.TestFunctions.dll .\plugins
copy ..\..\..\HeuristicLab.ThreadParallelEngine\bin\%1\HeuristicLab.ThreadParallelEngine.dll .\plugins
