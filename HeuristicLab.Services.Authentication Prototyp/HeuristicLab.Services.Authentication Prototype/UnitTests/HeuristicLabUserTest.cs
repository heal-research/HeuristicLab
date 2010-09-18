using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Persistence;

namespace UnitTests {
  /// <summary>
  ///This is a test class for HeuristicLabUserTest and is intended
  ///to contain all HeuristicLabUserTest Unit Tests
  ///</summary>
  [TestClass()]
  public class HeuristicLabUserTest : AbstractHeuristicLabTest {
    private TestContext testContextInstance;

    /// <summary>
    /// inserts, modifies and deletes a new user into the database
    /// </summary>
    [TestMethod()]
    public void modifyUserTest() {
      // insert new user
      HeuristicLabUser user = new HeuristicLabUser("testname", "testemail", "testquestion", "testcomment");
      db.HeuristicLabUsers.InsertOnSubmit(user);
      db.SubmitChanges();
      HeuristicLabUser u = db.HeuristicLabUsers.Single(x => x.UserName == "testname");
      Assert.IsNotNull(u);
      Assert.AreEqual<string>("testname", u.UserName);
      Assert.AreEqual<string>("testemail", u.Email);
      Assert.AreEqual<string>("testquestion", u.PasswordQuestion);
      Assert.AreEqual<string>("testcomment", u.Comment);

      // modify existing user
      u.Email = "testemail2";
      u.Comment = "testcomment2";
      db.SubmitChanges();
      u = db.HeuristicLabUsers.Single(x => x.UserName == "testname");
      Assert.IsNotNull(u);
      Assert.AreEqual<string>("testname", u.UserName);
      Assert.AreEqual<string>("testemail2", u.Email);
      Assert.AreEqual<string>("testquestion", u.PasswordQuestion);
      Assert.AreEqual<string>("testcomment2", u.Comment);

      // delete user
      db.HeuristicLabUsers.DeleteOnSubmit(u);
      db.SubmitChanges();
      List<HeuristicLabUser> uList = db.HeuristicLabUsers.Where(x => x.UserName == "testname").ToList<HeuristicLabUser>();
      Assert.AreEqual(0, uList.Count);
    }

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
    ///A test for UserName
    ///</summary>
    [TestMethod()]
    public void UserNameTest() {
      HeuristicLabUser target = new HeuristicLabUser("testname", "testemail", "testquestion", "testcomment");
      Assert.AreEqual<String>("testname", target.UserName);
    }

    /// <summary>
    ///A test for PasswordQuestion
    ///</summary>
    [TestMethod()]
    public void PasswordQuestionTest() {
      HeuristicLabUser target = new HeuristicLabUser("testname", "testemail", "testquestion", "testcomment");
      Assert.AreEqual<String>("testquestion", target.PasswordQuestion);
      target.ChangePasswordQuestionAndAnswer("INIT", "newquestion", "newanswer");
      Assert.AreEqual<String>("newquestion", target.PasswordQuestion);
    }

    /// <summary>
    ///A test for PasswordAnswer
    ///</summary>
    [TestMethod()]
    public void PasswordAnswerTest() {
      HeuristicLabUser target = new HeuristicLabUser("testname", "testemail", "testquestion", "testcomment");
      Assert.AreEqual<String>("", target.PasswordAnswer);
      target.ChangePasswordQuestionAndAnswer("INIT", "newquestion", "newanswer");
      Assert.AreEqual<String>("newquestion", target.PasswordQuestion);
      target.PasswordAnswer = "testanswer";
      Assert.AreEqual<String>("testanswer", target.PasswordAnswer);
    }

    /// <summary>
    ///A test for Password
    ///</summary>
    [TestMethod()]
    public void PasswordTest() {
      HeuristicLabUser target = new HeuristicLabUser("testname", "testemail", "testquestion", "testcomment");
      Assert.AreEqual<String>("INIT", target.Password);
      target.ChangePassword(target.Password, "pwd1");
      Assert.AreEqual<String>("pwd1", target.Password);
      target.ChangePassword("pwd1", "pwd2");
      Assert.AreEqual<String>("pwd2", target.Password);
    }

