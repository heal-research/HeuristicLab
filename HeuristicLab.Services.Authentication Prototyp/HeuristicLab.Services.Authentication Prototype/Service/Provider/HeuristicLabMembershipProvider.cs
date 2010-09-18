using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using System.Web.Security;
using Persistence;


namespace Service.Provider {
  class HeuristicLabMembershipProvider : MembershipProvider {

    #region variables

    private string pApplicationName;
    private bool pEnablePasswordReset;
    private bool pEnablePasswordRetrieval;
    private bool pRequiresQuestionAndAnswer;
    private bool pRequiresUniqueEmail;
    private int pMaxInvalidPasswordAttempts;
    private int pPasswordAttemptWindow;
    private int pMinRequiredPasswordLength;
    private MembershipPasswordFormat pPasswordFormat = MembershipPasswordFormat.Clear;

    #endregion

    #region properties

    public override string ApplicationName {
      get { return pApplicationName; }
      set { pApplicationName = value; }
    }

    public override bool EnablePasswordReset {
      get { return pEnablePasswordReset; }
    }

    public override bool EnablePasswordRetrieval {
      get { return pEnablePasswordRetrieval; }
    }

    public override int MaxInvalidPasswordAttempts {
      get { return pMaxInvalidPasswordAttempts; }
    }

    public override int MinRequiredNonAlphanumericCharacters {
      get { return 0; }
    }

    public override int MinRequiredPasswordLength {
      get { return pMinRequiredPasswordLength; }
    }

    public override int PasswordAttemptWindow {
      get { return pPasswordAttemptWindow; }
    }

    public override MembershipPasswordFormat PasswordFormat {
      get { return pPasswordFormat; }
    }

    public override string PasswordStrengthRegularExpression {
      get { return string.Empty; }
    }

    public override bool RequiresQuestionAndAnswer {
      get { return pRequiresQuestionAndAnswer; }
    }

    public override bool RequiresUniqueEmail {
      get { return pRequiresUniqueEmail; }
    }

    #endregion

    public override void Initialize(string name, NameValueCollection config) {
      //
      // Initialize values from web.config.
      //

      if (config == null)
        throw new ArgumentNullException("config");

      if (string.IsNullOrEmpty(name) || name.Length == 0)
        name = "HeuristicLabMembershipProvider";

      if (String.IsNullOrEmpty(config["description"])) {
        config.Remove("description");
        config.Add("description", "Heuristic Lab Membership provider");
      }

      // Initialize the abstract base class.
      base.Initialize(name, config);

      pApplicationName = GetConfigValue(config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
      pMaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
      pPasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
      pMinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
      pEnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
      pEnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
      pRequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
      pRequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));

      string tempFormat = config["passwordFormat"];
      if (tempFormat == null) {
        tempFormat = "Clear";
      }

      switch (tempFormat) {
        case "Hashed":
          pPasswordFormat = MembershipPasswordFormat.Hashed;
          break;
        case "Encrypted":
          pPasswordFormat = MembershipPasswordFormat.Encrypted;
          break;
        case "Clear":
          pPasswordFormat = MembershipPasswordFormat.Clear;
          break;
        default:
          throw new ProviderException("Password format not supported.");
      }
    }



    public override bool ChangePassword(string username, string oldPassword, string newPassword) {
      using (DataClassesDataContext db = DatabaseUtil.createDataClassesDataContext()) {
        // check database connection
        if (db == null) {
          return false;
        }
        try {
          // try to get user
          HeuristicLabUser u = db.HeuristicLabUsers.Single(x => x.UserName == username);
          if (u.ChangePassword(oldPassword, newPassword)) {
            // save user to database only if needed
            db.SubmitChanges();
            return true;
          } else {
            return false;
          }
        }
        catch (Exception) {
          return false;
        }
      }
    }

