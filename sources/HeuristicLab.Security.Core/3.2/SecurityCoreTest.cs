using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Security.Contracts.BusinessObjects;

namespace HeuristicLab.Security.Core {
    [ClassInfo(Name = "Security Test App",
        Description = "Test Application for the Security Service",
        AutoRestart = true)]
  class SecurityCoreTest : ApplicationBase {
      private void testGroups() {
        SecurityManager manager =
          new SecurityManager();

        User user = new User();
        user.Login = "anna";
        user.Password = "blubb";
        user.Name = "Anna";
        user = manager.AddNewUser(user);

        User user2 = new User();
        user2.Login = "stefan";
        user2.Name = "Stefan";

        UserGroup group = new UserGroup();
        group.Name = "Students";

        UserGroup group2 = new UserGroup();
        group2.Name = "Lazy Students";

        group.Members.Add(user);
        group.Members.Add(group2);

        group2.Members.Add(user2);

        manager.AddNewUserGroup(group);

        group = manager.GetUserGroupByName("Students");
        group2 = manager.GetUserGroupByName("Lazy Students");

        manager.RemoveUserGroup(group2.Id);

        group = manager.GetUserGroupByName("Students");
        group2 = manager.GetUserGroupByName("Lazy Students");

        manager.RemoveUser(user.Id);

        group = manager.GetUserGroupByName("Students");
        manager.RemoveUserGroup(group.Id);

        group = manager.GetUserGroupByName("Students");
        manager.RemoveUser(user2.Id);
      }

      private void testPermissions() {
        SecurityManager manager =
           new SecurityManager();

        PermissionManager permManager =
          new PermissionManager();

        Permission permission = new Permission();
        permission.Name = "ADD_JOBS";
        permission.Description = "Add jobs";
        permission.Plugin = "TestPlugin";

        manager.AddPermission(permission);

        permission = manager.GetPermissionById(permission.Id);

        User user = new User();
        user.Login = "anna";
        user.Password = "blubb";
        user.Name = "Anna";
        user = manager.AddNewUser(user);

        manager.GrantPermission(user.Id, permission.Id, Guid.Empty);

        Guid sessionId = permManager.Authenticate("anna", "blubb");

        bool hasPermission = 
          permManager.CheckPermission(sessionId, permission.Id, Guid.Empty);

        manager.RevokePermission(user.Id, permission.Id, Guid.Empty);

        hasPermission =
          permManager.CheckPermission(sessionId, permission.Id, Guid.Empty);

        permManager.EndSession(sessionId);

        manager.RemoveUser(user.Id);

        manager.RemovePermission(permission.Id);

        permission = manager.GetPermissionById(permission.Id);
      }

      private void testPermissionsGroup() {
        SecurityManager manager =
          new SecurityManager();

        PermissionManager permManager =
          new PermissionManager();

        User user = new User();
        user.Login = "anna";
        user.Password = "blubb";
        user.Name = "Anna";
        user = manager.AddNewUser(user);

        User user2 = new User();
        user2.Login = "stefan";
        user2.Password = "foo";
        user2.Name = "Stefan";

        UserGroup group = new UserGroup();
        group.Name = "Students";

        UserGroup group2 = new UserGroup();
        group2.Name = "Lazy Students";

        group.Members.Add(user);
        group.Members.Add(group2);

        group2.Members.Add(user2);

        manager.AddNewUserGroup(group);

        Permission permission = new Permission();
        permission.Name = "ADD_JOBS";
        permission.Description = "Add jobs";
        permission.Plugin = "TestPlugin";

        manager.AddPermission(permission);

        manager.GrantPermission(group.Id, permission.Id, Guid.Empty);
        manager.GrantPermission(group2.Id, permission.Id, Guid.Empty);

        Guid sessionId = permManager.Authenticate("anna", "blubb");

        bool hasPermission =
          permManager.CheckPermission(sessionId, permission.Id, Guid.Empty);

        Guid sessionId2 = permManager.Authenticate("stefan", "foo");

        hasPermission =
          permManager.CheckPermission(sessionId2, permission.Id, Guid.Empty);

        manager.RevokePermission(group.Id, permission.Id, Guid.Empty);

        hasPermission =
          permManager.CheckPermission(sessionId, permission.Id, Guid.Empty);

        hasPermission =
          permManager.CheckPermission(sessionId2, permission.Id, Guid.Empty);

        manager.RevokePermission(group2.Id, permission.Id, Guid.Empty);

        hasPermission =
          permManager.CheckPermission(sessionId2, permission.Id, Guid.Empty);

        permManager.EndSession(sessionId);
        permManager.EndSession(sessionId2);

        manager.RemoveUser(user.Id);
        manager.RemoveUser(user2.Id);
        manager.RemoveUserGroup(group.Id);
        manager.RemoveUserGroup(group2.Id);

        manager.RemovePermission(permission.Id);                        
      }
      
      public override void Run() {
        testPermissionsGroup();
      }
    }
}
