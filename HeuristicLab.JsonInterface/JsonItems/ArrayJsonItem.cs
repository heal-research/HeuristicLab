using Newtonsoft.Json.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  [StorableType("1A448B57-6587-4EB4-9A0A-B05C376894C6")]
  public abstract class ArrayJsonItem<T> : ValueJsonItem<T[]>, IArrayJsonItem {
    public virtual bool Resizable { get; set; }
    public override void SetJObject(JObject jObject) {
      base.SetJObject(jObject);
      Resizable = (jObject[nameof(IArrayJsonItem.Resizable)]?.ToObject<bool>()).GetValueOrDefault();
    }

    public ArrayJsonItem() { }

    [StorableConstructor]
    protected ArrayJsonItem(StorableConstructorFlag _) : base(_) { }

  }
}
