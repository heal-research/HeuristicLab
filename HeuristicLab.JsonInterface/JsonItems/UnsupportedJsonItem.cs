using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  [StorableType("0E2CA132-AA19-4FC7-B2FD-09572F2E0967")]
  public class UnsupportedJsonItem : JsonItem {

    [JsonIgnore]
    public override IEnumerable<IJsonItem> Children {
      get => Enumerable.Empty<IJsonItem>();
      protected set => throw new NotSupportedException();
    }

    protected override ValidationResult Validate() => ValidationResult.Successful();

    public UnsupportedJsonItem() { }

    [StorableConstructor]
    protected UnsupportedJsonItem(StorableConstructorFlag _) : base(_) { }
  }
}
