using System;
using Newtonsoft.Json.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  [StorableType("AA00656C-56D5-4F4E-9843-BA15636FC399")]
  public class ResultJsonItem : JsonItem, IResultJsonItem {
    public string ResultFormatterType { get; set; }

    public Type ValueType { get; set; }

    protected override ValidationResult Validate() => ValidationResult.Successful();

    public override void SetJObject(JObject jObject) {
      base.SetJObject(jObject);
      ResultFormatterType = (jObject[nameof(IResultJsonItem.ResultFormatterType)]?.ToObject<string>());
    }

    public ResultJsonItem() { }

    [StorableConstructor]
    protected ResultJsonItem(StorableConstructorFlag _) : base(_) { }

  }
}
