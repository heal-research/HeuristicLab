#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
#endregion

using System;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HEAL.Fossil;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// An operator which analyzes the best, average and worst quality of solutions in the scope tree.
  /// </summary>
  [Item("BestAverageWorstVRPToursAnalyzer", "An operator which analyzes the best, average and worst properties of the VRP tours in the scope tree.")]
  [StorableType("5DF1280C-5FEB-451E-8132-168E515CF47D")]
  public sealed class BestAverageWorstVRPToursAnalyzer : AlgorithmOperator, IAnalyzer, IGeneralVRPOperator {
    #region Parameter properties
    public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter {
      get { return (ILookupParameter<IVRPProblemInstance>)Parameters["ProblemInstance"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> DistanceParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Distance"]; }
    }
    public ValueLookupParameter<DoubleValue> BestDistanceParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestDistance"]; }
    }
    public ValueLookupParameter<DoubleValue> CurrentBestDistanceParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["CurrentBestDistance"]; }
    }
    public ValueLookupParameter<DoubleValue> CurrentAverageDistanceParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["CurrentAverageDistance"]; }
    }
    public ValueLookupParameter<DoubleValue> CurrentWorstDistanceParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["CurrentWorstDistance"]; }
    }
    public ValueLookupParameter<DataTable> DistancesParameter {
      get { return (ValueLookupParameter<DataTable>)Parameters["Distances"]; }
    }

    public ScopeTreeLookupParameter<DoubleValue> VehiclesUtilizedParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["VehiclesUtilized"]; }
    }
    public ValueLookupParameter<DoubleValue> BestVehiclesUtilizedParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestVehiclesUtilized"]; }
    }
    public ValueLookupParameter<DoubleValue> CurrentBestVehiclesUtilizedParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["CurrentBestVehiclesUtilized"]; }
    }
    public ValueLookupParameter<DoubleValue> CurrentAverageVehiclesUtilizedParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["CurrentAverageVehiclesUtilized"]; }
    }
    public ValueLookupParameter<DoubleValue> CurrentWorstVehiclesUtilizedParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["CurrentWorstVehiclesUtilized"]; }
    }
    public ValueLookupParameter<DataTable> VehiclesUtilizedValuesParameter {
      get { return (ValueLookupParameter<DataTable>)Parameters["VehiclesUtilizedValues"]; }
    }

    public ValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (ValueLookupParameter<VariableCollection>)Parameters["Results"]; }
    }
    #endregion

    #region Properties
    public bool EnabledByDefault {
      get { return true; }
    }
    private BestVRPToursMemorizer BestMemorizer {
      get { return (BestVRPToursMemorizer)OperatorGraph.InitialOperator; }
    }
    private BestAverageWorstVRPToursCalculator BestAverageWorstCalculator {
      get { return (BestAverageWorstVRPToursCalculator)BestMemorizer.Successor; }
    }
    #endregion

    public BestAverageWorstVRPToursAnalyzer()
      : base() {
      #region Create parameters
      Parameters.Add(new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The problem instance."));

      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Distance", "The distance of the VRP solutions which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestDistance", "The best distance value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentBestDistance", "The current best distance value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentAverageDistance", "The current average distance value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentWorstDistance", "The current worst distance value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DataTable>("Distances", "The data table to store the current best, current average, current worst, best and best known distance value."));

      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("VehiclesUtilized", "The vehicles utilized of the VRP solutions which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestVehiclesUtilized", "The best  vehicles utilized value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentBestVehiclesUtilized", "The current best  vehicles utilized value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentAverageVehiclesUtilized", "The current average  vehicles utilized value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentWorstVehiclesUtilized", "The current worst  vehicles utilized value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DataTable>("VehiclesUtilizedValues", "The data table to store the current best, current average, current worst, best and best known vehicles utilized value."));

      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The results collection where the analysis values should be stored."));
      #endregion

      #region Create operators
      BestVRPToursMemorizer bestMemorizer = new BestVRPToursMemorizer();
      BestAverageWorstVRPToursCalculator calculator = new BestAverageWorstVRPToursCalculator();
      ResultsCollector resultsCollector = new ResultsCollector();

      //Distance
      bestMemorizer.BestDistanceParameter.ActualName = BestDistanceParameter.Name;
      bestMemorizer.DistanceParameter.ActualName = DistanceParameter.Name;
      bestMemorizer.DistanceParameter.Depth = DistanceParameter.Depth;

      calculator.DistanceParameter.ActualName = DistanceParameter.Name;
      calculator.DistanceParameter.Depth = DistanceParameter.Depth;
      calculator.BestDistanceParameter.ActualName = CurrentBestDistanceParameter.Name;
      calculator.AverageDistanceParameter.ActualName = CurrentAverageDistanceParameter.Name;
      calculator.WorstDistanceParameter.ActualName = CurrentWorstDistanceParameter.Name;

      DataTableValuesCollector distanceDataTablesCollector = new DataTableValuesCollector();
      distanceDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("BestDistance", null, BestDistanceParameter.Name));
      distanceDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentBestDistance", null, CurrentBestDistanceParameter.Name));
      distanceDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentAverageDistance", null, CurrentAverageDistanceParameter.Name));
      distanceDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentWorstDistance", null, CurrentWorstDistanceParameter.Name));
      distanceDataTablesCollector.DataTableParameter.ActualName = DistancesParameter.Name;

      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>(DistancesParameter.Name));

      //Vehicles Utlized
      bestMemorizer.BestVehiclesUtilizedParameter.ActualName = BestVehiclesUtilizedParameter.Name;
      bestMemorizer.VehiclesUtilizedParameter.ActualName = VehiclesUtilizedParameter.Name;
      bestMemorizer.VehiclesUtilizedParameter.Depth = VehiclesUtilizedParameter.Depth;

      calculator.VehiclesUtilizedParameter.ActualName = VehiclesUtilizedParameter.Name;
      calculator.VehiclesUtilizedParameter.Depth = VehiclesUtilizedParameter.Depth;
      calculator.BestVehiclesUtilizedParameter.ActualName = CurrentBestVehiclesUtilizedParameter.Name;
      calculator.AverageVehiclesUtilizedParameter.ActualName = CurrentAverageVehiclesUtilizedParameter.Name;
      calculator.WorstVehiclesUtilizedParameter.ActualName = CurrentWorstVehiclesUtilizedParameter.Name;

      DataTableValuesCollector vehiclesUtilizedDataTablesCollector = new DataTableValuesCollector();
      vehiclesUtilizedDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("BestVehiclesUtilized", null, BestVehiclesUtilizedParameter.Name));
      vehiclesUtilizedDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentBestVehiclesUtilized", null, CurrentBestVehiclesUtilizedParameter.Name));
      vehiclesUtilizedDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentAverageVehiclesUtilized", null, CurrentAverageVehiclesUtilizedParameter.Name));
      vehiclesUtilizedDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentWorstVehiclesUtilized", null, CurrentWorstVehiclesUtilizedParameter.Name));
      vehiclesUtilizedDataTablesCollector.DataTableParameter.ActualName = VehiclesUtilizedValuesParameter.Name;

      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>(VehiclesUtilizedValuesParameter.Name));
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = bestMemorizer;
      bestMemorizer.Successor = calculator;
      calculator.Successor = distanceDataTablesCollector;
      distanceDataTablesCollector.Successor = vehiclesUtilizedDataTablesCollector;
      vehiclesUtilizedDataTablesCollector.Successor = resultsCollector;
      resultsCollector.Successor = null;
      #endregion

      Initialize();
    }
    [StorableConstructor]
    private BestAverageWorstVRPToursAnalyzer(StorableConstructorFlag _) : base(_) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestAverageWorstVRPToursAnalyzer(this, cloner);
    }

    private BestAverageWorstVRPToursAnalyzer(BestAverageWorstVRPToursAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      this.Initialize();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      DistanceParameter.DepthChanged += new EventHandler(DistanceParameter_DepthChanged);
      VehiclesUtilizedParameter.DepthChanged += new EventHandler(VehiclesUtilizedParameter_DepthChanged);
    }

    void DistanceParameter_DepthChanged(object sender, EventArgs e) {
      BestAverageWorstCalculator.DistanceParameter.Depth = DistanceParameter.Depth;
      BestMemorizer.DistanceParameter.Depth = DistanceParameter.Depth;
    }

    void VehiclesUtilizedParameter_DepthChanged(object sender, EventArgs e) {
      BestAverageWorstCalculator.VehiclesUtilizedParameter.Depth = VehiclesUtilizedParameter.Depth;
      BestMemorizer.VehiclesUtilizedParameter.Depth = DistanceParameter.Depth;
    }
  }
}
