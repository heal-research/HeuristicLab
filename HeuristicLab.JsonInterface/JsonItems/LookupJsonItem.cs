using Newtonsoft.Json.Linq;
using HEAL.Attic;
using System.Linq;
using System.Collections.Generic;

namespace HeuristicLab.JsonInterface {
  public class LookupJsonItem : JsonItem {
    public virtual string ActualName { get; set; }

    protected internal override void FromJObject(JObject jObject) {
      base.FromJObject(jObject);
      ActualName = jObject[nameof(ILookupJsonItem.ActualName)]?.ToString();
    }

    protected override ValidationResult Validate() => ValidationResult.Successful();

    public LookupJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }
  }
}
