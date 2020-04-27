using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public abstract class ArrayJsonItem<T> : ValueJsonItem<T[]>, IArrayJsonItem {
    public virtual bool Resizable { get; set; }
    public override void SetJObject(JObject jObject) {
      base.SetJObject(jObject);
      Resizable = (jObject[nameof(IArrayJsonItem.Resizable)]?.ToObject<bool>()).GetValueOrDefault();
    }
  }
}
