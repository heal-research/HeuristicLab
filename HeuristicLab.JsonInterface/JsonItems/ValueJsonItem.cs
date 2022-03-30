using System;
using Newtonsoft.Json.Linq;
using HEAL.Attic;
using System.Linq;
using System.Collections.Generic;

namespace HeuristicLab.JsonInterface {
  public abstract class ValueJsonItem : JsonItem {
    public object Value { get; set; }

    protected internal override void FromJObject(JObject jObject) {
      Value = jObject[nameof(IValueJsonItem.Value)]?.ToObject<object>();
    }

    public ValueJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }

  }

  public abstract class ValueJsonItem<T> : ValueJsonItem {
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

    protected internal override void FromJObject(JObject jObject) {
      if (jObject[nameof(IValueJsonItem<T>.Value)] != null)
        Value = jObject[nameof(IValueJsonItem<T>.Value)].ToObject<T>();
    }

    public ValueJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }
  }
}
