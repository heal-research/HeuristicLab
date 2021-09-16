using Newtonsoft.Json.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  [StorableType("B9B3843E-90B9-453F-AB7F-B3A4B8DF6809")]
  public class DoubleJsonItem : IntervalRestrictedValueJsonItem<double> {
    public override JObject GenerateJObject() {
      var obj = base.GenerateJObject();

      if (Minimum.CompareTo(double.MinValue) == 0)
        obj.Property("Minimum").Remove();

      if (Maximum.CompareTo(double.MaxValue) == 0)
        obj.Property("Maximum").Remove();

      return obj;
    }

    public override void SetJObject(JObject jObject) {
      Minimum = double.MinValue;
      Maximum = double.MaxValue;
      base.SetJObject(jObject);
    }

    public DoubleJsonItem() { }

    [StorableConstructor]
    protected DoubleJsonItem(StorableConstructorFlag _) : base(_) { }
  }

  [StorableType("D7E2BA3D-E9F7-4062-8423-182B517FA7CA")]
  public class DoubleArrayJsonItem : IntervalRestrictedArrayJsonItem<double> {
    public override JObject GenerateJObject() {
      var obj = base.GenerateJObject();

      if (Minimum.CompareTo(double.MinValue) == 0)
        obj.Property("Minimum").Remove();

      if (Maximum.CompareTo(double.MaxValue) == 0)
        obj.Property("Maximum").Remove();

      return obj;
    }

    public override void SetJObject(JObject jObject) {
      Minimum = double.MinValue;
      Maximum = double.MaxValue;
      base.SetJObject(jObject);
    }

    public DoubleArrayJsonItem() { }

    [StorableConstructor]
    protected DoubleArrayJsonItem(StorableConstructorFlag _) : base(_) { }
  }

  [StorableType("4ED29A62-F368-476B-B551-4283E428F7B9")]
  public class DoubleRangeJsonItem : RangedJsonItem<double> {
    public override JObject GenerateJObject() {
      var obj = base.GenerateJObject();

      if (Minimum.CompareTo(double.MinValue) == 0)
        obj.Property("Minimum").Remove();

      if (Maximum.CompareTo(double.MaxValue) == 0)
        obj.Property("Maximum").Remove();

      return obj;
    }

    public override void SetJObject(JObject jObject) {
      Minimum = double.MinValue;
      Maximum = double.MaxValue;
      base.SetJObject(jObject);
    }

    public DoubleRangeJsonItem() { }

    [StorableConstructor]
    protected DoubleRangeJsonItem(StorableConstructorFlag _) : base(_) { }
  }

  [StorableType("39C117DA-6274-4C1F-8CA8-A57B1A7A9C72")]
  public class DoubleMatrixJsonItem : IntervalRestrictedMatrixJsonItem<double> {
    public override JObject GenerateJObject() {
      var obj = base.GenerateJObject();

      if (Minimum.CompareTo(double.MinValue) == 0)
        obj.Property("Minimum").Remove();

      if (Maximum.CompareTo(double.MaxValue) == 0)
        obj.Property("Maximum").Remove();

      return obj;
    }

    public override void SetJObject(JObject jObject) {
      Minimum = double.MinValue;
      Maximum = double.MaxValue;
      base.SetJObject(jObject);
    }

    public DoubleMatrixJsonItem() { }

    [StorableConstructor]
    protected DoubleMatrixJsonItem(StorableConstructorFlag _) : base(_) { }
  }
}
