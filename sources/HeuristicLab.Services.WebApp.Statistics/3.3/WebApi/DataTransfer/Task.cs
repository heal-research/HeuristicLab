#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Services.WebApp.Statistics.WebApi.DataTransfer {
  public class Task {
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public string JobName { get; set; }
    public long TotalTime { get; set; }
    public double CalculatingTime { get; set; }
    public double WaitingTime { get; set; }
    public double TransferTime { get; set; }
    public double InitialWaitingTime { get; set; }
    public int NumCalculationRuns { get; set; }
    public int NumRetries { get; set; }
    public int CoresRequired { get; set; }
    public int MemoryRequired { get; set; }
    public int Priority { get; set; }
    public string State { get; set; }
    public Guid? LastClientId { get; set; }
    public string LastClientName { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Exception { get; set; }
  }
}