using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public class LookupJsonItem : JsonItem, ILookupJsonItem {
    public virtual string ActualName { get; set; }

    public override void SetJObject(JObject jObject) {
      base.SetJObject(jObject);
      ActualName = jObject[nameof(ILookupJsonItem.ActualName)]?.ToString();
    }

    protected override ValidationResult Validate() => ValidationResult.Successful();
  }
}
