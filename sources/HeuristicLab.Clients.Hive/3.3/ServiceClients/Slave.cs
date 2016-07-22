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

using HeuristicLab.Common;

namespace HeuristicLab.Clients.Hive {

  public partial class Slave : IContent, IDeepCloneable {

    public Slave() {
      SlaveState = SlaveState.Idle;
    }

    protected Slave(Slave original, Cloner cloner)
      : base(original, cloner) {
      this.Cores = original.Cores;
      this.FreeCores = original.FreeCores;
      this.CpuSpeed = original.CpuSpeed;
      this.Memory = original.Memory;
      this.FreeMemory = original.FreeMemory;
      this.SlaveState = original.SlaveState;
      this.IsAllowedToCalculate = original.IsAllowedToCalculate;
      this.OperatingSystem = original.OperatingSystem;
      this.CpuArchitecture = original.CpuArchitecture;
      this.LastHeartbeat = original.LastHeartbeat;
      this.CpuUtilization = original.CpuUtilization;
      this.IsDisposable = original.IsDisposable;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Slave(this, cloner);
    }

    public override string ToString() {
      return string.Format("Cores: {0}, FreeCores: {1}", Cores, FreeCores);
    }
  }
}
