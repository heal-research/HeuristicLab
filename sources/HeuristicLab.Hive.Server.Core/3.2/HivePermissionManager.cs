using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Security.Contracts.Interfaces;
using HeuristicLab.DataAccess;
using HeuristicLab.Security.Contracts.BusinessObjects;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts;
using HeuristicLab.Hive.Server.Core.InternalInterfaces;

namespace HeuristicLab.Hive.Server.Core {
  public class HivePermissionManager : IHivePermissionManager {
    
    private IPermissionManager permManager = ServiceLocator.GetPermissionManager();

    private IClientManager clientManager = ServiceLocator.GetClientManager();

    /// <summary>
    /// Authenticates a user and returns a valid guid if success.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public Guid Login(string username, string password) {
      return permManager.Authenticate(username, password);
    }

    /// <summary>
    /// Checks if a user has permission for a specified action.
    /// </summary>
    /// <param name="sessionID">The current session.</param>
    /// <param name="actionID">A pre-defined action that requires permission.</param>
    /// <param name="entityID">A resource (Job,...)</param>
    /// <returns></returns>
    public bool CheckPermission(Guid sessionID, Guid actionID, Guid entityId) {
      bool hasPerm = permManager.CheckPermission(sessionID, actionID, entityId);
      
      if (!hasPerm) {
        if ((actionID == PermissiveSecurityAction.Add_Job) ||
            (actionID == PermissiveSecurityAction.Remove_Job) ||
            (actionID == PermissiveSecurityAction.Request_Snapshot)||
            (actionID==PermissiveSecurityAction.Abort_Job))
          if (CheckPermissionHelper(sessionID, actionID, entityId)) return true;
      }

      return hasPerm;      
    }

    private bool CheckPermissionHelper(Guid sessionId, Guid actionId, Guid entityId) {
      IList<ClientGroup> groups = clientManager.GetAllGroupsOfResource(entityId).Obj;
      foreach (ClientGroup group in groups) 
        if (CheckPermission(sessionId, actionId, group.Id)) return true;
      return false;
    }
  }
}
