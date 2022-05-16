
using System;

namespace HEAL.Hive.Domain.Statistics {
  public class DimComputingResource {
    public Guid Id { get; set; }
    public Guid ComputingResourceId { get; set; }
    public Guid? ParentComputingResourceId { get; set; }
    public string Name { get; set; }
    public string ComputingResourceType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiredAt { get; set; }
  }
}
