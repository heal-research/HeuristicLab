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

using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Clients.Access {
  [Item("Resource", "A resource.")]
  public partial class Resource {
    protected Resource(Resource original, Cloner cloner)
      : base(original, cloner) {
      this.Name = original.Name;
      this.Description = original.Description;
    }

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.OrgChart; }
    }

    public Resource() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Resource(this, cloner);
    }

    public override string ToString() {
      return Name;
    }
  }
}
