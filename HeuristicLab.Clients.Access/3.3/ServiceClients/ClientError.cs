#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;

namespace HeuristicLab.Clients.Access {
  [Item("ClientError", "An object containing an error message.")]
  public partial class ClientError {
    protected ClientError(ClientError original, Cloner cloner)
      : base(original, cloner) {
      this.ClientId = original.ClientId;
      this.ConfigDump = original.ConfigDump;
      this.Exception = original.Exception;
      this.Timestamp = original.Timestamp;
      this.UserComment = original.UserComment;
      this.UserId = original.UserId;
    }

    public ClientError()
      : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ClientError(this, cloner);
    }
  }
}
