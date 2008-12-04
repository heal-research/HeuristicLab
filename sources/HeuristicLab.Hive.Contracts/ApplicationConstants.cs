using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Contracts {
  public class ApplicationConstants {
    public static string RESPONSE_LOGIN_USER_ALLREADY_ONLINE = "Login.UserAllreadyOnline";
    public static string RESPONSE_LOGIN_SUCCESS = "Login.Success";

    public static string RESPONSE_LOGOUT_CLIENT_NOT_REGISTERED = "Logout.ClientNotRegistered";
    public static string RESPONSE_LOGOUT_SUCCESS = "Logout.Success";

    public static string RESPONSE_USERROLE_GET_ALL_USERS = "UserRole.GetAllUsers";
    public static string RESPONSE_USERROLE_USER_EXISTS_ALLREADY = "UserRole.UserExistsAllready";
    public static string RESPONSE_USERROLE_USER_SUCCESSFULLY_ADDED = "UserRole.UserSuccessfullyAdded";
    public static string RESPONSE_USERROLE_USER_DOESNT_EXIST = "UserRole.UserDoesntExist";
    public static string RESPONSE_USERROLE_USER_REMOVED = "UserRole.UserRemoved";

  }
}
