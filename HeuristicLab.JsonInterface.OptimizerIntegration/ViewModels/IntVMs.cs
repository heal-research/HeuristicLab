using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  public class IntArrayValueVM : ArrayValueVM<int, IntArrayJsonItem> {

    protected override int MinTypeValue => int.MinValue;

    protected override int MaxTypeValue => int.MaxValue;

    public override UserControl Control =>
      new JsonItemBaseControl(this, new JsonItemIntArrayValueControl(this));

    public override int[] Value {
      get => Item.Value;
      set {
        Item.Value = value;
        OnPropertyChange(this, nameof(Value));
      }
    }
  }

  public class IntRangeVM : RangeVM<int, IntRangeJsonItem> {

    protected override int MinTypeValue => int.MinValue;

    protected override int MaxTypeValue => int.MaxValue;

    public override UserControl Control =>
      new JsonItemRangeControl(this);
  }

  public class IntValueVM : SingleValueVM<int, IntJsonItem> {

    protected override int MinTypeValue => int.MinValue;
    protected override int MaxTypeValue => int.MaxValue;

    public override UserControl Control =>
      new JsonItemIntValueControl(this);
  }

}
