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

namespace HeuristicLab.Services.Access {
  public class AccessServiceLocator : IAccessServiceLocator {
    private static IAccessServiceLocator instance;
    public static IAccessServiceLocator Instance {
      get {
        if (instance == null) instance = new AccessServiceLocator();
        return instance;
      }
      set { instance = value; }
    }

    private IRoleVerifier roleVerifier;
    public IRoleVerifier RoleVerifier {
      get {
        if (roleVerifier == null) roleVerifier = new RoleVerifier();
        return roleVerifier;
      }
    }

    private IUserManager userManager;
    public IUserManager UserManager {
      get {
        if (userManager == null) userManager = new UserManager();
        return userManager;
      }
    }
  }
}
