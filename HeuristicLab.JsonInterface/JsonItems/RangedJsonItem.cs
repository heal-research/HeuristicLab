using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  [StorableType("856DE916-D830-43A8-9B40-37E512A177DB")]
  public abstract class RangedJsonItem<T> : IntervalRestrictedJsonItem<T>, IRangedJsonItem<T>
      where T : IComparable {
    public T MinValue { get; set; }

    public T MaxValue { get; set; }

    protected override ValidationResult Validate() {
      IList<string> errors = new List<string>();
      bool successMin = (Minimum.CompareTo(MinValue) <= 0 && Maximum.CompareTo(MinValue) >= 0);
      bool successMax = (Minimum.CompareTo(MaxValue) <= 0 && Maximum.CompareTo(MaxValue) >= 0);
      if (!successMin) errors.Add($"[{Path}]: Value {MinValue} is not between {Minimum} and {Maximum}.");
      if (!successMax) errors.Add($"[{Path}]: Value {MaxValue} is not between {Minimum} and {Maximum}.");
      return new ValidationResult((successMin && successMax), errors);

    }

    public override void SetJObject(JObject jObject) {
      var minValueProp = jObject[nameof(IRangedJsonItem<T>.MinValue)];
      if (minValueProp != null) MinValue = minValueProp.ToObject<T>();

      var maxValueProp = jObject[nameof(IRangedJsonItem<T>.MaxValue)];
      if (maxValueProp != null) MaxValue = maxValueProp.ToObject<T>();

      base.SetJObject(jObject);
    }

    public RangedJsonItem() { }

    [StorableConstructor]
    protected RangedJsonItem(StorableConstructorFlag _) : base(_) { }

  }
}
