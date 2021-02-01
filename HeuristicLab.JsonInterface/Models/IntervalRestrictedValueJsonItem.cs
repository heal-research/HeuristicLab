using System;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public abstract class IntervalRestrictedValueJsonItem<T> : ValueJsonItem<T>, IIntervalRestrictedJsonItem<T>
    where T : IComparable {
    public T Minimum { get; set; }
    public T Maximum { get; set; }

    protected override ValidationResult Validate() {
      bool res = Minimum.CompareTo(Value) <= 0 && Maximum.CompareTo(Value) >= 0;
      if (res) return ValidationResult.Successful();
      else return ValidationResult.Faulty($"[{Path}]: Value {Value} is not between {Minimum} and {Maximum}.");
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
