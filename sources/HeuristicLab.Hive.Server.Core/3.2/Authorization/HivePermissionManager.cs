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
using System.ServiceModel;

namespace HeuristicLab.Hive.Server.Core {
  public class HivePermissionManager : IHivePermissionManager {

    private IJobManager jobManager = ServiceLocator.GetJobManager();
    
    private IPermissionManager permManager = ServiceLocator.GetPermissionManager();

    private IClientManager clientManager = ServiceLocator.GetClientManager();

    private PermissionCollection permissions = HivePermissions.GetPermissions();
    private PolicyCollection policyCollection = HivePermissions.GetPolicies();

    /// <summary>
    /// Authenticates an user and returns a valid guid if success.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public Guid Login(string username, string password) {
      return permManager.Authenticate(username, password);
    }

    /// <summary>
    /// Checks user permission against predefined policy.
    /// </summary>
    /// <param name="policyName">Policy Name defines the action.</param>
    /// <param name="sessionID">Session ID identifies a currently logged on user.</param>
    /// <param name="entityID">Entity ID can be some resource or emtpy.</param>
    public void Authorize(string policyName, Guid sessionID, Guid entityID) {
      //check if this policy has a permission with 'ANY' scope defined
      Permission p = policyCollection[policyName].GetPermissionByContext("Any");
      
      //check if user has 'xxx.Any' permission
      if (p != null)
        if (CheckPermission(sessionID, p.Id, entityID)) return;
      
      //check if this policy has a permission with 'PROJECT' scope defined
      p = policyCollection[policyName].GetPermissionByContext("Project");
      //check if user has 'xxx.Project' permission
      if (p != null)
        if (CheckPermission(sessionID, p.Id, jobManager.GetJobById(entityID).Obj.Project.Id)) return;

      //check if this policy has a permission with 'OWNER' scope defined
      p = policyCollection[policyName].GetPermissionByContext("User");
      //check if user has 'xxx.Owner' permission
      if (p != null)
        if (CheckPermission(sessionID, p.Id, jobManager.GetJobById(entityID).Obj.UserId)) return;
      //throw an exception when user access fails
      throw new PermissionException(policyName);
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
      PermissionCollection pc = HivePermissions.GetPermissions();
      if (!hasPerm)
          if (CheckPermissionHelper(sessionID, actionID, entityId)) return true;
      return hasPerm;
    }

    private bool CheckPermissionHelper(Guid sessionId, Guid actionId, Guid entityId) {
      if (entityId == Guid.Empty) return true;
      IList<ClientGroup> groups = clientManager.GetAllGroupsOfResource(entityId).Obj;
      foreach (ClientGroup group in groups)
        if (CheckPermission(sessionId, actionId, group.Id)) return true;
      return false;
    }
  }
}
