using System;
using Newtonsoft.Json.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  [StorableType("5BD32AD9-7CA2-4837-B1C3-D47D0CD83035")]
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

    public IntervalRestrictedValueJsonItem() { }

    [StorableConstructor]
    protected IntervalRestrictedValueJsonItem(StorableConstructorFlag _) : base(_) { }
  }
}
