#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.TravelingSalesman {
  [Item("Traveling Salesman Problem (TSP)", "Represents a symmetric Traveling Salesman Problem.")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 100)]
  [StorableType("8415476a-69de-45ad-95be-298ed7c97e84")]
  public class TSP : PermutationProblem, IProblemInstanceConsumer<TSPData>, IProblemInstanceExporter<TSPData> {
    /// <summary>
    /// This limit governs when a distance matrix is used. For all problems smaller than that, the distance matrix is
    /// computed. This greatly speeds up computation time.
    /// </summary>
    public static int DistanceMatrixSizeLimit { get; set; } = 1000;

    [Storable] public IValueParameter<ITSPData> TSPDataParameter { get; private set; }
    [Storable] public IValueParameter<ITSPSolution> BestKnownSolutionParameter { get; private set; }
    [Storable] protected IResultParameter<ITSPSolution> BestTSPSolutionParameter { get; private set; }

    public ITSPData TSPData {
      get { return TSPDataParameter.Value; }
      set { TSPDataParameter.Value = value; }
    }
    public ITSPSolution BestKnownSolution {
      get { return BestKnownSolutionParameter.Value; }
      set { BestKnownSolutionParameter.Value = value; }
    }

    [StorableConstructor]
    protected TSP(StorableConstructorFlag _) : base(_) { }

    protected TSP(TSP original, Cloner cloner)
      : base(original, cloner) {
      TSPDataParameter = cloner.Clone(original.TSPDataParameter);
      BestKnownSolutionParameter = cloner.Clone(original.BestKnownSolutionParameter);
      BestTSPSolutionParameter = cloner.Clone(original.BestTSPSolutionParameter);
    }

    public TSP() : base(new PermutationEncoding("Tour", 16, PermutationTypes.RelativeUndirected)) {
      Maximization = false;
      DimensionRefParameter.ReadOnly = Encoding.LengthParameter.ReadOnly = true;
      Parameters.Add(TSPDataParameter = new ValueParameter<ITSPData>("TSPData", "The main parameters of the TSP."));
      Parameters.Add(BestKnownSolutionParameter = new OptionalValueParameter<ITSPSolution>("BestKnownSolution", "The best known solution."));
      Parameters.Add(BestTSPSolutionParameter = new ResultParameter<ITSPSolution>("Best TSP Solution", "The best so far solution found."));

      TSPData = new EuclideanTSPData();
      Dimension = TSPData.Cities;

      InitializeOperators();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TSP(this, cloner);
    }

    public override ISingleObjectiveEvaluationResult Evaluate(Permutation tour, IRandom random, CancellationToken cancellationToken) {
      var quality = Evaluate(tour);
      return new SingleObjectiveEvaluationResult(quality);
    }

    public double Evaluate(Permutation tour) {
      var tourLength = 0.0;
      for (var i = 0; i < tour.Length - 1; i++) {
        tourLength += TSPData.GetDistance(tour[i], tour[i + 1]);
      }
      tourLength += TSPData.GetDistance(tour[tour.Length - 1], tour[0]);
      return tourLength;
    }

    public override void Analyze(ISingleObjectiveSolutionContext<Permutation>[] solutionContexts, IRandom random) {
      base.Analyze(solutionContexts, random);

      //TODO reimplement code below using results directly
      //int i = -1;
      //if (!Maximization)
      //  i = qualities.Select((x, index) => new { index, Fitness = x }).OrderBy(x => x.Fitness).First().index;
      //else i = qualities.Select((x, index) => new { index, Fitness = x }).OrderByDescending(x => x.Fitness).First().index;

      //if (double.IsNaN(BestKnownQuality) ||
      //    Maximization && qualities[i] > BestKnownQuality ||
      //    !Maximization && qualities[i] < BestKnownQuality) {
      //  var bestKnown = TSPData.GetSolution(solutions[i], qualities[i]);
      //  BestKnownQualityParameter.Value = new DoubleValue(qualities[i]);
      //  BestKnownSolutionParameter.Value = bestKnown;
      //}

      //var bestSolution = BestTSPSolutionParameter.ActualValue;
      //if (bestSolution == null || IsBetter(qualities[i], bestSolution.TourLength.Value)) {
      //  BestTSPSolutionParameter.ActualValue = TSPData.GetSolution(solutions[i], qualities[i]);
      //}
    }

    public override IEnumerable<Permutation> GetNeighbors(Permutation solution, IRandom random) {
      foreach (var move in ExhaustiveInversionMoveGenerator.Generate(solution)) {
        var clone = (Permutation)solution.Clone();
        InversionManipulator.Apply(clone, move.Index1, move.Index2);
        yield return clone;
      }
    }

    public void Load(TSPData data) {
      if (data.Coordinates == null && data.Distances == null)
        throw new System.IO.InvalidDataException("The given instance specifies neither coordinates nor distances!");
      if (data.Dimension > DistanceMatrixSizeLimit && (data.DistanceMeasure == DistanceMeasure.Att
        || data.DistanceMeasure == DistanceMeasure.Manhattan
        || data.DistanceMeasure == DistanceMeasure.Maximum))
        throw new System.IO.InvalidDataException("The given instance uses an unsupported distance measure and is too large for using a distance matrix.");
      if (data.Coordinates != null && data.Coordinates.GetLength(1) != 2)
        throw new System.IO.InvalidDataException("The coordinates of the given instance are not in the right format, there need to be one row for each customer and two columns for the x and y coordinates.");

      Dimension = data.Dimension;
      Name = data.Name;
      Description = data.Description;

      TSPData = GetDataFromInstance(data);
      BestKnownSolution = null;
      BestKnownQuality = double.NaN;

      if (data.BestKnownTour != null) {
        try {
          var tour = new Permutation(PermutationTypes.RelativeUndirected, data.BestKnownTour);
          var tourLength = Evaluate(tour);
          BestKnownSolution = new TSPSolution(TSPData, tour, new DoubleValue(tourLength));
          BestKnownQuality = tourLength;
        } catch (InvalidOperationException) {
          if (data.BestKnownQuality.HasValue)
            BestKnownQuality = data.BestKnownQuality.Value;
        }
      } else if (data.BestKnownQuality.HasValue) {
        BestKnownQuality = data.BestKnownQuality.Value;
      }
      OnReset();
    }

    public static ITSPData GetDataFromInstance(TSPData input) {
      ITSPData tspData;
      if (input.Dimension <= DistanceMatrixSizeLimit) {
        tspData = new MatrixTSPData(input.Name, input.GetDistanceMatrix(), input.Coordinates) { Description = input.Description };
      } else if (input.DistanceMeasure == DistanceMeasure.Direct && input.Distances != null) {
        tspData = new MatrixTSPData(input.Name, input.Distances, input.Coordinates) { Description = input.Description };
      } else {
        switch (input.DistanceMeasure) {
          case DistanceMeasure.Att:
            tspData = new AttTSPData(input.Name, input.Coordinates) { Description = input.Description };
            break;
          case DistanceMeasure.Euclidean:
            tspData = new EuclideanTSPData(input.Name, input.Coordinates, EuclideanTSPData.DistanceRounding.None) { Description = input.Description };
            break;
          case DistanceMeasure.RoundedEuclidean:
            tspData = new EuclideanTSPData(input.Name, input.Coordinates, EuclideanTSPData.DistanceRounding.Midpoint) { Description = input.Description };
            break;
          case DistanceMeasure.UpperEuclidean:
            tspData = new EuclideanTSPData(input.Name, input.Coordinates, EuclideanTSPData.DistanceRounding.Ceiling) { Description = input.Description };
            break;
          case DistanceMeasure.Geo:
            tspData = new GeoTSPData(input.Name, input.Coordinates) { Description = input.Description };
            break;
          case DistanceMeasure.Manhattan:
            tspData = new ManhattanTSPData(input.Name, input.Coordinates) { Description = input.Description };
            break;
          case DistanceMeasure.Maximum:
            tspData = new MaximumTSPData(input.Name, input.Coordinates) { Description = input.Description };
            break;
          default:
            throw new System.IO.InvalidDataException("An unknown distance measure is given in the instance!");
        }
      }
      return tspData;
    }

    public TSPData Export() {
      var instance = TSPData.Export();
      if (!double.IsNaN(BestKnownQuality))
        instance.BestKnownQuality = BestKnownQuality;
      if (BestKnownSolution?.Tour != null)
        instance.BestKnownTour = BestKnownSolution.Tour.ToArray();
      return instance;
    }

    private void InitializeOperators() {
      var ops = new List<IItem>() { new TSPImprovementOperator(), new TSPMultipleGuidesPathRelinker(),
        new TSPPathRelinker(), new TSPSimultaneousPathRelinker(), new TSPAlleleFrequencyAnalyzer() };
      foreach (var op in ApplicationManager.Manager.GetInstances<ITSPMoveEvaluator>()) {
        ops.Add(op);
      }
      Encoding.ConfigureOperators(ops);
      Operators.AddRange(ops);
    }

    protected override void ParameterizeOperators() {
      base.ParameterizeOperators();
      Parameterize();
    }

    private void Parameterize() {
      foreach (var op in Operators.OfType<TSPAlleleFrequencyAnalyzer>()) {
        op.MaximizationParameter.ActualName = MaximizationParameter.Name;
        op.TSPDataParameter.ActualName = TSPDataParameter.Name;
        op.SolutionParameter.ActualName = Encoding.Name;
        op.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        op.BestKnownSolutionParameter.ActualName = BestKnownSolutionParameter.Name;
        op.ResultsParameter.ActualName = "Results";
      }
      foreach (var op in Operators.OfType<ITSPMoveEvaluator>()) {
        op.TSPDataParameter.ActualName = TSPDataParameter.Name;
        op.TSPDataParameter.Hidden = true;
        op.TourLengthParameter.ActualName = Evaluator.QualityParameter.ActualName;
        op.TourLengthParameter.Hidden = true;
        op.TSPTourParameter.ActualName = Encoding.Name;
        op.TSPTourParameter.Hidden = true;
      }
    }
  }
}
