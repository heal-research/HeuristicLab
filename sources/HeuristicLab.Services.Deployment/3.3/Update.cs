using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using HeuristicLab.Services.Deployment.DataAccess;

namespace HeuristicLab.Services.Deployment {
  // NOTE: If you change the class name "Update" here, you must also update the reference to "Update" in App.config.
  public class Update : IUpdate {
    #region IUpdate Members

    public byte[] GetPlugin(PluginDescription description) {
      PluginStore store = new PluginStore();
      return store.PluginFile(description);
    }


    public IEnumerable<ProductDescription> GetProducts() {
      PluginStore store = new PluginStore();
      return store.Products;
    }

    public IEnumerable<PluginDescription> GetPlugins() {
      PluginStore store = new PluginStore();
      return store.Plugins;
    }
    #endregion
  }
}
