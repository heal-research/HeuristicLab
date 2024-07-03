using System;

namespace HeuristicLab.Clients.Common {
  public class WCFClientConfiguration {

    public string Address { get; set; }
    public int MaxBufferSize { get; set; } = int.MaxValue;
    public int MaxBufferPoolSize { get; set; } = int.MaxValue;
    public int MaxReceivedMessageSize { get; set; } = int.MaxValue;
    public int MaxDepth { get; set; } = int.MaxValue;
    public int MaxStringContentLength { get; set; } = int.MaxValue;
    public int MaxArrayLength { get; set; } = int.MaxValue;
    public int MaxBytesPerRead { get; set; } = int.MaxValue;
    public int MaxNameTableCharCount { get; set; } = int.MaxValue;
    public TimeSpan SendTimeout { get; set; } = TimeSpan.FromMinutes(20);
    public TimeSpan ReceiveTimeout { get; set; } = TimeSpan.FromMinutes(20);
    public BindingType Binding { get; set; } = BindingType.BASICHTTP;

    public enum BindingType {
      NETTCP,
      BASICHTTP
    }

  }
}