    public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer) {
      using (DataClassesDataContext db = DatabaseUtil.createDataClassesDataContext()) {
        // check database connection
        if (db == null) {
          return false;
        }
        try {
          // try to get user
          HeuristicLabUser u = db.HeuristicLabUsers.Single(x => x.UserName == username);
          if (u.ChangePasswordQuestionAndAnswer(password, newPasswordQuestion, newPasswordAnswer)) {
            // save user to database only if needed
            db.SubmitChanges();
            return true;
          } else {
            return false;
          }
        }
        catch (Exception) {
          return false;
        }
      }
    }

    public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status) {
      using (DataClassesDataContext db = DatabaseUtil.createDataClassesDataContext()) {
        // check database connection
        if (db == null) {
          status = MembershipCreateStatus.ProviderError;
          return null;
        }
        try {
          // check for duplicate entries
          if (db.HeuristicLabUsers.Count(x => x.UserName == username) > 0) {
            status = MembershipCreateStatus.DuplicateUserName;
            return null;
          }
          if (db.HeuristicLabUsers.Count(x => x.Email == email) > 0) {
            status = MembershipCreateStatus.DuplicateEmail;
            return null;
          }

          // create new user
          HeuristicLabUser u = new HeuristicLabUser(username, email, passwordQuestion, "");
          password = EncodePassword(password);
          u.ChangePassword("INIT", password);
          u.ChangePasswordQuestionAndAnswer(password, passwordQuestion, passwordAnswer);
          // save user into database
          db.HeuristicLabUsers.InsertOnSubmit(u);
          db.SubmitChanges();

          // success
          status = MembershipCreateStatus.Success;
          return u;
        }
        catch (Exception) {
          // error
          status = MembershipCreateStatus.ProviderError;
          return null;
        }
      }
    }

    public override bool DeleteUser(string username, bool deleteAllRelatedData) {
      using (DataClassesDataContext db = DatabaseUtil.createDataClassesDataContext()) {
        // check database connection
        if (db == null) {
          return false;
        }
        try {
          // try to get user
          HeuristicLabUser u =
            db.HeuristicLabUsers.Single<HeuristicLabUser>(x => x.UserName == username);

          // optionally delete related data
          if (deleteAllRelatedData) {
            db.HeuristicLabUserRole.DeleteAllOnSubmit<HeuristicLabUserRole>(u.HeuristicLabUserRoles);
          }

          // delete user
          db.HeuristicLabUsers.DeleteOnSubmit(u);
          db.SubmitChanges();
          return true;
        }
        catch (Exception) {
          return false;
        }
      }
    }



    public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords) {
      throw new NotImplementedException();
    }

    public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords) {
      throw new NotImplementedException();
    }

    // not for production use - fab and dkhan are currently working on that
    public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
      totalRecords = 0;
      using (DataClassesDataContext db = DatabaseUtil.createDataClassesDataContext()) {
        if (db == null) {
          totalRecords = 0;
          return new MembershipUserCollection();
        }

        // bail out if there are no records
        if (0 == (totalRecords = db.HeuristicLabUsers.Count<HeuristicLabUser>())) return new MembershipUserCollection();

        MembershipUserCollection userCollection = new MembershipUserCollection();
        int skip = (pageIndex == 0) ? 0 : (pageIndex * pageSize) - 1;

        var users = from u in db.HeuristicLabUsers select u;

        foreach (HeuristicLabUser u in users) {

          // this leads to a npe
          if (u != null) {
            userCollection.Add(u);
          }
        }
        return userCollection;
      }
    }

    public override int GetNumberOfUsersOnline() {
      throw new NotImplementedException();
    }

    public override string GetPassword(string username, string answer) {

      throw new NotImplementedException();
    }

    public override MembershipUser GetUser(string username, bool userIsOnline) {
      throw new NotImplementedException();
    }

    public override MembershipUser GetUser(object providerUserKey, bool userIsOnline) {
      throw new NotImplementedException();
    }

    public override string GetUserNameByEmail(string email) {
      throw new NotImplementedException();
    }



    public override string ResetPassword(string username, string answer) {
      throw new NotImplementedException();
    }

    public bool LockUser(string userName)
    {
        using (DataClassesDataContext db = DatabaseUtil.createDataClassesDataContext())
        {
            // check database connection
            if (db == null)
            {
                return false;
            }
            try
            {
                // try to get user
                HeuristicLabUser u =
                  db.HeuristicLabUsers.Single<HeuristicLabUser>(x => x.UserName == userName);

                // unlock user
                u.LockUser();
                db.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public override bool UnlockUser(string userName)
    {
        using (DataClassesDataContext db = DatabaseUtil.createDataClassesDataContext())
        {
            // check database connection
            if (db == null)
            {
                return false;
            }
            try
            {
                // try to get user
                HeuristicLabUser u =
                  db.HeuristicLabUsers.Single<HeuristicLabUser>(x => x.UserName == userName);

                // unlock user
                u.UnlockUser();
                db.SubmitChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public override void UpdateUser(MembershipUser user) {
      throw new NotImplementedException();
    }
    /// <summary>
    /// Validates a user
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public override bool ValidateUser(string username, string password) {
      bool isValid = false;
      using (DataClassesDataContext db = DatabaseUtil.createDataClassesDataContext()) {
        if (db == null) {
          return false;
        }
        HeuristicLabUser u = db.HeuristicLabUsers.Single(x => x.UserName == username);
        isValid = CheckPassword(password, u.GetPassword());
      }
      return isValid;
    }

    /// <summary>
    /// compaiers to passwords
    /// </summary>
    /// <param name="password"></param>
    /// <param name="dbpassword"></param>
    /// <returns></returns>
    private bool CheckPassword(string password, string dbpassword) {
      string pass1 = password;
      string pass2 = dbpassword;

      switch (PasswordFormat) {
        case MembershipPasswordFormat.Encrypted:
          pass2 = DecodePassword(dbpassword);
          break;
        case MembershipPasswordFormat.Hashed:
          pass1 = EncodePassword(password);
          break;
        default:
          break;
      }

      if (pass1 == pass2) {
        return true;
      }

      return false;
    }


    /// <summary>
    /// Encodes a password
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    private string EncodePassword(string password) {
      string encodedPassword = password;

      switch (PasswordFormat) {
        case MembershipPasswordFormat.Clear:
          break;
        case MembershipPasswordFormat.Encrypted:
          encodedPassword =
            Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
          break;
        case MembershipPasswordFormat.Hashed:
          SHA512 sha512 = SHA512.Create();
          ASCIIEncoding encoder = new ASCIIEncoding();
          byte[] combined = encoder.GetBytes(password);
          sha512.ComputeHash(combined);
          encodedPassword = Convert.ToBase64String(sha512.Hash);
          break;
        default:
          throw new ProviderException("Unsupported password format.");
      }

      return encodedPassword;
    }

    private readonly byte[] _rgbKey = new byte[]
                           {
                             182, 140, 37, 101, 52, 157, 80, 17, 65, 35, 130, 208, 101, 68, 161, 45, 197, 102, 112, 190,
                             187, 177, 37, 76, 63, 38, 190, 117, 247, 122, 94, 17
                           };
    private readonly byte[] _rgbIv = new byte[] { 60, 121, 178, 142, 50, 160, 226, 84, 41, 66, 158, 180, 26, 232, 42, 113 };

    protected override byte[] EncryptPassword(byte[] password) {
      SymmetricAlgorithm sa = Aes.Create();
      MemoryStream msEncrypt = new MemoryStream();
      CryptoStream csEncrypt = new CryptoStream(msEncrypt, sa.CreateEncryptor(_rgbKey, _rgbIv), CryptoStreamMode.Write);
      csEncrypt.Write(password, 0, password.Length);
      csEncrypt.Close();
      byte[] encryptedTextBytes = msEncrypt.ToArray();
      msEncrypt.Close();
      return encryptedTextBytes;
    }

    protected override byte[] DecryptPassword(byte[] encodedPassword) {
      SymmetricAlgorithm sa = Aes.Create();
      MemoryStream msDecrypt = new MemoryStream(encodedPassword);
      CryptoStream csDecrypt = new CryptoStream(msDecrypt, sa.CreateDecryptor(_rgbKey, _rgbIv), CryptoStreamMode.Read);
      byte[] decryptedTextBytes = new Byte[encodedPassword.Length];
      csDecrypt.Read(decryptedTextBytes, 0, encodedPassword.Length);
      csDecrypt.Close();
      msDecrypt.Close();
      return decryptedTextBytes;
    }

    /// <summary>
    /// Decodes a encoded Password
    /// </summary>
    /// <param name="encodedPassword"></param>
    /// <returns></returns>
    private string DecodePassword(string encodedPassword) {
      string password = encodedPassword;

      switch (PasswordFormat) {
        case MembershipPasswordFormat.Clear:
          break;
        case MembershipPasswordFormat.Encrypted:
          password =
            Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password))).TrimEnd('\0');


          break;
        case MembershipPasswordFormat.Hashed:
          throw new ProviderException("Cannot unencode a hashed password.");
        default:
          throw new ProviderException("Unsupported password format.");
      }

      return password;
    }

    /// <summary>
    /// returns the configuration string, if the value is null or empty the default value is returned 
    /// </summary>
    /// <param name="configValue"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    private string GetConfigValue(string configValue, string defaultValue) {
      if (String.IsNullOrEmpty(configValue))
        return defaultValue;

      return configValue;
    }
  }
}
