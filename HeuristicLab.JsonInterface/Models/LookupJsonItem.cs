using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public class LookupJsonItem : JsonItem, ILookupJsonItem {
    public virtual string ActualName { get; set; }

    public override void SetFromJObject(JObject jObject) {
      base.SetFromJObject(jObject);
      ActualName = jObject[nameof(ILookupJsonItem.ActualName)]?.ToString();
    }

    protected override bool Validate() => true;
  }
}
