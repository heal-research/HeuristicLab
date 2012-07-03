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
using DA = HeuristicLab.Services.Access.DataAccess;
using DT = HeuristicLab.Services.Access.DataTransfer;


namespace HeuristicLab.Services.Access {
  public static class Convert {

    #region Resource
    public static DT.Resource ToDto(DA.Resource source) {
      return new DT.Resource() {
        Id = source.Id,
        Description = source.Description,
        Name = source.Name
      };
    }

    public static DA.Resource ToEntity(DT.Resource source) {
      return new DA.Resource() {
        Id = source.Id,
        Description = source.Description,
        Name = source.Name
      };
    }
    #endregion

    #region ClientGroup
    public static DT.ClientGroup ToDto(DA.ClientGroup source) {
      return new DT.ClientGroup() {
        Id = source.Id,
        Description = source.Description,
        Name = source.Name
      };
    }

    public static DA.ClientGroup ToEntity(DT.ClientGroup source) {
      return new DA.ClientGroup() {
        Id = source.Id,
        Description = source.Description,
        Name = source.Name,

      };
    }
    #endregion

    #region Country
    public static DT.Country ToDto(DA.Country source) {
      if (source == null) {
        return null;
      } else {
        return new DT.Country() {
          Id = source.Id,
          Name = source.Name
        };
      }
    }

    public static DA.Country ToEntity(DT.Country source) {
      if (source == null) {
        return null;
      } else {
        return new DA.Country() {
          Id = source.Id,
          Name = source.Name,

        };
      }
    }
    #endregion

    #region OperatingSystem
    public static DT.OperatingSystem ToDto(DA.OperatingSystem source) {
      if (source == null) {
        return null;
      } else {
        return new DT.OperatingSystem() {
          Id = source.Id,
          Name = source.Name
        };
      }
    }

    public static DA.OperatingSystem ToEntity(DT.OperatingSystem source) {
      if (source == null) {
        return null;
      } else {
        return new DA.OperatingSystem() {
          Id = source.Id,
          Name = source.Name,
        };
      }
    }
    #endregion

    #region ClientType
    public static DT.ClientType ToDto(DA.ClientType source) {
      if (source == null) {
        return null;
      } else {
        return new DT.ClientType() {
          Id = source.Id,
          Name = source.Name
        };
      }
    }

    public static DA.ClientType ToEntity(DT.ClientType source) {
      if (source == null) {
        return null;
      } else {
        return new DA.ClientType() {
          Id = source.Id,
          Name = source.Name,

        };
      }
    }
    #endregion

    #region ClientConfiguration
    public static DT.ClientConfiguration ToDto(DA.ClientConfiguration source) {
      if (source == null) {
        return null;
      } else {
        return new DT.ClientConfiguration() {
          Id = source.Id,
          Hash = source.Hash,
          Description = source.Description
        };
      }
    }

    public static DA.ClientConfiguration ToEntity(DT.ClientConfiguration source) {
      if (source == null) {
        return null;
      } else {
        return new DA.ClientConfiguration() {
          Id = source.Id,
          Hash = source.Hash,
          Description = source.Description
        };
      }
    }
    #endregion

    #region Plugin
    public static DT.Plugin ToDto(DA.Plugin source) {
      return new DT.Plugin() {
        Id = source.Id,
        Name = source.Name,
        StrongName = source.StrongName,
        Version = source.Version
      };
    }

    public static DA.Plugin ToEntity(DT.Plugin source) {
      return new DA.Plugin() {
        Id = source.Id,
        Name = source.Name,
        StrongName = source.StrongName,
        Version = source.Version
      };
    }
    #endregion

    #region Client
    public static DT.Client ToDto(DA.Client source) {
      return new DT.Client() {
        Id = source.Id,
        Description = source.Description,
        Name = source.Name,
        ClientConfiguration = ToDto(source.ClientConfiguration),
        HeuristicLabVersion = source.HeuristicLabVersion,
        Country = ToDto(source.Country),
        OperatingSystem = ToDto(source.OperatingSystem),
        MemorySize = source.MemorySize.GetValueOrDefault(),
        Timestamp = source.Timestamp.GetValueOrDefault(),
        NumberOfCores = source.NumberOfCores.GetValueOrDefault(),
        ProcessorType = source.ProcessorType,
        PerformanceValue = source.PerformanceValue.GetValueOrDefault(),
        ClientType = ToDto(source.ClientType)
      };
    }

    public static DA.Client ToEntity(DT.Client source) {
      return new DA.Client() {
        Id = source.Id,
        Description = source.Description,
        Name = source.Name,
        ClientConfiguration = ToEntity(source.ClientConfiguration),
        HeuristicLabVersion = source.HeuristicLabVersion,
        Country = ToEntity(source.Country),
        OperatingSystem = ToEntity(source.OperatingSystem),
        MemorySize = source.MemorySize,
        Timestamp = source.Timestamp,
        NumberOfCores = source.NumberOfCores,
        ProcessorType = source.ProcessorType,
        PerformanceValue = source.PerformanceValue,
        ClientType = ToEntity(source.ClientType)
      };
    }
    #endregion

    #region ClientLog
    public static DT.ClientLog ToDto(DA.ClientLog source) {
      return new DT.ClientLog() {
        Timestamp = source.Timestamp,
        ResourceId = source.ResourceId,
        Message = source.Message
      };
    }

