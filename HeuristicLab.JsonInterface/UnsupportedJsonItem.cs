using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HeuristicLab.JsonInterface {
  public class UnsupportedJsonItem : JsonItem {
    public override string Name {
      get => throw new NotSupportedException();
      set => throw new NotSupportedException();
    }

    public override string Path {
      get => throw new NotSupportedException();
    }

    [JsonIgnore]
    public override IList<JsonItem> Children {
      get => throw new NotSupportedException();
      protected set => throw new NotSupportedException();
    }

    [JsonIgnore]
    public override JsonItem Parent {
      get => throw new NotSupportedException();
      set => throw new NotSupportedException();
    }

    public override object Value {
      get => throw new NotSupportedException();
      set => throw new NotSupportedException();
    }

    public override IEnumerable<object> Range {
      get => throw new NotSupportedException();
      set => throw new NotSupportedException();
    }

    public override string ActualName {
      get => throw new NotSupportedException();
      set => throw new NotSupportedException();
    }
  }
}
