using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Server.Core {
  /// <summary>
  /// PermissiveSecurityAction contains the GUIDs for 
  /// </summary>
  public static class PermissiveSecurityAction {
    public static Guid Add_Job {
      get { return new Guid("A477BEA9-9C05-477f-AD9E-F76C510DDB0B"); }
    }

    public static Guid Get_AllJobs {
      get { return new Guid("7FC39A7C-1807-4d76-B847-F0DE9DF0275E"); }
    }

    public static Guid Get_LastJobResult {
      get { return new Guid("79E8705E-23F5-4157-A3E5-BBE6CE2C6C01"); }
    }

    public static Guid Get_AllJobResults {
      get { return new Guid("F72B9338-22A4-4870-B6A3-14BA38DF3BEF"); }
    }

    public static Guid Remove_Job {
      get { return new Guid("FFDB7636-4A31-44b7-B843-289B2E8808BD"); }
    }

    public static Guid Abort_Job {
      get { return new Guid("DFACD3D6-E85E-42b7-B4AE-0FB33C598F24"); }
    }

    public static Guid Request_Snapshot {
      get { return new Guid("40D41BAA-6495-42c6-82B6-9A7CDB1F30EF"); }
    }

    public static Guid List_AllClients {
      get { return new Guid("D8EAC687-BE9A-4266-B410-35674A374462"); }
    }

    public static Guid List_AllClientGroups {
      get { return new Guid("29FACE0C-A1CA-428b-A757-7FAABD74AFB8"); }
    }

    public static Guid Show_Statistics {
      get { return new Guid("FAEFF433-037E-48b7-9CC7-34C560894304"); }
    }

    public static Guid Add_ClientGroup {
      get { return new Guid("81984753-7401-4a4b-B819-BC48BC662B0F"); }
    }

    public static Guid Add_Resource {
      get { return new Guid("8C76C1F1-31B6-4c66-BCFF-E92EA00CABF5"); }
    }

    public static Guid Delete_Resource {
      get { return new Guid("EC2362E0-0781-493f-8688-76F3CDFEF339"); }
    }

    public static Guid Add_User {
      get { return new Guid("4FE34091-6674-4c24-BE2E-8AE0DAAAD29C"); }
    }

    public static Guid Delete_User {
      get { return new Guid("835CF39C-F830-4130-BBAF-E0100B2760D2"); }
    }

    public static Guid Update_User {
      get { return new Guid("FEB4D84B-25CB-4708-8B70-3159F40D8F6E"); }
    }

    public static Guid Get_User {
      get { return new Guid("19EF2197-F71A-44ae-9916-3F5AEEC4E463"); }
    }

    public static Guid Grant_Permission {
      get { return new Guid("B7696208-1EA3-4675-8834-5659CD6BB5DC"); }
    }

    public static Guid Revoke_Permission {
      get { return new Guid("357DE1A5-198E-4b90-BC55-C007D504AF65"); }
    }

    public static Guid Get_AllGroupsOfResource {
      get { return new Guid("07436F36-E882-4aed-B787-861E4AADC5E6"); }
    }
  }
}