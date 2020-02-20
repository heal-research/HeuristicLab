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
    protected override void SaveCellData(double data, int row, int col) {
      DoubleMatrixValueVM vm = VM as DoubleMatrixValueVM;
      vm.SetCellValue(data, row, col);
    }
  }

  public class JsonItemIntArrayValueControl : JsonItemMultiValueControl<int> {
    public JsonItemIntArrayValueControl(IntArrayValueVM vm) : base(vm, vm.Value) { }

    protected override void SaveCellData(int data, int row, int col) {
      IntArrayValueVM vm = VM as IntArrayValueVM;
      vm.SetIndexValue(data, row);
    }
  }

  public class JsonItemDoubleArrayValueControl : JsonItemMultiValueControl<double> {
    public JsonItemDoubleArrayValueControl(DoubleArrayValueVM vm) : base(vm, vm.Value) { }

    protected override void SaveCellData(double data, int row, int col) {
      DoubleArrayValueVM vm = VM as DoubleArrayValueVM;
      vm.SetIndexValue(data, row);
    }
  }
  
  public abstract partial class JsonItemMultiValueControl<T> : JsonItemBaseControl {
    protected NumericRangeControl NumericRangeControl { get; set; }
    private int _rows;
    private int Rows {
      get => _rows;
      set {
        _rows = value;
        RefreshMatrix();
      }
    }

    private int _cols;
    private int Columns {
      get => _cols; 
      set {
        _cols = value;
        RefreshMatrix();
      } 
    }

    private T[][] Matrix { get; set; }

    public JsonItemMultiValueControl(IMatrixJsonItemVM vm, T[][] matrix) : base(vm) {
      InitializeComponent();

      checkBoxRows.DataBindings.Add("Checked", vm, nameof(IMatrixJsonItemVM.RowsResizable));
      checkBoxColumns.DataBindings.Add("Checked", vm, nameof(IMatrixJsonItemVM.ColumnsResizable));

      int rows = matrix.Length;
      int cols = matrix.Max(x => x.Length);

      Matrix = matrix;
      _cols = cols;
      _rows = rows;
      RefreshMatrix();
      InitSizeConfiguration(rows, cols);
      dataGridView.CellEndEdit += DataGridView_CellEndEdit;
      InitRangeBinding();
    }
    
    public JsonItemMultiValueControl(IArrayJsonItemVM vm, T[] array) : base(vm) {
      InitializeComponent();

      checkBoxRows.DataBindings.Add("Checked", vm, nameof(IArrayJsonItemVM.Resizable));

      int length = array.Length;

      Matrix = new T[array.Length][];
      for (int r = 0; r < length; ++r) {
        Matrix[r] = new T[1];
        Matrix[r][0] = array[r];
      }

      _cols = 1;
      _rows = length;
      RefreshMatrix();

      InitSizeConfiguration(length, null);
      dataGridView.CellEndEdit += DataGridView_CellEndEdit;
      InitRangeBinding();
    }
    
    protected abstract void SaveCellData(T data, int row, int col);

    private void InitSizeConfiguration(int? rows, int? columns) {
      if(rows != null) {
        checkBoxRows.CheckedChanged += CheckBoxRows_CheckedChanged;
        textBoxRows.Text = rows.ToString();
      } else {
        checkBoxRows.Enabled = false;
        textBoxRows.ReadOnly = true;
      }

      if (columns != null) {
        checkBoxColumns.CheckedChanged += CheckBoxColumns_CheckedChanged;
        textBoxColumns.Text = columns.ToString();
      } else {
        checkBoxColumns.Enabled = false;
        textBoxColumns.ReadOnly = true;
      }
    }

    private void RefreshMatrix() {
      dataGridView.Rows.Clear();
      dataGridView.Columns.Clear();

      T[][] tmp = Matrix;
      Matrix = new T[Rows][];

      // columns must added first
      for(int c = 0; c < Columns; ++c) {
        dataGridView.Columns.Add($"Column {c}", $"Column {c}");
      }

      for (int r = 0; r < Rows; ++r) {
        T[] newCol = new T[Columns];
        if(r < tmp.Length)
          Array.Copy(tmp[r], 0, newCol, 0, Math.Min(tmp[r].Length, Columns));
        Matrix[r] = newCol;
      }

      if(Rows > 0 && Columns > 0) {
        dataGridView.Rows.Add(Rows);
        for (int c = 0; c < Columns; ++c) {
          for (int r = 0; r < Rows; ++r) {
            //col and row is switched for dataGridView
            dataGridView[c, r].Value = Matrix[r][c];
            dataGridView.Rows[r].HeaderCell.Value = $"Row {r}";
          }
        }
      }
      dataGridView.RowHeadersWidth = 100;
    }

    private void InitRangeBinding() {
      NumericRangeControl = numericRangeControl1;
      NumericRangeControl.TBMinRange.DataBindings.Add("Text", VM, nameof(RangedValueBaseVM.MinRange));
      NumericRangeControl.TBMaxRange.DataBindings.Add("Text", VM, nameof(RangedValueBaseVM.MaxRange));
      NumericRangeControl.EnableMinRange.DataBindings.Add("Checked", VM, nameof(RangedValueBaseVM.EnableMinRange),
        false, DataSourceUpdateMode.OnPropertyChanged);
      NumericRangeControl.EnableMaxRange.DataBindings.Add("Checked", VM, nameof(RangedValueBaseVM.EnableMaxRange),
        false, DataSourceUpdateMode.OnPropertyChanged);
    }

    private void textBoxRows_Validating(object sender, CancelEventArgs e) {
      if (string.IsNullOrWhiteSpace(textBoxRows.Text)) {
        errorProvider.SetError(textBoxRows, "'Rows' must not be empty.");
        e.Cancel = true;
      } else if (!int.TryParse(textBoxRows.Text, out int r)) {
        errorProvider.SetError(textBoxRows, "Value of 'Rows' must be an integer.");
        e.Cancel = true;
      } else if (r == 0) {
        errorProvider.SetError(textBoxRows, "Value of 'Rows' must be an integer larger than 0.");
        e.Cancel = true;
      } else {
        errorProvider.SetError(textBoxRows, null);
      }
    }

    private void textBoxColumns_Validating(object sender, CancelEventArgs e) {
      if (string.IsNullOrWhiteSpace(textBoxColumns.Text)) {
        errorProvider.SetError(textBoxColumns, "'Columns' must not be empty.");
        e.Cancel = true;
      } else if (!int.TryParse(textBoxColumns.Text, out int r)) {
        errorProvider.SetError(textBoxColumns, "Value of 'Columns' must be an integer.");
        e.Cancel = true;
      } else if (r == 0) {
        errorProvider.SetError(textBoxColumns, "Value of 'Columns' must be an integer larger than 0.");
        e.Cancel = true;
      } else {
        errorProvider.SetError(textBoxColumns, null);
      }
    }
    
    private void textBoxRows_TextChanged(object sender, EventArgs e) {
      if(int.TryParse(textBoxRows.Text, out int r)) {
        Rows = r;
      }
    }

    private void textBoxColumns_TextChanged(object sender, EventArgs e) {
      if (int.TryParse(textBoxColumns.Text, out int c)) {
        Columns = c;
      }
    }

    private void CheckBoxColumns_CheckedChanged(object sender, EventArgs e) {
      textBoxColumns.ReadOnly = !checkBoxColumns.Checked;
    }

    private void CheckBoxRows_CheckedChanged(object sender, EventArgs e) {
      textBoxRows.ReadOnly = !checkBoxRows.Checked;
    }

    private void DataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
      object val = dataGridView[e.ColumnIndex, e.RowIndex].Value;
      Matrix[e.RowIndex][e.ColumnIndex] = (T)Convert.ChangeType(val.ToString().Replace(",", "."),
                                            typeof(T),
                                            System.Globalization.CultureInfo.InvariantCulture);
      SaveCellData(Matrix[e.RowIndex][e.ColumnIndex], e.RowIndex, e.ColumnIndex);
    }
  }
}
