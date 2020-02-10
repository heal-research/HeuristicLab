using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  
  public class JsonItemDoubleMatrixValueControl : JsonItemMultiValueControl<double> {

    public JsonItemDoubleMatrixValueControl(DoubleMatrixValueVM vm) : base(vm, vm.Value) { }
    protected override void SaveCellData(object data, int col, int row) {
      DoubleMatrixValueVM vm = VM as DoubleMatrixValueVM;
      vm.SetCellValue(data, col, row);
    }
  }

  public class JsonItemIntArrayValueControl : JsonItemMultiValueControl<int> {
    public JsonItemIntArrayValueControl(IntArrayValueVM vm) : base(vm, vm.Value) { }

    protected override void SaveCellData(object data, int col, int row) {
      IntArrayValueVM vm = VM as IntArrayValueVM;
      vm.SetIndexValue(data, row);
    }
  }

  public class JsonItemDoubleArrayValueControl : JsonItemMultiValueControl<double> {
    public JsonItemDoubleArrayValueControl(DoubleArrayValueVM vm) : base(vm, vm.Value) { }

    protected override void SaveCellData(object data, int col, int row) {
      DoubleArrayValueVM vm = VM as DoubleArrayValueVM;
      vm.SetIndexValue(data, row);
    }
  }
  
  public abstract partial class JsonItemMultiValueControl<T> : JsonItemBaseControl {
    protected NumericRangeControl NumericRangeControl { get; set; }

    public JsonItemMultiValueControl(JsonItemVMBase vm, T[][] matrix) : base(vm) {
      InitializeComponent();
      int rows = matrix.Length;
      int cols = matrix.Max(x => x.Length);

      // columns must added first
      for (int c = 0; c < cols; ++c) {
        dataGridView.Columns.Add($"Column {c}", $"Column {c}");
      }

      dataGridView.Rows.Add(rows);
      for (int c = 0; c < cols; ++c) {
        for(int r = 0; r < rows; ++r) {
          dataGridView[c, r].Value = matrix[r][c];
        }
      }

      dataGridView.CellEndEdit += DataGridView_CellEndEdit;
      InitRangeBinding();
    }

    public JsonItemMultiValueControl(JsonItemVMBase vm, T[] array) : base(vm) {
      InitializeComponent();
      int length = array.Length;

      dataGridView.Columns.Add("Value", "Value");
      dataGridView.Rows.Add(length);
      for(int i = 0; i < length; ++i) {
        dataGridView[0, i].Value = array[i];
      }
      dataGridView.CellEndEdit += DataGridView_CellEndEdit;
      InitRangeBinding();
    }

    private void DataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
      SaveCellData(dataGridView[e.ColumnIndex, e.RowIndex].Value, e.ColumnIndex, e.RowIndex);
    }

    protected abstract void SaveCellData(object data, int col, int row);

    private void InitRangeBinding() {
      NumericRangeControl = numericRangeControl1;
      NumericRangeControl.TBMinRange.DataBindings.Add("Text", VM, nameof(RangedValueBaseVM.MinRange));
      NumericRangeControl.TBMaxRange.DataBindings.Add("Text", VM, nameof(RangedValueBaseVM.MaxRange));
      NumericRangeControl.EnableMinRange.DataBindings.Add("Checked", VM, nameof(RangedValueBaseVM.EnableMinRange),
        false, DataSourceUpdateMode.OnPropertyChanged);
      NumericRangeControl.EnableMaxRange.DataBindings.Add("Checked", VM, nameof(RangedValueBaseVM.EnableMaxRange),
        false, DataSourceUpdateMode.OnPropertyChanged);
    }
  }
}
