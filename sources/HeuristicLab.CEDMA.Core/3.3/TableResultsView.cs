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
using HeuristicLab.CEDMA.DB.Interfaces;

namespace HeuristicLab.CEDMA.Core {

  public partial class TableResultsView : ViewBase {
    private Results Results {
      get { return (Results)Item; }
      set { Item = value; }
    }
    private bool suppressEvents;
    public TableResultsView(Results results) {
      suppressEvents = false;
      InitializeComponent();
      Results = results;
      results.Changed += new EventHandler(results_Changed);
    }

    void results_Changed(object sender, EventArgs e) {
      if (suppressEvents) return;
      UpdateControls();
    }

    protected override void UpdateControls() {
      suppressEvents = true;
      dataGridView.Rows.Clear();
      dataGridView.Columns.Clear();
      List<string> attributeNames = Results.SelectModelAttributes().ToList();
      foreach (var attribute in attributeNames) {
        dataGridView.Columns.Add(attribute, attribute);
      }

      var entries = Results.GetEntries();
      foreach (var entry in entries) {
        int rowIndex = dataGridView.Rows.Add();
        dataGridView.Rows[rowIndex].Tag = entry;
        foreach (string attrName in attributeNames) {
          dataGridView.Rows[rowIndex].Cells[attrName].Value = entry.Get(attrName);
        }
        if (entry.Selected) dataGridView.Rows[rowIndex].Selected = true;
      }
      dataGridView.Update();
      suppressEvents = false;
    }

    private void dataGridView_SelectionChanged(object sender, EventArgs e) {
      if (suppressEvents) return;
      foreach (DataGridViewRow row in dataGridView.Rows) {
        ((ResultsEntry)row.Tag).Selected = row.Selected;
      }
      suppressEvents = true;
      Results.FireChanged();
      suppressEvents = false;
    }

    private void dataGridView_MouseDoubleClick(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Left && e.Clicks == 2) {
        DataGridView.HitTestInfo hitInfo = dataGridView.HitTest(e.X, e.Y);
        ResultsEntry entry = (ResultsEntry)dataGridView.Rows[hitInfo.RowIndex].Tag;
        string serializedData =  (string)entry.Get(Ontology.PredicateSerializedData.Uri.Replace(Ontology.CedmaNameSpace, ""));
        var model = (IItem)PersistenceManager.RestoreFromGZip(Convert.FromBase64String(serializedData));
        PluginManager.ControlManager.ShowControl(model.CreateView());
      }
    }
  }

  public class TablesResultsViewFactory : IResultsViewFactory {
    #region IResultsViewFactory Members

    public string Name {
      get { return "Table"; }
    }

    public IControl CreateView(Results results) {
      return new TableResultsView(results);
    }

    #endregion
  }
}
