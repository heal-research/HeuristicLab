using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  [StorableType("B33EA674-10D1-4184-A564-9EE7F7BFC55B")]
  public class EmptyJsonItem : JsonItem {
    protected override ValidationResult Validate() => ValidationResult.Successful();

    public EmptyJsonItem() { }

    [StorableConstructor]
    protected EmptyJsonItem(StorableConstructorFlag _) : base(_) { }
  }
}
