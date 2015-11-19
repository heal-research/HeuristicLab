using System;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.DataPreprocessing.Implementations {
  [Item("PreprossingDataTable", "A table of data values.")]
  public class PreprocessingDataTable : DataTable {

    public PreprocessingDataTable()
      : base() {
      SelectedRows = new NamedItemCollection<DataRow>();
    }
    public PreprocessingDataTable(string name)
      : base(name) {
      SelectedRows = new NamedItemCollection<DataRow>();
    }

    protected PreprocessingDataTable(PreprocessingDataTable original, Cloner cloner)
      : base(original, cloner) {
      this.selectedRows = cloner.Clone(original.selectedRows);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PreprocessingDataTable(this, cloner);
    }

    private NamedItemCollection<DataRow> selectedRows;
    public NamedItemCollection<DataRow> SelectedRows {
      get { return selectedRows; }
      private set {
        if (selectedRows != null) throw new InvalidOperationException("Rows already set");
        selectedRows = value;
      }
    }

  }
}
