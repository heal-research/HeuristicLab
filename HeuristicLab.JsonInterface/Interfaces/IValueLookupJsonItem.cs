using Newtonsoft.Json;

namespace HeuristicLab.JsonInterface {
  public interface IValueLookupJsonItem : ILookupJsonItem {
    /// <summary>
    /// The IJsonItem representation of the actual value of an IValueLookupItem.
    /// </summary>
    [JsonIgnore]
    IJsonItem ActualValue { get; set; }
  }
}
