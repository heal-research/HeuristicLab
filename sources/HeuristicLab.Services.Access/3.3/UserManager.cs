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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using DA = HeuristicLab.Services.Access.DataAccess;

namespace HeuristicLab.Services.Access {
  public class UserManager : IUserManager {
    public MembershipUser CurrentUser {
      get { return TryAndRepeat(() => { return Membership.GetUser(); }); }
    }

    public Guid CurrentUserId {
      get { return (Guid)CurrentUser.ProviderUserKey; }
    }

    public MembershipUser GetUserByName(string username) {
      return Membership.GetUser(username);
    }

    public MembershipUser GetUserById(Guid userId) {
      return Membership.GetUser(userId);
    }

    public bool VerifyUser(Guid userId, List<Guid> allowedUserGroups) {
      List<DA.UserGroupUserGroup> userGroupBases;
      List<DA.UserGroup> groups;
      Dictionary<Guid, Guid> ugMapping = new Dictionary<Guid, Guid>();

      if (allowedUserGroups.Contains(userId)) return true;

      using (DA.AccessServiceDataContext context = new DA.AccessServiceDataContext()) {
        userGroupBases = context.UserGroupUserGroups.ToList();
        groups = context.UserGroupBases.OfType<DA.UserGroup>().ToList();
      }

      foreach (var ugug in userGroupBases) {
        ugMapping[ugug.UserGroupId] = ugug.UserGroupUserGroupId;
      }

      foreach (Guid guid in allowedUserGroups) {
        if (CheckInGroupHierarchy(userId, guid, ugMapping, groups)) return true;
      }
      return false;
    }

    private bool CheckInGroupHierarchy(Guid userId, Guid group, Dictionary<Guid, Guid> ugMapping, List<DA.UserGroup> groups) {
      //check all subgroups
      var childs = ugMapping.Where(x => x.Value == group).Select(x => x.Key);
      var childGroups = childs.Where(x => groups.Where(y => y.Id == x).Count() > 0).ToList();
      //also check if user is in group
      childGroups.Add(group);

      foreach (Guid id in childGroups) {
        if (ugMapping.Where(x => x.Value == id).Select(x => x.Key).Contains(userId)) {
          return true;
        }
      }
      return false;
    }

    private static T TryAndRepeat<T>(Func<T> action) {
      int repetitions = 5;
      while (true) {
        try { return action(); }
        catch (Exception e) {
          if (repetitions == 0) throw e;
          repetitions--;
        }
      }
    }
  }
}
