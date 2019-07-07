#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Runtime.Serialization;

namespace HeuristicLab.Services.Hive.DataTransfer {
  public enum CpuArchitecture {
    x86, x64
  }

  [DataContract]
  [Serializable]
  public class Slave : Resource {
    [DataMember]
    public int? Cores { get; set; }
    [DataMember]
    public int? FreeCores { get; set; }
    [DataMember]
    public int? CpuSpeed { get; set; } // MHz
    [DataMember]
    public CpuArchitecture CpuArchitecture { get; set; }
    [DataMember]
    public int? Memory { get; set; } // MB
    [DataMember]
    public int? FreeMemory { get; set; } // MB
    [DataMember]
    public string OperatingSystem { get; set; }
    [DataMember]
    public SlaveState SlaveState { get; set; }
    [DataMember]
    public bool IsAllowedToCalculate { get; set; }
    [DataMember]
    public DateTime? LastHeartbeat { get; set; }
    [DataMember]
    public double CpuUtilization { get; set; }
    [DataMember]
    public bool? IsDisposable { get; set; }

    public Slave() {
      SlaveState = DataTransfer.SlaveState.Idle;
    }

    public override string ToString() {
      return string.Format("Cores: {0}, FreeCores: {1}", Cores, FreeCores);
    }
  }
}
