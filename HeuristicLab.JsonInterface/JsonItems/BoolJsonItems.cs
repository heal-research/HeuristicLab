using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  [StorableType("01F11732-16D0-4115-82B7-1BE3A731DC14")]
  public class BoolJsonItem : ValueJsonItem<bool> {
    protected override ValidationResult Validate() => ValidationResult.Successful();

    public BoolJsonItem() { }

    [StorableConstructor]
    protected BoolJsonItem(StorableConstructorFlag _) : base(_) { }
  }

  [StorableType("49995C46-3B9B-49FC-952C-85645D0B565A")]
  public class BoolArrayJsonItem : ArrayJsonItem<bool> {
    protected override ValidationResult Validate() => ValidationResult.Successful();

    public BoolArrayJsonItem() { }

    [StorableConstructor]
    protected BoolArrayJsonItem(StorableConstructorFlag _) : base(_) { }
  }

  [StorableType("5680A943-A5C4-4310-977E-261252810766")]
  public class BoolMatrixJsonItem : MatrixJsonItem<bool> {
    protected override ValidationResult Validate() => ValidationResult.Successful();

    public BoolMatrixJsonItem() { }

    [StorableConstructor]
    protected BoolMatrixJsonItem(StorableConstructorFlag _) : base(_) { }
  }
}
