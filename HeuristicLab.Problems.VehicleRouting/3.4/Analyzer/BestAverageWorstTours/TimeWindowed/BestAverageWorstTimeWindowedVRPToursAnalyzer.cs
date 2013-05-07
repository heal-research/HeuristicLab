#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Interfaces;
using HeuristicLab.Problems.VehicleRouting.Variants;

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// An operator which analyzes the best, average and worst quality of solutions in the scope tree.
  /// </summary>
  [Item("BestAverageWorstTimeWindowedVRPToursAnalyzer", "An operator which analyzes the best, average and worst properties of the VRP tours in the scope tree.")]
  [StorableClass]
  public sealed class BestAverageWorstTimeWindowedVRPToursAnalyzer : AlgorithmOperator, IAnalyzer, ITimeWindowedOperator {
    #region Parameter properties
    public ILookupParameter<IVRPProblemInstance> ProblemInstanceParameter {
      get { return (ILookupParameter<IVRPProblemInstance>)Parameters["ProblemInstance"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> TardinessParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Tardiness"]; }
    }
    public ValueLookupParameter<DoubleValue> BestTardinessParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestTardiness"]; }
    }
    public ValueLookupParameter<DoubleValue> CurrentBestTardinessParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["CurrentBestTardiness"]; }
    }
    public ValueLookupParameter<DoubleValue> CurrentAverageTardinessParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["CurrentAverageTardiness"]; }
    }
    public ValueLookupParameter<DoubleValue> CurrentWorstTardinessParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["CurrentWorstTardiness"]; }
    }
    public ValueLookupParameter<DataTable> TardinessValuesParameter {
      get { return (ValueLookupParameter<DataTable>)Parameters["TardinessValues"]; }
    }

    public ScopeTreeLookupParameter<DoubleValue> TravelTimeParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["TravelTime"]; }
    }
    public ValueLookupParameter<DoubleValue> BestTravelTimeParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestTravelTime"]; }
    }
    public ValueLookupParameter<DoubleValue> CurrentBestTravelTimeParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["CurrentBestTravelTime"]; }
    }
    public ValueLookupParameter<DoubleValue> CurrentAverageTravelTimeParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["CurrentAverageTravelTime"]; }
    }
    public ValueLookupParameter<DoubleValue> CurrentWorstTravelTimeParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["CurrentWorstTravelTime"]; }
    }
    public ValueLookupParameter<DataTable> TravelTimesParameter {
      get { return (ValueLookupParameter<DataTable>)Parameters["TravelTimes"]; }
    }

    public ValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (ValueLookupParameter<VariableCollection>)Parameters["Results"]; }
    }
    #endregion

    #region Properties
    public bool EnabledByDefault {
      get { return true; }
    }
    private BestTimeWindowedVRPToursMemorizer BestMemorizer {
      get { return (BestTimeWindowedVRPToursMemorizer)OperatorGraph.InitialOperator; }
    }
    private BestAverageWorstTimeWindowedVRPToursCalculator BestAverageWorstCalculator {
      get { return (BestAverageWorstTimeWindowedVRPToursCalculator)BestMemorizer.Successor; }
    }
    #endregion

    public BestAverageWorstTimeWindowedVRPToursAnalyzer()
      : base() {
      #region Create parameters
      Parameters.Add(new LookupParameter<IVRPProblemInstance>("ProblemInstance", "The problem instance."));

      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Tardiness", "The tardiness of the VRP solutions which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestTardiness", "The best tardiness value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentBestTardiness", "The current best tardiness value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentAverageTardiness", "The current average tardiness value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentWorstTardiness", "The current worst tardiness value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DataTable>("TardinessValues", "The data table to store the current best, current average, current worst, best and best known tardiness value."));

      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("TravelTime", "The travel time of the VRP solutions which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestTravelTime", "The best travel time value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentBestTravelTime", "The current best travel time value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentAverageTravelTime", "The current average travel time value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentWorstTravelTime", "The current worst travel time value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DataTable>("TravelTimes", "The data table to store the current best, current average, current worst, best and best known travel time value."));

      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The results collection where the analysis values should be stored."));
      #endregion

      #region Create operators
      BestTimeWindowedVRPToursMemorizer bestMemorizer = new BestTimeWindowedVRPToursMemorizer();
      BestAverageWorstTimeWindowedVRPToursCalculator calculator = new BestAverageWorstTimeWindowedVRPToursCalculator();
      ResultsCollector resultsCollector = new ResultsCollector();

      //tardiness
      bestMemorizer.BestTardinessParameter.ActualName = BestTardinessParameter.Name;
      bestMemorizer.TardinessParameter.ActualName = TardinessParameter.Name;
      bestMemorizer.TardinessParameter.Depth = TardinessParameter.Depth;

      calculator.TardinessParameter.ActualName = TardinessParameter.Name;
      calculator.TardinessParameter.Depth = TardinessParameter.Depth;
      calculator.BestTardinessParameter.ActualName = CurrentBestTardinessParameter.Name;
      calculator.AverageTardinessParameter.ActualName = CurrentAverageTardinessParameter.Name;
      calculator.WorstTardinessParameter.ActualName = CurrentWorstTardinessParameter.Name;

      DataTableValuesCollector tardinessDataTablesCollector = new DataTableValuesCollector();
      tardinessDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("BestTardiness", null, BestTardinessParameter.Name));
      tardinessDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentBestTardiness", null, CurrentBestTardinessParameter.Name));
      tardinessDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentAverageTardiness", null, CurrentAverageTardinessParameter.Name));
      tardinessDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentWorstTardiness", null, CurrentWorstTardinessParameter.Name));
      tardinessDataTablesCollector.DataTableParameter.ActualName = TardinessValuesParameter.Name;

      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>(TardinessValuesParameter.Name));

      //Travel Time
      bestMemorizer.BestTravelTimeParameter.ActualName = BestTravelTimeParameter.Name;
      bestMemorizer.TravelTimeParameter.ActualName = TravelTimeParameter.Name;
      bestMemorizer.TravelTimeParameter.Depth = TravelTimeParameter.Depth;

      calculator.TravelTimeParameter.ActualName = TravelTimeParameter.Name;
      calculator.TravelTimeParameter.Depth = TravelTimeParameter.Depth;
      calculator.BestTravelTimeParameter.ActualName = CurrentBestTravelTimeParameter.Name;
      calculator.AverageTravelTimeParameter.ActualName = CurrentAverageTravelTimeParameter.Name;
      calculator.WorstTravelTimeParameter.ActualName = CurrentWorstTravelTimeParameter.Name;

      DataTableValuesCollector travelTimeDataTablesCollector = new DataTableValuesCollector();
      travelTimeDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("BestTravelTime", null, BestTravelTimeParameter.Name));
      travelTimeDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentBestTravelTime", null, CurrentBestTravelTimeParameter.Name));
      travelTimeDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentAverageTravelTime", null, CurrentAverageTravelTimeParameter.Name));
      travelTimeDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentWorstTravelTime", null, CurrentWorstTravelTimeParameter.Name));
      travelTimeDataTablesCollector.DataTableParameter.ActualName = TravelTimesParameter.Name;

      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>(TravelTimesParameter.Name));
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = bestMemorizer;
      bestMemorizer.Successor = calculator;
      calculator.Successor = tardinessDataTablesCollector;
      tardinessDataTablesCollector.Successor = travelTimeDataTablesCollector;
      travelTimeDataTablesCollector.Successor = resultsCollector;
      resultsCollector.Successor = null;
      #endregion

      Initialize();
    }
    [StorableConstructor]
    private BestAverageWorstTimeWindowedVRPToursAnalyzer(bool deserializing) : base() { }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      TardinessParameter.DepthChanged += new EventHandler(TardinessParameter_DepthChanged);
      TravelTimeParameter.DepthChanged += new EventHandler(TravelTimeParameter_DepthChanged);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestAverageWorstTimeWindowedVRPToursAnalyzer(this, cloner);
    }

    private BestAverageWorstTimeWindowedVRPToursAnalyzer(BestAverageWorstTimeWindowedVRPToursAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      this.Initialize();
    }

    void TardinessParameter_DepthChanged(object sender, EventArgs e) {
      BestAverageWorstCalculator.TardinessParameter.Depth = TardinessParameter.Depth;
      BestMemorizer.TardinessParameter.Depth = TardinessParameter.Depth;
    }

    void TravelTimeParameter_DepthChanged(object sender, EventArgs e) {
      BestAverageWorstCalculator.TravelTimeParameter.Depth = TravelTimeParameter.Depth;
      BestMemorizer.TravelTimeParameter.Depth = TravelTimeParameter.Depth;
    }
  }
}
