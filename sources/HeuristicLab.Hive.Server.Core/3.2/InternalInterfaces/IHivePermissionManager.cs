using System;
namespace HeuristicLab.Hive.Server.Core.InternalInterfaces {
  public interface IHivePermissionManager {
    bool CheckPermission(Guid sessionID, Guid actionID, Guid entityId);
    Guid Login(string username, string password);
  }
}
