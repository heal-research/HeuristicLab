using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;


using DefaultValueCb = System.Func<object>;
using RangeCb = System.Func<object[]>;
using ExtractCb = System.Func<object, System.Type, System.Collections.Generic.IEnumerable<HeuristicLab.JsonInterface.JsonItem>>;

namespace HeuristicLab.JsonInterface {

  /* 
   * Converter der mit Hilfe einer Konfiguration einen Typen
   * konvertieren kann. In einer Konfiguration sollten (als String)
   * die Properties/Fields stehen, welche er zur Umwandlung benutzt.
   * Wenn nur ein Property/Field konfiguriert ist, wird ein einfaches
   * JsonItem erstellt. Bei mehreren Properties/Fields werden diese
   * Daten als Parameter eingefügt. 
   * 
   */

  public enum ElementType { None, Property, Field }

  public class ConfigurableConverter : BaseConverter {
    public class Config {
      public string ElementName { get; set; }
      public ElementType ElementType { get; set; }
      public DefaultValueCb DefaultValue { get; set; }
      public RangeCb Range { get; set; }
      public ExtractCb Extract { get; set; }
    }

    private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    
    private IDictionary<string, Config> PConfigs { get; set; }
      = new Dictionary<string, Config>();

    private IDictionary<string, Config> FConfigs { get; set; }
      = new Dictionary<string, Config>();

    private IList<Config> TConfigs { get; set; } = new List<Config>();


    public ConfigurableConverter Primitive(string elementName, ElementType elementType, object defaultValue, params object[] range) =>
      Primitive(elementName, elementType, () => defaultValue, () => range);

    public ConfigurableConverter Primitive(string elementName, ElementType elementType, DefaultValueCb defaultValue) =>
      Primitive(elementName, elementType, defaultValue, () => new object[] { });

    public ConfigurableConverter Primitive(string elementName, ElementType elementType, DefaultValueCb defaultValue, RangeCb range) =>
      Add(elementName, elementType, defaultValue, range, (o,t) => {
        if (!t.IsPrimitive)
          throw new InvalidCastException($"Type {t.Name} is not a primitive type!");
        return new JsonItem[] {
          new JsonItem() {
            Name = elementName,
            Value = o,
            Range = range().Length == 0 ? 
              new object[] { GetMinValue(t), GetMaxValue(t) } : 
              range()
          }
        };
      });

    public ConfigurableConverter IItem(string elementName, DefaultValueCb defaultValue, RangeCb range, ElementType elementType) =>
      Add(elementName, elementType, defaultValue, range, (o,t) => {
        if (!t.IsEqualTo(typeof(IItem)))
          throw new InvalidCastException($"Type {t.Name} is not an IItem!");
        return new JsonItem[] {
          JsonItemConverter.Extract(o as IItem)
        };
      });

    public ConfigurableConverter Enumerable(string elementName, ElementType elementType, ExtractCb extract) =>
      Add(elementName, elementType, null, null, (o,t) => {
        if (!t.IsEqualTo(typeof(IEnumerable)))
          throw new InvalidCastException($"Type {t.Name} is not an IEnumerable!");
        return extract(o,t);
      });

    public ConfigurableConverter PrimitiveEnumerable(string elementName, ElementType elementType, object defaultValue) =>
      PrimitiveEnumerable(elementName, elementType, () => defaultValue);

    public ConfigurableConverter PrimitiveEnumerable(string elementName, ElementType elementType, DefaultValueCb defaultValue) =>
      Add(elementName, elementType, defaultValue, null, (o,t) => {
        if (!t.IsEqualTo(typeof(IEnumerable)))
          throw new InvalidCastException($"Type {t.Name} is not an IEnumerable!");

        return new JsonItem[] {
            new JsonItem() {
              Name = elementName,
              Value = o != null ? o : defaultValue()
            }
          };
      });

    public ConfigurableConverter This(ExtractCb extract) =>
      Add(null, ElementType.None, null, null, extract);

    public ConfigurableConverter Add(string elementName, ElementType elementType, DefaultValueCb defaultValue, RangeCb range, ExtractCb extract)
      => Add(new Config() { ElementName = elementName, 
                            ElementType = elementType,
                            DefaultValue = defaultValue,
                            Range = range,
                            Extract = extract 
                          });

    public ConfigurableConverter Add(Config config) {
      switch(config.ElementType) {
        case ElementType.None:
          TConfigs.Add(config);
          break;
        case ElementType.Field:
          FConfigs.Add(config.ElementName, config);
          break;
        case ElementType.Property:
          PConfigs.Add(config.ElementName, config);
          break;
      }
      return this;
    }

    public override JsonItem ExtractData(IItem value) {
      List<JsonItem> parameters = new List<JsonItem>();
      Type type = value.GetType();

      // Properties
      parameters.AddRange(IterateMemberInfo(value, type.GetProperties(flags), s => { 
        return PConfigs.TryGetValue(s, out Config c) ? c : null; 
      }));

      // Fields
      parameters.AddRange(IterateMemberInfo(value, type.GetFields(flags), s => { 
        return FConfigs.TryGetValue(s, out Config c) ? c : null; 
      }));

      // This Calls
      foreach (var c in TConfigs)
        parameters.AddRange(c.Extract(value, value.GetType()));

      JsonItem item = new JsonItem() {
        Parameters = parameters,
        Path = value.ItemName,
        Type = type.AssemblyQualifiedName
      };

      item.UpdatePath();

      return item;
    }

    public override void InjectData(IItem item, JsonItem data) {
      throw new NotImplementedException();
    }

    private IEnumerable<JsonItem> IterateMemberInfo(IItem item, IEnumerable<MemberInfo> infos, Func<string, Config> selector) {
      List<JsonItem> parameters = new List<JsonItem>();
      foreach(var info in infos) {
        Config config = selector(info.Name);
        if (config != null) {
          object o = GetValue(info, item);
          parameters.AddRange(config.Extract(o, GetType(info)));
        }
      }
      return parameters;
    }

    private JsonItem ConvertPrimitive(string name, object obj) =>
      new JsonItem() {
        Name = name,
        Value = obj
      };

    private object GetValue(MemberInfo info, object obj) {
      switch (info.MemberType) {
        case MemberTypes.Field:
          return (info as FieldInfo).GetValue(obj);
        case MemberTypes.Property:
          PropertyInfo pi = info as PropertyInfo;
          return (pi != null && pi.CanRead) ? pi.GetValue(obj) : null;
        default: return null;
      }
    }

    private Type GetType(MemberInfo info) {
      switch (info.MemberType) {
        case MemberTypes.Field:
          return (info as FieldInfo).FieldType;
        case MemberTypes.Property:
          return (info as PropertyInfo).PropertyType;
        default: return null;
      }
    }
  }
}
