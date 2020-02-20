using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {
  public class ArrayJsonItemBase<T> : JsonItem<T[], T>, IArrayJsonItem {
    public virtual bool Resizable { get; set; }
    public override void SetFromJObject(JObject jObject) {
      base.SetFromJObject(jObject);
      Resizable = (jObject[nameof(IArrayJsonItem.Resizable)]?.ToObject<bool>()).GetValueOrDefault();
    }
  }
}
