using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public class DoubleNamedMatrixValueVM : NamedMatrixValueVM<double, DoubleNamedMatrixJsonItem> {
    public override Type JsonItemType => typeof(DoubleNamedMatrixJsonItem);
    public override JsonItemBaseControl Control =>
      new JsonItemBaseControl(this, new JsonItemDoubleNamedMatrixValueControl(this));

    public override double[][] Value {
      get => ((DoubleNamedMatrixJsonItem)Item).Value;
      set {
        ((DoubleNamedMatrixJsonItem)Item).Value = value;
        OnPropertyChange(this, nameof(Value));
      }
    }

    protected override double MinTypeValue => double.MinValue;

    protected override double MaxTypeValue => double.MaxValue;
  }


  public abstract class NamedMatrixValueVM<T, JsonItemType> : MatrixValueVM<T, JsonItemType>, INamedMatrixJsonItemVM
    where JsonItemType : INamedMatrixJsonItem {
    public IEnumerable<string> RowNames {
      get => ((JsonItemType)Item).RowNames;
      set {
        ((JsonItemType)Item).RowNames = value;
        OnPropertyChange(this, nameof(RowNames));
      }
    }
    public IEnumerable<string> ColumnNames {
      get => ((JsonItemType)Item).ColumnNames;
      set {
        ((JsonItemType)Item).ColumnNames = value;
        OnPropertyChange(this, nameof(ColumnNames));
      }
    }
  }
}