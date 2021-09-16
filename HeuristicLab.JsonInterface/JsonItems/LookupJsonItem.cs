using Newtonsoft.Json.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  [StorableType("DE16C5EB-998D-4D34-BD98-7BF0E02A56BC")]
  public class LookupJsonItem : JsonItem, ILookupJsonItem {
    public virtual string ActualName { get; set; }

    public override void SetJObject(JObject jObject) {
      base.SetJObject(jObject);
      ActualName = jObject[nameof(ILookupJsonItem.ActualName)]?.ToString();
    }

    protected override ValidationResult Validate() => ValidationResult.Successful();

    public LookupJsonItem() { }

    [StorableConstructor]
    protected LookupJsonItem(StorableConstructorFlag _) : base(_) { }
  }
}
