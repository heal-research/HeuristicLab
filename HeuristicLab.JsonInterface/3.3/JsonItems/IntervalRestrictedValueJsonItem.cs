using System;
using Newtonsoft.Json.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  public abstract class IntervalRestrictedValueJsonItem<T> : ValueJsonItem<T>
      where T : IComparable {
    public T Minimum { get; set; }
    public T Maximum { get; set; }

    protected override ValidationResult Validate() {
      bool res = Minimum.CompareTo(Value) <= 0 && Maximum.CompareTo(Value) >= 0;
      if (res) return ValidationResult.Successful();
      else return ValidationResult.Faulty($"[{Path}]: Value {Value} is not between {Minimum} and {Maximum}.");
    }

    protected internal override void FromJObject(JObject jObject) {
      base.FromJObject(jObject);

      var minProp = jObject[nameof(IIntervalRestrictedJsonItem<T>.Minimum)];
      if (minProp != null) Minimum = minProp.ToObject<T>();

      var maxProp = jObject[nameof(IIntervalRestrictedJsonItem<T>.Maximum)];
      if (maxProp != null) Maximum = maxProp.ToObject<T>();
    }

    public IntervalRestrictedValueJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }
  }
}
