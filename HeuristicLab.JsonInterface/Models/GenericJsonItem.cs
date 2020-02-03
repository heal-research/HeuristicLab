using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {

  public class JsonItem<T> : JsonItem<T,T> { }

  public class JsonItem<V,R> : JsonItem {
    public new V Value {
      get {
        if(base.Value is IConvertible)
          return (V)Convert.ChangeType(base.Value, typeof(V));

        if(base.Value is JToken token)
          return token.ToObject<V>();

        return (V)base.Value;
      }
      set => base.Value = value;
    }

    public new IEnumerable<R> Range {
      get => base.Range?.Cast<R>();
      set => base.Range = value.Cast<object>();
    }
  }
}
