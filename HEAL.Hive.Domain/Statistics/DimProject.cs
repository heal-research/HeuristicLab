
using System;

namespace HEAL.Hive.Domain.Statistics {
  public class DimProject {

    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? ParentProjectId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid? OwnerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiredAt { get; set; }

  }
}
