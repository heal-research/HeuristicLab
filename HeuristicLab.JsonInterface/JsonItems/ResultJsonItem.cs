using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public class ResultJsonItem : JsonItem, IResultJsonItem {
    public string ResultFormatterType { get; set; }

    public Type ValueType { get; set; }

    protected override ValidationResult Validate() => ValidationResult.Successful();

    public override void SetJObject(JObject jObject) {
      base.SetJObject(jObject);
      ResultFormatterType = (jObject[nameof(IResultJsonItem.ResultFormatterType)]?.ToObject<string>());
    }

  }
}
