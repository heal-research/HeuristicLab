using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Security.Contracts.BusinessObjects;
using HeuristicLab.Hive.Server.Core;

namespace HeuristicLab.Security.Core {
    [ClassInfo(Name = "Security Test App",
        Description = "Test Application for the Security Service",
        AutoRestart = true)]
  class SecurityCoreTest : ApplicationBase {

      SecurityManager manager = new SecurityManager();
      PermissionManager permManager =  new PermissionManager();


      private void testGroups() {

        User user = new User();
        user.Login = "anna";
        user.SetHashedPassword("blubb");
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

        Permission permission = new Permission();
        permission.Name = "ADD_JOBS";
        permission.Description = "Add jobs";
        permission.Plugin = "TestPlugin";

        manager.AddPermission(permission);

        permission = manager.GetPermissionById(permission.Id);

        User user = new User();
        user.Login = "anna";
        user.SetHashedPassword("blubb");
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

        User user = new User();
        user.Login = "anna";
        user.SetHashedPassword("blubb");
        user.Name = "Anna";
        user = manager.AddNewUser(user);

        User user2 = new User();
        user2.Login = "stefan";
        user2.SetHashedPassword("foo");
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
      #region TEST DATA INSERT SECTION for MB

      private void InsertTestDataForPermissionCheck() {
        int numberOfUsers = 10;

        List<User> users = new List<User>();
        for (int i = 0; i < numberOfUsers; i++) {
          users.Add(MB_InsertTestUser());
        }
        MB_AssignGroups(users);
        
      }

      private User MB_InsertTestUser() {
        Random rand = new Random(DateTime.Now.Millisecond);
        int usr = rand.Next(1000);
        User user = new User();
        user.Login = "test" + usr.ToString();
        user.SetHashedPassword("test");
        user.Name = "test" + usr.ToString();
        return manager.AddNewUser(user);
      }

      /// <summary>
      /// Splits the given users into two groups and assigns them. Some will be not assigned.
      /// </summary>
      /// <param name="users"></param>
      private void MB_AssignGroups(List<User> users) {
        UserGroup group01 = new UserGroup();
        group01.Name = "Test Group 01";

        UserGroup group02 = new UserGroup();
        group02.Name = "Test Group 02";

        //three-way split users into group1, group2 and none
        int idx = users.Count / 3;
        for (int i = 0; i < idx; i++) {
          group01.Members.Add(users[i]);
        }
        for (int i = idx; i < users.Count / 2; i++) {
          group02.Members.Add(users[i]);
        }
        manager.AddNewUserGroup(group01);
        manager.AddNewUserGroup(group02);
        
      }

      private void MB_AddPermission(List<User> users) {
        if (users.Count < 2) return;
        Permission permission = new Permission();
        //permission.Id = PermissiveSecurityAction.Add_Job;
        permission.Name = "ADD_JOB";
        permission.Description = "Add new jobs";
        permission.Plugin = "HeuristicLab.Hive.Server";
        
        manager.AddPermission(permission);
        //grant permission to random users
        Random rand = new Random(DateTime.Now.Millisecond);
        for (int i = 0; i < users.Count/2; i++) {
          int idx = rand.Next(users.Count);
          manager.GrantPermission(users[i].Id, permission.Id, Guid.Empty);
        }
      }

      #endregion
      public override void Run() {
        //testPermissionsGroup();
        InsertTestDataForPermissionCheck();
      }
    }
}
