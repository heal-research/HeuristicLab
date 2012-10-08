using HeuristicLab.Core;
using HeuristicLab.Encodings.ScheduleEncoding;

namespace HeuristicLab.Problems.Scheduling {
  public interface IJSSPOperator : IOperator {
    ILookupParameter<ItemList<Job>> JobDataParameter { get; }
  }
}
