using System;
using System.Collections.Specialized;
using Service.Provider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Security;
using Persistence;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace UnitTests {
  /// <summary>
  ///This is a test class for HeuristicLabMembershipProviderTest and is intended
  ///to contain all HeuristicLabMembershipProviderTest Unit Tests
  ///</summary>
  [TestClass()]
  public class HeuristicLabMembershipProviderTest : AbstractHeuristicLabTest {
    private TestContext testContextInstance;

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
    ///A test for RequiresUniqueEmail
    ///</summary>
    [TestMethod()]
    public void RequiresUniqueEmailTest() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider(); // TODO: Initialize to an appropriate value
      bool actual;
      actual = target.RequiresUniqueEmail;
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for RequiresQuestionAndAnswer
    ///</summary>
    [TestMethod()]
    public void RequiresQuestionAndAnswerTest() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider(); // TODO: Initialize to an appropriate value
      bool actual;
      actual = target.RequiresQuestionAndAnswer;
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for PasswordStrengthRegularExpression
    ///</summary>
    [TestMethod()]
    public void PasswordStrengthRegularExpressionTest() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider(); // TODO: Initialize to an appropriate value
      string actual;
      actual = target.PasswordStrengthRegularExpression;
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for PasswordFormat
    ///</summary>
    [TestMethod()]
    public void PasswordFormatTest() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider(); // TODO: Initialize to an appropriate value
      MembershipPasswordFormat actual;
      actual = target.PasswordFormat;
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for PasswordAttemptWindow
    ///</summary>
    [TestMethod()]
    public void PasswordAttemptWindowTest() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider(); // TODO: Initialize to an appropriate value
      int actual;
      actual = target.PasswordAttemptWindow;
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for MinRequiredPasswordLength
    ///</summary>
    [TestMethod()]
    public void MinRequiredPasswordLengthTest() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider(); // TODO: Initialize to an appropriate value
      int actual;
      actual = target.MinRequiredPasswordLength;
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for MinRequiredNonAlphanumericCharacters
    ///</summary>
    [TestMethod()]
    public void MinRequiredNonAlphanumericCharactersTest() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider(); // TODO: Initialize to an appropriate value
      int actual;
      actual = target.MinRequiredNonAlphanumericCharacters;
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for MaxInvalidPasswordAttempts
    ///</summary>
    [TestMethod()]
    public void MaxInvalidPasswordAttemptsTest() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider(); // TODO: Initialize to an appropriate value
      int actual;
      actual = target.MaxInvalidPasswordAttempts;
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for EnablePasswordRetrieval
    ///</summary>
    [TestMethod()]
    public void EnablePasswordRetrievalTest() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider(); // TODO: Initialize to an appropriate value
      bool actual;
      actual = target.EnablePasswordRetrieval;
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for EnablePasswordReset
    ///</summary>
    [TestMethod()]
    public void EnablePasswordResetTest() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider(); // TODO: Initialize to an appropriate value
      bool actual;
      actual = target.EnablePasswordReset;
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for ApplicationName
    ///</summary>
    [TestMethod()]
    public void ApplicationNameTest() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider(); // TODO: Initialize to an appropriate value
      string expected = string.Empty; // TODO: Initialize to an appropriate value
      string actual;
      target.ApplicationName = expected;
      actual = target.ApplicationName;
      Assert.AreEqual(expected, actual);
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for ValidateUser
    ///</summary>
    [TestMethod()]
    public void ValidateUserTest() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider();
      MembershipCreateStatus status;
      target.CreateUser("testname", "newPassword", "testemail", "testquestion", "testanswer", true, null, out status);
      Assert.IsTrue(target.ValidateUser("testname", "newPassword"));
    }

    /// <summary>
    ///A test for ValidateUser
    ///</summary>
    [TestMethod()]
    public void ValidateUserTestEncrypted() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider();
      NameValueCollection config = new NameValueCollection();
      config.Add("passwordFormat", "Encrypted");
      target.Initialize("", config);
      MembershipCreateStatus status;
      target.CreateUser("testname", "newPassword", "testemail", "testquestion", "testanswer", true, null, out status);
      Assert.IsTrue(target.ValidateUser("testname", "newPassword"));
    }

    /// <summary>
    ///A test for ValidateUser
    ///</summary>
    [TestMethod()]
    public void ValidateUserTestHashed() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider();
      NameValueCollection config = new NameValueCollection();
      config.Add("passwordFormat", "Hashed");
      target.Initialize("", config);
      MembershipCreateStatus status;
      target.CreateUser("testname", "newPassword", "testemail", "testquestion", "testanswer", true, null, out status);
      Assert.IsTrue(target.ValidateUser("testname", "newPassword"));
    }

    /// <summary>
    ///A test for UpdateUser
    ///</summary>
    [TestMethod()]
    public void UpdateUserTest() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider(); // TODO: Initialize to an appropriate value
      MembershipUser user = null; // TODO: Initialize to an appropriate value
      target.UpdateUser(user);
      Assert.Inconclusive("A method that does not return a value cannot be verified.");
    }

    /// <summary>
    ///A test for LockUser
    ///</summary>
    [TestMethod()]
    public void LockUserTest()
    {
        HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider();
        MembershipCreateStatus status;
        HeuristicLabUser user = (HeuristicLabUser)target.CreateUser("testname", "newPassword", "testemail", "testquestion", "testanswer", true, null, out status);
        user.UnlockUser();
        Assert.IsFalse(user.Locked);
        user.LockUser();
        Assert.IsTrue(user.Locked);
        Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for UnlockUser
    ///</summary>
    [TestMethod()]
    public void UnlockUserTest()
    {
        HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider();
        MembershipCreateStatus status;
        HeuristicLabUser user = (HeuristicLabUser)target.CreateUser("testname", "newPassword", "testemail", "testquestion", "testanswer", true, null, out status);
        user.LockUser();
        Assert.IsTrue(user.Locked);
        user.UnlockUser();
        Assert.IsFalse(user.Locked);
        Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for ResetPassword
    ///</summary>
    [TestMethod()]
    public void ResetPasswordTest() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider(); // TODO: Initialize to an appropriate value
      string username = string.Empty; // TODO: Initialize to an appropriate value
      string answer = string.Empty; // TODO: Initialize to an appropriate value
      string expected = string.Empty; // TODO: Initialize to an appropriate value
      string actual;
      actual = target.ResetPassword(username, answer);
      Assert.AreEqual(expected, actual);
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for GetUserNameByEmail
    ///</summary>
    [TestMethod()]
    public void GetUserNameByEmailTest() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider(); // TODO: Initialize to an appropriate value
      string email = string.Empty; // TODO: Initialize to an appropriate value
      string expected = string.Empty; // TODO: Initialize to an appropriate value
      string actual;
      actual = target.GetUserNameByEmail(email);
      Assert.AreEqual(expected, actual);
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for GetUser
    ///</summary>
    [TestMethod()]
    public void GetUserTest1() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider(); // TODO: Initialize to an appropriate value
      object providerUserKey = null; // TODO: Initialize to an appropriate value
      bool userIsOnline = false; // TODO: Initialize to an appropriate value
      MembershipUser expected = null; // TODO: Initialize to an appropriate value
      MembershipUser actual;
      actual = target.GetUser(providerUserKey, userIsOnline);
      Assert.AreEqual(expected, actual);
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for GetUser
    ///</summary>
    [TestMethod()]
    public void GetUserTest() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider(); // TODO: Initialize to an appropriate value
      string username = string.Empty; // TODO: Initialize to an appropriate value
      bool userIsOnline = false; // TODO: Initialize to an appropriate value
      MembershipUser expected = null; // TODO: Initialize to an appropriate value
      MembershipUser actual;
      actual = target.GetUser(username, userIsOnline);
      Assert.AreEqual(expected, actual);
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for GetPassword
    ///</summary>
    [TestMethod()]
    public void GetPasswordTest() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider(); // TODO: Initialize to an appropriate value
      string username = string.Empty; // TODO: Initialize to an appropriate value
      string answer = string.Empty; // TODO: Initialize to an appropriate value
      string expected = string.Empty; // TODO: Initialize to an appropriate value
      string actual;
      actual = target.GetPassword(username, answer);
      Assert.AreEqual(expected, actual);
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for GetNumberOfUsersOnline
    ///</summary>
    [TestMethod()]
    public void GetNumberOfUsersOnlineTest() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider(); // TODO: Initialize to an appropriate value
      int expected = 0; // TODO: Initialize to an appropriate value
      int actual;
      actual = target.GetNumberOfUsersOnline();
      Assert.AreEqual(expected, actual);
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for GetAllUsers
    ///
    /// work in progress
    ///</summary>
    [TestMethod()]
    public void GetAllUsersTest() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider();
      MembershipCreateStatus status;
      int totalRecords;

      // Create some users
      for (int i = 0; i < 50; i++)
      {
          target.CreateUser("User " + i, "newPassword " + i, "testemail " + i, "testquestion " + i, "testanswer " + i, true, null, out status);
      }

      MembershipUserCollection users = target.GetAllUsers(0, 5, out totalRecords);

      int j = 0;
      foreach (HeuristicLabUser user in users) {
        Assert.Equals("User " + j, user.UserName);
        j++;
      }


    }

    /// <summary>
    ///A test for FindUsersByName
    ///</summary>
    [TestMethod()]
    public void FindUsersByNameTest() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider(); // TODO: Initialize to an appropriate value
      string usernameToMatch = string.Empty; // TODO: Initialize to an appropriate value
      int pageIndex = 0; // TODO: Initialize to an appropriate value
      int pageSize = 0; // TODO: Initialize to an appropriate value
      int totalRecords = 0; // TODO: Initialize to an appropriate value
      int totalRecordsExpected = 0; // TODO: Initialize to an appropriate value
      MembershipUserCollection expected = null; // TODO: Initialize to an appropriate value
      MembershipUserCollection actual;
      actual = target.FindUsersByName(usernameToMatch, pageIndex, pageSize, out totalRecords);
      Assert.AreEqual(totalRecordsExpected, totalRecords);
      Assert.AreEqual(expected, actual);
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for FindUsersByEmail
    ///</summary>
    [TestMethod()]
    public void FindUsersByEmailTest() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider(); // TODO: Initialize to an appropriate value
      string emailToMatch = string.Empty; // TODO: Initialize to an appropriate value
      int pageIndex = 0; // TODO: Initialize to an appropriate value
      int pageSize = 0; // TODO: Initialize to an appropriate value
      int totalRecords = 0; // TODO: Initialize to an appropriate value
      int totalRecordsExpected = 0; // TODO: Initialize to an appropriate value
      MembershipUserCollection expected = null; // TODO: Initialize to an appropriate value
      MembershipUserCollection actual;
      actual = target.FindUsersByEmail(emailToMatch, pageIndex, pageSize, out totalRecords);
      Assert.AreEqual(totalRecordsExpected, totalRecords);
      Assert.AreEqual(expected, actual);
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for DeleteUser
    ///</summary>
    [TestMethod()]
    public void DeleteUserTest() {
      // insert new user
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider();
      MembershipCreateStatus status;
      target.CreateUser("testname", "newPassword", "testemail", "testquestion", "testanswer", true, null, out status);
      Assert.AreEqual(MembershipCreateStatus.Success, status);

      // delete user
      Assert.IsTrue(target.DeleteUser("testname", true));
      Assert.AreEqual(0, db.HeuristicLabUsers.Count(x => x.UserName == "testname"));
    }

    /// <summary>
    ///A test for CreateUser
    ///</summary>
    [TestMethod()]
    public void CreateUserTest() {
      // create user
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider();
      MembershipCreateStatus status;
      target.CreateUser("testname", "newPassword", "testemail", "testquestion", "testanswer", true, null, out status);
      Assert.AreEqual(MembershipCreateStatus.Success, status);

      // check if user is OK
      HeuristicLabUser u = db.HeuristicLabUsers.Single<HeuristicLabUser>(x => x.UserName == "testname");
      Assert.AreEqual<string>("testname", u.UserName);
      Assert.AreEqual<string>("testemail", u.Email);
      Assert.AreEqual<string>("newPassword", u.Password);
      Assert.AreEqual<string>("testquestion", u.PasswordQuestion);
      Assert.AreEqual<string>("testanswer", u.PasswordAnswer);
      Assert.AreEqual<string>("", u.Comment);

      // check for duplicate errors
      target.CreateUser("testname", "newPassword", "testemail", "testquestion", "testanswer", true, null, out status);
      Assert.AreEqual(MembershipCreateStatus.DuplicateUserName, status);
      target.CreateUser("testname2", "newPassword", "testemail", "testquestion", "testanswer", true, null, out status);
      Assert.AreEqual(MembershipCreateStatus.DuplicateEmail, status);
    }

    /// <summary>
    ///A test for ChangePasswordQuestionAndAnswer
    ///</summary>
    [TestMethod()]
    public void ChangePasswordQuestionAndAnswerTest() {
      // create user
      HeuristicLabUser u = new HeuristicLabUser("testname", "testemail", "testquestion", "testcomment");
      db.HeuristicLabUsers.InsertOnSubmit(u);
      db.SubmitChanges();

      // check if user is stored
      u = db.HeuristicLabUsers.Single<HeuristicLabUser>(x => x.UserName == "testname");
      Assert.AreEqual<String>("testquestion", u.PasswordQuestion);
      Assert.AreEqual<String>("", u.PasswordAnswer);

      // change data and check again
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider();
      // gibt zwar true zurück, was schon mal gut ist
      Assert.IsTrue(target.ChangePasswordQuestionAndAnswer("testname", "INIT", "newquestion", "newanswer"));
      // aber hier ist die änderung noch nicht da!! es ist immer noch die alte frage + alte antwort
      u = db.HeuristicLabUsers.Single<HeuristicLabUser>(x => x.UserName == "testname");
      Assert.AreEqual<String>("newquestion", u.PasswordQuestion);
      Assert.AreEqual<String>("newanswer", u.PasswordAnswer);
    }

    /// <summary>
    ///A test for ChangePassword
    ///</summary>
    [TestMethod()]
    public void ChangePasswordTest() {
      // create user
      HeuristicLabUser u = new HeuristicLabUser("testname", "testemail", "testquestion", "testcomment");
      db.HeuristicLabUsers.InsertOnSubmit(u);
      db.SubmitChanges();

      // check if user is stored
      u = db.HeuristicLabUsers.Single<HeuristicLabUser>(x => x.UserName == "testname");
      Assert.AreEqual<String>("INIT", u.Password);

      // change data and check again
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider();
      Assert.IsTrue(target.ChangePassword("testname", "INIT", "newPassword"));
      // hat nix gemacht!! :(
      u = db.HeuristicLabUsers.Single<HeuristicLabUser>(x => x.UserName == "testname");
      Assert.AreEqual<String>("newPassword", u.Password);
    }

    /// <summary>
    ///A test for HeuristicLabMembershipProvider Constructor
    ///</summary>
    [TestMethod()]
    public void HeuristicLabMembershipProviderConstructorTest() {
      HeuristicLabMembershipProvider target = new HeuristicLabMembershipProvider();
      Assert.Inconclusive("TODO: Implement code to verify target");
    }

    [TestMethod()]
    public void NoDuplicateUserNameTest() {
      try {
        Persistence.HeuristicLabUser user = new Persistence.HeuristicLabUser("duplicateName", "mail", "myQuestion", "myComment");
        db.HeuristicLabUsers.InsertOnSubmit(user);
        Persistence.HeuristicLabUser user2 = new Persistence.HeuristicLabUser("duplicateName", "mail2", "myQuestion2", "myComment2");
        db.HeuristicLabUsers.InsertOnSubmit(user2);
        db.SubmitChanges();
        Assert.Fail();
      }
      catch (SqlException) {
        //swallowing Exception because it is expected that a SQL Exception is thrown        
      }
    }
    [TestMethod()]
    public void NoDuplicateEmailTest() {
      try {
        Persistence.HeuristicLabUser user = new Persistence.HeuristicLabUser("duplicateName", "mail", "myQuestion", "myComment");
        db.HeuristicLabUsers.InsertOnSubmit(user);
        Persistence.HeuristicLabUser user2 = new Persistence.HeuristicLabUser("duplicateName2", "mail", "myQuestion2", "myComment2");
        db.HeuristicLabUsers.InsertOnSubmit(user2);
        db.SubmitChanges();
        Assert.Fail();
      }
      catch (SqlException) {
        //swallowing Exception because it is expected that a SQL Exception is thrown        
      }
    }
  }
}
