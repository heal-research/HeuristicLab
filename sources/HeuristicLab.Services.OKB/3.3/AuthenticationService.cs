#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ServiceModel;
using System.Web.Security;

namespace HeuristicLab.Services.OKB {
  /// <summary>
  /// Implementation of the OKB authentication service (interface <see cref="IAuthenticationService"/>).
  /// </summary>
  [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
  public class AuthenticationService : IAuthenticationService {
    public DataTransfer.User GetUser(Guid id) {
      MembershipUser user = Membership.GetUser(id);
      if (user != null) return new DataTransfer.User { Id = id, Name = user.UserName };
      else return null;
    }
    public IEnumerable<DataTransfer.User> GetUsers() {
      List<DataTransfer.User> users = new List<DataTransfer.User>();
      foreach (MembershipUser user in Membership.GetAllUsers())
        users.Add(new DataTransfer.User { Id = (Guid)user.ProviderUserKey, Name = user.UserName });
      return users;
    }
  }
}
