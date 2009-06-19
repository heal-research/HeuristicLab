using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using HeuristicLab.Security.Contracts.BusinessObjects;
using System.Diagnostics;

namespace HeuristicLab.Hive.Server.Core  {
  
  public static class HivePermissions {
    private const string PERMISSIONFILE = "Authorization//HivePermissionSet.xml";
    private const string POLICIESFILE = "Authorization//HivePermissionPolicy.xml";

    public static class Jobmanagement {
      [Flags]
      public enum Assign {
        ToAnyResource = 0x02,
        ToProject = 0x04
      }
      [Flags]
      public enum Abort {
        /// <summary>Can abort any job.</summary>
        Any = 0x02,
        /// <summary>Can abort jobs from specific project only.</summary>
        ProjectOnly = 0x04,
        /// <summary>Can abort only owned jobs.</summary>
        OwnedOnly = 0x08
      }
      [Flags]
      public enum Create {
        /// <summary>Can create a job everywhere.</summary>
        Any = 0x02,
        /// <summary>Can create a job in project context only.</summary>
        ProjectOnly = 0x04,
        /// <summary>Can create a job for owned resources only.</summary>
        OwnedOnly = 0x08
      }
      [Flags]
      public enum Read {
        /// <summary>Can read any job.</summary>
        Any = 0x02,
        /// <summary>Can read a job in project context only.</summary>
        ProjectOnly = 0x04,
        /// <summary>Can read only owned job.</summary>
        OwnedOnly = 0x08
      }
      [Flags]
      public enum Delete {
        /// <summary>Can delete any job.</summary>
        Any = 0x02,
        /// <summary>Can delete a job in project context only.</summary>
        ProjectOnly = 0x04,
        /// <summary>Can delete only owned job.</summary>
        OwnedOnly = 0x08
      }
    }

    public static class Usermanagement {
      [Flags]
      public enum User {
        Create = 0x02,
        Read = 0x04,
        Update = 0x08,
        Delete = 0x16
      }

      [Flags]
      public enum UserGroup {
        Create = 0x02,
        Read = 0x04,
        Update = 0x08,
        Delete = 0x16
      }

      [Flags]
      public enum Client {
        Create = 0x02,
        Read = 0x04,
        Update = 0x08,
        Delete = 0x16
      }

      [Flags]
      public enum ClientGroup {
        Create = 0x02,
        Read = 0x04,
        Update = 0x08,
        Delete = 0x16
      }
    }

    public static class PermissionManagement {
      [Flags]
      public enum Permission {
        Grant = 0x02,
        Revoke = 0x04
      }
    }

    public static class ResourceManagement {
      [Flags]
      public enum Project {
        Create = 0x02,
        Read = 0x04,
        Update = 0x08,
        Delete = 0x16
      }
    }

    private static PermissionCollection pc;

    private static PolicyCollection pol;

    public static PermissionCollection GetPermissions() {
      if (pc == null) {
        pc = new PermissionCollection();
        LoadFromXml(pc.Permissions, PERMISSIONFILE);
      }
      return pc;
    }

    public static PolicyCollection GetPolicies() {
      if (pol == null) {
        pol = new PolicyCollection();
        LoadFromXml(pol.Policies, POLICIESFILE);
      }
      return pol;
    }

    public static string ConvertEnumType(object obj) {
      string retVal = string.Empty;
      Type t = obj.GetType();
      string value = obj.ToString();
      retVal = (t.FullName.Replace(t.Namespace + ".", "")).Replace("+", ".") + "." + value;
      return retVal;
    }

    /// <summary>
    /// Permission
    /// </summary>
    /// <param name="perm"></param>
    private static void LoadFromXml(IList<Permission> perm, string filename) {
      Permission p = null;
      XPathDocument doc;
      doc = new XPathDocument(filename);
      XPathNavigator nav = doc.CreateNavigator();
      nav.MoveToRoot();
      do {
        if (nav.NodeType == XPathNodeType.Element && nav.Name == "Permission") {
          p = new Permission();
          p.Name = nav.GetAttribute("name", "");
          nav.MoveToFollowing(XPathNodeType.Element);
          if (nav.Name == "ID")
            p.Id = new Guid(nav.Value);
          nav.MoveToFollowing(XPathNodeType.Element);
          if (nav.Name == "Description")
            p.Description = nav.Value;
          nav.MoveToFollowing(XPathNodeType.Element);
          if (nav.Name == "Plugin")
            p.Plugin = nav.Value;
          perm.Add(p);
        }
      } while (nav.MoveToFollowing(XPathNodeType.Element));
    }

    /// <summary>
    /// Policy
    /// </summary>
    /// <param name="pol"></param>
    /// <param name="filename"></param>
    private static void LoadFromXml(IList<Policy> policyList, string filename) {
      PermissionCollection permissionCollection = GetPermissions();
      XPathDocument doc = new XPathDocument(filename);
      XPathNavigator nav = doc.CreateNavigator();
      nav.MoveToRoot();
      //receive all policies -> Element <Policy name="xxx">...</Policy>
      do {
        if (nav.NodeType == XPathNodeType.Element && nav.Name == "Policy") {
          string policyName = nav.GetAttribute("name", "");
          Policy policy = new Policy(policyName);
          nav.MoveToFollowing(XPathNodeType.Element);
          //receive all permissions -> Element <Permission.../>
          do {
            if (nav.Name == "Permission" && nav.HasAttributes) {
              PermissionContext ctx = new PermissionContext();
              int prior = 0;
              int.TryParse(nav.GetAttribute("priority", ""), out prior);
              ctx.Priority = prior;
              ctx.Elevation = nav.GetAttribute("context", "");
              string permissionName = nav.GetAttribute("name", "");
              if (!string.IsNullOrEmpty(permissionName)) {
                Permission permission = permissionCollection[permissionName];
                Debug.WriteLineIf(permission == null, "Permission '" + permissionName + "' not found in PermissionCollection!");
                if (permission != null)
                  policy.AddPermission(permission, ctx);
              }
            }
          } while (nav.MoveToNext());
          policyList.Add(policy);
        }
      } while (nav.MoveToFollowing(XPathNodeType.Element));
    }
  }
  
}
