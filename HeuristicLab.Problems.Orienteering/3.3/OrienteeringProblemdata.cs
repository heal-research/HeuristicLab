using System;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Problems.Instances.Types;
using HeuristicLab.Problems.TravelingSalesman;

namespace HeuristicLab.Problems.Orienteering {
  [StorableType("dd2d0ecc-372e-46f1-846f-fb4ca2afa124")]
  public interface IOrienteeringProblemData : INamedItem {
    ITSPData RoutingData { get; }
    int StartingPoint { get; }
    int TerminalPoint { get; }
    double MaximumTravelCosts { get; }
    double PointVisitingCosts { get; }

    double GetScore(int city);
    OrienteeringSolution GetSolution(IntegerVector route, double quality, double score, double travelCosts);
    OPData Export();
  }

  [Item("Orienteering Problem Data", "Represents the main data for an orienteering problem.")]
  [StorableType("d7d2d61a-7d51-4254-ab68-375942106058")]
  public class OrienteeringProblemData : NamedItem, IOrienteeringProblemData {
    [Storable] public ITSPData RoutingData { get; protected set; }
    [Storable] public int StartingPoint { get; protected set; }
    [Storable] public int TerminalPoint { get; protected set; }
    [Storable] public DoubleArray Scores { get; protected set; }
    [Storable] public double MaximumTravelCosts { get; protected set; }
    [Storable] public double PointVisitingCosts { get; protected set; }

    [StorableConstructor]
    protected OrienteeringProblemData(StorableConstructorFlag _) : base(_) { }
    protected OrienteeringProblemData(OrienteeringProblemData original, Cloner cloner) : base(original, cloner) {
      RoutingData = original.RoutingData;
      StartingPoint = original.StartingPoint;
      TerminalPoint = original.TerminalPoint;
      Scores = original.Scores;
      MaximumTravelCosts = original.MaximumTravelCosts;
      PointVisitingCosts = original.PointVisitingCosts;
    }
    public OrienteeringProblemData() {
      RoutingData = new EuclideanTSPData("HL OP Default", defaultCoordinates);
      Name = RoutingData.Name;
      StartingPoint = 0;
      TerminalPoint = 20;
      Scores = new DoubleArray(Enumerable.Repeat(1.0, RoutingData.Cities).ToArray(), @readonly: true);
      MaximumTravelCosts = 30;
      PointVisitingCosts = 0;
    }
    public OrienteeringProblemData(ITSPData tspData, int startingPoint, int terminalPoint, double[] scores, double maxDist, double pointVisitCosts)
      : this(tspData, startingPoint, terminalPoint, new DoubleArray(scores, @readonly: true), maxDist, pointVisitCosts) { }
    public OrienteeringProblemData(ITSPData tspData, int startingPoint, int terminalPoint, DoubleArray scores, double maxDist, double pointVisitCosts)
      : base(tspData.Name, tspData.Description) {
      if (tspData.Cities != scores.Length) throw new ArgumentException("Unequal number of cities and scores.");
      if (startingPoint < 0 || startingPoint >= tspData.Cities) throw new ArgumentException("Starting point is not in the range of cities.", "startingPoint");
      if (terminalPoint < 0 || terminalPoint >= tspData.Cities) throw new ArgumentException("Starting point is not in the range of cities.", "startingPoint");
      RoutingData = tspData;
      StartingPoint = startingPoint;
      TerminalPoint = terminalPoint;
      Scores = scores.AsReadOnly();
      MaximumTravelCosts = maxDist;
      PointVisitingCosts = pointVisitCosts;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OrienteeringProblemData(this, cloner);
    }

    public double GetScore(int site) => Scores[site];

    public OrienteeringSolution GetSolution(IntegerVector route, double quality, double score, double travelCosts) {
      return new OrienteeringSolution(route, this, quality, score, travelCosts);
    }

    public OPData Export() {
      var tspExport = RoutingData.Export();
      return new OPData() {
        Name = tspExport.Name,
        Description = tspExport.Description,
        Coordinates = tspExport.Coordinates,
        Dimension = tspExport.Dimension,
        DistanceMeasure = tspExport.DistanceMeasure,
        Distances = tspExport.Distances,
        MaximumDistance = MaximumTravelCosts,
        Scores = Scores.CloneAsArray(),
        StartingPoint = StartingPoint,
        TerminalPoint = TerminalPoint,
        PointVisitingCosts = PointVisitingCosts
      };
    }

    private static double[,] defaultCoordinates = new double[21, 2] {
      {  4.60,  7.10 }, {  5.70, 11.40 }, {  4.40, 12.30 }, {  2.80, 14.30 }, {  3.20, 10.30 },
      {  3.50,  9.80 }, {  4.40,  8.40 }, {  7.80, 11.00 }, {  8.80,  9.80 }, {  7.70,  8.20 },
      {  6.30,  7.90 }, {  5.40,  8.20 }, {  5.80,  6.80 }, {  6.70,  5.80 }, { 13.80, 13.10 },
      { 14.10, 14.20 }, { 11.20, 13.60 }, {  9.70, 16.40 }, {  9.50, 18.80 }, {  4.70, 16.80 },
      {  5.00,  5.60 }
    };
    private static double[] defaultScores = new double[21] { 0, 20, 20, 30, 15, 15, 10, 20, 20, 20, 15, 10, 10, 25, 40, 40, 30, 30, 50, 30, 0 };
  }
}
