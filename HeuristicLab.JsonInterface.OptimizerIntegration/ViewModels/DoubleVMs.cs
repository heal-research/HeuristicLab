using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public class DoubleRangeVM : RangeVM<double, DoubleRangeJsonItem> {

    protected override double MinTypeValue => double.MinValue;

    protected override double MaxTypeValue => double.MaxValue;

    public override UserControl Control =>
      new JsonItemRangeControl(this);
  }

  public class DoubleArrayValueVM : ArrayValueVM<double, DoubleArrayJsonItem> {

    protected override double MinTypeValue => double.MinValue;

    protected override double MaxTypeValue => double.MaxValue;

    public override UserControl Control =>
      new JsonItemDoubleArrayValueControl(this);

    public override double[] Value {
      get => Item.Value;
      set {
        Item.Value = value;
        OnPropertyChange(this, nameof(Value));
      }
    }
  }

  public class DoubleMatrixValueVM : MatrixValueVM<double, DoubleMatrixJsonItem> {
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

  public class DoubleValueVM : SingleValueVM<double, DoubleJsonItem> {

    protected override double MinTypeValue => double.MinValue;
    protected override double MaxTypeValue => double.MaxValue;

    public override UserControl Control =>
       new JsonItemDoubleValueControl(this);
  }
}
