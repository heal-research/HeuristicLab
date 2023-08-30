using HEAL.Attic;
using HeuristicLab.Core;

namespace HeuristicLab.Problems.Dynamic {
  [StorableType("DFBAA8DC-6CC1-495D-A517-CAAFCF5BA130")]
  public interface IMultiObjectiveDynamicProblemTracker<in TSolution, in TState>
    : IDynamicProblemTracker<TState>
    where TSolution : IItem
  {
    void OnEvaluation(TState state, TSolution solution, double[] quality, long version, long time);
  }
}
