using System.Collections.Generic;
using System.Linq;

namespace HeuristicLab.JsonInterface {

  public abstract class ConcreteRestrictedValueJsonItem<T> : ValueJsonItem<T>, IConcreteRestrictedJsonItem<T> {

    public IEnumerable<T> ConcreteRestrictedItems { get; set; }

    protected override ValidationResult Validate() {
      if (ConcreteRestrictedItems == null) return ValidationResult.Successful();
      foreach (var x in ConcreteRestrictedItems)
        if (Value.Equals(x)) return ValidationResult.Successful();
      return ValidationResult.Faulty(
        $"[{Path}]: Value {Value} is not one of the allowed values: " +
        $"'{ string.Join(",", ConcreteRestrictedItems.Select(s => s.ToString()).ToArray()) }'.");
    }
  }
}
