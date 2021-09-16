using System;
using Newtonsoft.Json.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  [StorableType("4D76601B-9627-4ABD-A24B-49179D59AB11")]
  public abstract class IntervalRestrictedJsonItem<T> : JsonItem, IIntervalRestrictedJsonItem<T>
      where T : IComparable {
    public T Minimum { get; set; }
    public T Maximum { get; set; }

    public override void SetJObject(JObject jObject) {
      base.SetJObject(jObject);

      var minProp = jObject[nameof(IIntervalRestrictedJsonItem<T>.Minimum)];
      if (minProp != null) Minimum = minProp.ToObject<T>();

      var maxProp = jObject[nameof(IIntervalRestrictedJsonItem<T>.Maximum)];
      if (maxProp != null) Maximum = maxProp.ToObject<T>();
    }

    public IntervalRestrictedJsonItem() { }

    [StorableConstructor]
    protected IntervalRestrictedJsonItem(StorableConstructorFlag _) : base(_) { }
  }
}
