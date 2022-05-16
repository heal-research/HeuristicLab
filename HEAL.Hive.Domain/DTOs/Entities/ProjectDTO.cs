using System;
using System.Collections.Generic;

namespace HEAL.Hive.Domain.DTOs.Entities {
  public class ProjectDTO : OwnedResourceDTO {
    public string Description { get; set; }
    public Guid? ParentProjectId { get; set; }
    public ICollection<Guid> ChildProjects { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ICollection<Guid> HiveJobs { get; set; }
    public ICollection<Guid> AssignedComputingResources { get; set; }
    public ICollection<Guid> ProjectPermissions { get; set; }
  }
}
