using Newtonsoft.Json.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  public class IntJsonItem : IntervalRestrictedValueJsonItem<int> {
    protected internal override JObject ToJObject() {
      var obj = base.ToJObject();

      if (Minimum.CompareTo(int.MinValue) == 0)
        obj.Property("Minimum").Remove();

      if (Maximum.CompareTo(int.MaxValue) == 0)
        obj.Property("Maximum").Remove();

      return obj;
    }

    protected internal override void FromJObject(JObject jObject) {
      Minimum = int.MinValue;
      Maximum = int.MaxValue;
      base.FromJObject(jObject);
    }

    public IntJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }
  }

  public class IntArrayJsonItem : IntervalRestrictedArrayJsonItem<int> {
    protected internal override JObject ToJObject() {
      var obj = base.ToJObject();

      if (Minimum.CompareTo(int.MinValue) == 0)
        obj.Property("Minimum").Remove();

      if (Maximum.CompareTo(int.MaxValue) == 0)
        obj.Property("Maximum").Remove();

      return obj;
    }

    protected internal override void FromJObject(JObject jObject) {
      Minimum = int.MinValue;
      Maximum = int.MaxValue;
      base.FromJObject(jObject);
    }

    public IntArrayJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }
  }

  public class IntRangeJsonItem : RangeJsonItem<int> {
    protected internal override JObject ToJObject() {
      var obj = base.ToJObject();

      if (Minimum.CompareTo(int.MinValue) == 0)
        obj.Property("Minimum").Remove();

      if (Maximum.CompareTo(int.MaxValue) == 0)
        obj.Property("Maximum").Remove();

      return obj;
    }

    protected internal override void FromJObject(JObject jObject) {
      Minimum = int.MinValue;
      Maximum = int.MaxValue;
      base.FromJObject(jObject);
    }

    public IntRangeJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }
  }

  public class IntMatrixJsonItem : IntervalRestrictedMatrixJsonItem<int> {
    protected internal override JObject ToJObject() {
      var obj = base.ToJObject();

      if (Minimum.CompareTo(int.MinValue) == 0)
        obj.Property("Minimum").Remove();

      if (Maximum.CompareTo(int.MaxValue) == 0)
        obj.Property("Maximum").Remove();

      return obj;
    }

    protected internal override void FromJObject(JObject jObject) {
      Minimum = int.MinValue;
      Maximum = int.MaxValue;
      base.FromJObject(jObject);
    }

    public IntMatrixJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }
  }
}
