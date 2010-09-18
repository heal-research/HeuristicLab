using System;
using System.Web.Security;

namespace Persistence {
  /// <summary>
  /// implements the MembershipUser for HeuristicLab
  /// </summary>
  partial class HeuristicLabUser : MembershipUser {
    public HeuristicLabUser(string name, string email, string passwordQuestion, string comment) : this() {
      _UserName = name;
      Password = "INIT"; // just for tests
      _LastPasswordChangedDate = System.DateTime.Today;
      _PasswordQuestion = passwordQuestion;
      PasswordAnswer = "";
      Email = email;
      Comment = comment;
    }

    public override bool ChangePassword(string oldPassword, string newPassword) {
      if (oldPassword == null) {
        throw new ArgumentNullException("oldPassword");
      }
      if (newPassword == null) {
        throw new ArgumentNullException("newPassword");
      }
      if (oldPassword.Length == 0) {
        throw new ArgumentException("Parameter oldPassword must not be empty!");
      }
      if (newPassword.Length == 0) {
        throw new ArgumentException("Parameter newPassword must not be empty!");
      }

      if (Password.CompareTo(oldPassword) == 0) {
        Password = newPassword;
        return true;
      } else {
        return false;
      }
    }

    public override bool ChangePasswordQuestionAndAnswer(string password, string newPasswordQuestion, string newPasswordAnswer) {
      if (password == null) {
        throw new ArgumentNullException("password");
      }
      if (newPasswordQuestion == null) {
        throw new ArgumentNullException("newPasswordQuestion");
      }
      if (newPasswordAnswer == null) {
        throw new ArgumentNullException("newPasswordAnswer");
      }
      if (password.Length == 0) {
        throw new ArgumentException("Parameter password must not be empty!");
      }
      if (newPasswordQuestion.Length == 0) {
        throw new ArgumentException("Parameter newPasswordQuestion must not be empty!");
      }
      if (newPasswordAnswer.Length == 0) {
        throw new ArgumentException("Parameter newPasswordAnswer must not be empty!");
      }

      if (Password.CompareTo(password) == 0) {
        _PasswordQuestion = newPasswordQuestion;
        PasswordAnswer = newPasswordAnswer;
        return true;
      } else {
        return false;
      }
    }

    public override string GetPassword() {
      return Password;
    }

    public override string GetPassword(string passwordAnswer) {
      if (PasswordAnswer == passwordAnswer) {
        return Password;
      } else {
        return "";
      }
    }

    public override string ResetPassword() {
      Password = "INIT";
      return Password;
    }

    public override string ResetPassword(string passwordAnswer) {
      if (PasswordAnswer == passwordAnswer) {
        Password = "INIT";
        return Password;
      } else {
        return "";
      }
    }

    public override string ToString() {
      return UserName;
    }

    public bool LockUser()
    {
        Locked = true;
        return Locked;
    }

    //
    // Summary:
    //     Clears the locked-out state of the user so that the membership user can be
    //     validated.
    //
    // Returns:
    //     true if the membership user was successfully unlocked; otherwise, false.
    public override bool UnlockUser()
    {
        Locked = false;
        return !Locked;
    }
  }
}
