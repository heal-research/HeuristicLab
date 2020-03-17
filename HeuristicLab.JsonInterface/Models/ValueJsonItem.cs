using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public abstract class ValueJsonItem : JsonItem, IValueJsonItem {
    public object Value { get; set; }

    public override void SetJObject(JObject jObject) {
      Value = jObject[nameof(IValueJsonItem.Value)]?.ToObject<object>();
    }

  }

  public abstract class ValueJsonItem<T> : ValueJsonItem, IValueJsonItem<T> {
    public new T Value {
      get => ConvertObject(base.Value);
      set => base.Value = value;
    }

    private T ConvertObject(object obj) {
      if (obj is IConvertible)
        return (T)Convert.ChangeType(obj, typeof(T));

      if (obj is JToken token)
        return token.ToObject<T>();

      return (T)obj;
    }

    public override void SetJObject(JObject jObject) {
      if(jObject[nameof(IValueJsonItem<T>.Value)] != null)
        Value = jObject[nameof(IValueJsonItem<T>.Value)].ToObject<T>();
    }
  }
}
