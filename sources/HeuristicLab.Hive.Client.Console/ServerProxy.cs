using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Client.Console {
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
  [System.ServiceModel.ServiceContractAttribute(ConfigurationName = "IClientConsoleCommunicator")]
  public interface IClientConsoleCommunicator {

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IClientConsoleCommunicator/GetStatusInfos", ReplyAction = "http://tempuri.org/IClientConsoleCommunicator/GetStatusInfosResponse")]
    HeuristicLab.Hive.Client.Communication.ClientConsole.StatusCommons GetStatusInfos();

    [System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = "http://tempuri.org/IClientConsoleCommunicator/GetStatusInfos", ReplyAction = "http://tempuri.org/IClientConsoleCommunicator/GetStatusInfosResponse")]
    System.IAsyncResult BeginGetStatusInfos(System.AsyncCallback callback, object asyncState);

    HeuristicLab.Hive.Client.Communication.ClientConsole.StatusCommons EndGetStatusInfos(System.IAsyncResult result);

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IClientConsoleCommunicator/GetConnection", ReplyAction = "http://tempuri.org/IClientConsoleCommunicator/GetConnectionResponse")]
    HeuristicLab.Hive.Client.Communication.ClientConsole.ConnectionContainer GetConnection();

    [System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = "http://tempuri.org/IClientConsoleCommunicator/GetConnection", ReplyAction = "http://tempuri.org/IClientConsoleCommunicator/GetConnectionResponse")]
    System.IAsyncResult BeginGetConnection(System.AsyncCallback callback, object asyncState);

    HeuristicLab.Hive.Client.Communication.ClientConsole.ConnectionContainer EndGetConnection(System.IAsyncResult result);

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IClientConsoleCommunicator/SetConnection", ReplyAction = "http://tempuri.org/IClientConsoleCommunicator/SetConnectionResponse")]
    void SetConnection(HeuristicLab.Hive.Client.Communication.ClientConsole.ConnectionContainer container);

    [System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = "http://tempuri.org/IClientConsoleCommunicator/SetConnection", ReplyAction = "http://tempuri.org/IClientConsoleCommunicator/SetConnectionResponse")]
    System.IAsyncResult BeginSetConnection(HeuristicLab.Hive.Client.Communication.ClientConsole.ConnectionContainer container, System.AsyncCallback callback, object asyncState);

    void EndSetConnection(System.IAsyncResult result);
  }

  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
  public interface IClientConsoleCommunicatorChannel : IClientConsoleCommunicator, System.ServiceModel.IClientChannel {
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
  public partial class GetStatusInfosCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {

    private object[] results;

    public GetStatusInfosCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
      base(exception, cancelled, userState) {
      this.results = results;
    }

    public HeuristicLab.Hive.Client.Communication.ClientConsole.StatusCommons Result {
      get {
        base.RaiseExceptionIfNecessary();
        return ((HeuristicLab.Hive.Client.Communication.ClientConsole.StatusCommons)(this.results[0]));
      }
    }
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
  public partial class GetConnectionCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {

    private object[] results;

    public GetConnectionCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
      base(exception, cancelled, userState) {
      this.results = results;
    }

    public HeuristicLab.Hive.Client.Communication.ClientConsole.ConnectionContainer Result {
      get {
        base.RaiseExceptionIfNecessary();
        return ((HeuristicLab.Hive.Client.Communication.ClientConsole.ConnectionContainer)(this.results[0]));
      }
    }
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
  public partial class ClientConsoleCommunicatorClient : System.ServiceModel.ClientBase<IClientConsoleCommunicator>, IClientConsoleCommunicator {

    private BeginOperationDelegate onBeginGetStatusInfosDelegate;

    private EndOperationDelegate onEndGetStatusInfosDelegate;

    private System.Threading.SendOrPostCallback onGetStatusInfosCompletedDelegate;

    private BeginOperationDelegate onBeginGetConnectionDelegate;

    private EndOperationDelegate onEndGetConnectionDelegate;

    private System.Threading.SendOrPostCallback onGetConnectionCompletedDelegate;

    private BeginOperationDelegate onBeginSetConnectionDelegate;

    private EndOperationDelegate onEndSetConnectionDelegate;

    private System.Threading.SendOrPostCallback onSetConnectionCompletedDelegate;

    public ClientConsoleCommunicatorClient() {
    }

    public ClientConsoleCommunicatorClient(string endpointConfigurationName) :
      base(endpointConfigurationName) {
    }

    public ClientConsoleCommunicatorClient(string endpointConfigurationName, string remoteAddress) :
      base(endpointConfigurationName, remoteAddress) {
    }

    public ClientConsoleCommunicatorClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
      base(endpointConfigurationName, remoteAddress) {
    }

    public ClientConsoleCommunicatorClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
      base(binding, remoteAddress) {
    }

    public event System.EventHandler<GetStatusInfosCompletedEventArgs> GetStatusInfosCompleted;

    public event System.EventHandler<GetConnectionCompletedEventArgs> GetConnectionCompleted;

    public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> SetConnectionCompleted;

    public HeuristicLab.Hive.Client.Communication.ClientConsole.StatusCommons GetStatusInfos() {
      return base.Channel.GetStatusInfos();
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    public System.IAsyncResult BeginGetStatusInfos(System.AsyncCallback callback, object asyncState) {
      return base.Channel.BeginGetStatusInfos(callback, asyncState);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    public HeuristicLab.Hive.Client.Communication.ClientConsole.StatusCommons EndGetStatusInfos(System.IAsyncResult result) {
      return base.Channel.EndGetStatusInfos(result);
    }

    private System.IAsyncResult OnBeginGetStatusInfos(object[] inValues, System.AsyncCallback callback, object asyncState) {
      return this.BeginGetStatusInfos(callback, asyncState);
    }

    private object[] OnEndGetStatusInfos(System.IAsyncResult result) {
      HeuristicLab.Hive.Client.Communication.ClientConsole.StatusCommons retVal = this.EndGetStatusInfos(result);
      return new object[] {
                retVal};
    }

    private void OnGetStatusInfosCompleted(object state) {
      if ((this.GetStatusInfosCompleted != null)) {
        InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
        this.GetStatusInfosCompleted(this, new GetStatusInfosCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
      }
    }

    public void GetStatusInfosAsync() {
      this.GetStatusInfosAsync(null);
    }

    public void GetStatusInfosAsync(object userState) {
      if ((this.onBeginGetStatusInfosDelegate == null)) {
        this.onBeginGetStatusInfosDelegate = new BeginOperationDelegate(this.OnBeginGetStatusInfos);
      }
      if ((this.onEndGetStatusInfosDelegate == null)) {
        this.onEndGetStatusInfosDelegate = new EndOperationDelegate(this.OnEndGetStatusInfos);
      }
      if ((this.onGetStatusInfosCompletedDelegate == null)) {
        this.onGetStatusInfosCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetStatusInfosCompleted);
      }
      base.InvokeAsync(this.onBeginGetStatusInfosDelegate, null, this.onEndGetStatusInfosDelegate, this.onGetStatusInfosCompletedDelegate, userState);
    }

    public HeuristicLab.Hive.Client.Communication.ClientConsole.ConnectionContainer GetConnection() {
      return base.Channel.GetConnection();
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    public System.IAsyncResult BeginGetConnection(System.AsyncCallback callback, object asyncState) {
      return base.Channel.BeginGetConnection(callback, asyncState);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    public HeuristicLab.Hive.Client.Communication.ClientConsole.ConnectionContainer EndGetConnection(System.IAsyncResult result) {
      return base.Channel.EndGetConnection(result);
    }

    private System.IAsyncResult OnBeginGetConnection(object[] inValues, System.AsyncCallback callback, object asyncState) {
      return this.BeginGetConnection(callback, asyncState);
    }

    private object[] OnEndGetConnection(System.IAsyncResult result) {
      HeuristicLab.Hive.Client.Communication.ClientConsole.ConnectionContainer retVal = this.EndGetConnection(result);
      return new object[] {
                retVal};
    }

    private void OnGetConnectionCompleted(object state) {
      if ((this.GetConnectionCompleted != null)) {
        InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
        this.GetConnectionCompleted(this, new GetConnectionCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
      }
    }

    public void GetConnectionAsync() {
      this.GetConnectionAsync(null);
    }

    public void GetConnectionAsync(object userState) {
      if ((this.onBeginGetConnectionDelegate == null)) {
        this.onBeginGetConnectionDelegate = new BeginOperationDelegate(this.OnBeginGetConnection);
      }
      if ((this.onEndGetConnectionDelegate == null)) {
        this.onEndGetConnectionDelegate = new EndOperationDelegate(this.OnEndGetConnection);
      }
      if ((this.onGetConnectionCompletedDelegate == null)) {
        this.onGetConnectionCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetConnectionCompleted);
      }
      base.InvokeAsync(this.onBeginGetConnectionDelegate, null, this.onEndGetConnectionDelegate, this.onGetConnectionCompletedDelegate, userState);
    }

    public void SetConnection(HeuristicLab.Hive.Client.Communication.ClientConsole.ConnectionContainer container) {
      base.Channel.SetConnection(container);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    public System.IAsyncResult BeginSetConnection(HeuristicLab.Hive.Client.Communication.ClientConsole.ConnectionContainer container, System.AsyncCallback callback, object asyncState) {
      return base.Channel.BeginSetConnection(container, callback, asyncState);
    }

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    public void EndSetConnection(System.IAsyncResult result) {
      base.Channel.EndSetConnection(result);
    }

    private System.IAsyncResult OnBeginSetConnection(object[] inValues, System.AsyncCallback callback, object asyncState) {
      HeuristicLab.Hive.Client.Communication.ClientConsole.ConnectionContainer container = ((HeuristicLab.Hive.Client.Communication.ClientConsole.ConnectionContainer)(inValues[0]));
      return this.BeginSetConnection(container, callback, asyncState);
    }

    private object[] OnEndSetConnection(System.IAsyncResult result) {
      this.EndSetConnection(result);
      return null;
    }

    private void OnSetConnectionCompleted(object state) {
      if ((this.SetConnectionCompleted != null)) {
        InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
        this.SetConnectionCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
      }
    }

    public void SetConnectionAsync(HeuristicLab.Hive.Client.Communication.ClientConsole.ConnectionContainer container) {
      this.SetConnectionAsync(container, null);
    }

    public void SetConnectionAsync(HeuristicLab.Hive.Client.Communication.ClientConsole.ConnectionContainer container, object userState) {
      if ((this.onBeginSetConnectionDelegate == null)) {
        this.onBeginSetConnectionDelegate = new BeginOperationDelegate(this.OnBeginSetConnection);
      }
      if ((this.onEndSetConnectionDelegate == null)) {
        this.onEndSetConnectionDelegate = new EndOperationDelegate(this.OnEndSetConnection);
      }
      if ((this.onSetConnectionCompletedDelegate == null)) {
        this.onSetConnectionCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnSetConnectionCompleted);
      }
      base.InvokeAsync(this.onBeginSetConnectionDelegate, new object[] {
                    container}, this.onEndSetConnectionDelegate, this.onSetConnectionCompletedDelegate, userState);
    }
  }
}
