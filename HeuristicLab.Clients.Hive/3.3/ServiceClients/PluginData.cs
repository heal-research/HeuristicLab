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
using HeuristicLab.Common;

namespace HeuristicLab.Clients.Hive {

  public partial class PluginData : IDeepCloneable, IContent {

    public PluginData() { }

    protected PluginData(PluginData original, Cloner cloner)
      : base(original, cloner) {
      if (original.Data != null) {
        this.Data = new byte[original.Data.Length];
        Array.Copy(original.Data, this.Data, original.Data.Length);
      }
      this.FileName = original.FileName;
      this.PluginId = original.PluginId;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PluginData(this, cloner);
    }
  }
}
