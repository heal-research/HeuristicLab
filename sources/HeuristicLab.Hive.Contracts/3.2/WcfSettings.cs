//#define USE_MSG_BINDING
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

    public const string SERVERCERT = "HIVE-Server";
    public const int DEFAULTPORT = 9000;
    
    /// <summary>
    /// Gets a pre-defined binding using TCP for secure transport.
    /// </summary>
    /// <returns>A binding type of <see cref="NetTcpBinding"/></returns>
    public static Binding GetBinding() {
#if USE_MSG_BINDING
      NetTcpBinding binding = new NetTcpBinding(SecurityMode.Message);
#else
      NetTcpBinding binding = new NetTcpBinding();
#endif
      return binding;
    }

    /// <summary>
    /// Defines the used certificate for authentification located in a certification store.
    /// </summary>
    /// <param name="svchost">A service for which this certificate is applicable.</param>
    public static void SetServiceCertificate(ServiceHost svchost) {
#if USE_MSG_BINDING
      svchost.Credentials.ServiceCertificate.SetCertificate(
        StoreLocation.LocalMachine,
        StoreName.My,
        X509FindType.FindBySubjectName,
        SERVERCERT);
#endif
    }

    /// <summary>
    /// Gets the currently active IP address.
    /// <remarks>If more than one IP connections is active, the first one will be used.</remarks>
    /// </summary>
    /// <returns></returns>
    public static string GetActiveIP() {
      return System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections()[0].LocalEndPoint.Address.ToString();
    }

    /// <summary>
    /// Gets the default port used for HIVE services.
    /// </summary>
    /// <returns></returns>
    public static int GetDefaultPort() {
      return DEFAULTPORT;
    }
  }

  /// <summary>
  /// This class verifies the certificate defined by <see cref="SetServerCertificate"></see> method. Normally,
  /// the verification process is managed by the underlying operating system.
  /// </summary>
  /// <remarks>
  /// WARNUNG: Dieser Code wird nur für Testzertifikate benötigt, wie sie beispielsweise von makecert erstellt werden.
  /// Sie sollten diesen Code nicht in einer Produktionsumgebung verwenden.
  /// </remarks>
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
