using HEAL.Attic;
using HeuristicLab.Core;

namespace HeuristicLab.Problems.Dynamic {
  [StorableType("4BD297C3-994A-42D2-9489-7063E87B4C05")]
  public interface ISingleObjectiveDynamicProblemTracker<in TSolution,in TProblemData> 
    : IDynamicProblemTracker<TProblemData> where TSolution: IItem {
    void OnEvaluation(TSolution solution, double quality, long version, long time);

  }
}