    /// <summary>
    ///A test for LastPasswordChangedDate
    ///</summary>
    [TestMethod()]
    public void LastPasswordChangedDateTest() {
      HeuristicLabUser target = new HeuristicLabUser("testname", "testemail", "testquestion", "testcomment");
      Assert.AreEqual<DateTime>(System.DateTime.Today, target.LastPasswordChangedDate);
      target.ChangePassword(target.Password, "pwd1");
      Assert.AreEqual<DateTime>(System.DateTime.Today, target.LastPasswordChangedDate);
    }

    /// <summary>
    ///A test for Email
    ///</summary>
    [TestMethod()]
    public void EmailTest() {
      HeuristicLabUser target = new HeuristicLabUser("testname", "testemail", "testquestion", "testcomment");
      Assert.AreEqual<String>("testemail", target.Email);
      target.Email = "testemail2";
      Assert.AreEqual<String>("testemail2", target.Email);
    }

    /// <summary>
    ///A test for Comment
    ///</summary>
    [TestMethod()]
    public void CommentTest() {
      HeuristicLabUser target = new HeuristicLabUser("testname", "testemail", "testquestion", "testcomment");
      target.Comment = "testcomment2";
      Assert.AreEqual<String>("testcomment2", target.Comment);
    }

    /// <summary>
    ///A test for UnlockUser
    ///</summary>
    [TestMethod()]
    public void UnlockUserTest() {
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for ToString
    ///</summary>
    [TestMethod()]
    public void ToStringTest() {
      HeuristicLabUser target = new HeuristicLabUser("testname", "testemail", "testquestion", "testcomment");
      Assert.AreEqual<String>("testname", target.ToString());
    }

    /// <summary>
    ///A test for SendPropertyChanging
    ///</summary>
    [TestMethod()]
    [DeploymentItem("Persistence.dll")]
    public void SendPropertyChangingTest() {
      HeuristicLabUser_Accessor target = new HeuristicLabUser_Accessor(); // TODO: Initialize to an appropriate value
      target.SendPropertyChanging();
      Assert.Inconclusive("A method that does not return a value cannot be verified.");
    }

    /// <summary>
    ///A test for SendPropertyChanged
    ///</summary>
    [TestMethod()]
    [DeploymentItem("Persistence.dll")]
    public void SendPropertyChangedTest() {
      HeuristicLabUser_Accessor target = new HeuristicLabUser_Accessor(); // TODO: Initialize to an appropriate value
      string propertyName = string.Empty; // TODO: Initialize to an appropriate value
      target.SendPropertyChanged(propertyName);
      Assert.Inconclusive("A method that does not return a value cannot be verified.");
    }

    /// <summary>
    ///A test for ResetPassword
    ///</summary>
    [TestMethod()]
    public void ResetPasswordTest1() {
      HeuristicLabUser target = new HeuristicLabUser("testname", "testemail", "testquestion", "testcomment");
      target.ChangePassword(target.Password, "pwd1");
      Assert.AreEqual<String>("pwd1", target.GetPassword());
      target.ResetPassword();
      Assert.AreEqual<String>("INIT", target.GetPassword());
    }

    /// <summary>
    ///A test for ResetPassword
    ///</summary>
    [TestMethod()]
    public void ResetPasswordTest() {
      HeuristicLabUser target = new HeuristicLabUser("testname", "testemail", "testquestion", "testcomment");
      target.ChangePassword(target.Password, "pwd1");
      Assert.AreEqual<String>("pwd1", target.GetPassword());
      Assert.AreEqual<String>("", target.ResetPassword("wrongAnswer"));
      Assert.AreEqual<String>("pwd1", target.GetPassword());
      Assert.AreEqual<String>("INIT", target.ResetPassword(""));
      Assert.AreEqual<String>("INIT", target.GetPassword());
    }

    /// <summary>
    ///A test for GetPassword
    ///</summary>
    [TestMethod()]
    public void GetPasswordTest1() {
      HeuristicLabUser target = new HeuristicLabUser("testname", "testemail", "testquestion", "testcomment");
      target.PasswordAnswer = "answer";
      Assert.AreEqual<String>("INIT", target.GetPassword("answer"));
      target.ChangePassword(target.Password, "pwd1");
      Assert.AreEqual<String>("pwd1", target.GetPassword("answer"));
      target.ChangePassword("pwd1", "pwd2");
      Assert.AreEqual<String>("pwd2", target.GetPassword("answer"));
      Assert.AreEqual<String>("", target.GetPassword("wrong"));
    }

    /// <summary>
    ///A test for GetPassword
    ///</summary>
    [TestMethod()]
    public void GetPasswordTest() {
      HeuristicLabUser target = new HeuristicLabUser("testname", "testemail", "testquestion", "testcomment");
      Assert.AreEqual<String>("INIT", target.GetPassword());
      target.ChangePassword(target.Password, "pwd1");
      Assert.AreEqual<String>("pwd1", target.GetPassword());
      target.ChangePassword("pwd1", "pwd2");
      Assert.AreEqual<String>("pwd2", target.GetPassword());
    }

    /// <summary>
    ///A test for ChangePasswordQuestionAndAnswer
    ///</summary>
    [TestMethod()]
    public void ChangePasswordQuestionAndAnswerTest() {
      HeuristicLabUser target = new HeuristicLabUser("testname", "testemail", "testquestion", "testcomment");
      Assert.AreEqual<String>("testquestion", target.PasswordQuestion);
      Assert.AreEqual<String>("", target.PasswordAnswer);

      // check all exceptions
      try {
        target.ChangePasswordQuestionAndAnswer(null, null, null);
        Assert.Fail();
      }
      catch {
      }
      try {
        target.ChangePasswordQuestionAndAnswer("", null, null);
        Assert.Fail();
      }
      catch {
      }
      try {
        target.ChangePasswordQuestionAndAnswer("", "", null);
        Assert.Fail();
      }
      catch {
      }
      try {
        target.ChangePasswordQuestionAndAnswer("", "x", "x");
        Assert.Fail();
      }
      catch {
      }
      try {
        target.ChangePasswordQuestionAndAnswer("x", "", "x");
        Assert.Fail();
      }
      catch {
      }
      try {
        target.ChangePasswordQuestionAndAnswer("x", "x", "");
        Assert.Fail();
      }
      catch {
      }

      // wrong old password
      Assert.IsFalse(target.ChangePasswordQuestionAndAnswer("wrong", "newquestion", "newanswer"));
      Assert.AreEqual<String>("testquestion", target.PasswordQuestion);
      Assert.AreEqual<String>("", target.PasswordAnswer);

      // should work
      Assert.IsTrue(target.ChangePasswordQuestionAndAnswer("INIT", "newquestion", "newanswer"));
      Assert.AreEqual<String>("newquestion", target.PasswordQuestion);
      Assert.AreEqual<String>("newanswer", target.PasswordAnswer);
    }

    /// <summary>
    ///A test for ChangePassword
    ///</summary>
    [TestMethod()]
    public void ChangePasswordTest() {
      HeuristicLabUser target = new HeuristicLabUser("testname", "testemail", "testquestion", "testcomment");
      Assert.AreEqual<String>("INIT", target.Password);

      // check all exceptions
      try {
        target.ChangePassword(null, null);
        Assert.Fail();
      }
      catch {
      }
      try {
        target.ChangePassword("", "abc");
        Assert.Fail();
      }
      catch {
      }
      try {
        target.ChangePassword("INIT", null);
        Assert.Fail();
      }
      catch {
      }
      try {
        target.ChangePassword("INIT", "");
        Assert.Fail();
      }
      catch {
      }

      target.ChangePassword(target.Password, "pwd1");
      Assert.AreEqual<String>("pwd1", target.Password);
      target.ChangePassword("pwd1", "pwd2");
      Assert.AreEqual<String>("pwd2", target.Password);
      try {
        target.ChangePassword("abc", "def");
        Assert.Fail();
      }
      catch (Exception) {
      }
    }

    /// <summary>
    ///A test for HeuristicLabUser Constructor
    ///</summary>
    [TestMethod()]
    public void HeuristicLabUserConstructorTest() {
      HeuristicLabUser target = new HeuristicLabUser("testname", "testemail", "testquestion", "testcomment");
      Assert.AreEqual<String>("testname", target.UserName);
      Assert.AreEqual<String>("INIT", target.Password);
      Assert.AreEqual<DateTime>(System.DateTime.Today, target.LastPasswordChangedDate);
      Assert.AreEqual<String>("testquestion", target.PasswordQuestion);
      Assert.AreEqual<String>("", target.PasswordAnswer);
      Assert.AreEqual<String>("testemail", target.Email);
      Assert.AreEqual<String>("testcomment", target.Comment);
    }
  }
}
