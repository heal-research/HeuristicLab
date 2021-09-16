using Newtonsoft.Json.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  [StorableType("4FFDAC9B-34F3-4FDB-ABD1-BB2128CE5A49")]
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

    public IntJsonItem() { }

    [StorableConstructor]
    protected IntJsonItem(StorableConstructorFlag _) : base(_) { }
  }

  [StorableType("73E22461-6CA0-4BE1-A5FE-9EF7FFD320AD")]
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

    public IntArrayJsonItem() { }

    [StorableConstructor]
    protected IntArrayJsonItem(StorableConstructorFlag _) : base(_) { }
  }

  [StorableType("A6FBA509-FC37-4E54-9578-5AFB764CFB89")]
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

    public IntRangeJsonItem() { }

    [StorableConstructor]
    protected IntRangeJsonItem(StorableConstructorFlag _) : base(_) { }
  }

  [StorableType("3CFF8D3E-8608-41BA-85A5-42CD4DA45685")]
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

    public IntMatrixJsonItem() { }

    [StorableConstructor]
    protected IntMatrixJsonItem(StorableConstructorFlag _) : base(_) { }
  }
}
