using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;

namespace HeuristicLab.PluginInfrastructure.Advanced.DeploymentService {
  public static class UpdateClientFactory {
    private static byte[] serverCrtData;

    static UpdateClientFactory() {
      var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.servdev.cer");
      serverCrtData = new byte[stream.Length];
      stream.Read(serverCrtData, 0, serverCrtData.Length);
    }

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
