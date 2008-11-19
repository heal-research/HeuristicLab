using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Server.Interfaces;

namespace HeuristicLab.Hive.Client.Communication {
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
  [System.ServiceModel.ServiceContractAttribute(ConfigurationName = "IClientCommunicator")]
  public interface IClientCommunicator {

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IClientCommunicator/Login", ReplyAction = "http://tempuri.org/IClientCommunicator/LoginResponse")]
    HeuristicLab.Hive.Server.Response Login(HeuristicLab.Hive.Contracts.BusinessObjects.Client clientInfo);

    [System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = "http://tempuri.org/IClientCommunicator/Login", ReplyAction = "http://tempuri.org/IClientCommunicator/LoginResponse")]
    System.IAsyncResult BeginLogin(HeuristicLab.Hive.Contracts.BusinessObjects.Client clientInfo, System.AsyncCallback callback, object asyncState);

    HeuristicLab.Hive.Server.Response EndLogin(System.IAsyncResult result);
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
  public interface IClientCommunicatorChannel : IClientCommunicator, System.ServiceModel.IClientChannel {
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
  public partial class LoginCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {

    private object[] results;

    public LoginCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
      base(exception, cancelled, userState) {
      this.results = results;
    }

    public HeuristicLab.Hive.Server.Response Result {
      get {
        base.RaiseExceptionIfNecessary();
        return ((HeuristicLab.Hive.Server.Response)(this.results[0]));
      }
    }
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
  public partial class ClientCommunicatorClient : System.ServiceModel.ClientBase<IClientCommunicator>, IClientCommunicator {

    private BeginOperationDelegate onBeginLoginDelegate;

    private EndOperationDelegate onEndLoginDelegate;

    private System.Threading.SendOrPostCallback onLoginCompletedDelegate;

    public ClientCommunicatorClient() {
    }

    public ClientCommunicatorClient(string endpointConfigurationName) :
      base(endpointConfigurationName) {
    }

    public ClientCommunicatorClient(string endpointConfigurationName, string remoteAddress) :
      base(endpointConfigurationName, remoteAddress) {
    }

    public ClientCommunicatorClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
      base(endpointConfigurationName, remoteAddress) {
    }

    public ClientCommunicatorClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
      base(binding, remoteAddress) {
    }

    public event System.EventHandler<LoginCompletedEventArgs> LoginCompleted;

    public HeuristicLab.Hive.Server.Response Login(HeuristicLab.Hive.Contracts.BusinessObjects.Client clientInfo) {
      return base.Channel.Login(clientInfo);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    public System.IAsyncResult BeginLogin(HeuristicLab.Hive.Contracts.BusinessObjects.Client clientInfo, System.AsyncCallback callback, object asyncState) {
      return base.Channel.BeginLogin(clientInfo, callback, asyncState);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    public HeuristicLab.Hive.Server.Response EndLogin(System.IAsyncResult result) {
      return base.Channel.EndLogin(result);
    }

    private System.IAsyncResult OnBeginLogin(object[] inValues, System.AsyncCallback callback, object asyncState) {
      HeuristicLab.Hive.Contracts.BusinessObjects.Client clientInfo = ((HeuristicLab.Hive.Contracts.BusinessObjects.Client)(inValues[0]));
      return this.BeginLogin(clientInfo, callback, asyncState);
    }

    private object[] OnEndLogin(System.IAsyncResult result) {
      HeuristicLab.Hive.Server.Response retVal = this.EndLogin(result);
      return new object[] {
                retVal};
    }

    private void OnLoginCompleted(object state) {
      if ((this.LoginCompleted != null)) {
        InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
        this.LoginCompleted(this, new LoginCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
      }
    }

    public void LoginAsync(HeuristicLab.Hive.Contracts.BusinessObjects.Client clientInfo) {
      this.LoginAsync(clientInfo, null);
    }

    public void LoginAsync(HeuristicLab.Hive.Contracts.BusinessObjects.Client clientInfo, object userState) {
      if ((this.onBeginLoginDelegate == null)) {
        this.onBeginLoginDelegate = new BeginOperationDelegate(this.OnBeginLogin);
      }
      if ((this.onEndLoginDelegate == null)) {
        this.onEndLoginDelegate = new EndOperationDelegate(this.OnEndLogin);
      }
      if ((this.onLoginCompletedDelegate == null)) {
        this.onLoginCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnLoginCompleted);
      }
      base.InvokeAsync(this.onBeginLoginDelegate, new object[] {
                    clientInfo}, this.onEndLoginDelegate, this.onLoginCompletedDelegate, userState);
    }
  }
}
