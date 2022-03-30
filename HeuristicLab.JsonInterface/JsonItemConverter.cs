using System;
using System.Collections.Generic;
using System.Linq;
namespace HeuristicLab.JsonInterface {
  /// <summary>
  /// Class for handling json converters.
  /// </summary>
  public class JsonItemConverter {

    #region Properties
    private IDictionary<int, JsonItem> ConvertCache { get; set; }
      = new Dictionary<int, JsonItem>();
    #endregion

    internal void AddToCache(object obj, JsonItem item) => 
      ConvertCache.Add(obj.GetHashCode(), item);

    public JsonItem ConvertToJson(IJsonConvertable convertable) {
      if (ConvertCache.TryGetValue(convertable.GetHashCode(), out JsonItem val))
        return val;
      else
        return convertable.Extract(this);
    }

    public void ConvertFromJson(IJsonConvertable convertable, JsonItem item) {
      var hash = item.GetHashCode();
      if (!ConvertCache.ContainsKey(hash)) {
        ConvertCache.Add(hash, item);
        convertable.Inject(item, this);
      }
    }
  }
}
