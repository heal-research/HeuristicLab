using System;
using System.Collections.Generic;

namespace HeuristicLab.Hive.Client.Console
{
  public class RecurrentEvent
  {
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public bool AllDay { get; set; }
    public int IncWeeks { get; set; }
    public HashSet<DayOfWeek> WeekDays { get; set; }
  }
}
