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

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// An operator which analyzes the best, average and worst quality of solutions in the scope tree.
  /// </summary>
  [Item("BestAverageWorstVRPToursAnalyzer", "An operator which analyzes the best, average and worst properties of the VRP tours in the scope tree.")]
  [StorableClass]
  public sealed class BestAverageWorstVRPToursAnalyzer : AlgorithmOperator, IAnalyzer {
    #region Parameter properties
    public ScopeTreeLookupParameter<DoubleValue> OverloadParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Overload"]; }
    }
    public ValueLookupParameter<DoubleValue> BestOverloadParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["BestOverload"]; }
    }
    public ValueLookupParameter<DoubleValue> CurrentBestOverloadParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["CurrentBestOverload"]; }
    }
    public ValueLookupParameter<DoubleValue> CurrentAverageOverloadParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["CurrentAverageOverload"]; }
    }
    public ValueLookupParameter<DoubleValue> CurrentWorstOverloadParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["CurrentWorstOverload"]; }
    }
    public ValueLookupParameter<DataTable> OverloadsParameter {
      get { return (ValueLookupParameter<DataTable>)Parameters["Overloads"]; }
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
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Overload", "The overloads of the VRP solutions which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestOverload", "The best overload value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentBestOverload", "The current best overload value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentAverageOverload", "The current average overload value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentWorstOverload", "The current worst overload value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DataTable>("Overloads", "The data table to store the current best, current average, current worst, best and best known overload value."));

      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Tardiness", "The tardiness of the VRP solutions which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestTardiness", "The best tardiness value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentBestTardiness", "The current best tardiness value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentAverageTardiness", "The current average tardiness value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentWorstTardiness", "The current worst tardiness value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DataTable>("TardinessValues", "The data table to store the current best, current average, current worst, best and best known tardiness value."));

      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Distance", "The distance of the VRP solutions which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestDistance", "The best distance value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentBestDistance", "The current best distance value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentAverageDistance", "The current average distance value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentWorstDistance", "The current worst distance value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DataTable>("Distances", "The data table to store the current best, current average, current worst, best and best known distance value."));

      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("TravelTime", "The travel time of the VRP solutions which should be analyzed."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestTravelTime", "The best travel time value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentBestTravelTime", "The current best travel time value."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentAverageTravelTime", "The current average travel time value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("CurrentWorstTravelTime", "The current worst travel time value of all solutions."));
      Parameters.Add(new ValueLookupParameter<DataTable>("TravelTimes", "The data table to store the current best, current average, current worst, best and best known travel time value."));

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

      //overload
      bestMemorizer.BestOverloadParameter.ActualName = BestOverloadParameter.Name;
      bestMemorizer.OverloadParameter.ActualName = OverloadParameter.Name;
      bestMemorizer.OverloadParameter.Depth = OverloadParameter.Depth;

      calculator.OverloadParameter.ActualName = OverloadParameter.Name;
      calculator.OverloadParameter.Depth = OverloadParameter.Depth;
      calculator.BestOverloadParameter.ActualName = CurrentBestOverloadParameter.Name;
      calculator.AverageOverloadParameter.ActualName = CurrentAverageOverloadParameter.Name;
      calculator.WorstOverloadParameter.ActualName = CurrentWorstOverloadParameter.Name;

      DataTableValuesCollector overloadDataTablesCollector = new DataTableValuesCollector();
      overloadDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("BestOverload", null, BestOverloadParameter.Name));
      overloadDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentBestOverload", null, CurrentBestOverloadParameter.Name));
      overloadDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentAverageOverload", null, CurrentAverageOverloadParameter.Name));
      overloadDataTablesCollector.CollectedValues.Add(new LookupParameter<DoubleValue>("CurrentWorstOverload", null, CurrentWorstOverloadParameter.Name));
      overloadDataTablesCollector.DataTableParameter.ActualName = OverloadsParameter.Name;

      resultsCollector.CollectedValues.Add(new LookupParameter<DataTable>(OverloadsParameter.Name));

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
      calculator.Successor = overloadDataTablesCollector;
      overloadDataTablesCollector.Successor = tardinessDataTablesCollector;
      tardinessDataTablesCollector.Successor = distanceDataTablesCollector;
      distanceDataTablesCollector.Successor = travelTimeDataTablesCollector;
      travelTimeDataTablesCollector.Successor = vehiclesUtilizedDataTablesCollector;
      vehiclesUtilizedDataTablesCollector.Successor = resultsCollector;
      resultsCollector.Successor = null;
      #endregion

      Initialize();
    }
    [StorableConstructor]
    private BestAverageWorstVRPToursAnalyzer(bool deserializing) : base() { }
    private BestAverageWorstVRPToursAnalyzer(BestAverageWorstVRPToursAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestAverageWorstVRPToursAnalyzer(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }
    private void Initialize() {
      OverloadParameter.DepthChanged += new EventHandler(OverloadParameter_DepthChanged);
      TardinessParameter.DepthChanged += new EventHandler(TardinessParameter_DepthChanged);
      DistanceParameter.DepthChanged += new EventHandler(DistanceParameter_DepthChanged);
      TravelTimeParameter.DepthChanged += new EventHandler(TravelTimeParameter_DepthChanged);
      VehiclesUtilizedParameter.DepthChanged += new EventHandler(VehiclesUtilizedParameter_DepthChanged);
    }


    void OverloadParameter_DepthChanged(object sender, EventArgs e) {
      BestAverageWorstCalculator.OverloadParameter.Depth = OverloadParameter.Depth;
      BestMemorizer.OverloadParameter.Depth = OverloadParameter.Depth;
    }

    void TardinessParameter_DepthChanged(object sender, EventArgs e) {
      BestAverageWorstCalculator.TardinessParameter.Depth = TardinessParameter.Depth;
      BestMemorizer.TardinessParameter.Depth = TardinessParameter.Depth;
    }

    void DistanceParameter_DepthChanged(object sender, EventArgs e) {
      BestAverageWorstCalculator.DistanceParameter.Depth = DistanceParameter.Depth;
      BestMemorizer.DistanceParameter.Depth = DistanceParameter.Depth;
    }

    void TravelTimeParameter_DepthChanged(object sender, EventArgs e) {
      BestAverageWorstCalculator.TravelTimeParameter.Depth = TravelTimeParameter.Depth;
      BestMemorizer.TravelTimeParameter.Depth = DistanceParameter.Depth;
    }

    void VehiclesUtilizedParameter_DepthChanged(object sender, EventArgs e) {
      BestAverageWorstCalculator.VehiclesUtilizedParameter.Depth = VehiclesUtilizedParameter.Depth;
      BestMemorizer.VehiclesUtilizedParameter.Depth = VehiclesUtilizedParameter.Depth;
    }
  }
}
