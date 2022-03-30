using Newtonsoft.Json.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  public class DoubleJsonItem : IntervalRestrictedValueJsonItem<double> {
    protected internal override JObject ToJObject() {
      var obj = base.ToJObject();

      if (Minimum == double.MinValue)
        obj.Property("Minimum").Remove();

      if (Maximum == double.MaxValue)
        obj.Property("Maximum").Remove();

      return obj;
    }

    protected internal override void FromJObject(JObject jObject) {
      Minimum = double.MinValue;
      Maximum = double.MaxValue;
      base.FromJObject(jObject);
    }

    public DoubleJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }
  }

  public class DoubleArrayJsonItem : IntervalRestrictedArrayJsonItem<double> {
    protected internal override JObject ToJObject() {
      var obj = base.ToJObject();

      if (Minimum.CompareTo(double.MinValue) == 0)
        obj.Property("Minimum").Remove();

      if (Maximum.CompareTo(double.MaxValue) == 0)
        obj.Property("Maximum").Remove();

      return obj;
    }

    protected internal override void FromJObject(JObject jObject) {
      Minimum = double.MinValue;
      Maximum = double.MaxValue;
      base.FromJObject(jObject);
    }

    public DoubleArrayJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }
  }

  public class DoubleRangeJsonItem : RangeJsonItem<double> {
    protected internal override JObject ToJObject() {
      var obj = base.ToJObject();

      if (Minimum.CompareTo(double.MinValue) == 0)
        obj.Property("Minimum").Remove();

      if (Maximum.CompareTo(double.MaxValue) == 0)
        obj.Property("Maximum").Remove();

      return obj;
    }

    protected internal override void FromJObject(JObject jObject) {
      Minimum = double.MinValue;
      Maximum = double.MaxValue;
      base.FromJObject(jObject);
    }

    public DoubleRangeJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }
  }

  public class DoubleMatrixJsonItem : IntervalRestrictedMatrixJsonItem<double> {
    protected internal override JObject ToJObject() {
      var obj = base.ToJObject();

      if (Minimum.CompareTo(double.MinValue) == 0)
        obj.Property("Minimum").Remove();

      if (Maximum.CompareTo(double.MaxValue) == 0)
        obj.Property("Maximum").Remove();

      return obj;
    }

    protected internal override void FromJObject(JObject jObject) {
      Minimum = double.MinValue;
      Maximum = double.MaxValue;
      base.FromJObject(jObject);
    }

    public DoubleMatrixJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }
  }
}
