using HeuristicLab.Common;

namespace HeuristicLab.Clients.Hive {
  public partial class AssignedJobResource : IDeepCloneable, IContent {
    public AssignedJobResource() { }

    public AssignedJobResource(AssignedJobResource original, Cloner cloner) : base(original, cloner) {
      ResourceId = original.ResourceId;
      JobId = original.JobId;
    }
  }
}
