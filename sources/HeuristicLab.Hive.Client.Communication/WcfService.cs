using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using HeuristicLab.Hive.Contracts.Interfaces;
using HeuristicLab.Hive.Contracts;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Client.Common;
using HeuristicLab.Hive.Client.Communication.ServerService;

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

    public DateTime ConnectedSince { get; private set; }    
    public NetworkEnum.WcfConnState ConnState { get; private set; }
    public string ServerIP { get; private set; }
    public int ServerPort { get; private set; }

    public event EventHandler ConnectionRestored;    
    public event EventHandler ServerChanged;
    public event EventHandler Connected;    

    public ClientCommunicatorClient proxy = null;

    private WcfService() {
      ConnState = NetworkEnum.WcfConnState.Disconnected;
    }
    public void Connect() {
      try {
        proxy = new ClientCommunicatorClient(
          new NetTcpBinding(),
          new EndpointAddress("net.tcp://" + ServerIP + ":" + ServerPort + "/HiveServer/ClientCommunicator")
        );

        proxy.LoginCompleted += new EventHandler<LoginCompletedEventArgs>(proxy_LoginCompleted);
        proxy.PullJobCompleted += new EventHandler<PullJobCompletedEventArgs>(proxy_PullJobCompleted);
        proxy.SendJobResultCompleted += new EventHandler<SendJobResultCompletedEventArgs>(proxy_SendJobResultCompleted);
        proxy.SendHeartBeatCompleted += new EventHandler<SendHeartBeatCompletedEventArgs>(proxy_SendHeartBeatCompleted);
        proxy.Open();
        if (ConnState == NetworkEnum.WcfConnState.Failed)
          ConnectionRestored(this, new EventArgs());

        ConnState = NetworkEnum.WcfConnState.Connected;
        ConnectedSince = DateTime.Now;        
        if (Connected != null)
          Connected(this, new EventArgs());                               
      }
      catch (Exception ex) {
        NetworkErrorHandling(ex);
      }
    }

    public void Connect(String serverIP, int serverPort) {
      String oldIp = this.ServerIP;
      int oldPort = this.ServerPort;
      this.ServerIP = serverIP;
      this.ServerPort = serverPort;      
      Connect();
      if (oldIp != serverIP || oldPort != ServerPort)
        if(ServerChanged != null) 
          ServerChanged(this, new EventArgs());
    }

    public void Disconnect() {
      ConnState = NetworkEnum.WcfConnState.Disconnected;
    }

    private void NetworkErrorHandling(Exception e) {
      ConnState = NetworkEnum.WcfConnState.Failed;
      Logging.GetInstance().Error(this.ToString(), "exception: ", e);
    }

    #region Login
    public event System.EventHandler<LoginCompletedEventArgs> LoginCompleted;
    public void LoginAsync(ClientInfo clientInfo) {
      if (ConnState == NetworkEnum.WcfConnState.Connected)
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
      if (ConnState == NetworkEnum.WcfConnState.Connected)        
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
      if (ConnState == NetworkEnum.WcfConnState.Connected)
        proxy.SendJobResultAsync(result, finished);
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
      if (ConnState == NetworkEnum.WcfConnState.Connected)
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
