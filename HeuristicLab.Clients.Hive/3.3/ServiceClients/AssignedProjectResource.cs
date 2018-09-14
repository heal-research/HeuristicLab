using HeuristicLab.Common;

namespace HeuristicLab.Clients.Hive {
  public partial class AssignedProjectResource : IDeepCloneable, IContent {
    public AssignedProjectResource() { }

    public AssignedProjectResource(AssignedProjectResource original, Cloner cloner) : base(original, cloner) {
      ResourceId = original.ResourceId;
      ProjectId = original.ProjectId;
    }
  }
}
