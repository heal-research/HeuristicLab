using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Core;
namespace HeuristicLab.JsonInterface {
  /// <summary>
  /// Class for handling json converters.
  /// </summary>
  public class JsonItemConverter : IJsonItemConverter {

    #region Properties
    private IEnumerable<IJsonItemConverter> Converters { get; set; }
      = Enumerable.Empty<IJsonItemConverter>();

    private IDictionary<int, IJsonItem> InjectCache { get; set; }
      = new Dictionary<int, IJsonItem>();

    private IDictionary<int, IJsonItem> ExtractCache { get; set; }
      = new Dictionary<int, IJsonItem>();

    public int Priority => throw new NotImplementedException();

    public Type ConvertableType => throw new NotImplementedException();

    public bool CanConvertType(Type t) => throw new NotImplementedException();
    #endregion

    /// <summary>
    /// GetConverter a converter for a specific type.
    /// </summary>
    /// <param name="type">The type for which the converter will be selected.</param>
    /// <returns>An IJsonItemConverter object.</returns>
    public IJsonItemConverter GetConverter(Type type) { 
      IList<IJsonItemConverter> possibleConverters = new List<IJsonItemConverter>();
      
      foreach (var x in Converters) {
        if (x.CanConvertType(type))
          possibleConverters.Add(x);
      }

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
    
    public void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      if (item != null && !InjectCache.ContainsKey(item.GetHashCode())) {
        IJsonItemConverter converter = GetConverter(item.GetType());
        if(converter != null) converter.Inject(item, data, root);
        InjectCache.Add(item.GetHashCode(), data);
      }
    }

    public IJsonItem Extract(IItem item, IJsonItemConverter root) {
      int hash = item.GetHashCode();
      if (ExtractCache.TryGetValue(hash, out IJsonItem val))
        return val;
      else {
        IJsonItemConverter converter = GetConverter(item.GetType());
        if (converter == null) 
          return new UnsupportedJsonItem() { Name = $"{item.ItemName} (unsupported)" };
        IJsonItem tmp = converter.Extract(item, root);
        ExtractCache.Add(hash, tmp);
        return tmp;
      }
    }
    
    public static void Inject(IItem item, IJsonItem data) {
      IJsonItemConverter c = JsonItemConverterFactory.Create();
      c.Inject(item, data, c);
    }

    public static IJsonItem Extract(IItem item) {
      IJsonItemConverter c = JsonItemConverterFactory.Create();
      return c.Extract(item, c);
    }

    /// <summary>
    /// Static constructor for default converter configuration.
    /// </summary>
    internal JsonItemConverter(IEnumerable<IJsonItemConverter> converters) {
      Converters = converters;
    }
  }
}
