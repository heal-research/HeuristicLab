using Newtonsoft.Json.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  public abstract class ArrayJsonItem<T> : ValueJsonItem<T[]> {
    public virtual bool Resizable { get; set; }
    protected internal override void FromJObject(JObject jObject) {
      base.FromJObject(jObject);
      Resizable = (jObject[nameof(IArrayJsonItem.Resizable)]?.ToObject<bool>()).GetValueOrDefault();
    }

    public ArrayJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }

  }
}
