using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Contracts {
  public class ApplicationConstants {

    public static string RESPONSE_SERVERCONSOLE_LOGIN_SUCCESS = "Logged in";
    public static string RESPONSE_SERVERCONSOLE_LOGIN_FAILED = "Login failed";

    public static string RESPONSE_USERROLE_GET_ALL_USERS = "UserRole.GetAllUsers";
    public static string RESPONSE_USERROLE_USER_EXISTS_ALLREADY = "UserRole.UserExistsAllready";
    public static string RESPONSE_USERROLE_USER_SUCCESSFULLY_ADDED = "UserRole.UserSuccessfullyAdded";
    public static string RESPONSE_USERROLE_USER_DOESNT_EXIST = "UserRole.UserDoesntExist";
    public static string RESPONSE_USERROLE_USER_REMOVED = "UserRole.UserRemoved";
    public static string RESPONSE_USERROLE_USERGROUP_EXISTS_ALLREADY = "UserRole.UsergroupExistsAllready";
    public static string RESPONSE_USERROLE_USERGROUP_ADDED = "UserRole.UserGroupAdded";
    public static string RESPONSE_USERROLE_USERGROUP_DOESNT_EXIST = "UserRole.UserGroupDoesntExist";
    public static string RESPONSE_USERROLE_PERMISSIONOWNER_DOESNT_EXIST = "UserRole.PermissionOwnerDoesntExist";
    public static string RESPONSE_USERROLE_PERMISSIONOWNER_REMOVED = "UserRole.PermissionOwnerRemoved";
    public static string RESPONSE_USERROLE_PERMISSIONOWNER_ADDED = "UserRole.PermissionOwnerAdded";
    public static string RESPONSE_USERROLE_GET_ALL_USERGROUPS = "UserRole.AllUserGroupsReturned";
    public static string RESPONSE_USERROLE_ID_MUST_NOT_BE_SET = "UserRole.IdMustNotBeSet";
    public static string RESPONSE_USERROLE_USERGROUP_ADDED_TO_USERGROUP = "UserRole.UserGroupAddedToUserGroup";
    public static string RESPONSE_USERROLE_USERNAME_EXISTS_ALLREADY = "UserRole.UsernameExistsAllready";

    public static string RESPONSE_CLIENT_GET_ALL_CLIENTS = "Client.GetAllClients";
    public static string RESPONSE_CLIENT_GET_ALL_CLIENTGROUPS = "Client.GetAllClientGroups";
    public static string RESPONSE_CLIENT_ID_MUST_NOT_BE_SET = "Client.IdMustNotBeSet";
    public static string RESPONSE_CLIENT_CLIENTGROUP_ADDED = "Client.ClientGroupAdded";
    public static string RESPONSE_CLIENT_CLIENTGROUP_DOESNT_EXIST = "Client.ClientGroupDoesntExist";
    public static string RESPONSE_CLIENT_RESOURCE_ADDED_TO_GROUP = "Client.ResourceAddedToGroup";
    public static string RESPONSE_CLIENT_RESOURCE_REMOVED = "Client.ResourceRemoved";
    public static string RESPONSE_CLIENT_RESOURCE_NOT_FOUND = "Client.ResourceNotFound";
    public static string RESPONSE_CLIENT_PERMISSIONOWNER_DOESNT_EXIST = "Client.PermissionOwnerDoesntExist";

    public static string RESPONSE_COMMUNICATOR_HARDBEAT_RECEIVED = "Communicator.HeardbeatReceived";
    public static string RESPONSE_COMMUNICATOR_JOB_PULLED = "Communicator.JobPulled";
    public static string RESPONSE_COMMUNICATOR_JOBRESULT_RECEIVED = "Communicator.JobResultReceived";
    public static string RESPONSE_COMMUNICATOR_LOGIN_USER_ALLREADY_ONLINE = "Communicator.LoginUserAllreadyOnline";
    public static string RESPONSE_COMMUNICATOR_LOGIN_SUCCESS = "Communicator.LoginSuccess";
    public static string RESPONSE_COMMUNICATOR_LOGOUT_CLIENT_NOT_REGISTERED = "Communicator.LogoutClientNotRegistered";
    public static string RESPONSE_COMMUNICATOR_LOGOUT_SUCCESS = "Communicator.LogoutSuccess";
    public static string RESPONSE_COMMUNICATOR_NO_JOBS_LEFT = "Communicator.NoJobsLeft";
    public static string RESPONSE_COMMUNICATOR_ID_MUST_NOT_BE_SET = "Communicator.IdMustNotBeSet";
    public static string RESPONSE_COMMUNICATOR_NO_JO_WITH_THIS_ID = "Communicator.NoJobWithThisId";
    public static string RESPONSE_COMMUNICATOR_WRONG_JOB_STATE = "Communicator.WrongJobState";

    public static string RESPONSE_JOB_ALL_JOBS = "Job.AllJobs";
    public static string RESPONSE_JOB_ID_MUST_NOT_BE_SET = "Job.IdMustNotBeSet";
    public static string RESPONSE_JOB_JOB_ADDED = "Job.JobAdded";
    public static string RESPONSE_JOB_JOB_NULL = "Job.JobNull";
    public static string RESPONSE_JOB_JOB_DOESNT_EXIST = "JobDoesntExist";
    public static string RESPONSE_JOB_JOB_REMOVED = "Job.JobRemoved";
    public static string RESPONSE_JOB_JOBSTATE_MUST_BE_OFFLINE = "Job.JobStateMustBeOffline";
  }
}
