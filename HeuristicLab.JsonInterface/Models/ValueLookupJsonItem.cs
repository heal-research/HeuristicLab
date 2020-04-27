using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public class ValueLookupJsonItem : LookupJsonItem, IValueLookupJsonItem {
    public IJsonItem ActualValue { get; set; }

    protected override ValidationResult Validate() {
      if (ActualValue == null) return ValidationResult.Successful();
      return ActualValue.GetValidator().Validate();
    }

    public override JObject GenerateJObject() {
      var obj = base.GenerateJObject();
      if(ActualValue != null) {
        obj.Add(nameof(IValueLookupJsonItem.ActualValue), ActualValue.Path);
      }
      return obj;
    }

    public override IEnumerator<IJsonItem> GetEnumerator() {
      using (var it = base.GetEnumerator()) {
        while(it.MoveNext()) {
          yield return it.Current;
        }
      }
      if(ActualValue != null) {
        using (var it = ActualValue.GetEnumerator()) {
          while (it.MoveNext()) {
            yield return it.Current;
          }
        }
      }
    }
  }
}
