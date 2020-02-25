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

  public class JsonItemDoubleNamedMatrixValueControl : JsonItemMultiValueControl<double> {
    protected override IEnumerable<string> RowNames { 
      get => ((DoubleNamedMatrixValueVM)VM).RowNames;
      set => ((DoubleNamedMatrixValueVM)VM).RowNames = value;
    }
    protected override IEnumerable<string> ColumnNames {
      get => ((DoubleNamedMatrixValueVM)VM).ColumnNames;
      set => ((DoubleNamedMatrixValueVM)VM).ColumnNames = value;
    }

    public JsonItemDoubleNamedMatrixValueControl(DoubleNamedMatrixValueVM vm) : base(vm, vm.Value) { }

    protected override void SaveCellData(double data, int row, int col) {
      DoubleNamedMatrixValueVM vm = VM as DoubleNamedMatrixValueVM;
      vm.SetCellValue(data, row, col);
    }
  }

  public class JsonItemDoubleMatrixValueControl : JsonItemMultiValueControl<double> {
    protected override IEnumerable<string> RowNames { get => null; set { } }
    protected override IEnumerable<string> ColumnNames { get => null; set { } }

    public JsonItemDoubleMatrixValueControl(DoubleMatrixValueVM vm) : base(vm, vm.Value) { }

    protected override void SaveCellData(double data, int row, int col) {
      DoubleMatrixValueVM vm = VM as DoubleMatrixValueVM;
      vm.SetCellValue(data, row, col);
    }
  }

  public class JsonItemIntArrayValueControl : JsonItemMultiValueControl<int> {
    protected override IEnumerable<string> RowNames { get => null; set { } }
    protected override IEnumerable<string> ColumnNames { get => null; set { } }

    public JsonItemIntArrayValueControl(IntArrayValueVM vm) : base(vm, vm.Value) { }

    protected override void SaveCellData(int data, int row, int col) {
      IntArrayValueVM vm = VM as IntArrayValueVM;
      vm.SetIndexValue(data, row);
    }
  }

  public class JsonItemDoubleArrayValueControl : JsonItemMultiValueControl<double> {
    protected override IEnumerable<string> RowNames { get => null; set { } }
    protected override IEnumerable<string> ColumnNames { get => null; set { } }

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

    protected abstract IEnumerable<string> RowNames { get; set; }
    protected abstract IEnumerable<string> ColumnNames { get; set; }

    public JsonItemMultiValueControl(IMatrixJsonItemVM vm, T[][] matrix) : base(vm) {
      InitializeComponent();

      checkBoxRows.DataBindings.Add("Checked", vm, nameof(IMatrixJsonItemVM.RowsResizable));
      checkBoxColumns.DataBindings.Add("Checked", vm, nameof(IMatrixJsonItemVM.ColumnsResizable));

      int cols = matrix.Length;
      int rows = matrix.Max(x => x.Length);

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

      Matrix = new T[1][];
      Matrix[0] = array;
      /*
      for (int r = 0; r < length; ++r) {
        Matrix[r] = new T[1];
        Matrix[r][0] = array[r];
      }
      */
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
      Matrix = new T[Columns][];

      // columns must added first
      if(RowNames != null && RowNames.Count() == Columns) {
        foreach(var name in RowNames) {
          dataGridView.Columns.Add(name, name);
        }
      } else {
        for(int c = 0; c < Columns; ++c) {
          dataGridView.Columns.Add($"Column {c}", $"Column {c}");
        }
      }

      for (int c = 0; c < Columns; ++c) {
        T[] newCol = new T[Rows];
        if(c < tmp.Length)
          Array.Copy(tmp[c], 0, newCol, 0, Math.Min(tmp[c].Length, Rows));
        Matrix[c] = newCol;
      }

      if(Rows > 0 && Columns > 0) {
        dataGridView.Rows.Add(Rows);
        for (int c = 0; c < Columns; ++c) {
          for (int r = 0; r < Rows; ++r) {
            //col and row is switched for dataGridView
            dataGridView[c, r].Value = Matrix[c][r];
            if (ColumnNames != null && r < ColumnNames.Count())
              dataGridView.Rows[r].HeaderCell.Value = ColumnNames.ElementAt(r);
            else
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
      Matrix[e.ColumnIndex][e.RowIndex] = (T)Convert.ChangeType(val.ToString().Replace(",", "."),
                                            typeof(T),
                                            System.Globalization.CultureInfo.InvariantCulture);
      SaveCellData(Matrix[e.ColumnIndex][e.RowIndex], e.ColumnIndex, e.RowIndex);
    }
  }
}
