using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.CEDMA.Core;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.CEDMA.Charting;

namespace HeuristicLab.CEDMA.Core {

  public partial class TableResultsView : ViewBase {
    private VisualMatrix VisualMatrix {
      get { return (VisualMatrix)Item; }
      set { Item = value; }
    }
    private bool suppressEvents;
    public TableResultsView(VisualMatrix visualMatrix) {
      suppressEvents = false;
      InitializeComponent();
      VisualMatrix = visualMatrix;
      VisualMatrix.Changed += new EventHandler(VisualMatrixChanged);
    }

    private void VisualMatrixChanged(object sender, EventArgs e) {
      if (suppressEvents) return;
      UpdateControls();
    }

    protected override void UpdateControls() {
      suppressEvents = true;
      dataGridView.Rows.Clear();
      dataGridView.Columns.Clear();
      foreach (var attribute in VisualMatrix.Attributes) {
        dataGridView.Columns.Add(attribute, attribute);
      }

      foreach (var row in VisualMatrix.Rows) {
        if (row.Visible) {
          int rowIndex = dataGridView.Rows.Add();
          dataGridView.Rows[rowIndex].Tag = row;
          foreach (string attrName in VisualMatrix.Attributes) {
            dataGridView.Rows[rowIndex].Cells[attrName].Value = row.Get(attrName);
          }
          if (row.Selected) dataGridView.Rows[rowIndex].Selected = true;
        }
      }
      dataGridView.Update();
      suppressEvents = false;
    }

    private void dataGridView_SelectionChanged(object sender, EventArgs e) {
      if (suppressEvents) return;
      foreach (DataGridViewRow row in dataGridView.Rows) {
        ((VisualMatrixRow)row.Tag).Selected = row.Selected;
      }
      suppressEvents = true;
      VisualMatrix.FireChanged();
      suppressEvents = false;
    }

    private void dataGridView_MouseDoubleClick(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Left && e.Clicks == 2) {
        DataGridView.HitTestInfo hitInfo = dataGridView.HitTest(e.X, e.Y);
        VisualMatrixRow entry = (VisualMatrixRow)dataGridView.Rows[hitInfo.RowIndex].Tag;        
        var model = (IItem)PersistenceManager.RestoreFromGZip((byte[])entry.Get("PersistedData"));
        PluginManager.ControlManager.ShowControl(model.CreateView());
      }
    }
  }

  public class TablesResultsViewFactory : IResultsViewFactory {
    #region IResultsViewFactory Members

    public string Name {
      get { return "Table"; }
    }

    public IControl CreateView(VisualMatrix matrix) {
      return new TableResultsView(matrix);
    }

    #endregion
  }
}
