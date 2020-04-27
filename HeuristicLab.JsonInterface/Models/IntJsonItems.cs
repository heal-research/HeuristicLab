using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public class IntJsonItem : IntervalRestrictedValueJsonItem<int> {
    public override JObject GenerateJObject() {
      var obj = base.GenerateJObject();

      if (Minimum.CompareTo(int.MinValue) == 0)
        obj.Property("Minimum").Remove();

      if (Maximum.CompareTo(int.MaxValue) == 0)
        obj.Property("Maximum").Remove();

      return obj;
    }

    public override void SetJObject(JObject jObject) {
      Minimum = int.MinValue;
      Maximum = int.MaxValue;
      base.SetJObject(jObject);
    }
  }
  public class IntArrayJsonItem : IntervalRestrictedArrayJsonItem<int> {
    public override JObject GenerateJObject() {
      var obj = base.GenerateJObject();

      if (Minimum.CompareTo(int.MinValue) == 0)
        obj.Property("Minimum").Remove();

      if (Maximum.CompareTo(int.MaxValue) == 0)
        obj.Property("Maximum").Remove();

      return obj;
    }

    public override void SetJObject(JObject jObject) {
      Minimum = int.MinValue;
      Maximum = int.MaxValue;
      base.SetJObject(jObject);
    }
  }
  public class IntRangeJsonItem : RangedJsonItem<int> {
    public override JObject GenerateJObject() {
      var obj = base.GenerateJObject();

      if (Minimum.CompareTo(int.MinValue) == 0)
        obj.Property("Minimum").Remove();

      if (Maximum.CompareTo(int.MaxValue) == 0)
        obj.Property("Maximum").Remove();

      return obj;
    }

    public override void SetJObject(JObject jObject) {
      Minimum = int.MinValue;
      Maximum = int.MaxValue;
      base.SetJObject(jObject);
    }
  }
  public class IntMatrixJsonItem : IntervalRestrictedMatrixJsonItem<int> {
    public override JObject GenerateJObject() {
      var obj = base.GenerateJObject();

      if (Minimum.CompareTo(int.MinValue) == 0)
        obj.Property("Minimum").Remove();

      if (Maximum.CompareTo(int.MaxValue) == 0)
        obj.Property("Maximum").Remove();

      return obj;
    }

    public override void SetJObject(JObject jObject) {
      Minimum = int.MinValue;
      Maximum = int.MaxValue;
      base.SetJObject(jObject);
    }
  }
}
