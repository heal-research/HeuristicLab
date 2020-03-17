using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public abstract class IntervalRestrictedMatrixJsonItem<T> : MatrixJsonItem<T>, IIntervalRestrictedJsonItem<T>
    where T : IComparable {
    public T Minimum { get; set; }
    public T Maximum { get; set; }
    protected override bool Validate() {
      foreach (var x in Value) {
        foreach(var y in x) {
          if (Minimum.CompareTo(y) > 0 || Maximum.CompareTo(y) < 0)
            return false;
        }
      }
      return true;
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
