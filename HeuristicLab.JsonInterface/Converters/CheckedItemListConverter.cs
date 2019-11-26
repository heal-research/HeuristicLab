using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface.Converters {
  public class CheckedItemListConverter : BaseConverter {
    public override JsonItem ExtractData(IItem value) {
      /*
      ICheckedItemList<IItem> list = value as ICheckedItemList<IItem>;

      IList<JsonItem> items = new List<JsonItem>();

      foreach(var i in list) {
        items.Add(new JsonItem() {
          Name = op.Name,
          Value = val.Operators.ItemChecked(op),
          Range = new object[] { false, true },
          Path = data.Path + "." + op.Name
        });
      }
      list.ItemChecked

      data.Operators.Add(new JsonItem() {
        Name = op.Name,
        Value = val.Operators.ItemChecked(op),
        Range = new object[] { false, true },
        Path = data.Path + "." + op.Name
      });
      */
      throw new NotImplementedException();
    }

    public override void InjectData(IItem item, JsonItem data) {
      throw new NotImplementedException();
    }
  }
}
