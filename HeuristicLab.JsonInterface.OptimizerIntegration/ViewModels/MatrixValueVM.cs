using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {

  public class DoubleMatrixValueVM : MatrixValueVM<double, DoubleMatrixJsonItem> {
    public override Type TargetedJsonItemType => typeof(DoubleMatrixJsonItem);
    public override UserControl Control =>
      new JsonItemDoubleMatrixValueControl(this);

    public override double[][] Value {
      get => Item.Value;
      set {
        Item.Value = value;
        OnPropertyChange(this, nameof(Value));
      }
    }

    protected override double MinTypeValue => double.MinValue;

    protected override double MaxTypeValue => double.MaxValue;
  }

  public abstract class MatrixValueVM<T, JsonItemType> : RangedValueBaseVM<T, JsonItemType>, IMatrixJsonItemVM
    where T : IComparable
    where JsonItemType : class, IMatrixJsonItem, IIntervalRestrictedJsonItem<T> {
    public abstract T[][] Value { get; set; }
    public bool RowsResizable {
      get => Item.RowsResizable; 
      set {
        Item.RowsResizable = value;
        OnPropertyChange(this, nameof(RowsResizable));
      }
    }

    public bool ColumnsResizable {
      get => Item.ColumnsResizable;
      set {
        Item.ColumnsResizable = value;
        OnPropertyChange(this, nameof(ColumnsResizable));
      }
    }

    public IEnumerable<string> RowNames {
      get => Item.RowNames;
      set {
        Item.RowNames = value;
        OnPropertyChange(this, nameof(RowNames));
      }
    }
    public IEnumerable<string> ColumnNames {
      get => Item.ColumnNames;
      set {
        Item.ColumnNames = value;
        OnPropertyChange(this, nameof(ColumnNames));
      }
    }
  }
}
