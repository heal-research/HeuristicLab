using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Client.Console {
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
  [System.Runtime.Serialization.DataContractAttribute(Name = "StatusCommons", Namespace = "http://schemas.datacontract.org/2004/07/HeuristicLab.Hive.Client.Core.ClientConso" +
      "leService")]
  public partial class StatusCommons : object, System.Runtime.Serialization.IExtensibleDataObject {

    private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

    private System.Guid ClientGuidk__BackingFieldField;

    private System.DateTime ConnectedSincek__BackingFieldField;

    private int JobsAbortedk__BackingFieldField;

    private int JobsDonek__BackingFieldField;

    private int JobsFetchedk__BackingFieldField;

    private HeuristicLab.Hive.Client.Core.ClientConsoleService.JobStatus[] Jobsk__BackingFieldField;

    private HeuristicLab.Hive.Client.Common.NetworkEnumWcfConnState Statusk__BackingFieldField;

    public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
      get {
        return this.extensionDataField;
      }
      set {
        this.extensionDataField = value;
      }
    }

    [System.Runtime.Serialization.DataMemberAttribute(Name = "<ClientGuid>k__BackingField", IsRequired = true)]
    public System.Guid ClientGuidk__BackingField {
      get {
        return this.ClientGuidk__BackingFieldField;
      }
      set {
        this.ClientGuidk__BackingFieldField = value;
      }
    }

    [System.Runtime.Serialization.DataMemberAttribute(Name = "<ConnectedSince>k__BackingField", IsRequired = true)]
    public System.DateTime ConnectedSincek__BackingField {
      get {
        return this.ConnectedSincek__BackingFieldField;
      }
      set {
        this.ConnectedSincek__BackingFieldField = value;
      }
    }

    [System.Runtime.Serialization.DataMemberAttribute(Name = "<JobsAborted>k__BackingField", IsRequired = true)]
    public int JobsAbortedk__BackingField {
      get {
        return this.JobsAbortedk__BackingFieldField;
      }
      set {
        this.JobsAbortedk__BackingFieldField = value;
      }
    }

    [System.Runtime.Serialization.DataMemberAttribute(Name = "<JobsDone>k__BackingField", IsRequired = true)]
    public int JobsDonek__BackingField {
      get {
        return this.JobsDonek__BackingFieldField;
      }
      set {
        this.JobsDonek__BackingFieldField = value;
      }
    }

    [System.Runtime.Serialization.DataMemberAttribute(Name = "<JobsFetched>k__BackingField", IsRequired = true)]
    public int JobsFetchedk__BackingField {
      get {
        return this.JobsFetchedk__BackingFieldField;
      }
      set {
        this.JobsFetchedk__BackingFieldField = value;
      }
    }

    [System.Runtime.Serialization.DataMemberAttribute(Name = "<Jobs>k__BackingField", IsRequired = true)]
    public HeuristicLab.Hive.Client.Core.ClientConsoleService.JobStatus[] Jobsk__BackingField {
      get {
        return this.Jobsk__BackingFieldField;
      }
      set {
        this.Jobsk__BackingFieldField = value;
      }
    }

    [System.Runtime.Serialization.DataMemberAttribute(Name = "<Status>k__BackingField", IsRequired = true)]
    public HeuristicLab.Hive.Client.Common.NetworkEnumWcfConnState Statusk__BackingField {
      get {
        return this.Statusk__BackingFieldField;
      }
      set {
        this.Statusk__BackingFieldField = value;
      }
    }
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
  [System.Runtime.Serialization.DataContractAttribute(Name = "JobStatus", Namespace = "http://schemas.datacontract.org/2004/07/HeuristicLab.Hive.Client.Core.ClientConso" +
      "leService")]
  public partial class JobStatus : object, System.Runtime.Serialization.IExtensibleDataObject {

    private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

    private long JobIdk__BackingFieldField;

    private double Progressk__BackingFieldField;

    private System.DateTime Sincek__BackingFieldField;

    public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
      get {
        return this.extensionDataField;
      }
      set {
        this.extensionDataField = value;
      }
    }

    [System.Runtime.Serialization.DataMemberAttribute(Name = "<JobId>k__BackingField", IsRequired = true)]
    public long JobIdk__BackingField {
      get {
        return this.JobIdk__BackingFieldField;
      }
      set {
        this.JobIdk__BackingFieldField = value;
      }
    }

    [System.Runtime.Serialization.DataMemberAttribute(Name = "<Progress>k__BackingField", IsRequired = true)]
    public double Progressk__BackingField {
      get {
        return this.Progressk__BackingFieldField;
      }
      set {
        this.Progressk__BackingFieldField = value;
      }
    }

    [System.Runtime.Serialization.DataMemberAttribute(Name = "<Since>k__BackingField", IsRequired = true)]
    public System.DateTime Sincek__BackingField {
      get {
        return this.Sincek__BackingFieldField;
      }
      set {
        this.Sincek__BackingFieldField = value;
      }
    }
  }

  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
  [System.Runtime.Serialization.DataContractAttribute(Name = "ConnectionContainer", Namespace = "http://schemas.datacontract.org/2004/07/HeuristicLab.Hive.Client.Core.ClientConso" +
      "leService")]
  public partial class ConnectionContainer : object, System.Runtime.Serialization.IExtensibleDataObject {

    private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

    private string IPAdressk__BackingFieldField;

    private int Portk__BackingFieldField;

    public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
      get {
        return this.extensionDataField;
      }
      set {
        this.extensionDataField = value;
      }
    }

    [System.Runtime.Serialization.DataMemberAttribute(Name = "<IPAdress>k__BackingField", IsRequired = true)]
    public string IPAdressk__BackingField {
      get {
        return this.IPAdressk__BackingFieldField;
      }
      set {
        this.IPAdressk__BackingFieldField = value;
      }
    }

    [System.Runtime.Serialization.DataMemberAttribute(Name = "<Port>k__BackingField", IsRequired = true)]
    public int Portk__BackingField {
      get {
        return this.Portk__BackingFieldField;
      }
      set {
        this.Portk__BackingFieldField = value;
      }
    }
  }
}
namespace HeuristicLab.Hive.Client.Common {
  using System.Runtime.Serialization;


