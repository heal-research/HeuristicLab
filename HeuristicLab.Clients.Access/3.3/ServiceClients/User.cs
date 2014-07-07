#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("User ", "A user.")]
  public partial class User {
    protected User(User original, Cloner cloner)
      : base(original, cloner) {
      this.Comment = original.Comment;
      this.Email = original.Email;
      this.CreationDate = original.CreationDate;
      this.FullName = original.FullName;
      this.IsApproved = original.IsApproved;
      this.LastActivityDate = original.LastActivityDate;
      this.LastLoginDate = original.LastLoginDate;
      this.LastPasswordChangedDate = original.LastPasswordChangedDate;
      this.UserName = original.UserName;
    }

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.User; }
    }

    public User() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new User(this, cloner);
    }

    public override string ToString() {
      return UserName + "(" + FullName + ")";
    }
  }
}
