using System.Linq;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {

  public class StringValueVM : ConcreteRestrictedJsonItemVM<StringJsonItem, string, string> {
    protected override string GetDefaultValue() => Range.FirstOrDefault();

    protected override bool RangeContainsValue() => Range.Contains(Value);
  }

  public class StringArrayVM : ConcreteRestrictedJsonItemVM<StringArrayJsonItem, string, string[]> {
    protected override string[] GetDefaultValue() => Range.ToArray();

    protected override bool RangeContainsValue() => Value.All(x => Range.Any(y => x == y));
  }
}
