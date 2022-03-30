using System;
using Newtonsoft.Json.Linq;
using HEAL.Attic;
using System.Collections.Generic;
using System.Linq;

namespace HeuristicLab.JsonInterface {
  public abstract class IntervalRestrictedJsonItem<T> : JsonItem
      where T : IComparable {
    public T Minimum { get; set; }
    public T Maximum { get; set; }

    protected internal override void FromJObject(JObject jObject) {
      base.FromJObject(jObject);

      var minProp = jObject[nameof(IIntervalRestrictedJsonItem<T>.Minimum)];
      if (minProp != null) Minimum = minProp.ToObject<T>();

      var maxProp = jObject[nameof(IIntervalRestrictedJsonItem<T>.Maximum)];
      if (maxProp != null) Maximum = maxProp.ToObject<T>();
    }

    public IntervalRestrictedJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }
  }
}
