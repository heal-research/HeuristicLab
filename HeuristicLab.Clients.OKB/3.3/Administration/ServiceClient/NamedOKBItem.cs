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

using System.ComponentModel;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Clients.OKB.Administration {
  [Item("NamedOKBItem", "Base class for all named OKB items.")]
  public partial class NamedOKBItem : INamedOKBItem {
    protected NamedOKBItem(NamedOKBItem original, Cloner cloner)
      : base(original, cloner) {
      Name = original.Name;
      Description = original.Description;
    }
    public NamedOKBItem() {
      Name = "New OKB Item";
      Description = string.Empty;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NamedOKBItem(this, cloner);
    }

    public override string ToString() {
      return Name;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
      base.OnPropertyChanged(e);
      if (e.PropertyName == "Name")
        OnToStringChanged();
    }
  }
}
