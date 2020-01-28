using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface {

  public class JsonItem<T> : JsonItem<T,T> { }

  public class JsonItem<V,R> : JsonItem {
    public new V Value {
      get => (V)Convert.ChangeType(base.Value, typeof(V));
      set => base.Value = value;
    }

    public new IEnumerable<R> Range {
      get => base.Range.Cast<R>();
      set => base.Range = value.Cast<object>();
    }
  }
}
