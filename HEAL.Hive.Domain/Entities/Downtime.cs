using HEAL.Hive.Domain.Enums;
using System;

namespace HEAL.Hive.Domain.Entities {
  public class Downtime : Resource {
    public Guid ComputingResourceId { get; set; }
    public DowntimeType DowntimeType { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public bool AllDayEvent { get; set; }
    public bool Recurring { get; set; }
  }
}
