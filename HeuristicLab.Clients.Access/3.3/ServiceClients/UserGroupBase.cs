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
using HeuristicLab.Core;

namespace HeuristicLab.Clients.Access {
  [Item("UserGroupBase", "A UserGroupBase.")]
  public partial class UserGroupBase {
    protected UserGroupBase(UserGroupBase original, Cloner cloner)
      : base(original, cloner) {
      this.Id = Guid.NewGuid();
    }

    public UserGroupBase() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new UserGroupBase(this, cloner);
    }

    public override bool Equals(object obj) {
      var o = obj as UserGroupBase;
      if (o == null) {
        return false;
      } else {
        return this.Id == o.Id;
      }
    }

    public override int GetHashCode() {
      return Id.GetHashCode();
    }
  }
}
