using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.Manufacture {
  public class MultiCheckedOperatorTransformer : ParameterizedItemTransformer {
    public override Component ExtractData(IItem value) {
      Component data = base.ExtractData(value);

      data.Default = value.GetType().Name;
      data.Operators = new List<Component>();
      /*
      if (data.ParameterizedItems == null)
        data.ParameterizedItems = new List<Component>();
      data.ParameterizedItems.Add(data);
      */
      dynamic val = value.Cast<dynamic>();
      foreach (var op in val.Operators) {
        data.Operators.Add(new Component() {
          Name = op.Name,
          Default = val.Operators.ItemChecked(op),
          Range = new object[] { false, true },
          Path = data.Path + "." + op.Name
        });
      }
      return data;
    }

    public override void InjectData(IItem item, Component data) {
      base.InjectData(item, data);

      dynamic val = item.Cast<dynamic>();
      foreach (var op in val.Operators) {
        val.Operators.SetItemCheckedState(op, GetOperatorState(op.Name, data));
      }
    }

    private bool GetOperatorState(string name, Component data) {
      foreach(var op in data.Operators) {
        if (op.Name == name) return op.Default.Cast<bool>();
      }
      return false;
    }
  }
}
