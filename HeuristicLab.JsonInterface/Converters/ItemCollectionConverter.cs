using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface.Converters {
  public class ItemCollectionConverter : BaseConverter {
    public override JsonItem ExtractData(IItem value) {
      IItemCollection<IItem> col = value as IItemCollection<IItem>;
      IList<JsonItem> items = new List<JsonItem>();

      foreach (var x in col)
        items.Add(JsonItemConverter.Extract(x));

      return new JsonItem() {
        Parameters = items
      };
    }

    public override void InjectData(IItem item, JsonItem data) {
      IItemCollection<IItem> col = item as IItemCollection<IItem>;
      foreach (var x in data.Parameters) {
        IItem tmp = Instantiate(col.GetType().GetGenericArguments()[0]);
        JsonItemConverter.Inject(tmp, x);
        col.Add(tmp);
      }
    }
  }
}
