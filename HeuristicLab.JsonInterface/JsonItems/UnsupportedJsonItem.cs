using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace HeuristicLab.JsonInterface {
  public class UnsupportedJsonItem : JsonItem {

    [JsonIgnore]
    public override IEnumerable<IJsonItem> Children {
      get => Enumerable.Empty<IJsonItem>();
      protected set => throw new NotSupportedException();
    }

    protected override ValidationResult Validate() => ValidationResult.Successful();
  }
}
