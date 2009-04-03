using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;
using System.Net;

namespace HeuristicLab.Hive.Contracts {
  public static class WcfSettings {

    public static Binding GetBinding() {
      NetTcpBinding binding = new NetTcpBinding(SecurityMode.Message);
      return binding;
    }

    public static string GetActiveIP() {
      return System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections()[0].LocalEndPoint.Address.ToString();
    }

    public static int GetDefaultPort() {
      return 9000;
    }
  }

  // WARNUNG: Dieser Code wird nur für Testzertifikate benötigt, wie sie beispielsweise von makecert erstellt werden. 
  // Sie sollten diesen Code nicht in einer Produktionsumgebung verwenden.
  public class PermissiveCertificatePolicy {
    string subjectName;
    static PermissiveCertificatePolicy currentPolicy;
    PermissiveCertificatePolicy(string subjectName) {
      this.subjectName = subjectName;
      ServicePointManager.ServerCertificateValidationCallback +=
          new System.Net.Security.RemoteCertificateValidationCallback(RemoteCertValidate);
    }

    public static void Enact(string subjectName) {
      currentPolicy = new PermissiveCertificatePolicy(subjectName);
    }

    bool RemoteCertValidate(object sender, X509Certificate cert, X509Chain chain, System.Net.Security.SslPolicyErrors error) {
      if (cert.Subject == subjectName) {
        return true;
      }

      return false;
    }
  }
}
