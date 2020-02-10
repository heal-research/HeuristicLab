using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {

  public class DoubleMatrixValueVM : MatrixValueVM<double> {
    public override Type JsonItemType => typeof(DoubleMatrixJsonItem);
    public override JsonItemBaseControl GetControl() => 
      new JsonItemDoubleMatrixValueControl(this);

    public override double[][] Value {
      get => ((DoubleMatrixJsonItem)Item).Value;
      set {
        ((DoubleMatrixJsonItem)Item).Value = value;
        OnPropertyChange(this, nameof(Value));
      }
    }

    protected override double MinTypeValue => double.MinValue;

    protected override double MaxTypeValue => double.MaxValue;
  }

  public abstract class MatrixValueVM<T> : RangedValueBaseVM<T> {

    public MatrixValueVM() {}
    public void SetCellValue(object obj, int col, int row) {
      T[][] tmp = Value;
      
      if (row >= tmp.Length) { // increasing array
        T[][] newArr = new T[row + 1][];
        Array.Copy(tmp, 0, newArr, 0, tmp.Length);
        newArr[row] = new T[0];
        tmp = newArr;
      }
      for(int i = 0; i < tmp.Length; ++i) {
        if(col >= tmp[i].Length) {
          T[] newArr = new T[col + 1];
          Array.Copy(tmp[i], 0, newArr, 0, tmp[i].Length);
          tmp[i] = newArr;
        }
      }
      tmp[row][col] = (T)Convert.ChangeType(obj.ToString().Replace(",","."), 
                                            typeof(T), 
                                            System.Globalization.CultureInfo.InvariantCulture);
      Value = tmp;
    }

    public abstract T[][] Value { get; set; }

  }
}
