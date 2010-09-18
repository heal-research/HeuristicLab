using Service.Provider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
namespace UnitTests {
  /// <summary>
  ///This is a test class for HeuristicLabRoleProviderTest and is intended
  ///to contain all HeuristicLabRoleProviderTest Unit Tests
  ///</summary>
  [TestClass()]
  public class HeuristicLabRoleProviderTest : AbstractHeuristicLabTest {
    private TestContext testContextInstance;
    private const string TEST_ROLE_NAME = "testRole";
    private const string TEST_USER_NAME = "testUser";
    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext {
      get {
        return testContextInstance;
      }
      set {
        testContextInstance = value;
      }
    }

    #region Additional test attributes
    // 
    //You can use the following additional attributes as you write your tests:
    //
    //Use ClassInitialize to run code before running the first test in the class
    //[ClassInitialize()]
    //public static void MyClassInitialize(TestContext testContext)
    //{
    //}
    //
    //Use ClassCleanup to run code after all tests in a class have run
    //[ClassCleanup()]
    //public static void MyClassCleanup()
    //{
    //}
    //
    //Use TestInitialize to run code before running each test
    //[TestInitialize()]
    //public void MyTestInitialize()
    //{
    //}
    //
    //Use TestCleanup to run code after each test has run
    //[TestCleanup()]
    //public void MyTestCleanup()
    //{
    //}
    //
    #endregion


    /// <summary>
    ///A test for ApplicationName
    ///</summary>
    [TestMethod()]
    public void ApplicationNameTest() {
      HeuristicLabRoleProvider target = new HeuristicLabRoleProvider();
      string expected = "JavaIsEvenCooler";
      string actual;
      target.ApplicationName = expected;
      actual = target.ApplicationName;
      Assert.AreEqual(expected, actual);
    }

    /// <summary>
    ///tests if the RoleExits method works --> test is done in a positiv and negativ way
    ///</summary>
    [TestMethod()]
    public void RoleExistsTest() {
      HeuristicLabRoleProvider target = new HeuristicLabRoleProvider();
      Persistence.HeuristicLabRole role = new Persistence.HeuristicLabRole();
      role.RoleName = TEST_ROLE_NAME;
      db.HeuristicLabRole.InsertOnSubmit((Persistence.HeuristicLabRole)role);
      db.SubmitChanges();
      Assert.IsTrue(target.RoleExists(TEST_ROLE_NAME));
      Assert.IsFalse(target.RoleExists(TEST_ROLE_NAME + TEST_ROLE_NAME));
    }

    /// <summary>
    ///A test for RemoveUsersFromRoles
    ///</summary>
    [TestMethod()]
    public void RemoveUsersFromRolesTest() {

      //creates users dkhan,hmayer,bfarka (with AddUsersToRolesTest())
      //and add all these users to groups admin and users
      AddUsersToRolesTest();
      HeuristicLabRoleProvider target = new HeuristicLabRoleProvider();
      string[] users = new string[2];
      string[] roles = new string[1];

      //remove hmayer from role admin
      users[0] = "hmayr";
      roles[0] = "admin";


      //before removing, check if all three users exits correctly in admin group
      Assert.IsTrue((target.GetRolesForUser("hmayr").ToList()).Contains(roles[0]));
      Assert.IsTrue((target.GetRolesForUser("dkhan").ToList()).Contains(roles[0]));
      Assert.IsTrue((target.GetRolesForUser("bfarka").ToList()).Contains(roles[0]));

      //remmove hmayr from  admin group
      target.RemoveUsersFromRoles(users, roles);

      //final check
      Assert.IsTrue((target.GetRolesForUser("dkhan").ToList()).Contains(roles[0]));
      Assert.IsTrue((target.GetRolesForUser("bfarka").ToList()).Contains(roles[0]));
      Assert.IsFalse((target.GetRolesForUser("hmayr").ToList()).Contains(roles[0]));

    }

