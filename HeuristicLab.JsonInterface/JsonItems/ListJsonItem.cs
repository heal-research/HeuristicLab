using System;
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  [StorableType("43E86513-0F25-4BA8-85B4-4FB50FF62FA8")]
  public class ListJsonItem : ArrayJsonItem<IJsonItem>, IListJsonItem {

    public string TargetTypeGUID { get; set; }

    protected override ValidationResult Validate() {
      var validationResults = Value.Select(x => x.GetValidator().Validate());
      ValidationResult result = new ValidationResult(
        validationResults.Any(x => !x.Success),
        validationResults.SelectMany(x => x.Errors));
      return result;
    }

    public override void SetJObject(JObject jObject) {
      var guidJObj = jObject[nameof(IListJsonItem.TargetTypeGUID)];
      IList<IJsonItem> items = new List<IJsonItem>();
      if (guidJObj != null) {
        TargetTypeGUID = jObject[nameof(IListJsonItem.TargetTypeGUID)].ToString();
        var targetType = Mapper.StaticCache.GetType(new Guid(TargetTypeGUID));
        foreach (JObject obj in jObject[nameof(IValueJsonItem.Value)]) {
          items.Add((IJsonItem)obj.ToObject(targetType));
        }
      }
      Value = items.ToArray();
      Resizable = (jObject[nameof(IArrayJsonItem.Resizable)]?.ToObject<bool>()).GetValueOrDefault();
    }

    public ListJsonItem() { }

    [StorableConstructor]
    protected ListJsonItem(StorableConstructorFlag _) : base(_) { }
  }
}
