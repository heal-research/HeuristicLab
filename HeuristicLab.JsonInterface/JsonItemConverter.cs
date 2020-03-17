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

    private IDictionary<int, IJsonItem> InjectCache { get; set; }
      = new Dictionary<int, IJsonItem>();

    private IDictionary<int, IJsonItem> ExtractCache { get; set; }
      = new Dictionary<int, IJsonItem>();

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
        if (CompareTypes(type, x.Key))
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
        if (converter == null) return new UnsupportedJsonItem();
        IJsonItem tmp = GetConverter(item.GetType()).Extract(item, root);
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
    internal JsonItemConverter(IDictionary<Type, IJsonItemConverter> converters) {
      Converters = converters;
    }

    private bool CompareGenericTypes(Type t1, Type t2) =>
      (t1.IsGenericType && t1.GetGenericTypeDefinition() == t2) ||
      (t2.IsGenericType && t2.GetGenericTypeDefinition() == t1);

    private bool CompareTypes(Type t1, Type t2) =>
      t1 == t2 || t1.IsAssignableFrom(t2) ||
      t1.GetInterfaces().Any(
        i => i == t2 || CompareGenericTypes(i, t2)
      ) ||
      CompareGenericTypes(t1, t2);
  }
}
