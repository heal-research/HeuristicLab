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
  
  public abstract partial class JsonItemMultiValueControl<T> : UserControl {
    protected IJsonItemVM VM { get; set; }
    protected NumericRangeControl NumericRangeControl { get; set; }
    private int Rows { get; set; }
    private int Columns { get; set; }

    private T[][] Matrix { get; set; }
    
    protected IEnumerable<string> RowNames {
      get {
        if(VM is IMatrixJsonItemVM mVM)
          return mVM.RowNames;
        return null;
      }
      set {
        if (VM is IMatrixJsonItemVM mVM)
          mVM.RowNames = value;
      }
    }
    protected IEnumerable<string> ColumnNames {
      get {
        if (VM is IMatrixJsonItemVM mVM)
          return mVM.ColumnNames;
        return null;
      }
      set {
        if (VM is IMatrixJsonItemVM mVM)
          mVM.ColumnNames = value;
      }
    }
    
    public JsonItemMultiValueControl(IMatrixJsonItemVM vm, T[][] matrix) {
      InitializeComponent();
      VM = vm;
      checkBoxRows.DataBindings.Add("Checked", vm, nameof(IMatrixJsonItemVM.RowsResizable));
      checkBoxColumns.DataBindings.Add("Checked", vm, nameof(IMatrixJsonItemVM.ColumnsResizable));

      int cols = matrix.Length;
      int rows = matrix.Max(x => x.Length);

      Matrix = matrix;
      Columns = cols;
      Rows = rows;
      RefreshMatrix();
      InitSizeConfiguration(rows, cols);
      dataGridView.CellEndEdit += DataGridView_CellEndEdit;
      InitRangeBinding();
    }
    
    public JsonItemMultiValueControl(IArrayJsonItemVM vm, T[] array) {
      InitializeComponent();
      VM = vm;
      checkBoxRows.DataBindings.Add("Checked", vm, nameof(IArrayJsonItemVM.Resizable));

      int length = array.Length;

      Matrix = new T[1][];
      Matrix[0] = array;
      Columns = 1;
      Rows = length;
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
      
      for (int c = 0; c < Columns; ++c) {
        string name = $"Column {c}";
        if (RowNames != null && c < RowNames.Count())
          name = RowNames.ElementAt(c);
        dataGridView.Columns.Add(name, name);
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
            string name = $"Row {r}";
            if (ColumnNames != null && r < ColumnNames.Count())
              name = ColumnNames.ElementAt(r);
            dataGridView.Rows[r].HeaderCell.Value = name;
          }
        }
      }
      dataGridView.RowHeadersWidth = 100;
    }

    private void InitRangeBinding() {
      NumericRangeControl = numericRangeControl1;
      NumericRangeControl.TBMinRange.DataBindings.Add("Text", VM, nameof(RangedValueBaseVM<int, IntJsonItem>.MinRange));
      NumericRangeControl.TBMaxRange.DataBindings.Add("Text", VM, nameof(RangedValueBaseVM<int, IntJsonItem>.MaxRange));
      NumericRangeControl.EnableMinRange.DataBindings.Add("Checked", VM, nameof(RangedValueBaseVM<int, IntJsonItem>.EnableMinRange),
        false, DataSourceUpdateMode.OnPropertyChanged);
      NumericRangeControl.EnableMaxRange.DataBindings.Add("Checked", VM, nameof(RangedValueBaseVM<int, IntJsonItem>.EnableMaxRange),
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
      if(!textBoxRows.ReadOnly && int.TryParse(textBoxRows.Text, out int r) && Rows != r) {
        Rows = r;
        RefreshMatrix();
      }
    }

    private void textBoxColumns_TextChanged(object sender, EventArgs e) {
      if (!textBoxColumns.ReadOnly && int.TryParse(textBoxColumns.Text, out int c) && Columns != c) {
        Columns = c;
        RefreshMatrix();
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
