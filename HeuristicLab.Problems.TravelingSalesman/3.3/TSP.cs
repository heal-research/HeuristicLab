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

    public override bool Maximization => false;

    [Storable] public IValueParameter<ITSPData> TSPDataParameter { get; private set; }
    [Storable] public IValueParameter<ITSPSolution> BestKnownSolutionParameter { get; private set; }

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
    }

    public TSP() : base(new PermutationEncoding("Tour", 16, PermutationTypes.RelativeUndirected)) {
      Parameters.Add(TSPDataParameter = new ValueParameter<ITSPData>("TSPData", "The main parameters of the TSP."));
      Parameters.Add(BestKnownSolutionParameter = new OptionalValueParameter<ITSPSolution>("BestKnownSolution", "The best known solution."));

      TSPData = new EuclideanTSPData();
      Encoding.Length = TSPData.Cities;

      InitializeOperators();
    }

    // TODO: encoding length should not be changeable

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TSP(this, cloner);
    }

    public override double Evaluate(Permutation tour, IRandom random) {
      return Evaluate(tour);
    }

    public double Evaluate(Permutation tour) {
      var tourLength = 0.0;
      for (var i = 0; i < tour.Length - 1; i++) {
        tourLength += TSPData.GetDistance(tour[i], tour[i + 1]);
      }
      tourLength += TSPData.GetDistance(tour[tour.Length - 1], tour[0]);
      return tourLength;
    }

    public override void Analyze(Permutation[] solutions, double[] qualities, ResultCollection results, IRandom random) {
      base.Analyze(solutions, qualities, results, random);
      int i = -1;
      if (!Maximization)
        i = qualities.Select((x, index) => new { index, Fitness = x }).OrderBy(x => x.Fitness).First().index;
      else i = qualities.Select((x, index) => new { index, Fitness = x }).OrderByDescending(x => x.Fitness).First().index;

      if (double.IsNaN(BestKnownQuality) ||
          Maximization && qualities[i] > BestKnownQuality ||
          !Maximization && qualities[i] < BestKnownQuality) {
        var bestKnown = TSPData.GetSolution(solutions[i], qualities[i]);
        BestKnownQualityParameter.Value = new DoubleValue(qualities[i]);
        BestKnownSolutionParameter.Value = bestKnown;
      }

      IResult bestSolutionResult;
      if (results.TryGetValue("Best TSP Solution", out bestSolutionResult)) {
        var bestSolution = bestSolutionResult.Value as ITSPSolution;
        if (bestSolution == null || Maximization && bestSolution.TourLength.Value < qualities[i]
          || !Maximization && bestSolution.TourLength.Value > qualities[i]) {
          bestSolutionResult.Value = TSPData.GetSolution(solutions[i], qualities[i]);
        }
      } else results.Add(new Result("Best TSP Solution", TSPData.GetSolution(solutions[i], qualities[i])));
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

      Encoding.Length = data.Dimension;
      Name = data.Name;
      Description = data.Description;

      if (data.Dimension <= DistanceMatrixSizeLimit) {
        TSPData = new MatrixTSPData(data.Name, data.GetDistanceMatrix(), data.Coordinates) { Description = data.Description };
      } else if (data.DistanceMeasure == DistanceMeasure.Direct && data.Distances != null) {
        TSPData = new MatrixTSPData(data.Name, data.Distances, data.Coordinates) { Description = data.Description };
      } else {
        switch (data.DistanceMeasure) {
          case DistanceMeasure.Att:
            TSPData = new AttTSPData(data.Name, data.Coordinates) { Description = data.Description };
            break;
          case DistanceMeasure.Euclidean:
            TSPData = new EuclideanTSPData(data.Name, data.Coordinates, EuclideanTSPData.DistanceRounding.None) { Description = data.Description };
            break;
          case DistanceMeasure.RoundedEuclidean:
            TSPData = new EuclideanTSPData(data.Name, data.Coordinates, EuclideanTSPData.DistanceRounding.Midpoint) { Description = data.Description };
            break;
          case DistanceMeasure.UpperEuclidean:
            TSPData = new EuclideanTSPData(data.Name, data.Coordinates, EuclideanTSPData.DistanceRounding.Ceiling) { Description = data.Description };
            break;
          case DistanceMeasure.Geo:
            TSPData = new GeoTSPData(data.Name, data.Coordinates) { Description = data.Description };
            break;
          case DistanceMeasure.Manhattan:
            TSPData = new ManhattanTSPData(data.Name, data.Coordinates) { Description = data.Description };
            break;
          case DistanceMeasure.Maximum:
            TSPData = new MaximumTSPData(data.Name, data.Coordinates) { Description = data.Description };
            break;
          default:
            throw new System.IO.InvalidDataException("An unknown distance measure is given in the instance!");
        }
      }
      BestKnownSolution = null;
      BestKnownQuality = double.NaN;

      if (data.BestKnownTour != null) {
        try {
          var tour = new Permutation(PermutationTypes.RelativeUndirected, data.BestKnownTour);
          var tourLength = Evaluate(tour);
          BestKnownSolution = new TSPSolution(data.Coordinates != null ? new DoubleMatrix(data.Coordinates) : null, tour, new DoubleValue(tourLength));
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

    public TSPData Export() {
      var instance = TSPData.Export();
      if (!double.IsNaN(BestKnownQuality))
        instance.BestKnownQuality = BestKnownQuality;
      if (BestKnownSolution?.Tour != null)
        instance.BestKnownTour = BestKnownSolution.Tour.ToArray();
      return instance;
    }


    protected override void OnEncodingChanged() {
      base.OnEncodingChanged();
      Encoding.Length = TSPData.Cities;
      ParameterizeOperators();
    }

    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      ParameterizeOperators();
    }

    private void InitializeOperators() {
      Operators.Add(new TSPImprovementOperator());
      Operators.Add(new TSPMultipleGuidesPathRelinker());
      Operators.Add(new TSPPathRelinker());
      Operators.Add(new TSPSimultaneousPathRelinker());

      Operators.Add(new TSPAlleleFrequencyAnalyzer());
      foreach (var op in ApplicationManager.Manager.GetInstances<ITSPMoveEvaluator>()) {
        Encoding.ConfigureOperator(op);
        Operators.Add(op);
      }
      ParameterizeOperators();
    }

    private void ParameterizeOperators() {
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
      foreach (var op in Operators.OfType<ISingleObjectiveImprovementOperator>()) {
        op.SolutionParameter.ActualName = Encoding.Name;
        op.SolutionParameter.Hidden = true;
      }
      foreach (ISingleObjectivePathRelinker op in Operators.OfType<ISingleObjectivePathRelinker>()) {
        op.ParentsParameter.ActualName = Encoding.Name;
        op.ParentsParameter.Hidden = true;
      }
      foreach (ISolutionSimilarityCalculator op in Operators.OfType<ISolutionSimilarityCalculator>()) {
        op.SolutionVariableName = Encoding.Name;
        op.QualityVariableName = Evaluator.QualityParameter.ActualName;
      }
    }
  }
}
