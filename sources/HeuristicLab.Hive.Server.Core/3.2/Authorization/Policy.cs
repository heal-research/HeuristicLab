using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Security.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Server.Core {
  public class Policy {
    private string _name;
    public string Name { get { return _name; } set { this._name = value; } }

    private IDictionary<Permission, PermissionContext> _permissions = new Dictionary<Permission, PermissionContext>();
    public IDictionary<Permission, PermissionContext> Permissions { get { return this._permissions; } set { this._permissions = value; } }

    public Policy(string name) {
      this._name = name;
    }

    public void AddPermission(Permission permission, PermissionContext ctx) {
      this._permissions.Add(permission, ctx);
    }

    public Permission GetPermissionByContext(string context) {
      foreach (KeyValuePair<Permission, PermissionContext> item in _permissions) {
        if (item.Value.Elevation == context)
          return item.Key;
      }
      return null;
    }
  }
}
