using System;
using System.Collections.Generic;

namespace HEAL.Hive.Domain.Entities {
  public class Project : OwnedResource {
    public string Description { get; set; }
    public Guid? ParentProjectId { get; set; }
    public ICollection<Project> ChildProjects { get; set; }
    public ICollection<HiveJob> HiveJobs { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ICollection<ComputingResource> AssignedComputingResources { get; set; }
    public ICollection<ProjectPermission> ProjectPermissions { get; set; }
  }
}
