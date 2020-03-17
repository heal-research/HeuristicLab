using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
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
  }
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
  }
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
  }
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
  }
}
