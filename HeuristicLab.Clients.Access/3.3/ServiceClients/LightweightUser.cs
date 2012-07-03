#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Clients.Access {
  [Item("LightweightUser", "A user.")]
  public partial class LightweightUser {
    protected LightweightUser(LightweightUser original, Cloner cloner)
      : base(original, cloner) {
      this.FullName = original.FullName;
      this.UserName = original.UserName;
      this.EMail = original.EMail;
      this.Groups = original.Groups != null ? original.Groups.Select(x => (UserGroup)x.Clone(cloner)).ToList() : null;
      this.Roles = original.Roles != null ? original.Roles.Select(x => (Role)x.Clone(cloner)).ToList() : null;
    }

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.User; }
    }

    public LightweightUser() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LightweightUser(this, cloner);
    }

    public override string ToString() {
      return (UserName != null && FullName != null) ? UserName + " (" + FullName + ")" : string.Empty;
    }
  }
}
