using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.TravelingSalesman {
  [StorableType("17477ad2-2c84-4b02-b5ac-f35ef6cc667f")]
  public interface ITSPMoveEvaluator : IOperator {
    ILookupParameter<Permutation> TSPTourParameter { get; }
    ILookupParameter<ITSPData> TSPDataParameter { get; }
    ILookupParameter<DoubleValue> TourLengthParameter { get; }
    ILookupParameter<DoubleValue> TourLengthWithMoveParameter { get; }
  }

  [Item("TSP Move Evaluator", "Base class for all move evaluators of the TSP.")]
  [StorableType("44af3845-ccdf-4014-805a-5878c64f67f5")]
  public abstract class TSPMoveEvaluator : SingleSuccessorOperator, ITSPMoveEvaluator {
    [Storable] public ILookupParameter<Permutation> TSPTourParameter { get; private set; }
    [Storable] public ILookupParameter<ITSPData> TSPDataParameter { get; private set; }
    [Storable] public ILookupParameter<DoubleValue> TourLengthParameter { get; private set; }
    [Storable] public ILookupParameter<DoubleValue> TourLengthWithMoveParameter { get; private set; }

    [StorableConstructor]
    protected TSPMoveEvaluator(StorableConstructorFlag _) : base(_) { }
    protected TSPMoveEvaluator(TSPMoveEvaluator original, Cloner cloner)
      : base(original, cloner) {
      TSPTourParameter = cloner.Clone(original.TSPTourParameter);
      TSPDataParameter = cloner.Clone(original.TSPDataParameter);
      TourLengthParameter = cloner.Clone(original.TourLengthParameter);
      TourLengthWithMoveParameter = cloner.Clone(original.TourLengthWithMoveParameter);
    }
    public TSPMoveEvaluator() {
      Parameters.Add(TSPTourParameter = new LookupParameter<Permutation>("TSPTour", "The tour that describes a solution to the TSP."));
      Parameters.Add(TSPDataParameter = new LookupParameter<ITSPData>("TSPData", "The main parameters of the TSP."));
      Parameters.Add(TourLengthParameter = new LookupParameter<DoubleValue>("TourLength", "The length of a TSP tour."));
      Parameters.Add(TourLengthWithMoveParameter = new LookupParameter<DoubleValue>("TourLengthWithMove", "The length of the TSP tour if the move was applied."));
    }

    public sealed override IOperation Apply() {
      var tour = TSPTourParameter.ActualValue;
      var tspData = TSPDataParameter.ActualValue;
      var tourLength = TourLengthParameter.ActualValue.Value;
      TourLengthWithMoveParameter.ActualValue = new DoubleValue(
        CalculateTourLengthWithMove(tspData, tour, tourLength)
      );
      return base.Apply();
    }

    protected abstract double CalculateTourLengthWithMove(ITSPData tspData, Permutation tspTour, double tourLength);
  }
}
