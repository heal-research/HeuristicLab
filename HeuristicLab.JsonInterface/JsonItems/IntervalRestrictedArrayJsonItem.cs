using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  [StorableType("97466581-CCF1-45BD-B5F8-87B8660DBE13")]
  public abstract class IntervalRestrictedArrayJsonItem<T> : ArrayJsonItem<T>, IIntervalRestrictedJsonItem<T>
      where T : IComparable {
    public T Minimum { get; set; }
    public T Maximum { get; set; }

    protected override ValidationResult Validate() {
      IList<string> errors = new List<string>();
      bool success = true;
      foreach (var x in Value) {
        if (Minimum.CompareTo(x) > 0 && Maximum.CompareTo(x) < 0) {
          success = false;
          errors.Add($"[{Path}]: Value {x} is not between {Minimum} and {Maximum}.");
        }
      }
      return new ValidationResult(success, errors);
    }

    public override void SetJObject(JObject jObject) {
      base.SetJObject(jObject);

      var minProp = jObject[nameof(IIntervalRestrictedJsonItem<T>.Minimum)];
      if (minProp != null) Minimum = minProp.ToObject<T>();


      var maxProp = jObject[nameof(IIntervalRestrictedJsonItem<T>.Maximum)];
      if (maxProp != null) Maximum = maxProp.ToObject<T>();
    }

    public IntervalRestrictedArrayJsonItem() { }

    [StorableConstructor]
    protected IntervalRestrictedArrayJsonItem(StorableConstructorFlag _) : base(_) { }
  }
}