    /// <summary>
    /// test if user is in Role (positive and negative Assertion)
    ///</summary>
    [TestMethod()]
    public void IsUserInRoleTest() {
      HeuristicLabRoleProvider target = new HeuristicLabRoleProvider(); // TODO: Initialize to an appropriate value
      Dictionary<string, Persistence.HeuristicLabUser> users = new Dictionary<string, Persistence.HeuristicLabUser>();
      List<string> roles = new List<string>();
      users.Add("mholper", new Persistence.HeuristicLabUser("mholper", "foo", "password", "comment"));

      roles.Add("admin");
      roles.Add("users");
      foreach (string role in roles) {
        target.CreateRole(role);
      }
      foreach (Persistence.HeuristicLabUser user in users.Values) {
        db.HeuristicLabUsers.InsertOnSubmit(user);
      }
      db.SubmitChanges();
      string[] rolesToTest = new string[1];
      rolesToTest[0] = "admin";
      target.AddUsersToRoles(users.Keys.ToArray(), rolesToTest); // roles.ToArray());
      Assert.IsTrue(target.IsUserInRole("mholper", "admin"));
      Assert.IsFalse(target.IsUserInRole("mholper", "user"));
    }

    /// <summary>
    ///A test for GetUsersInRole
    ///</summary>
    [TestMethod()]
    public void GetUsersInRoleTest() {
      HeuristicLabRoleProvider target = new HeuristicLabRoleProvider(); // TODO: Initialize to an appropriate value
      string dummyRole = "dummyRole";
      Assert.IsTrue(target.GetUsersInRole(null).Length == 0);
      Assert.IsTrue(target.GetUsersInRole(dummyRole).Length == 0);
      Persistence.HeuristicLabUser user = new Persistence.HeuristicLabUser("dummyUser", "foo1", "foo", "foo");
      Persistence.HeuristicLabUser user2 = new Persistence.HeuristicLabUser("dummyUser2", "foo2", "foo", "foo");
      db.HeuristicLabUsers.InsertOnSubmit(user);
      db.HeuristicLabUsers.InsertOnSubmit(user2);
      db.SubmitChanges();
      target.CreateRole("testRole1");
      target.CreateRole("testRole2");
      List<string> users = new List<string>();
      List<string> roles = new List<string>();
      roles.Add("testRole1");
      roles.Add("testRole2");
      users.Add("dummyUser");

      //--> dummyUser get Role testRole1+testRole2
      target.AddUsersToRoles(users.ToArray(), roles.ToArray());
      string[] usersForRole = target.GetUsersInRole("testRole2");
      Assert.IsTrue(usersForRole.Length == 1);
      Assert.IsTrue(usersForRole.Contains("dummyUser"));

      //--> dummyUser2 get onl Role testRole1
      roles.Remove("testRole2");
      users.Remove("dummyUser");
      users.Add("dummyUser2");
      target.AddUsersToRoles(users.ToArray(), roles.ToArray());

      usersForRole = target.GetUsersInRole("testRole1");
      Assert.IsTrue(usersForRole.Length == 2);
      Assert.IsTrue(usersForRole.Contains("dummyUser"));
      Assert.IsTrue(usersForRole.Contains("dummyUser2"));

    }

    /// <summary>
    ///A test for GetRolesForUser
    ///</summary>
    [TestMethod()]
    public void GetRolesForUserTest() {
      HeuristicLabRoleProvider target = new HeuristicLabRoleProvider(); // TODO: Initialize to an appropriate value
      string dummyUser = "dummyUser";
      Assert.IsTrue(target.GetRolesForUser(null).Length == 0);
      Assert.IsTrue(target.GetRolesForUser(dummyUser).Length == 0);
      Persistence.HeuristicLabUser user = new Persistence.HeuristicLabUser("dummyUser", "foo", "foo", "foo");
      Persistence.HeuristicLabUser user2 = new Persistence.HeuristicLabUser("dummyUser2", "foo2", "foo", "foo");
      db.HeuristicLabUsers.InsertOnSubmit(user);
      db.HeuristicLabUsers.InsertOnSubmit(user2);
      db.SubmitChanges();
      target.CreateRole("testRole1");
      target.CreateRole("testRole2");
      List<string> users = new List<string>();
      List<string> roles = new List<string>();
      users.Add("dummyUser");
      users.Add("dummyUser2");
      roles.Add("testRole1");
      target.AddUsersToRoles(users.ToArray(), roles.ToArray());
      users.Remove("dummyUser2");
      roles.Add("testRole2");
      roles.Remove("testRole1");
      target.AddUsersToRoles(users.ToArray(), roles.ToArray());
      string[] rolesForUser = target.GetRolesForUser("dummyUser");
      Assert.IsTrue(rolesForUser.Length == 2);
      Assert.IsTrue(rolesForUser.Contains("testRole1"));
      Assert.IsTrue(rolesForUser.Contains("testRole2"));

      rolesForUser = target.GetRolesForUser("dummyUser2");
      Assert.IsTrue(rolesForUser.Length == 1);
      Assert.IsTrue(rolesForUser.Contains("testRole1"));



    }