    public static DA.ClientLog ToEntity(DT.ClientLog source) {
      return new DA.ClientLog() {
        Timestamp = source.Timestamp,
        ResourceId = source.ResourceId,
        Message = source.Message
      };
    }
    #endregion

    #region ClientError
    public static DT.ClientError ToDto(DA.ClientError source) {
      return new DT.ClientError() {
        Id = source.Id,
        Timestamp = source.Timestamp,
        Exception = source.Exception,
        UserComment = source.UserComment,
        ConfigDump = source.ConfigDump,
        ClientId = source.ClientId.GetValueOrDefault(),
        UserId = source.UserId.GetValueOrDefault()
      };
    }

    public static DA.ClientError ToEntity(DT.ClientError source) {
      return new DA.ClientError() {
        Id = source.Id,
        Timestamp = source.Timestamp,
        Exception = source.Exception,
        UserComment = source.UserComment,
        ConfigDump = source.ConfigDump,
        ClientId = source.ClientId,
        UserId = source.UserId
      };
    }
    #endregion

    #region UserGroup
    public static DT.UserGroup ToDto(DA.UserGroup source) {
      return new DT.UserGroup() {
        Id = source.Id,
        Name = source.Name
      };
    }

    public static DA.UserGroup ToEntity(DT.UserGroup source) {
      return new DA.UserGroup() {
        Id = source.Id,
        Name = source.Name
      };
    }

    public static void ToEntity(DT.UserGroup source, DA.UserGroup target) {
      target.Name = source.Name;
    }
    #endregion

    #region User

    public static DT.LightweightUser ToDto(DA.User source, DA.aspnet_User aspUserSource, DA.aspnet_Membership aspMembershipSource, List<DA.aspnet_Role> roles, List<DA.UserGroup> groups) {
      return new DT.LightweightUser() {
        Id = source.Id,
        FullName = source.FullName,
        UserName = aspUserSource.UserName,
        EMail = aspMembershipSource.Email,
        //TODO: check if the roles and groups are include in source
        Roles = roles.Select(x => Convert.ToDto(x)).ToArray(),
        Groups = groups.Select(x => Convert.ToDto(x)).ToArray()
      };
    }

    public static DT.User ToDto(DA.User source, DA.aspnet_User aspUserSource, DA.aspnet_Membership aspMembershipSource) {
      return new DT.User() {
        Id = source.Id,
        FullName = source.FullName,
        Comment = aspMembershipSource.Comment,
        CreationDate = aspMembershipSource.CreateDate,
        Email = aspMembershipSource.Email,
        IsApproved = aspMembershipSource.IsApproved,
        LastActivityDate = aspUserSource.LastActivityDate,
        LastLoginDate = aspMembershipSource.LastLoginDate,
        LastPasswordChangedDate = aspMembershipSource.LastPasswordChangedDate,
        UserName = aspUserSource.UserName
      };
    }

    public static DA.User ToEntity(DT.User source) {
      return new DA.User() { Id = source.Id, FullName = source.FullName };
    }

    public static void ToEntity(DT.User source, out DA.User accessUser, out DA.aspnet_User aspUser, out DA.aspnet_Membership aspMembership, out bool userExistsInASP) {
      userExistsInASP = false;
      accessUser = new DA.User();
      aspUser = new DA.aspnet_User();
      aspMembership = new DA.aspnet_Membership();

      if (source.Id != Guid.Empty) {
        using (DA.ASPNETAuthenticationDataContext context = new DA.ASPNETAuthenticationDataContext()) {
          var userCol = context.aspnet_Users.Where(s => s.UserId == source.Id);
          var membershipCol = context.aspnet_Memberships.Where(s => s.UserId == source.Id);
          if (userCol.Count() > 0 && membershipCol.Count() > 0) {
            aspUser = userCol.First();
            aspMembership = membershipCol.First();
            userExistsInASP = true;
          }
        }
      }

      accessUser.Id = source.Id;
      accessUser.FullName = source.FullName;

      aspUser.UserId = source.Id;
      aspUser.LastActivityDate = source.LastActivityDate;
      aspUser.UserName = source.UserName;

      aspMembership.UserId = source.Id;
      aspMembership.Comment = source.Comment;
      aspMembership.CreateDate = source.CreationDate;
      aspMembership.Email = source.Email;
      aspMembership.IsApproved = source.IsApproved;
      aspMembership.LastLoginDate = source.LastLoginDate;
      aspMembership.LastPasswordChangedDate = source.LastPasswordChangedDate;
    }
    #endregion

    #region ClientGroupMapping
    public static DT.ClientGroupMapping ToDto(DA.ResourceResourceGroup source) {
      return new DT.ClientGroupMapping() {
        Child = source.ResourceId, Parent = source.ResourceGroupId
      };
    }
    #endregion

    #region UserGroupBase
    public static DT.UserGroupBase ToDto(DA.UserGroupBase source) {
      return new DT.UserGroupBase() {
        Id = source.Id
      };
    }
    #endregion

    #region UserGroupMapping
    public static DT.UserGroupMapping ToDto(DA.UserGroupUserGroup source) {
      return new DT.UserGroupMapping() {
        Child = source.UserGroupId, Parent = source.UserGroupUserGroupId
      };
    }
    #endregion

    #region Role
    public static DT.Role ToDto(DA.aspnet_Role r) {
      return new DT.Role() {
        Name = r.RoleName
      };
    }
    public static DA.aspnet_Role ToEntity(DT.Role r) {
      return new DA.aspnet_Role() {
        RoleName = r.Name
      };
    }
    #endregion
  }
}
