using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace HeuristicLab.Visualization.DataExport {
  public partial class ExportDialog : Form {
    public ExportDialog() {
      InitializeComponent();

      Type[] types = Assembly.GetExecutingAssembly().GetTypes();
      foreach (Type type in types) {
        if (type.GetInterface(typeof (IExporter).FullName) != null) {
          IExporter exporter = (IExporter)Activator.CreateInstance(type);

          lbExporters.Items.Add(CreateListBoxItem(exporter));
        }
      }
    }

    private static ListBoxItem CreateListBoxItem(IExporter exporter) {
      return new ListBoxItem(exporter);
    }

    private void btnCancel_Click(object sender, EventArgs e) {
      selectedExporter = null;
      Close();
    }

    private void btnSelectExporter_Click(object sender, EventArgs e) {
      selectedExporter = ((ListBoxItem)lbExporters.SelectedItem).Exporter;
      Close();
    }

    private IExporter selectedExporter;

    public IExporter SelectedExporter {
      get { return selectedExporter; }
    }

    private class ListBoxItem {
      private readonly IExporter exporter;

      public ListBoxItem(IExporter exporter) {
        this.exporter = exporter;
      }

      public IExporter Exporter {
        get { return exporter; }
      }

      public override string ToString() {
        return exporter.Name;
      }
    }
  }
}