    /// <summary>
    ///A test for GetAllRoles
    ///</summary>
    [TestMethod()]
    public void GetAllRolesTest() {
      HeuristicLabRoleProvider target = new HeuristicLabRoleProvider();
      List<string> roleNames = new List<string>();
      roleNames.Add("Pascal");
      roleNames.Add("Java");
      roleNames.Add("c++");
      roleNames.Add("VisalBasic");
      foreach (string s in roleNames) {
        target.CreateRole(s);
      }
      target.CreateRole(null);
      string[] roles = target.GetAllRoles();
      foreach (string role in roles) {
        Assert.IsTrue(roleNames.Remove(role));
      }
      Assert.IsTrue(roleNames.Count == 0);
    }

    /// <summary>
    ///A test for FindUsersInRole
    ///</summary>
    [TestMethod()]
    public void FindUsersInRoleTest() {
      HeuristicLabRoleProvider target = new HeuristicLabRoleProvider();
      Assert.IsTrue(target.FindUsersInRole(null, null).Length == 0);
      Assert.IsTrue(target.FindUsersInRole("dummyRole", null).Length == 0);
      Assert.IsTrue(target.FindUsersInRole(null, "dummyUser").Length == 0);
      Persistence.HeuristicLabUser user = new Persistence.HeuristicLabUser("dummyUser", "foo", "foo", "foo");
      Persistence.HeuristicLabUser user2 = new Persistence.HeuristicLabUser("dummyUser2", "foo2", "foo", "foo");
      db.HeuristicLabUsers.InsertOnSubmit(user);
      db.HeuristicLabUsers.InsertOnSubmit(user2);
      db.SubmitChanges();
      target.CreateRole("testRole");
      Assert.IsTrue(target.FindUsersInRole("testRole", "dummyUser").Length == 0);
      Assert.IsTrue(target.FindUsersInRole("testRole", "dummyUser2").Length == 0);
      Assert.IsTrue(target.FindUsersInRole("testRole", "dummyUser3").Length == 0);

      target.AddUsersToRoles(new string[] { "dummyUser", "dummyUser2" }, new string[] { "testRole" });
      Assert.IsTrue(target.FindUsersInRole("testRole", "dummyUser").Length == 2);
      Assert.IsTrue(target.FindUsersInRole("testRole", "dummyUser2").Length == 1);
      Assert.IsTrue(target.FindUsersInRole("testRole", "dummyUser3").Length == 0);


    }

    /// <summary>
    ///A test for DeleteRole
    ///</summary>
    [TestMethod()]
    public void DeleteRoleTest() {

      //creates users dkhan,hmayer,bfarka (with AddUsersToRolesTest())
      //and add all these users to groups admin and users
      AddUsersToRolesTest();
      HeuristicLabRoleProvider target = new HeuristicLabRoleProvider();
      string[] users = new string[2];
      string[] roles = new string[1];

      //remove hmayer from role admin
      users[0] = "hmayr";
      roles[0] = "admin";

      //before removing, check if all three users exits correctly in admin group
      Assert.IsTrue((target.GetRolesForUser("hmayr").ToList()).Contains(roles[0]));
      Assert.IsTrue((target.GetRolesForUser("dkhan").ToList()).Contains(roles[0]));
      Assert.IsTrue((target.GetRolesForUser("bfarka").ToList()).Contains(roles[0]));

      //try to delete Role admin (with user hmayr from  admin group exists)
      Assert.IsFalse(target.DeleteRole(roles[0], true));

      Assert.IsTrue(target.IsUserInRole(users[0], roles[0]));

      //final populateRole if false
      //try to delete Role admin (with user hmayr from  admin group exists)
      Assert.IsTrue(target.DeleteRole(roles[0], false));
      Assert.IsFalse(target.IsUserInRole(users[0], roles[0]));
      Assert.IsFalse((target.GetRolesForUser("hmayr").ToList()).Contains(roles[0]));
    }

