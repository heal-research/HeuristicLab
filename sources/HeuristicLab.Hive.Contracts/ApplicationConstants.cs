using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Contracts {
  public class ApplicationConstants {

    public static string RESPONSE_USERROLE_GET_ALL_USERS = "UserRole.GetAllUsers";
    public static string RESPONSE_USERROLE_USER_EXISTS_ALLREADY = "UserRole.UserExistsAllready";
    public static string RESPONSE_USERROLE_USER_SUCCESSFULLY_ADDED = "UserRole.UserSuccessfullyAdded";
    public static string RESPONSE_USERROLE_USER_DOESNT_EXIST = "UserRole.UserDoesntExist";
    public static string RESPONSE_USERROLE_USER_REMOVED = "UserRole.UserRemoved";

    public static string RESPONSE_CLIENT_GET_ALL_CLIENTS = "Client.GetAllClients";

    public static string RESPONSE_COMMUNICATOR_HARDBEAT_RECEIVED = "Communicator.HeardbeatReceived";
    public static string RESPONSE_COMMUNICATOR_JOB_PULLED = "Communicator.JobPulled";
    public static string RESPONSE_COMMUNICATOR_JOBRESULT_RECEIVED = "Communicator.JobResultReceived";
    public static string RESPONSE_COMMUNICATOR_LOGIN_USER_ALLREADY_ONLINE = "Communicator.LoginUserAllreadyOnline";
    public static string RESPONSE_COMMUNICATOR_LOGIN_SUCCESS = "Communicator.LoginSuccess";
    public static string RESPONSE_COMMUNICATOR_LOGOUT_CLIENT_NOT_REGISTERED = "Communicator.LogoutClientNotRegistered";
    public static string RESPONSE_COMMUNICATOR_LOGOUT_SUCCESS = "Logout.Success";

  }
}
