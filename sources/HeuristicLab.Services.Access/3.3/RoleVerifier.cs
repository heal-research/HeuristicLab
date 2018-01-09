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

using System.Linq;
using System.Security;
using System.Web.Security;

namespace HeuristicLab.Services.Access {
  public class RoleVerifier : IRoleVerifier {
    public bool IsInRole(string role) {
      return Roles.IsUserInRole(role);
    }
    public bool IsInAnyRole(params string[] roles) {
      return roles.Any(x => Roles.IsUserInRole(x));
    }
    public bool IsInAllRoles(params string[] roles) {
      return roles.All(x => Roles.IsUserInRole(x));
    }
    public void AuthenticateForAnyRole(params string[] roles) {
      if (!IsInAnyRole(roles))
        throw new SecurityException("Could not authenticate user for any role required.");
    }
    public void AuthenticateForAllRoles(params string[] roles) {
      if (!IsInAllRoles(roles))
        throw new SecurityException("Could not authenticate user for all roles required.");
    }
  }
}
