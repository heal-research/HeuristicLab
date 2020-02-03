using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public class StringValueVM : JsonItemVMBase {
    public override Type JsonItemType => typeof(StringJsonItem);
    public override JsonItemBaseControl GetControl() =>
       new JsonItemValidValuesControl(this);

    public string Value {
      get => Item.Value?.ToString();
      set {
        Item.Value = value;
        OnPropertyChange(this, nameof(Value));
      }
    }

    public IEnumerable<string> Range {
      get => Item.Range.Cast<string>();
      set {
        Item.Range = value;
        //check if value is still in range
        if (!Range.Any(x => x == Value)) {
          Value = Range.FirstOrDefault();
          if (Range.Count() == 0)
            //if no elements exists -> deselect item
            base.Selected = false;
          OnPropertyChange(this, nameof(Value));
        }
        
        OnPropertyChange(this, nameof(Range));
      }
    }
  }
}
