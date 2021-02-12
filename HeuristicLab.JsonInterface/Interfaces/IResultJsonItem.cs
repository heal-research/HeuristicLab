using System;
using Newtonsoft.Json;

namespace HeuristicLab.JsonInterface {
  /// <summary>
  /// JsonItem, which indicates a result. For example 'BestQuality'.
  /// Types of this JsonItems are stored in the result section of the template.
  /// </summary>
  public interface IResultJsonItem : IJsonItem {
    /// <summary>
    /// the result formatter type's fullname 
    /// </summary>
    string ResultFormatterType { get; set; }

    /// <summary>
    /// the type of the result value
    /// </summary>
    [JsonIgnore]
    Type ValueType { get; set; }
  }
}
