#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Contracts {
  public class ApplicationConstants {

    public static int HEARTBEAT_MAX_DIF = 120; // value in seconds
    public static int JOB_TIME_TO_LIVE = 5;

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
    public static string RESPONSE_CLIENT_GET_GROUPS_OF_CLIENT = "Get all groups of a client";

    public static string RESPONSE_COMMUNICATOR_HEARTBEAT_RECEIVED = "Communicator.HeardbeatReceived";
    public static string RESPONSE_COMMUNICATOR_JOB_PULLED = "Communicator.JobPulled";
    public static string RESPONSE_COMMUNICATOR_JOBRESULT_RECEIVED = "Communicator.JobResultReceived";
    public static string RESPONSE_COMMUNICATOR_LOGIN_USER_ALLREADY_ONLINE = "Communicator.LoginUserAllreadyOnline";
    public static string RESPONSE_COMMUNICATOR_LOGIN_SUCCESS = "Communicator.LoginSuccess";
    public static string RESPONSE_COMMUNICATOR_LOGOUT_CLIENT_NOT_REGISTERED = "Communicator.LogoutClientNotRegistered";
    public static string RESPONSE_COMMUNICATOR_LOGOUT_SUCCESS = "Communicator.LogoutSuccess";
    public static string RESPONSE_COMMUNICATOR_NO_JOBS_LEFT = "Communicator.NoJobsLeft";
    public static string RESPONSE_COMMUNICATOR_ID_MUST_NOT_BE_SET = "Communicator.IdMustNotBeSet";
    public static string RESPONSE_COMMUNICATOR_NO_JOB_WITH_THIS_ID = "Communicator.NoJobWithThisId";
    public static string RESPONSE_COMMUNICATOR_WRONG_JOB_STATE = "Communicator.WrongJobState";
    public static string RESPONSE_COMMUNICATOR_USER_NOT_LOGGED_IN = "Communicator.UserNotLoggedIn";
    public static string RESPONSE_COMMUNICATOR_JOB_IS_NOT_BEEING_CALCULATED = "Communicator.JobIsNotBeenigCalculated";
    public static string RESPONSE_COMMUNICATOR_WRONG_CLIENT_FOR_JOB = "Communicator.WrongClientForJob";
    public static string RESPONSE_COMMUNICATOR_JOB_ALLREADY_FINISHED = "Job allready finished. Not needed anymore";
    public static string RESPONSE_COMMUNICATOR_JOB_DOESNT_EXIST = "No job exists with this id";
    public static string RESPONSE_COMMUNICATOR_SEND_JOBRESULT = "Please send the Jobresult to the server";
    public static string RESPONSE_COMMUNICATOR_PLUGINS_SENT = "Communicator.PluginsSent";

    public static string RESPONSE_JOB_ALL_JOBS = "Job.AllJobs";
    public static string RESPONSE_JOB_ID_MUST_NOT_BE_SET = "Job.IdMustNotBeSet";
    public static string RESPONSE_JOB_JOB_ADDED = "Job.JobAdded";
    public static string RESPONSE_JOB_JOB_NULL = "Job.JobNull";
    public static string RESPONSE_JOB_JOB_DOESNT_EXIST = "JobDoesntExist";
    public static string RESPONSE_JOB_JOB_REMOVED = "Job.JobRemoved";
    public static string RESPONSE_JOB_JOBSTATE_MUST_BE_OFFLINE = "Job.JobStateMustBeOffline";
    public static string RESPONSE_JOB_IS_NOT_BEEING_CALCULATED = "Job.IsNotBeeingCalculated";
    public static string RESPONSE_JOB_JOB_RESULT_SENT = "Job.JobResultSent";
    public static string RESPONSE_JOB_REQUEST_ALLREADY_SET = "Job.RequestAllreadySet";
    public static string RESPONSE_JOB_ABORT_REQUEST_ALLREADY_SET = "Job.AbortRequestAllreadySet";
    public static string RESPONSE_JOB_REQUEST_SET = "Job.RequestSet";
    public static string RESPONSE_JOB_ABORT_REQUEST_SET = "Job.AbortRequestSet";
  }
}
