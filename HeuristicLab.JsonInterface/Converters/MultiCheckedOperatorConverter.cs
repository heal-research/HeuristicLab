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
      foreach (var op in val.Operators) {
        IJsonItem childItem = GetChildItem(op.Name, data);
        if(childItem != null) {
          if(childItem.Value is bool b) {
            val.Operators.SetItemCheckedState(op, b);
          }
          root.Inject((IItem)op, childItem, root);
        }
      }
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) {
      IJsonItem item = base.Extract(value, root);
      dynamic val = value as dynamic;
      foreach (var op in val.Operators) {
        IJsonItem operatorItem = new BoolJsonItem() {
          Name = op.Name,
          Description = op.Description,
          Value = val.Operators.ItemChecked(op),
          Range = new bool[] { false, true }
        };
        IJsonItem c = root.Extract((IItem)op, root);
        if(c != null && c.Children != null)
          operatorItem.AddChildren(c.Children);
        item.AddChildren(operatorItem);
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

    private IJsonItem GetChildItem(string name, IJsonItem parent) {
      foreach(var c in parent.Children) {
        if (c.Name == name) return c;
      }
      return null;
    }
    #endregion
  }
}
