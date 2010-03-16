using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using HeuristicLab.Services.Deployment.DataAccess;

namespace HeuristicLab.Services.Deployment {
  // NOTE: If you change the interface name "IUpdate" here, you must also update the reference to "IUpdate" in App.config.
  [ServiceContract]
  public interface IUpdate {
    [OperationContract]
    byte[] GetPlugin(PluginDescription description);

    [OperationContract]
    IEnumerable<ProductDescription> GetProducts();

    [OperationContract]
    IEnumerable<PluginDescription> GetPlugins();
  }
}
