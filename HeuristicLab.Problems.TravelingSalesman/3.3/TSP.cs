using System;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.TravelingSalesman {
  [Item("Traveling Salesman Problem (TSP)", "Represents a symmetric Traveling Salesman Problem.")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 100)]
  [StorableType("60511a03-b8b4-47cd-a119-78f97358a6b0")]
  public class TSP : SingleObjectiveProblem<PermutationEncoding, Permutation>, IProblemInstanceConsumer<TSPData> {
    public static int DistanceMatrixSizeLimit { get; set; } = 1000;

    public override bool Maximization => false;

    [Storable]
    private IValueParameter<ITSPData> tspDataParameter;
    public IValueParameter<ITSPData> TSPDataParameter {
      get { return tspDataParameter; }
    }
    [Storable]
    private IValueParameter<Permutation> bestKnownSolutionParameter;
    public IValueParameter<Permutation> BestKnownSolutionParameter {
      get { return bestKnownSolutionParameter; }
    }

    public ITSPData TSPData {
      get { return tspDataParameter.Value; }
      set { tspDataParameter.Value = value; }
    }
    public Permutation BestKnownSolution {
      get { return bestKnownSolutionParameter.Value; }
      set { bestKnownSolutionParameter.Value = value; }
    }

    [StorableConstructor]
    protected TSP(StorableConstructorFlag _) : base(_) { }

    protected TSP(TSP original, Cloner cloner)
      : base(original, cloner) {
      tspDataParameter = cloner.Clone(original.tspDataParameter);
      bestKnownSolutionParameter = cloner.Clone(original.bestKnownSolutionParameter);
    }

    public TSP() : base(new PermutationEncoding("Tour", 16, PermutationTypes.RelativeUndirected)) {
      TSPData = new RoundedEuclideanCoordinatesTSPData() {
        Coordinates = new DoubleMatrix(new double[,] {
        { 100, 100 }, { 100, 200 }, { 100, 300 }, { 100, 400 },
        { 200, 100 }, { 200, 200 }, { 200, 300 }, { 200, 400 },
        { 300, 100 }, { 300, 200 }, { 300, 300 }, { 300, 400 },
        { 400, 100 }, { 400, 200 }, { 400, 300 }, { 400, 400 }
        })
      };
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
            throw new NotImplementedException();
            break;
          case DistanceMeasure.RoundedEuclidean:
            TSPData = new RoundedEuclideanCoordinatesTSPData(data.Coordinates);
            break;
          case DistanceMeasure.UpperEuclidean:
            throw new NotImplementedException();
            break;
          case DistanceMeasure.Geo:
            throw new NotImplementedException();
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
  }
}
