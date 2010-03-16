using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using HeuristicLab.Services.Deployment.DataAccess;
using System.Security.Permissions;

namespace HeuristicLab.Services.Deployment {
  // NOTE: If you change the class name "Admin" here, you must also update the reference to "Admin" in App.config.
  public class Admin : IAdmin {
    #region IAdmin Members
    [PrincipalPermission(SecurityAction.Demand, Role = "Managers")]
    public void DeployProduct(ProductDescription product) {
      var store = new PluginStore();
      store.Persist(product);
    }
    [PrincipalPermission(SecurityAction.Demand, Role = "Managers")]
    public void DeployPlugin(PluginDescription plugin, byte[] zipFile) {
      var store = new PluginStore();
      store.Persist(plugin, zipFile);
    }

    #endregion
  }
}
