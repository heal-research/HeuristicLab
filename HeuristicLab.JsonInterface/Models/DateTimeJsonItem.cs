using System;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public class DateTimeJsonItem : IntervalRestrictedValueJsonItem<DateTime> {
    
    public override JObject GenerateJObject() {
      var obj = base.GenerateJObject();

      if (Minimum.CompareTo(DateTime.MinValue) == 0)
        obj.Property("Minimum").Remove();

      if (Maximum.CompareTo(DateTime.MaxValue) == 0)
        obj.Property("Maximum").Remove();

      return obj;
    }

    public override void SetJObject(JObject jObject) {
      Minimum = DateTime.MinValue;
      Maximum = DateTime.MaxValue;
      base.SetJObject(jObject);
    }
  }
}
