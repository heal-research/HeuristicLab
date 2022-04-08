//using System;
//using System.Collections.Generic;
//using System.Windows.Forms;

//namespace HeuristicLab.JsonInterface.OptimizerIntegration {


//  public abstract class MatrixValueVM<T, JsonItemType> : RangedValueBaseVM<T, JsonItemType>, IMatrixJsonItemVM
//    where T : IComparable
//    where JsonItemType : IntervalRestrictedMatrixJsonItem<T> {

//    public override UserControl Control => CompoundControl.Create(base.Control, MatrixJsonItemControl.Create(this));

//    public bool RowsResizable {
//      get => Item.RowsResizable; 
//      set {
//        Item.RowsResizable = value;
//        OnPropertyChange(this, nameof(RowsResizable));
//      }
//    }

//    public bool ColumnsResizable {
//      get => Item.ColumnsResizable;
//      set {
//        Item.ColumnsResizable = value;
//        OnPropertyChange(this, nameof(ColumnsResizable));
//      }
//    }

//    public IEnumerable<string> RowNames {
//      get => Item.RowNames;
//      set {
//        Item.RowNames = value;
//        OnPropertyChange(this, nameof(RowNames));
//      }
//    }
//    public IEnumerable<string> ColumnNames {
//      get => Item.ColumnNames;
//      set {
//        Item.ColumnNames = value;
//        OnPropertyChange(this, nameof(ColumnNames));
//      }
//    }
//  }
//}
