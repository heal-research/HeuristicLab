using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.TravelingSalesman {
  [Item("TSP Inversion Move Evaluator", "Evaluates the quality of an inversion move based on an efficient delta computation.")]
  [StorableType("74ac2d20-2371-4f4c-8796-1f318d1b33c0")]
  public class TSPInversionMoveEvaluator : TSPMoveEvaluator, IPermutationInversionMoveOperator {
    [Storable] public ILookupParameter<InversionMove> MoveParameter { get; private set; }

    ILookupParameter<InversionMove> IPermutationInversionMoveOperator.InversionMoveParameter => MoveParameter;
    ILookupParameter<Permutation> IPermutationMoveOperator.PermutationParameter => TSPTourParameter;

    [StorableConstructor]
    protected TSPInversionMoveEvaluator(StorableConstructorFlag _) : base(_) { }
    protected TSPInversionMoveEvaluator(TSPInversionMoveEvaluator original, Cloner cloner)
      : base(original, cloner) {
      MoveParameter = cloner.Clone(original.MoveParameter);
    }
    public TSPInversionMoveEvaluator()
      : base() {
      Parameters.Add(MoveParameter = new LookupParameter<InversionMove>("Move", "The inversion move that is to be evaluated."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TSPInversionMoveEvaluator(this, cloner);
    }

    protected override double CalculateTourLengthWithMove(ITSPData tspData, Permutation tspTour, double tourLength) {
      var move = MoveParameter.ActualValue;
      return CalculateTourLengthDelta(tspData, tspTour, move);
    }

    public static double CalculateTourLengthDelta(ITSPData tspData, Permutation tspTour, InversionMove move) {
      int edge1source = tspTour.GetCircular(move.Index1 - 1);
      int edge1target = tspTour[move.Index1];
      int edge2source = tspTour[move.Index2];
      int edge2target = tspTour.GetCircular(move.Index2 + 1);
      if (move.Index2 - move.Index1 >= tspTour.Length - 2) return 0;
      double moveQuality = 0;
      // remove two edges
      moveQuality -= tspData.GetDistance(edge1source, edge1target);
      moveQuality -= tspData.GetDistance(edge2source, edge2target);
      // add two edges
      moveQuality += tspData.GetDistance(edge1source, edge2source);
      moveQuality += tspData.GetDistance(edge1target, edge2target);
      return moveQuality;
    }
  }
}
