#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.TravelingSalesman {
  public enum TSPDistanceFunction { Euclidean, RoundedEuclidean, UpperEuclidean, Geo, DistanceMatrix };

  [Item("Traveling Salesman Problem (TSP)", "Represents a symmetric Traveling Salesman Problem.")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 999)]
  [StorableClass]
  public sealed class TravelingSalesmanProblem : SingleObjectiveBasicProblem<PermutationEncoding>, IStorableContent, IProblemInstanceConsumer<TSPData> {
    public const double PI = 3.141592;
    public const double RADIUS = 6378.388;
    private static readonly int DistanceMatrixSizeLimit = 1000;

    public override bool Maximization {
      get { return false; }
    }

    #region Parameter Properties
    [Storable]
    private IFixedValueParameter<EnumValue<TSPDistanceFunction>> distanceFunctionParameter;
    public IFixedValueParameter<EnumValue<TSPDistanceFunction>> DistanceFunctionParameter {
      get { return distanceFunctionParameter; }
    }
    [Storable]
    private OptionalValueParameter<DoubleMatrix> coordinatesParameter;
    public OptionalValueParameter<DoubleMatrix> CoordinatesParameter {
      get { return coordinatesParameter; }
    }
    [Storable]
    private OptionalValueParameter<DistanceMatrix> distanceMatrixParameter;
    public OptionalValueParameter<DistanceMatrix> DistanceMatrixParameter {
      get { return distanceMatrixParameter; }
    }
    [Storable]
    private IFixedValueParameter<BoolValue> useDistanceMatrixParameter;
    public IFixedValueParameter<BoolValue> UseDistanceMatrixParameter {
      get { return useDistanceMatrixParameter; }
    }
    [Storable]
    private OptionalValueParameter<Permutation> bestKnownSolutionParameter;
    public OptionalValueParameter<Permutation> BestKnownSolutionParameter {
      get { return bestKnownSolutionParameter; }
    }
    #endregion

    #region Properties
    public TSPDistanceFunction DistanceFunction {
      get { return distanceFunctionParameter.Value.Value; }
      set { distanceFunctionParameter.Value.Value = value; }
    }
    public DoubleMatrix Coordinates {
      get { return coordinatesParameter.Value; }
      set { coordinatesParameter.Value = value; }
    }
    public DistanceMatrix DistanceMatrix {
      get { return distanceMatrixParameter.Value; }
      set { distanceMatrixParameter.Value = value; }
    }
    public bool UseDistanceMatrix {
      get { return useDistanceMatrixParameter.Value.Value; }
      set { useDistanceMatrixParameter.Value.Value = value; }
    }
    public Permutation BestKnownSolution {
      get { return bestKnownSolutionParameter.Value; }
      set { bestKnownSolutionParameter.Value = value; }
    }
    private BestTSPSolutionAnalyzer BestTSPSolutionAnalyzer {
      get { return Operators.OfType<BestTSPSolutionAnalyzer>().FirstOrDefault(); }
    }
    private TSPAlleleFrequencyAnalyzer TSPAlleleFrequencyAnalyzer {
      get { return Operators.OfType<TSPAlleleFrequencyAnalyzer>().FirstOrDefault(); }
    }
    #endregion

    [StorableConstructor]
    private TravelingSalesmanProblem(bool deserializing) : base(deserializing) { }
    private TravelingSalesmanProblem(TravelingSalesmanProblem original, Cloner cloner)
      : base(original, cloner) {
      distanceFunctionParameter = cloner.Clone(original.distanceFunctionParameter);
      coordinatesParameter = cloner.Clone(original.coordinatesParameter);
      distanceMatrixParameter = cloner.Clone(original.distanceMatrixParameter);
      useDistanceMatrixParameter = cloner.Clone(original.useDistanceMatrixParameter);
      bestKnownSolutionParameter = cloner.Clone(original.bestKnownSolutionParameter);
      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new TravelingSalesmanProblem(this, cloner);
    }
    public TravelingSalesmanProblem() : base() {
      Encoding = new PermutationEncoding("TSPTour") { Length = 16 };

      Parameters.Add(distanceFunctionParameter = new FixedValueParameter<EnumValue<TSPDistanceFunction>>("DistanceFunction", "The distance function that is used to calculate distance among the coordinates.", new EnumValue<TSPDistanceFunction>(TSPDistanceFunction.RoundedEuclidean)));
      Parameters.Add(coordinatesParameter = new OptionalValueParameter<DoubleMatrix>("Coordinates", "The x- and y-Coordinates of the cities."));
      Parameters.Add(distanceMatrixParameter = new OptionalValueParameter<DistanceMatrix>("DistanceMatrix", "The matrix which contains the distances between the cities."));
      Parameters.Add(useDistanceMatrixParameter = new FixedValueParameter<BoolValue>("UseDistanceMatrix", "True if the coordinates based evaluators should calculate the distance matrix from the coordinates and use it for evaluation similar to the distance matrix evaluator, otherwise false.", new BoolValue(true)));
      Parameters.Add(bestKnownSolutionParameter = new OptionalValueParameter<Permutation>("BestKnownSolution", "The best known solution of this TSP instance."));
      
      useDistanceMatrixParameter.Hidden = true;
      distanceMatrixParameter.ReactOnValueToStringChangedAndValueItemImageChanged = false;

      Coordinates = new DoubleMatrix(new double[,] {
        { 100, 100 }, { 100, 200 }, { 100, 300 }, { 100, 400 },
        { 200, 100 }, { 200, 200 }, { 200, 300 }, { 200, 400 },
        { 300, 100 }, { 300, 200 }, { 300, 300 }, { 300, 400 },
        { 400, 100 }, { 400, 200 }, { 400, 300 }, { 400, 400 }
      });
      
      Evaluator.QualityParameter.ActualName = "TSPTourLength";

      InitializeOperators();
      RegisterEventHandlers();
    }
    
    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    public override double Evaluate(Individual individual, IRandom random) {
      var tour = individual.Permutation(Encoding.Name);
      return Evaluate(tour);
    }

    public double Evaluate(Permutation tour) {
      if (UseDistanceMatrix) return EvaluateWithDistanceMatrix(tour);

      switch (DistanceFunction) {
        case TSPDistanceFunction.DistanceMatrix:
          return EvaluateWithDistanceMatrix(tour);
        case TSPDistanceFunction.Euclidean:
        case TSPDistanceFunction.RoundedEuclidean:
        case TSPDistanceFunction.UpperEuclidean:
        case TSPDistanceFunction.Geo:
          return EvaluateWithDistanceCalculation(tour);
        default: throw new InvalidOperationException(string.Format("Unknown distance function: {0}", DistanceFunction));
      }
    }

    private double EvaluateWithDistanceMatrix(Permutation tour) {
      var distances = DistanceMatrix;
      double length = 0;
      for (var i = 0; i < tour.Length - 1; i++)
        length += distances[tour[i], tour[i + 1]];
      length += distances[tour[tour.Length - 1], tour[0]];
      return length;
    }

    private double EvaluateWithDistanceCalculation(Permutation tour) {
      var coordinates = Coordinates;
      var distanceFunction = DistanceFunction;
      double length = 0;
      for (var i = 0; i < tour.Length - 1; i++)
        length += CalculateDistance(distanceFunction, coordinates[tour[i], 0], coordinates[tour[i], 1], coordinates[tour[i + 1], 0], coordinates[tour[i + 1], 1]);
      length += CalculateDistance(distanceFunction, coordinates[tour[tour.Length - 1], 0], coordinates[tour[tour.Length - 1], 1], coordinates[tour[0], 0], coordinates[tour[0], 1]);
      return length;
    }

    private void RegisterEventHandlers() {
      coordinatesParameter.ValueChanged += coordinatesParameter_ValueChanged;
      if (Coordinates != null) {
        Coordinates.ItemChanged += Coordinates_ItemChanged;
        Coordinates.Reset += Coordinates_Reset;
      }
      Evaluator.QualityParameter.ActualNameChanged += Evaluator_QualityParameter_ActualNameChanged;
    }

    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeAnalyzers();
      ParameterizeOperators();
    }

    private void Coordinates_Reset(object sender, EventArgs e) {
      DistanceMatrix = null;
    }

    private void Coordinates_ItemChanged(object sender, EventArgs<int, int> e) {
      DistanceMatrix = null;
    }

    private void coordinatesParameter_ValueChanged(object sender, EventArgs e) {
      if (Coordinates != null) {
        Coordinates.ItemChanged += new EventHandler<EventArgs<int, int>>(Coordinates_ItemChanged);
        Coordinates.Reset += new EventHandler(Coordinates_Reset);
        DistanceMatrix = null;
      }
    }

    private void InitializeOperators() {
      Operators.Add(new TSPImprovementOperator());
      Operators.Add(new TSPMultipleGuidesPathRelinker());
      Operators.Add(new TSPPathRelinker());
      Operators.Add(new TSPSimultaneousPathRelinker());
      Operators.Add(new HammingSimilarityCalculator());
      Operators.Add(new QualitySimilarityCalculator());

      Operators.Add(new BestTSPSolutionAnalyzer());
      Operators.Add(new TSPAlleleFrequencyAnalyzer());
      Operators.Add(new PopulationSimilarityAnalyzer(Operators.OfType<ISolutionSimilarityCalculator>()));
      Operators.AddRange(ApplicationManager.Manager.GetInstances<ITSPMoveEvaluator>());

      ParameterizeAnalyzers();
      ParameterizeOperators();
      UpdateMoveEvaluators();
    }
    private void UpdateMoveEvaluators() {
      ParameterizeOperators();
      OnOperatorsChanged();
    }
    private void ParameterizeAnalyzers() {
      if (BestTSPSolutionAnalyzer != null) {
        BestTSPSolutionAnalyzer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        BestTSPSolutionAnalyzer.CoordinatesParameter.ActualName = coordinatesParameter.Name;
        BestTSPSolutionAnalyzer.PermutationParameter.ActualName = Encoding.SolutionCreator.PermutationParameter.ActualName;
        BestTSPSolutionAnalyzer.ResultsParameter.ActualName = "Results";
        BestTSPSolutionAnalyzer.BestKnownQualityParameter.ActualName = BestKnownQualityParameter.Name;
        BestTSPSolutionAnalyzer.BestKnownSolutionParameter.ActualName = bestKnownSolutionParameter.Name;
        BestTSPSolutionAnalyzer.MaximizationParameter.ActualName = ((ISingleObjectiveHeuristicOptimizationProblem)this).MaximizationParameter.Name;
      }

      if (TSPAlleleFrequencyAnalyzer != null) {
        TSPAlleleFrequencyAnalyzer.MaximizationParameter.ActualName = ((ISingleObjectiveHeuristicOptimizationProblem)this).MaximizationParameter.Name;
        TSPAlleleFrequencyAnalyzer.CoordinatesParameter.ActualName = coordinatesParameter.Name;
        TSPAlleleFrequencyAnalyzer.DistanceMatrixParameter.ActualName = distanceMatrixParameter.Name;
        TSPAlleleFrequencyAnalyzer.SolutionParameter.ActualName = Encoding.SolutionCreator.PermutationParameter.ActualName;
        TSPAlleleFrequencyAnalyzer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        TSPAlleleFrequencyAnalyzer.BestKnownSolutionParameter.ActualName = bestKnownSolutionParameter.Name;
        TSPAlleleFrequencyAnalyzer.ResultsParameter.ActualName = "Results";
      }
    }
    private void ParameterizeOperators() {
      foreach (ITSPPathMoveEvaluator op in Operators.OfType<ITSPPathMoveEvaluator>()) {
        op.DistanceFunctionParameter.ActualName = distanceFunctionParameter.Name;
        op.DistanceFunctionParameter.Hidden = true;
        op.CoordinatesParameter.ActualName = coordinatesParameter.Name;
        op.CoordinatesParameter.Hidden = true;
        op.DistanceMatrixParameter.ActualName = distanceMatrixParameter.Name;
        op.DistanceMatrixParameter.Hidden = true;
        op.UseDistanceMatrixParameter.ActualName = useDistanceMatrixParameter.Name;
        op.UseDistanceMatrixParameter.Hidden = true;
        op.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        op.QualityParameter.Hidden = true;
        op.PermutationParameter.ActualName = Encoding.SolutionCreator.PermutationParameter.ActualName;
        op.PermutationParameter.Hidden = true;
      }
      foreach (ISingleObjectiveImprovementOperator op in Operators.OfType<ISingleObjectiveImprovementOperator>()) {
        op.SolutionParameter.ActualName = Encoding.SolutionCreator.PermutationParameter.ActualName;
        op.SolutionParameter.Hidden = true;
      }
      foreach (ISingleObjectivePathRelinker op in Operators.OfType<ISingleObjectivePathRelinker>()) {
        op.ParentsParameter.ActualName = Encoding.SolutionCreator.PermutationParameter.ActualName;
        op.ParentsParameter.Hidden = true;
      }
      foreach (ISolutionSimilarityCalculator op in Operators.OfType<ISolutionSimilarityCalculator>()) {
        op.SolutionVariableName = Encoding.SolutionCreator.PermutationParameter.ActualName;
        op.QualityVariableName = Evaluator.QualityParameter.ActualName;
      }
    }

    public void UpdateDistanceMatrix() {
      var df = DistanceFunction;
      var c = Coordinates;
      if (c == null) throw new InvalidOperationException("No coordinates are given to calculate distance matrix.");
      DistanceMatrix = CalculateDistanceMatrix(df, c);
    }

    public static DistanceMatrix CalculateDistanceMatrix(TSPDistanceFunction distance, DoubleMatrix coordinates) {
      var dm = new double[coordinates.Rows, coordinates.Rows];
      for (var i = 0; i < dm.GetLength(0); i++) {
        for (var j = 0; j < dm.GetLength(1); j++)
          dm[i, j] = CalculateDistance(distance, coordinates[i, 0], coordinates[i, 1], coordinates[j, 0], coordinates[j, 1]);
      }
      return new DistanceMatrix(dm, readOnly: true);
    }
    #endregion

    public void Load(TSPData data) {
      if (data.Coordinates == null && data.Distances == null)
        throw new System.IO.InvalidDataException("The given instance specifies neither coordinates nor distances!");
      if (data.Dimension > DistanceMatrixSizeLimit && (data.DistanceMeasure == DistanceMeasure.Att
        || data.DistanceMeasure == DistanceMeasure.Manhattan
        || data.DistanceMeasure == DistanceMeasure.Maximum))
        throw new System.IO.InvalidDataException("The given instance uses an unsupported distance measure and is too large for using a distance matrix.");
      if (data.Coordinates != null && data.Coordinates.GetLength(1) != 2)
        throw new System.IO.InvalidDataException("The coordinates of the given instance are not in the right format, there need to be one row for each customer and two columns for the x and y coordinates.");

      Name = data.Name;
      Description = data.Description;
      
      if (data.Coordinates != null && data.Coordinates.GetLength(0) > 0)
        Coordinates = new DoubleMatrix(data.Coordinates);
      else Coordinates = null;
      
      if (data.DistanceMeasure == DistanceMeasure.Att
        || data.DistanceMeasure == DistanceMeasure.Manhattan
        || data.DistanceMeasure == DistanceMeasure.Maximum) {
        UseDistanceMatrix = true;
        DistanceMatrix = new DistanceMatrix(data.GetDistanceMatrix());
        DistanceFunction = TSPDistanceFunction.DistanceMatrix;
      } else if (data.DistanceMeasure == DistanceMeasure.Direct && data.Distances != null) {
        UseDistanceMatrix = true;
        DistanceMatrix = new DistanceMatrix(data.Distances);
        DistanceFunction = TSPDistanceFunction.DistanceMatrix;
      } else {
        UseDistanceMatrix = data.Dimension <= DistanceMatrixSizeLimit;
        switch (data.DistanceMeasure) {
          case DistanceMeasure.Euclidean:
            DistanceFunction = TSPDistanceFunction.Euclidean;
            break;
          case DistanceMeasure.RoundedEuclidean:
            DistanceFunction = TSPDistanceFunction.RoundedEuclidean;
            break;
          case DistanceMeasure.UpperEuclidean:
            DistanceFunction = TSPDistanceFunction.UpperEuclidean;
            break;
          case DistanceMeasure.Geo:
            DistanceFunction = TSPDistanceFunction.Geo;
            break;
          default:
            throw new InvalidDataException("An unknown distance measure is given in the instance!");
        }
        if (UseDistanceMatrix) UpdateDistanceMatrix();
        else DistanceMatrix = null;
      }
      Evaluator.QualityParameter.ActualName = "TSPTourLength";
      
      BestKnownSolution = null;
      BestKnownQualityParameter.Value = null;

      if (data.BestKnownTour != null) {
        try {
          EvaluateAndLoadTour(data.BestKnownTour);
        } catch (InvalidOperationException) {
          if (data.BestKnownQuality.HasValue)
            BestKnownQuality = data.BestKnownQuality.Value;
        }
      } else if (data.BestKnownQuality.HasValue) {
        BestKnownQuality = data.BestKnownQuality.Value;
      }
      Encoding.Length = data.Dimension;

      OnReset();
    }

    public void EvaluateAndLoadTour(int[] tour) {
      var route = new Permutation(PermutationTypes.RelativeUndirected, tour);
      BestKnownSolution = route;
      BestKnownQuality = Evaluate(route);
    }

    public static double CalculateDistance(TSPDistanceFunction distanceFunction, double x1, double y1, double x2, double y2) {
      switch (distanceFunction) {
        case TSPDistanceFunction.Euclidean:
          return CalculateEuclideanDistance(x1, y1, x2, y2);
        case TSPDistanceFunction.RoundedEuclidean:
          return CalculateRoundedEuclideanDistance(x1, y1, x2, y2);
        case TSPDistanceFunction.UpperEuclidean:
          return CalculateUpperEuclideanDistance(x1, y1, x2, y2);
        case TSPDistanceFunction.Geo:
          return CalculateGeoDistance(x1, y1, x2, y2);
        default: throw new ArgumentException(string.Format("Distance calculation not available for {0}", distanceFunction));
      }
    }

    public static double CalculateEuclideanDistance(double x1, double y1, double x2, double y2) {
      return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
    }

    public static double CalculateRoundedEuclideanDistance(double x1, double y1, double x2, double y2) {
      return Math.Round(Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
    }

    public static double CalculateUpperEuclideanDistance(double x1, double y1, double x2, double y2) {
      return Math.Ceiling(Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
    }

    public static double CalculateGeoDistance(double x1, double y1, double x2, double y2) {
      double latitude1, longitude1, latitude2, longitude2;
      double q1, q2, q3;
      double length;

      latitude1 = ConvertToRadian(x1);
      longitude1 = ConvertToRadian(y1);
      latitude2 = ConvertToRadian(x2);
      longitude2 = ConvertToRadian(y2);

      q1 = Math.Cos(longitude1 - longitude2);
      q2 = Math.Cos(latitude1 - latitude2);
      q3 = Math.Cos(latitude1 + latitude2);

      length = (int)(RADIUS * Math.Acos(0.5 * ((1.0 + q1) * q2 - (1.0 - q1) * q3)) + 1.0);
      return (length);
    }

    private static double ConvertToRadian(double x) {
      return PI * (Math.Truncate(x) + 5.0 * (x - Math.Truncate(x)) / 3.0) / 180.0;
    }
  }
}
