using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public abstract class ConcreteRestrictedValueJsonItem<T> : ValueJsonItem<T>{

    public IEnumerable<T> ConcreteRestrictedItems { get; set; }

    protected override ValidationResult Validate() {
      if (ConcreteRestrictedItems == null) return ValidationResult.Successful();
      foreach (var x in ConcreteRestrictedItems)
        if (Value.Equals(x)) return ValidationResult.Successful();
      return ValidationResult.Faulty(
        $"[{Path}]: Value {Value} is not one of the allowed values: " +
        $"'{ string.Join(",", ConcreteRestrictedItems.Select(s => s.ToString()).ToArray()) }'.");
    }

    protected internal override void FromJObject(JObject jObject) {
      base.FromJObject(jObject);
      ConcreteRestrictedItems =
        (jObject[nameof(IConcreteRestrictedJsonItem<T>.ConcreteRestrictedItems)]?
        .ToObject<IEnumerable<T>>());
    }

    public ConcreteRestrictedValueJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }

  }
}
