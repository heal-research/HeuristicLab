using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.JsonInterface {
  public class MultiCheckedOperatorConverter : ParameterizedItemConverter {
    public override int Priority => 3;
    public override Type ConvertableType => typeof(ICheckedMultiOperator<>);
    
    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      base.Inject(item, data, root);

      dynamic val = item as dynamic;
      foreach (var op in val.Operators)
        val.Operators.SetItemCheckedState(op, GetOperatorState(op.Name, data));
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      IJsonItem item = base.Extract(value, root);
      dynamic val = value as dynamic;
      foreach (var op in val.Operators) {
        item.AddChilds(new JsonItem() {
          Name = op.Name,
          Value = val.Operators.ItemChecked(op),
          Range = new object[] { false, true }
        });
      }
      return item;
    }


    #region Helper
    private bool GetOperatorState(string name, IJsonItem data) {
      foreach(var op in data.Children) {
        if (op.Name == name && op.Value is bool) return (bool)op.Value;
      }
      return false;
    }
    #endregion
  }
}