  [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
  [System.Runtime.Serialization.DataContractAttribute(Name = "NetworkEnum.WcfConnState", Namespace = "http://schemas.datacontract.org/2004/07/HeuristicLab.Hive.Client.Common")]
  public enum NetworkEnumWcfConnState : int {

    [System.Runtime.Serialization.EnumMemberAttribute()]
    Connected = 0,

    [System.Runtime.Serialization.EnumMemberAttribute()]
    Disconnected = 1,

    [System.Runtime.Serialization.EnumMemberAttribute()]
    Failed = 2,
  }
}


[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(ConfigurationName = "IClientConsoleCommunicator")]
public interface IClientConsoleCommunicator {

  [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IClientConsoleCommunicator/GetStatusInfos", ReplyAction = "http://tempuri.org/IClientConsoleCommunicator/GetStatusInfosResponse")]
  HeuristicLab.Hive.Client.Core.ClientConsoleService.StatusCommons GetStatusInfos();

  [System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = "http://tempuri.org/IClientConsoleCommunicator/GetStatusInfos", ReplyAction = "http://tempuri.org/IClientConsoleCommunicator/GetStatusInfosResponse")]
  System.IAsyncResult BeginGetStatusInfos(System.AsyncCallback callback, object asyncState);

  HeuristicLab.Hive.Client.Core.ClientConsoleService.StatusCommons EndGetStatusInfos(System.IAsyncResult result);

  [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IClientConsoleCommunicator/GetConnection", ReplyAction = "http://tempuri.org/IClientConsoleCommunicator/GetConnectionResponse")]
  HeuristicLab.Hive.Client.Core.ClientConsoleService.ConnectionContainer GetConnection();

  [System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = "http://tempuri.org/IClientConsoleCommunicator/GetConnection", ReplyAction = "http://tempuri.org/IClientConsoleCommunicator/GetConnectionResponse")]
  System.IAsyncResult BeginGetConnection(System.AsyncCallback callback, object asyncState);

  HeuristicLab.Hive.Client.Core.ClientConsoleService.ConnectionContainer EndGetConnection(System.IAsyncResult result);

  [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IClientConsoleCommunicator/SetConnection", ReplyAction = "http://tempuri.org/IClientConsoleCommunicator/SetConnectionResponse")]
  void SetConnection(HeuristicLab.Hive.Client.Core.ClientConsoleService.ConnectionContainer container);

  [System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = "http://tempuri.org/IClientConsoleCommunicator/SetConnection", ReplyAction = "http://tempuri.org/IClientConsoleCommunicator/SetConnectionResponse")]
  System.IAsyncResult BeginSetConnection(HeuristicLab.Hive.Client.Core.ClientConsoleService.ConnectionContainer container, System.AsyncCallback callback, object asyncState);

  void EndSetConnection(System.IAsyncResult result);

  [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IClientConsoleCommunicator/Disconnect", ReplyAction = "http://tempuri.org/IClientConsoleCommunicator/DisconnectResponse")]
  void Disconnect();

  [System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = "http://tempuri.org/IClientConsoleCommunicator/Disconnect", ReplyAction = "http://tempuri.org/IClientConsoleCommunicator/DisconnectResponse")]
  System.IAsyncResult BeginDisconnect(System.AsyncCallback callback, object asyncState);

  void EndDisconnect(System.IAsyncResult result);
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

