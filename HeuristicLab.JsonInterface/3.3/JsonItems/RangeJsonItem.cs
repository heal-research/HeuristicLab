using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  public abstract class RangeJsonItem<T> : IntervalRestrictedJsonItem<T>
      where T : IComparable {
    public T Start { get; set; }

    public T End { get; set; }

    protected override ValidationResult Validate() {
      IList<string> errors = new List<string>();
      bool successMin = (Minimum.CompareTo(Start) <= 0 && Maximum.CompareTo(Start) >= 0);
      bool successMax = (Minimum.CompareTo(End) <= 0 && Maximum.CompareTo(End) >= 0);
      if (!successMin) errors.Add($"[{Path}]: Value {Start} is not between {Minimum} and {Maximum}.");
      if (!successMax) errors.Add($"[{Path}]: Value {End} is not between {Minimum} and {Maximum}.");
      return new ValidationResult((successMin && successMax), errors);
    }

    protected internal override void FromJObject(JObject jObject) {
      var minValueProp = jObject[nameof(IRangedJsonItem<T>.Start)];
      if (minValueProp != null) Start = minValueProp.ToObject<T>();

      var maxValueProp = jObject[nameof(IRangedJsonItem<T>.End)];
      if (maxValueProp != null) End = maxValueProp.ToObject<T>();

      base.FromJObject(jObject);
    }

    public RangeJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }

  }
}
