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

using System.Web.Security;
using HeuristicLab.Services.WebApp.Controllers.DataTransfer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HeuristicLab.Services.WebApp.Controllers {

  [Authorize]
  public class AuthenticationController : ControllerBase {

    [AllowAnonymous]
    public bool Login(User user) {
      // TODO ASP.NET membership should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/proper-to-2x/membership-to-core-identity.
      if (ModelState.IsValid && Membership.ValidateUser(user.Username, user.Password)) {
        FormsAuthentication.SetAuthCookie(user.Username, user.RememberMe);
        return true;
      }
      // TODO ASP.NET membership should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/proper-to-2x/membership-to-core-identity.
      FormsAuthentication.SignOut();
      return false;
    }

    [HttpGet, HttpPost]
    public bool Logout() {
      // TODO ASP.NET membership should be replaced with ASP.NET Core identity. For more details see https://docs.microsoft.com/aspnet/core/migration/proper-to-2x/membership-to-core-identity.
      FormsAuthentication.SignOut();
      return true;
    }
  }
}