using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.PluginInfrastructure;
using HEAL.Attic;
using System.Collections;

namespace HeuristicLab.JsonInterface {
  /// <summary>
  /// Class for handling json converters.
  /// </summary>
  public class JsonItemConverter : IJsonItemConverter {
    
    #region Properties
    private IDictionary<Type, IJsonItemConverter> Converters { get; set; } 
      = new Dictionary<Type, IJsonItemConverter>();

    private IDictionary<int, JsonItem> Cache { get; set; }
      = new Dictionary<int, JsonItem>();

    public int Priority => throw new NotImplementedException();

    public Type ConvertableType => throw new NotImplementedException();
    #endregion

    /// <summary>
    /// GetConverter a converter for a specific type.
    /// </summary>
    /// <param name="type">The type for which the converter will be selected.</param>
    /// <returns>An IJsonItemConverter object.</returns>
    public IJsonItemConverter GetConverter(Type type) { 
      IList<IJsonItemConverter> possibleConverters = new List<IJsonItemConverter>();
      
      foreach (var x in Converters)
        if (type.IsEqualTo(x.Key))
          possibleConverters.Add(x.Value);

      if(possibleConverters.Count > 0) {
        IJsonItemConverter best = possibleConverters.First();
        foreach (var x in possibleConverters) {
          if (x.Priority > best.Priority)
            best = x;
        }
        return best;
      }
      return null;
    }
    
    public void Inject(IItem item, JsonItem data, IJsonItemConverter root) {
      if(!Cache.ContainsKey(item.GetHashCode())) {
        IJsonItemConverter converter = GetConverter(item.GetType());
        if(converter != null) converter.Inject(item, data, root);
      }
    }

    public JsonItem Extract(IItem item, IJsonItemConverter root) {
      int hash = item.GetHashCode();
      if (Cache.TryGetValue(hash, out JsonItem val))
        return val;
      else {
        IJsonItemConverter converter = GetConverter(item.GetType());
        if (converter == null) return new UnsupportedJsonItem();
        JsonItem tmp = GetConverter(item.GetType()).Extract(item, root);
        Cache.Add(hash, tmp);
        return tmp;
      }
    }
    
    public static void Inject(IItem item, JsonItem data) {
      IJsonItemConverter c = JsonItemConverterFactory.Create();
      c.Inject(item, data, c);
    }

    public static JsonItem Extract(IItem item) {
      IJsonItemConverter c = JsonItemConverterFactory.Create();
      return c.Extract(item, c);
    }

    /// <summary>
    /// Static constructor for default converter configuration.
    /// </summary>
    internal JsonItemConverter(IDictionary<Type, IJsonItemConverter> converters) {
      Converters = converters;
    }
  }
}
