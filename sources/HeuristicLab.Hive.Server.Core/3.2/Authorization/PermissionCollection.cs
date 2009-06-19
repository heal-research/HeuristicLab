using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Security.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Server.Core {
  public class PermissionCollection : IEnumerable<Permission> {
    public IList<Permission> perm = new List<Permission>();

    public PermissionCollection() {
    }

    public IList<Permission> Permissions {
      get { return perm; }
    }

    /// <summary>
    /// Gets the Permission identified by FQN type.
    /// </summary>
    /// <param name="permissionName">eg. 'HivePermissions.Jobmanagement.Create.Any'</param>
    /// <returns></returns>
    public Permission this[string permissionName] {
      get {
        foreach (Permission item in perm) {
          if (item.Name == permissionName)
            return item;
        }
        return null;
      }
    }

    /// <summary>
    /// Converts an enumeration to a fully qualified string.
    /// </summary>
    /// <param name="obj">Any enumeration type.</param>
    /// <returns>Type Information in Full Qualified Notation (FQN).</returns>
    public string Convert(object obj) {
      string retVal = string.Empty;
      Type t = obj.GetType();
      string value = obj.ToString();
      retVal = (t.FullName.Replace(t.Namespace + ".", "")).Replace("+", ".") + "." + value;
      return retVal;
    }

    public IEnumerator<Permission> GetEnumerator() {
      return perm.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return perm.GetEnumerator();
    }

  }

}
