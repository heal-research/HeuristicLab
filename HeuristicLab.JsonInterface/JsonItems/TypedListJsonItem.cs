using System;
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public class TypedListJsonItem : ArrayJsonItem<JsonItem> {

    public string TargetTypeGUID { get; set; }

    protected override ValidationResult Validate() {
      var validationResults = Value.Select(x => x.GetValidator().Validate());
      ValidationResult result = new ValidationResult(
        validationResults.Any(x => !x.Success),
        validationResults.SelectMany(x => x.Errors));
      return result;
    }

    protected internal override void FromJObject(JObject jObject) {
      var guidJObj = jObject[nameof(IListJsonItem.TargetTypeGUID)];
      IList<JsonItem> items = new List<JsonItem>();
      if (guidJObj != null) {
        TargetTypeGUID = jObject[nameof(TypedListJsonItem.TargetTypeGUID)].ToString();
        var targetType = Mapper.StaticCache.GetType(new Guid(TargetTypeGUID));
        foreach (JObject obj in jObject[nameof(IValueJsonItem.Value)]) {
          items.Add((JsonItem)obj.ToObject(targetType));
        }
      }
      Value = items.ToArray();
      Resizable = (jObject[nameof(IArrayJsonItem.Resizable)]?.ToObject<bool>()).GetValueOrDefault();
    }

    public TypedListJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }
  }
}
