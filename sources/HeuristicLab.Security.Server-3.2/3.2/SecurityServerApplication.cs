using System;
using System.Collections.Generic;
using HeuristicLab.PluginInfrastructure;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Net;
using HeuristicLab.Security.Contracts.Interfaces;
using System.Windows.Forms;

namespace HeuristicLab.Security.Server {

  [ClassInfo(Name = "Security Server",
  Description = "Server application for the security.",
  AutoRestart = true)]
  public class SecurityServerApplication : ApplicationBase {
    public const string STR_PermissionManager = "PermissionManager";
    public const string STR_SecurityManager = "SecurityManager";   

    int DEFAULT_PORT_SM = 9111;
    int DEFAULT_PORT_PM = 9112;

    private DiscoveryService discService = new DiscoveryService();
    private Dictionary<string, ServiceHost> runningServices = new Dictionary<string, ServiceHost>();
    private NetTcpBinding binding = new NetTcpBinding(SecurityMode.None, true);

    private enum Services {
      SecurityManager,
      PermissionManager,
      All
    }
    
    private bool AddMexEndpoint(ServiceHost serviceHost) {
      if (serviceHost != null) {
        ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
        serviceHost.Description.Behaviors.Add(behavior);

        return serviceHost.AddServiceEndpoint(
          typeof(IMetadataExchange),
          MetadataExchangeBindings.CreateMexTcpBinding(),
          "mex") != null;
      } else
        return false;
    }

    private Uri StartService(Services svc, IPAddress ipAddress, int port) {
      string curServiceHost = "";
      Uri uriTcp;
      ISecurityManager[] securityManagerInstances = discService.GetInstances<ISecurityManager>();
      IPermissionManager[] permissionManagerInstances = discService.GetInstances<IPermissionManager>();
      ServiceHost serviceHost = null;
      switch (svc) {
        case Services.PermissionManager:
          if (securityManagerInstances.Length > 0) {
            uriTcp = new Uri("net.tcp://" + ipAddress + ":" + port + "/PermissionManager/"); 
            serviceHost = new ServiceHost(permissionManagerInstances[0].GetType(), uriTcp);
            serviceHost.AddServiceEndpoint(typeof(IPermissionManager), binding, STR_PermissionManager);
            curServiceHost = STR_PermissionManager;
          }
          break;
        case Services.SecurityManager:
          if (securityManagerInstances.Length > 0) {
            uriTcp = new Uri("net.tcp://" + ipAddress + ":" + port + "/SecurityManager/");
            serviceHost = new ServiceHost(securityManagerInstances[0].GetType(), uriTcp);
            serviceHost.AddServiceEndpoint(typeof(ISecurityManager), binding, STR_SecurityManager);
            curServiceHost = STR_SecurityManager;
          }
          break;
        case Services.All:
          throw new InvalidOperationException("Not supported!");
        default:
          return null;
      }
      if ((serviceHost != null) && (!String.IsNullOrEmpty(curServiceHost))) {
        AddMexEndpoint(serviceHost);
    //    WcfSettings.SetServiceCertificate(serviceHost);
        serviceHost.Open();
        runningServices.Add(curServiceHost, serviceHost);
        return serviceHost.BaseAddresses[0];
      } else
        return null;
    }

    private void StopService(Services svc) {
      ServiceHost svcHost = null;
      switch (svc) {
        case Services.PermissionManager:
          runningServices.TryGetValue(STR_PermissionManager, out svcHost);
          break;
        case Services.SecurityManager:
          runningServices.TryGetValue(STR_SecurityManager, out svcHost);
          break;
        case Services.All:
          foreach (KeyValuePair<string, ServiceHost> item in runningServices)
            item.Value.Close();
          return;
        default:
          throw new InvalidOperationException("Not supported!");
      }
      svcHost.Close();
    }

    public override void Run() {
      IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
      int index = 0;
      if (System.Environment.OSVersion.Version.Major >= 6) {
        for (index = addresses.Length - 1; index >= 0; index--)
          if (addresses[index].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            break;
      }
      
      //Start services and record their base address
      Dictionary<string, Uri> baseAddrDict = new Dictionary<string, Uri>();
      baseAddrDict.Add(STR_PermissionManager,
        StartService(Services.PermissionManager, addresses[index], DEFAULT_PORT_PM));
      baseAddrDict.Add(STR_SecurityManager,
        StartService(Services.SecurityManager, addresses[index], DEFAULT_PORT_SM));

      SecurityServer securityServer = new SecurityServer(baseAddrDict);

      Application.Run(securityServer);

      StopService(Services.All);
    }
  }
}