    /// <summary>
    ///A test for CreateRole
    ///</summary>
    [TestMethod()]
    public void CreateRoleTest() {
      HeuristicLabRoleProvider target = new HeuristicLabRoleProvider();
      target.CreateRole("role1");
      target.CreateRole("role2");
      target.CreateRole("role3", true);
      Assert.IsTrue(db.HeuristicLabRole.Count(r => r.RoleName == "role1" && r.IsPermission == false) == 1);
      Assert.IsTrue(db.HeuristicLabRole.Count(r => r.RoleName == "role2" && r.IsPermission == false) == 1);
      Assert.IsTrue(db.HeuristicLabRole.Count(r => r.RoleName == "role3" && r.IsPermission == true) == 1);
    }

    protected int getUserRolesCount() {
      return db.HeuristicLabUserRole.Count();
    }


    /// <summary>
    ///A test for AddUsersToRoles
    ///</summary>
    [TestMethod()]
    public void AddUsersToRolesTest() {
      HeuristicLabRoleProvider target = new HeuristicLabRoleProvider(); // TODO: Initialize to an appropriate value
      Dictionary<string, Persistence.HeuristicLabUser> users = new Dictionary<string, Persistence.HeuristicLabUser>();
      List<string> roles = new List<string>();
      users.Add("dkhan", new Persistence.HeuristicLabUser("dkhan", "foo1", "password", "comment"));
      users.Add("hmayr", new Persistence.HeuristicLabUser("hmayr", "foo2", "password", "comment"));
      users.Add("bfarka", new Persistence.HeuristicLabUser("bfarka", "foo3", "password", "comment"));

      roles.Add("admin");
      roles.Add("users");
      //testing to create roles with users that doesn't exists
      target.AddUsersToRoles(users.Keys.ToArray(), roles.ToArray());
      Assert.IsTrue(getUserRolesCount() == 0);

      foreach (string role in roles) {
        target.CreateRole(role);
      }
      target.AddUsersToRoles(users.Keys.ToArray(), roles.ToArray());
      Assert.IsTrue(getUserRolesCount() == 0);
      foreach (Persistence.HeuristicLabUser user in users.Values) {
        db.HeuristicLabUsers.InsertOnSubmit(user);
      }
      db.SubmitChanges();
      target.AddUsersToRoles(users.Keys.ToArray(), roles.ToArray());
      Assert.IsTrue(getUserRolesCount() == (roles.Count * users.Count));

    }

    /// <summary>
    ///A test for HeuristicLabRoleProvider Constructor
    ///</summary>
    [TestMethod()]
    public void HeuristicLabRoleProviderConstructorTest() {
      HeuristicLabRoleProvider target = new HeuristicLabRoleProvider();
      Assert.IsNotNull(target);
    }
    [TestMethod()]
    public void NoDuplicateRoleTest() {
      try {
        Persistence.HeuristicLabRole role = new Persistence.HeuristicLabRole();
        role.RoleName = "role1";
        Persistence.HeuristicLabRole role1 = new Persistence.HeuristicLabRole();
        role1.RoleName = "role1";
        db.HeuristicLabRole.InsertOnSubmit(role);
        db.HeuristicLabRole.InsertOnSubmit(role1);
        db.SubmitChanges();
        Assert.Fail();
      }
      catch (SqlException) {
        //swallowing Exception because it is expected that a SQL Exception is thrown        
      }
    }
  }
} //HeuristicLabRoleProviderTest
