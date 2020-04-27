using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public abstract class IntervalRestrictedMatrixJsonItem<T> : MatrixJsonItem<T>, IIntervalRestrictedJsonItem<T>
    where T : IComparable {
    public T Minimum { get; set; }
    public T Maximum { get; set; }

    protected override ValidationResult Validate() {
      IList<string> errors = new List<string>();
      bool success = true;
      foreach (var x in Value) {
        foreach (var y in x) {
          if (Minimum.CompareTo(y) > 0 || Maximum.CompareTo(y) < 0) {
            success = false;
            errors.Add($"[{Path}]: Value {y} is not between {Minimum} and {Maximum}.");
          }
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
  }
}
