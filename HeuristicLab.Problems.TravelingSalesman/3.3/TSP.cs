using System;
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
  public class TSP : PermutationProblem, IProblemInstanceConsumer<TSPData> {
    /// <summary>
    /// This limit governs when a distance matrix is used. For all problems smaller than that, the distance matrix is
    /// computed. This greatly speeds up computation time.
    /// </summary>
    public static int DistanceMatrixSizeLimit { get; set; } = 1000;

    public override bool Maximization => false;

    [Storable] public IValueParameter<ITSPData> TSPDataParameter { get; private set; }
    [Storable] public IValueParameter<Permutation> BestKnownSolutionParameter { get; private set; }

    public ITSPData TSPData {
      get { return TSPDataParameter.Value; }
      set { TSPDataParameter.Value = value; }
    }
    public Permutation BestKnownSolution {
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
      Parameters.Add(BestKnownSolutionParameter = new OptionalValueParameter<Permutation>("BestKnownSolution", "The best known solution."));
      
      TSPData = new EuclideanTSPData() {
        Coordinates = new DoubleMatrix(new double[,] {
        { 100, 100 }, { 100, 200 }, { 100, 300 }, { 100, 400 },
        { 200, 100 }, { 200, 200 }, { 200, 300 }, { 200, 400 },
        { 300, 100 }, { 300, 200 }, { 300, 300 }, { 300, 400 },
        { 400, 100 }, { 400, 200 }, { 400, 300 }, { 400, 400 }
        }),
        Rounding = EuclideanTSPData.RoundingMode.Midpoint
      };

      InitializeOperators();
    }

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
        BestKnownQualityParameter.ActualValue = new DoubleValue(qualities[i]);
        BestKnownSolutionParameter.ActualValue = (Permutation)solutions[i].Clone();
      }

      var solution = TSPData.GetSolution(solutions[i], qualities[i]);
      results.AddOrUpdateResult("Best TSP Solution", solution);
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

      if (data.DistanceMeasure == DistanceMeasure.Att
        || data.DistanceMeasure == DistanceMeasure.Manhattan
        || data.DistanceMeasure == DistanceMeasure.Maximum
        || data.Dimension <= DistanceMatrixSizeLimit) {
        TSPData = new MatrixTSPData(data.GetDistanceMatrix(), data.Coordinates);
      } else if (data.DistanceMeasure == DistanceMeasure.Direct && data.Distances != null) {
        TSPData = new MatrixTSPData(data.Distances, data.Coordinates);
      } else {
        switch (data.DistanceMeasure) {
          case DistanceMeasure.Euclidean:
            TSPData = new EuclideanTSPData(data.Coordinates) { Rounding = EuclideanTSPData.RoundingMode.None };
            break;
          case DistanceMeasure.RoundedEuclidean:
            TSPData = new EuclideanTSPData(data.Coordinates) { Rounding = EuclideanTSPData.RoundingMode.Midpoint };
            break;
          case DistanceMeasure.UpperEuclidean:
            TSPData = new EuclideanTSPData(data.Coordinates) { Rounding = EuclideanTSPData.RoundingMode.Ceiling };
            break;
          case DistanceMeasure.Geo:
            TSPData = new GeoTSPData(data.Coordinates);
            break;
          default:
            throw new System.IO.InvalidDataException("An unknown distance measure is given in the instance!");
        }
      }
      BestKnownSolution = null;
      BestKnownQuality = double.NaN;

      if (data.BestKnownTour != null) {
        try {
          BestKnownSolution = new Permutation(PermutationTypes.RelativeUndirected, data.BestKnownTour);
          BestKnownQuality = Evaluate(BestKnownSolution);
        } catch (InvalidOperationException) {
          if (data.BestKnownQuality.HasValue)
            BestKnownQuality = data.BestKnownQuality.Value;
        }
      } else if (data.BestKnownQuality.HasValue) {
        BestKnownQuality = data.BestKnownQuality.Value;
      }
      OnReset();
    }

    protected override void OnEncodingChanged() {
      base.OnEncodingChanged();
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
      foreach (var op in ApplicationManager.Manager.GetInstances<ITSPMoveEvaluator>())
        Operators.Add(op);

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
