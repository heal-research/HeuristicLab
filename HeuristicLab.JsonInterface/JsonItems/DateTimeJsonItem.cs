using System;
using Newtonsoft.Json.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  [StorableType("C700603D-50E5-448B-B785-9974463A91A8")]
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

    public DateTimeJsonItem() { }

    [StorableConstructor]
    protected DateTimeJsonItem(StorableConstructorFlag _) : base(_) { }
  }
}
