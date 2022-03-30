using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HeuristicLab.JsonInterface.JsonItems {
  public class ListJsonItem : JsonItem {

    private IList<JsonItem> items = new List<JsonItem>();
    [JsonIgnore]
    public IEnumerable<JsonItem> Items => items;


    public ListJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }

    protected override ValidationResult Validate() {
      throw new NotImplementedException();
    }

    public void AddItem(JsonItem item) => items.Add(item);

    //public override IEnumerator<JsonItem> Iterate() {
    //  foreach (var item in this)
    //    yield return item;

    //  foreach (var item in items)
    //    foreach (var c in item)
    //      yield return c;
    //}
  }
}