  public HeuristicLab.Hive.Client.Core.ClientConsoleService.StatusCommons Result {
    get {
      base.RaiseExceptionIfNecessary();
      return ((HeuristicLab.Hive.Client.Core.ClientConsoleService.StatusCommons)(this.results[0]));
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

  public HeuristicLab.Hive.Client.Core.ClientConsoleService.ConnectionContainer Result {
    get {
      base.RaiseExceptionIfNecessary();
      return ((HeuristicLab.Hive.Client.Core.ClientConsoleService.ConnectionContainer)(this.results[0]));
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

  private BeginOperationDelegate onBeginDisconnectDelegate;

  private EndOperationDelegate onEndDisconnectDelegate;

  private System.Threading.SendOrPostCallback onDisconnectCompletedDelegate;

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

  public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> DisconnectCompleted;

  public HeuristicLab.Hive.Client.Core.ClientConsoleService.StatusCommons GetStatusInfos() {
    return base.Channel.GetStatusInfos();
  }

  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  public System.IAsyncResult BeginGetStatusInfos(System.AsyncCallback callback, object asyncState) {
    return base.Channel.BeginGetStatusInfos(callback, asyncState);
  }

  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  public HeuristicLab.Hive.Client.Core.ClientConsoleService.StatusCommons EndGetStatusInfos(System.IAsyncResult result) {
    return base.Channel.EndGetStatusInfos(result);
  }

  private System.IAsyncResult OnBeginGetStatusInfos(object[] inValues, System.AsyncCallback callback, object asyncState) {
    return this.BeginGetStatusInfos(callback, asyncState);
  }

  private object[] OnEndGetStatusInfos(System.IAsyncResult result) {
    HeuristicLab.Hive.Client.Core.ClientConsoleService.StatusCommons retVal = this.EndGetStatusInfos(result);
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

  public HeuristicLab.Hive.Client.Core.ClientConsoleService.ConnectionContainer GetConnection() {
    return base.Channel.GetConnection();
  }

  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  public System.IAsyncResult BeginGetConnection(System.AsyncCallback callback, object asyncState) {
    return base.Channel.BeginGetConnection(callback, asyncState);
  }

  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  public HeuristicLab.Hive.Client.Core.ClientConsoleService.ConnectionContainer EndGetConnection(System.IAsyncResult result) {
    return base.Channel.EndGetConnection(result);
  }

  private System.IAsyncResult OnBeginGetConnection(object[] inValues, System.AsyncCallback callback, object asyncState) {
    return this.BeginGetConnection(callback, asyncState);
  }

  private object[] OnEndGetConnection(System.IAsyncResult result) {
    HeuristicLab.Hive.Client.Core.ClientConsoleService.ConnectionContainer retVal = this.EndGetConnection(result);
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

  public void SetConnection(HeuristicLab.Hive.Client.Core.ClientConsoleService.ConnectionContainer container) {
    base.Channel.SetConnection(container);
  }

  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  public System.IAsyncResult BeginSetConnection(HeuristicLab.Hive.Client.Core.ClientConsoleService.ConnectionContainer container, System.AsyncCallback callback, object asyncState) {
    return base.Channel.BeginSetConnection(container, callback, asyncState);
  }

  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  public void EndSetConnection(System.IAsyncResult result) {
    base.Channel.EndSetConnection(result);
  }

  private System.IAsyncResult OnBeginSetConnection(object[] inValues, System.AsyncCallback callback, object asyncState) {
    HeuristicLab.Hive.Client.Core.ClientConsoleService.ConnectionContainer container = ((HeuristicLab.Hive.Client.Core.ClientConsoleService.ConnectionContainer)(inValues[0]));
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

  public void SetConnectionAsync(HeuristicLab.Hive.Client.Core.ClientConsoleService.ConnectionContainer container) {
    this.SetConnectionAsync(container, null);
  }

  public void SetConnectionAsync(HeuristicLab.Hive.Client.Core.ClientConsoleService.ConnectionContainer container, object userState) {
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

  public void Disconnect() {
    base.Channel.Disconnect();
  }

  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  public System.IAsyncResult BeginDisconnect(System.AsyncCallback callback, object asyncState) {
    return base.Channel.BeginDisconnect(callback, asyncState);
  }

  [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
  public void EndDisconnect(System.IAsyncResult result) {
    base.Channel.EndDisconnect(result);
  }

  private System.IAsyncResult OnBeginDisconnect(object[] inValues, System.AsyncCallback callback, object asyncState) {
    return this.BeginDisconnect(callback, asyncState);
  }

  private object[] OnEndDisconnect(System.IAsyncResult result) {
    this.EndDisconnect(result);
    return null;
  }

  private void OnDisconnectCompleted(object state) {
    if ((this.DisconnectCompleted != null)) {
      InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
      this.DisconnectCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
    }
  }

  public void DisconnectAsync() {
    this.DisconnectAsync(null);
  }

  public void DisconnectAsync(object userState) {
    if ((this.onBeginDisconnectDelegate == null)) {
      this.onBeginDisconnectDelegate = new BeginOperationDelegate(this.OnBeginDisconnect);
    }
    if ((this.onEndDisconnectDelegate == null)) {
      this.onEndDisconnectDelegate = new EndOperationDelegate(this.OnEndDisconnect);
    }
    if ((this.onDisconnectCompletedDelegate == null)) {
      this.onDisconnectCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnDisconnectCompleted);
    }
    base.InvokeAsync(this.onBeginDisconnectDelegate, null, this.onEndDisconnectDelegate, this.onDisconnectCompletedDelegate, userState);
  }

}
