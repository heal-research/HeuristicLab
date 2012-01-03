#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting {
  /// <summary>
  /// An operator for analyzing the best solution of Vehicle Routing Problems.
  /// </summary>
  [Item("BestVRPSolutionAnalyzer", "An operator for analyzing the best solution of Vehicle Routing Problems.")]
  [StorableClass]
  public sealed class BestVRPSolutionAnalyzer : SingleSuccessorOperator, IAnalyzer {
    public bool EnabledByDefault {
      get { return true; }
    }

    public ScopeTreeLookupParameter<IVRPEncoding> VRPToursParameter {
      get { return (ScopeTreeLookupParameter<IVRPEncoding>)Parameters["VRPTours"]; }
    }
    public ILookupParameter<DoubleMatrix> DistanceMatrixParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["DistanceMatrix"]; }
    }
    public ILookupParameter<BoolValue> UseDistanceMatrixParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["UseDistanceMatrix"]; }
    }
    public LookupParameter<DoubleMatrix> CoordinatesParameter {
      get { return (LookupParameter<DoubleMatrix>)Parameters["Coordinates"]; }
    }
    public ILookupParameter<DoubleArray> ReadyTimeParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["ReadyTime"]; }
    }
    public ILookupParameter<DoubleArray> DueTimeParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["DueTime"]; }
    }
    public ILookupParameter<DoubleArray> ServiceTimeParameter {
      get { return (ILookupParameter<DoubleArray>)Parameters["ServiceTime"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> OverloadParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Overload"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> TardinessParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Tardiness"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> DistanceParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Distance"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> TravelTimeParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["TravelTime"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> VehiclesUtilizedParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["VehiclesUtilized"]; }
    }
    public LookupParameter<VRPSolution> BestSolutionParameter {
      get { return (LookupParameter<VRPSolution>)Parameters["BestSolution"]; }
    }
    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    public LookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    public LookupParameter<IVRPEncoding> BestKnownSolutionParameter {
      get { return (LookupParameter<IVRPEncoding>)Parameters["BestKnownSolution"]; }
    }

    [StorableConstructor]
    private BestVRPSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    private BestVRPSolutionAnalyzer(BestVRPSolutionAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }
    public BestVRPSolutionAnalyzer()
      : base() {
      Parameters.Add(new ScopeTreeLookupParameter<IVRPEncoding>("VRPTours", "The VRP tours which should be evaluated."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Coordinates", "The x- and y-Coordinates of the cities."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("DistanceMatrix", "The matrix which contains the distances between the cities."));
      Parameters.Add(new LookupParameter<BoolValue>("UseDistanceMatrix", "True if a distance matrix should be calculated and used for evaluation, otherwise false."));
      Parameters.Add(new LookupParameter<DoubleArray>("ReadyTime", "The ready time of each customer."));
      Parameters.Add(new LookupParameter<DoubleArray>("DueTime", "The due time of each customer."));
      Parameters.Add(new LookupParameter<DoubleArray>("ServiceTime", "The service time of each customer."));

      Parameters.Add(new LookupParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this VRP instance."));
      Parameters.Add(new LookupParameter<IVRPEncoding>("BestKnownSolution", "The best known solution of this VRP instance."));

      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the VRP solutions which should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Distance", "The distances of the VRP solutions which should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Overload", "The overloads of the VRP solutions which should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Tardiness", "The tardiness of the VRP solutions which should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("TravelTime", "The travel times of the VRP solutions which should be analyzed."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("VehiclesUtilized", "The utilized vehicles of the VRP solutions which should be analyzed."));
      Parameters.Add(new LookupParameter<VRPSolution>("BestSolution", "The best VRP solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the best VRP solution should be stored."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestVRPSolutionAnalyzer(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserializationHook() {
      #region Backwards Compatibility
      if (!Parameters.ContainsKey("BestKnownQuality")) {
        Parameters.Add(new LookupParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this VRP instance."));
      }
      if (!Parameters.ContainsKey("BestKnownSolution")) {
        Parameters.Add(new LookupParameter<IVRPEncoding>("BestKnownSolution", "The best known solution of this VRP instance."));
      }
      #endregion
    }

    public override IOperation Apply() {
      DoubleMatrix coordinates = CoordinatesParameter.ActualValue;
      ItemArray<IVRPEncoding> solutions = VRPToursParameter.ActualValue;
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      ItemArray<DoubleValue> overloads = OverloadParameter.ActualValue;
      ItemArray<DoubleValue> tardinesses = TardinessParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;
      ItemArray<DoubleValue> distances = DistanceParameter.ActualValue;
      ItemArray<DoubleValue> travelTimes = TravelTimeParameter.ActualValue;
      ItemArray<DoubleValue> vehiclesUtilizations = VehiclesUtilizedParameter.ActualValue;

      DoubleValue bestKnownQuality = BestKnownQualityParameter.ActualValue;

      int i = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;

      IVRPEncoding best = solutions[i].Clone() as IVRPEncoding;
      VRPSolution solution = BestSolutionParameter.ActualValue;
      if (solution == null) {
        solution = new VRPSolution(coordinates, best.Clone() as IVRPEncoding, new DoubleValue(qualities[i].Value),
          new DoubleValue(distances[i].Value), new DoubleValue(overloads[i].Value), new DoubleValue(tardinesses[i].Value),
          new DoubleValue(travelTimes[i].Value), new DoubleValue(vehiclesUtilizations[i].Value),
          DistanceMatrixParameter.ActualValue, UseDistanceMatrixParameter.ActualValue,
          ReadyTimeParameter.ActualValue, DueTimeParameter.ActualValue, ServiceTimeParameter.ActualValue);
        BestSolutionParameter.ActualValue = solution;
        results.Add(new Result("Best VRP Solution", solution));

        results.Add(new Result("Best VRP Solution TravelTime", new DoubleValue(travelTimes[i].Value)));
        results.Add(new Result("Best VRP Solution Distance", new DoubleValue(distances[i].Value)));
        results.Add(new Result("Best VRP Solution VehicleUtilization", new DoubleValue(vehiclesUtilizations[i].Value)));
        results.Add(new Result("Best VRP Solution Overload", new DoubleValue(overloads[i].Value)));
        results.Add(new Result("Best VRP Solution Tardiness", new DoubleValue(tardinesses[i].Value)));

      } else {
        if (qualities[i].Value <= solution.Quality.Value) {
          solution.Coordinates = coordinates;
          solution.Solution = best.Clone() as IVRPEncoding;
          solution.Quality.Value = qualities[i].Value;
          solution.Distance.Value = (results["Best VRP Solution Distance"].Value as DoubleValue).Value = distances[i].Value;
          solution.Overload.Value = (results["Best VRP Solution Overload"].Value as DoubleValue).Value = overloads[i].Value;
          solution.Tardiness.Value = (results["Best VRP Solution Tardiness"].Value as DoubleValue).Value = tardinesses[i].Value;
          solution.TravelTime.Value = (results["Best VRP Solution TravelTime"].Value as DoubleValue).Value = travelTimes[i].Value;
          solution.VehicleUtilization.Value = (results["Best VRP Solution VehicleUtilization"].Value as DoubleValue).Value = vehiclesUtilizations[i].Value;
          solution.DistanceMatrix = DistanceMatrixParameter.ActualValue;
          solution.UseDistanceMatrix = UseDistanceMatrixParameter.ActualValue;
          solution.ReadyTime = ReadyTimeParameter.ActualValue;
          solution.DueTime = DueTimeParameter.ActualValue;
          solution.ServiceTime = ServiceTimeParameter.ActualValue;
        }
      }

      if (bestKnownQuality == null ||
          qualities[i].Value < bestKnownQuality.Value) {
        BestKnownQualityParameter.ActualValue = new DoubleValue(qualities[i].Value);
        BestKnownSolutionParameter.ActualValue = (IVRPEncoding)best.Clone();
      }

      return base.Apply();
    }
  }
}
