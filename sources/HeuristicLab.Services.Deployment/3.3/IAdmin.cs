using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using HeuristicLab.Services.Deployment.DataAccess;

namespace HeuristicLab.Services.Deployment {
  // NOTE: If you change the interface name "IAdmin" here, you must also update the reference to "IAdmin" in App.config.
  [ServiceContract]
  public interface IAdmin {
    [OperationContract]
    void DeployProduct(ProductDescription product);

    [OperationContract]
    void DeployPlugin(PluginDescription plugin, byte[] zipFile);
  }
}
