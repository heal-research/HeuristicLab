using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;

namespace HeuristicLab.PluginInfrastructure.Advanced.DeploymentService {
  /// <summary>
  /// Factory class to generated update client instances for the deployment service.
  /// </summary>
  public static class UpdateClientFactory {
    private static byte[] serverCrtData;

    /// <summary>
    /// static constructor loads the embedded service certificate 
    /// </summary>
    static UpdateClientFactory() {
      var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.servdev.cer");
      serverCrtData = new byte[stream.Length];
      stream.Read(serverCrtData, 0, serverCrtData.Length);
    }

    /// <summary>
    /// Factory method to create new update clients for the deployment service.
    /// Sets the connection string and user credentials from values provided in settings.
    /// HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationUserName
    /// HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationPassword
    /// HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocation
    /// 
    /// </summary>
    /// <returns>A new instance of an update client</returns>
    public static UpdateClient CreateClient() {
      var client = new UpdateClient();
      client.ClientCredentials.UserName.UserName = HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationUserName;
      client.ClientCredentials.UserName.Password = HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationPassword;
      client.Endpoint.Address = new EndpointAddress(HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocation);
      client.ClientCredentials.ServiceCertificate.DefaultCertificate = new X509Certificate2(serverCrtData);
      client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;
      return client;
    }
  }
}
