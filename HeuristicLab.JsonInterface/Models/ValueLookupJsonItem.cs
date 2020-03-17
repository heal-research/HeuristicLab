using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
        var actualValue = ActualValue.GenerateJObject();
        obj.Add(nameof(IValueLookupJsonItem.ActualValue), actualValue);
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
