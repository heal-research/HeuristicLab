using System;
using Newtonsoft.Json.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  [StorableType("29139288-7ABB-4391-926E-5975CF38141E")]
  public abstract class ValueJsonItem : JsonItem, IValueJsonItem {
    public object Value { get; set; }

    public override void SetJObject(JObject jObject) {
      Value = jObject[nameof(IValueJsonItem.Value)]?.ToObject<object>();
    }

    public ValueJsonItem() { }

    [StorableConstructor]
    protected ValueJsonItem(StorableConstructorFlag _) : base(_) {
    }

  }

  [StorableType("86085358-50D6-4486-9265-F6CEA8C8FA19")]
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
      if (jObject[nameof(IValueJsonItem<T>.Value)] != null)
        Value = jObject[nameof(IValueJsonItem<T>.Value)].ToObject<T>();
    }

    public ValueJsonItem() { }

    [StorableConstructor]
    protected ValueJsonItem(StorableConstructorFlag _) : base(_) { }
  }
}
