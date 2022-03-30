using System;
using Newtonsoft.Json.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  public class DateTimeJsonItem : IntervalRestrictedValueJsonItem<DateTime> {

    protected internal override JObject ToJObject() {
      var obj = base.ToJObject();

      if (Minimum.CompareTo(DateTime.MinValue) == 0)
        obj.Property("Minimum").Remove();

      if (Maximum.CompareTo(DateTime.MaxValue) == 0)
        obj.Property("Maximum").Remove();

      return obj;
    }

    protected internal override void FromJObject(JObject jObject) {
      Minimum = DateTime.MinValue;
      Maximum = DateTime.MaxValue;
      base.FromJObject(jObject);
    }

    public DateTimeJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }
  }
}
