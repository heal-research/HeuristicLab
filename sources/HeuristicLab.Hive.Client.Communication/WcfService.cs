using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Client.Common;


namespace HeuristicLab.Hive.Client.Communication {
  public class WcfService {
    private static WcfService instance;
    public static WcfService Instance {
      get {
        if (instance == null) {
          instance = new WcfService();
        }
        return instance;
      }
    }

    public enum ConnectionState { connected, disconnected, failed };

    private ClientCommunicatorClient proxy = null;

    public ConnectionState ConnState { get; set; }

    private string serverIP;
    private string serverPort;

    private WcfService() {
    }
    public void Connect() {
      try {
        if (proxy == null) {
          proxy = new ClientCommunicatorClient(
            new NetTcpBinding(),
            new EndpointAddress("net.tcp://" + serverIP + ":" + serverPort + "/HiveServer/ClientCommunicator")
            );
        }

        proxy.LoginCompleted += new EventHandler<LoginCompletedEventArgs>(proxy_LoginCompleted);
        proxy.PullJobCompleted += new EventHandler<PullJobCompletedEventArgs>(proxy_PullJobCompleted);
        proxy.SendJobResultCompleted += new EventHandler<SendJobResultCompletedEventArgs>(proxy_SendJobResultCompleted);
        proxy.SendHeartBeatCompleted += new EventHandler<SendHeartBeatCompletedEventArgs>(proxy_SendHeartBeatCompleted);
      }
      catch (Exception ex) {
        NetworkErrorHandling(ex);
      }
    }

    public void Connect(String serverIP, String serverPort) {
      this.serverIP = serverIP;
      this.serverPort = serverPort;
      Connect();
    }

    private void NetworkErrorHandling(Exception e) {
      ConnState = ConnectionState.failed;
      Logging.GetInstance().Error(this.ToString(), "exception: ", e);
    }

    #region Login
    public event System.EventHandler<LoginCompletedEventArgs> LoginCompleted;
    public void LoginAsync(ClientInfo clientInfo) {
      if (ConnState == ConnectionState.connected)
        proxy.LoginAsync(clientInfo);
    }
    private void proxy_LoginCompleted(object sender, LoginCompletedEventArgs e) {
      if (e.Error == null)
        LoginCompleted(sender, e);
      else
        NetworkErrorHandling(e.Error.InnerException);
    }
    #endregion

    #region PullJob
    public event System.EventHandler<PullJobCompletedEventArgs> PullJobCompleted;
    public void PullJobAsync(Guid guid) {
      if (ConnState == ConnectionState.connected)
        proxy.PullJobAsync(guid);
    }
    void proxy_PullJobCompleted(object sender, PullJobCompletedEventArgs e) {
      if (e.Error == null)
        PullJobCompleted(sender, e);
      else
        NetworkErrorHandling(e.Error);
    }
    #endregion

    #region SendJobResults
    public event System.EventHandler<SendJobResultCompletedEventArgs> SendJobResultCompleted;
    public void SendJobResultAsync(JobResult result, bool finished) {
      if (ConnState == ConnectionState.connected)
        proxy.SendJobResult(result, finished);
    }
    private void proxy_SendJobResultCompleted(object sender, SendJobResultCompletedEventArgs e) {
      if (e.Error == null)
        SendJobResultCompleted(sender, e);
      else
        NetworkErrorHandling(e.Error);
    }

    #endregion

    #region Heartbeat

    public event System.EventHandler<SendHeartBeatCompletedEventArgs> SendHeartBeatCompleted;
    public void SendHeartBeatAsync(HeartBeatData hbd) {
      if (ConnState == ConnectionState.connected)
        proxy.SendHeartBeatAsync(hbd);
    }

    private void proxy_SendHeartBeatCompleted(object sender, SendHeartBeatCompletedEventArgs e) {
      if (e.Error == null)
        SendHeartBeatCompleted(sender, e);
      else
        NetworkErrorHandling(e.Error);
    }

    #endregion
  }
}
