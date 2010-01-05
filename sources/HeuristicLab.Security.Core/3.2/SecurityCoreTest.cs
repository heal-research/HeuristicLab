using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Security.Contracts.BusinessObjects;
using HeuristicLab.Hive.Server.Core;
using System.Diagnostics;

namespace HeuristicLab.Security.Core {
    [Application("Security Test App", "Test Application for the Security Service")]
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
 

      public void InsertClientUser() {
        PermissionCollection pc = HivePermissions.GetPermissions();
        Debug.WriteLine("Revoke permissions....");
        try {
          UserGroup group = manager.GetUserGroupByName("Projektmitglieder");
          manager.RevokePermission(group.Id, pc[pc.Convert(HivePermissions.Usermanagement.Client.Read)].Id, Guid.Empty);
          manager.RevokePermission(group.Id, pc[pc.Convert(HivePermissions.Usermanagement.ClientGroup.Read)].Id, Guid.Empty);
        }
        catch (Exception ex) {
          Debug.WriteLine(ex.Message);
        }
        Debug.WriteLine("Remove user...");
        try {
          manager.RemoveUser(manager.GetUserByName("HIVE User").Id);
        }
        catch (Exception ex) {
          Debug.WriteLine(ex.Message);
        }
        Debug.WriteLine("Remove group....");
        try {
          manager.RemoveUserGroup(manager.GetUserGroupByName("Projektmitglieder").Id);
        }
        catch (Exception ex) {
          Debug.WriteLine(ex.Message);
        }

        User user = new User();
        user.Login = "hive";
        user.SetHashedPassword("hive");
        user.Name = "HIVE User";
        Debug.WriteLine("Adding user...");
        try {
          user = manager.AddNewUser(user);
        }
        catch (Exception ex) {
          Debug.WriteLine(ex.Message);
        }

        UserGroup grp = new UserGroup();
        grp.Name = "Projektmitglieder";
        grp.Members.Add(user);
        Debug.WriteLine("Adding group...");
        try {
          manager.AddNewUserGroup(grp);
        }
        catch (Exception ex) {
          Debug.WriteLine(ex.Message);
        }
        Debug.WriteLine("Grant permissions...");
        try {
          manager.GrantPermission(grp.Id, pc[pc.Convert(HivePermissions.Usermanagement.Client.Read)].Id, Guid.Empty);
          manager.GrantPermission(grp.Id, pc[pc.Convert(HivePermissions.Usermanagement.ClientGroup.Read)].Id, Guid.Empty);
        }
        catch (Exception ex) {
          Debug.WriteLine(ex.Message);
        }
      }

      public void InsertSuperUser() {
        Debug.WriteLine("Revoke permissions...");
        try {
          UserGroup group = manager.GetUserGroupByName("Administratoren");
          foreach (Permission item in HivePermissions.GetPermissions()) {
            manager.RevokePermission(group.Id, item.Id, Guid.Empty);
          }
        }
        catch (Exception ex) {
          Debug.WriteLine(ex.Message);
        }
        Debug.WriteLine("Revoke permissions from admin...");
        try {
          Guid g = manager.GetUserByName("HIVE Admin").Id;
          foreach (Permission item in HivePermissions.GetPermissions()) {
            manager.RevokePermission(g, item.Id, Guid.Empty);
          }
        }
        catch (Exception ex) {
          Debug.WriteLine(ex.Message);
        }
        Debug.WriteLine("Removing user...");
        try {
          manager.RemoveUser(manager.GetUserByName("HIVE Admin").Id);
        }
        catch (Exception ex) {
          Debug.WriteLine(ex.Message);
        }
        Debug.WriteLine("Removing group...");
        try {
          manager.RemoveUserGroup(manager.GetUserGroupByName("Administratoren").Id);
        }
        catch (Exception ex) {
          Debug.WriteLine(ex.Message);
        }

        User user = new User();
        user.Login = "admin";
        user.SetHashedPassword("admin");
        user.Name = "HIVE Admin";
        User tmp = manager.AddNewUser(user);

        Debug.WriteLine("Grant ALL permissions to admin...");
        foreach (Permission item in HivePermissions.GetPermissions()) {
          try {
            manager.GrantPermission(tmp.Id, item.Id, Guid.Empty);
          }
          catch (Exception ex) {
            Debug.WriteLine(ex.Message);
          }
        }
        Debug.WriteLine("Adding user...");
        try {
          user = manager.AddNewUser(user);
        }
        catch (Exception ex) {
          Debug.WriteLine(ex.Message);
        }

        UserGroup grp = new UserGroup();
        grp.Name = "Administratoren";
        grp.Members.Add(user);
        Debug.WriteLine("Adding group...");
        try {
          manager.AddNewUserGroup(grp);
        }
        catch (Exception ex) {
          Debug.WriteLine(ex.Message);
        }
        Debug.WriteLine("Adding permissions...");
        //admins allowed to do everything ;)
        foreach (Permission item in HivePermissions.GetPermissions()) {
          try {
            manager.GrantPermission(grp.Id, item.Id, Guid.Empty);
          }
          catch (Exception ex) {
            Debug.WriteLine(ex.Message);
          }
        }
      }

      public void InsertHivePermissions() {
        foreach (Permission item in HivePermissions.GetPermissions()) {
          try {
            manager.AddPermission(item);
          }
          catch (Exception ex) {
            Debug.WriteLine(ex.Message);
          }
        }
      }

      public override void Run() {
        //testPermissionsGroup();
        
        InsertHivePermissions();
        InsertClientUser();
        InsertSuperUser();
        Debug.WriteLine("Complete!");
      }
    }
}
