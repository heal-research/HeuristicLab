#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;

namespace HeuristicLab.Services.WebApp.Status.WebApi.DataTransfer {

  public class CoreStatus {
    public int TotalCores { get; set; }
    public int UsedCores { get; set; }
    public int ActiveCores { get; set; }
    public int CalculatingCores { get; set; }
  }

  public class CpuUtilizationStatus {
    public double TotalCpuUtilization { get; set; }
    public double ActiveCpuUtilization { get; set; }
    public double CalculatingCpuUtilization { get; set; }
  }

  public class MemoryStatus {
    public int TotalMemory { get; set; }
    public int UsedMemory { get; set; }
    public int ActiveMemory { get; set; }
    public int CalculatingMemory { get; set; }
  }

  public class TaskStatus {
    public User User { get; set; }
    public int CalculatingTasks { get; set; }
    public int WaitingTasks { get; set; }
  }

  public class SlaveStatus {
    public Slave Slave { get; set; }
    public double CpuUtilization { get; set; }
    public int Cores { get; set; }
    public int FreeCores { get; set; }
    public int Memory { get; set; }
    public int FreeMemory { get; set; }
    public bool IsAllowedToCalculate { get; set; }
    public string State { get; set; }
  }

  public class TimeStatus {
    public long MinCalculatingTime { get; set; }
    public long MaxCalculatingTime { get; set; }
    public long AvgCalculatingTime { get; set; }
    public long StandardDeviationCalculatingTime { get; set; }
    public long AvgWaitingTime { get; set; }
    public long TotalCpuTime { get; set; }
    public DateTime? BeginDate { get; set; }
  }

  public class Status {
    public CoreStatus CoreStatus { get; set; }
    public CpuUtilizationStatus CpuUtilizationStatus { get; set; }
    public MemoryStatus MemoryStatus { get; set; }
    public TimeStatus TimeStatus { get; set; }
    public IEnumerable<TaskStatus> TasksStatus { get; set; }
    public IEnumerable<SlaveStatus> SlavesStatus { get; set; }
    public long Timestamp { get; set; }
  }

}