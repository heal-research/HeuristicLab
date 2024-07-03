namespace HeuristicLab.Clients.Common {
  public class WCFClientConfiguration {

    public string Address { get; set; }
    public int MaxBufferPoolSize { get; set; } = int.MaxValue;
    public int MaxReceivedMessageSize { get; set; } = int.MaxValue;
    public int MaxDepth { get; set; } = int.MaxValue;
    public int MaxStringContentLength { get; set; } = int.MaxValue;
    public int MaxArrayLength { get; set; } = int.MaxValue;
    public int MaxBytesPerRead { get; set; } = int.MaxValue;
    public int MaxNameTableCharCount { get; set; } = int.MaxValue;

  }
}
