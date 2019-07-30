#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Runtime.Serialization;

namespace HeuristicLab.Services.Hive.DataTransfer {

  [DataContract]
  public class Heartbeat {
    [DataMember]
    public Guid SlaveId { get; set; }
    [DataMember]
    public int FreeMemory { get; set; }
    [DataMember]
    public int FreeCores { get; set; }
    [DataMember]
    public Dictionary<Guid, TimeSpan> JobProgress { get; set; }
    [DataMember]
    public bool AssignJob { get; set; } // if false, the server will not assign a new task
    [DataMember]
    public float CpuUtilization { get; set; }
    [DataMember]
    public int HbInterval { get; set; }

    public override string ToString() {
      String val = "SlaveId: " + SlaveId + ", FreeCores: " + FreeCores;
      foreach (KeyValuePair<Guid, TimeSpan> kvp in JobProgress) {
        val += Environment.NewLine + "Id" + kvp.Key + " ExecutionTime " + kvp.Value;
      }
      return val;
    }
  }
}
