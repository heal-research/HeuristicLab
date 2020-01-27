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

    public override void Populate(IItem value, IJsonItem item, IJsonItemConverter root) {
      base.Populate(value, item, root);

      dynamic val = value as dynamic;
      foreach (var op in val.Operators) {
        item.AddChilds(new JsonItem() {
          Name = op.Name,
          Value = val.Operators.ItemChecked(op),
          Range = new object[] { false, true }
        });
      }
    }

    public override void InjectData(IItem item, IJsonItem data, IJsonItemConverter root) {
      base.InjectData(item, data, root);

      dynamic val = item as dynamic;
      foreach (var op in val.Operators)
        val.Operators.SetItemCheckedState(op, GetOperatorState(op.Name, data));
    }

    private bool GetOperatorState(string name, IJsonItem data) {
      foreach(var op in data.Children) {
        if (op.Name == name && op.Value is bool) return (bool)op.Value;
      }
      return false;
    }
  }
}
