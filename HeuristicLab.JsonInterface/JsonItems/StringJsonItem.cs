using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  [StorableType("81C18291-25B0-4B6C-8707-7B59125B7A93")]
  public class StringJsonItem : ConcreteRestrictedValueJsonItem<string> {
    public StringJsonItem() { }

    [StorableConstructor]
    protected StringJsonItem(StorableConstructorFlag _) : base(_) { }
  }

  [StorableType("6086E5FB-2DC2-4848-9388-0D957B887795")]
  public class StringArrayJsonItem : ConcreteRestrictedArrayJsonItem<string> {
    public StringArrayJsonItem() { }

    [StorableConstructor]
    protected StringArrayJsonItem(StorableConstructorFlag _) : base(_) { }
  }
  /*
  public class StringArrayValueJsonItem : ArrayJsonItem<string> {
    protected override ValidationResult Validate() => ValidationResult.Successful();
  }*/
}
