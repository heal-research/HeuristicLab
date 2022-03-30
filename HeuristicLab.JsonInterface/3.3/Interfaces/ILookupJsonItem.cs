namespace HeuristicLab.JsonInterface {
  public interface ILookupJsonItem : IJsonItem {
    /// <summary>
    /// The actual name for lookup items.
    /// </summary>
    string ActualName { get; set; }
  }
}
