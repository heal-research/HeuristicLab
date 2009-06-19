using System;
namespace HeuristicLab.Hive.Server.Core.InternalInterfaces {
  public interface IHivePermissionManager {
    /// <summary>
    /// Checks user permission against predefined policy in database.
    /// </summary>
    /// <param name="permission">the name of policy defined in xml-file</param>
    /// <param name="sessionID">the users current session ID</param>
    /// <param name="entityID"></param>
    /// <exception cref="PermissionException">thrown when access denied</exception>
    void Authorize(string policyName, Guid sessionID, Guid entityID);
    bool CheckPermission(Guid sessionID, Guid actionID, Guid entityId);
    Guid Login(string username, string password);
  }
}
