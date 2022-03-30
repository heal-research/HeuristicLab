using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  public class ValueLookupJsonItem : LookupJsonItem {
    public JsonItem ActualValue { get; set; }

    protected override ValidationResult Validate() {
      if (ActualValue == null) return ValidationResult.Successful();
      return ActualValue.GetValidator().Validate();
    }

    protected internal override JObject ToJObject() {
      var obj = base.ToJObject();
      if (ActualValue != null) {
        obj.Add(nameof(IValueLookupJsonItem.ActualValue), ActualValue.Path);
      }
      return obj;
    }

    //public override IEnumerator<JsonItem> Iterate() {
    //  using (var it = base.Iterate()) {
    //    while (it.MoveNext()) {
    //      yield return it.Current;
    //    }
    //  }
    //  if (ActualValue != null) {
    //    using (var it = ActualValue.Iterate()) {
    //      while (it.MoveNext()) {
    //        yield return it.Current;
    //      }
    //    }
    //  }
    //}

    public ValueLookupJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }
  }
}
