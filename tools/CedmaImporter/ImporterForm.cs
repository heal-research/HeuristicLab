using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using HeuristicLab.CEDMA.Server;

namespace CedmaImporter {
  public partial class ImporterForm : Form {
    private Problem problem;

    public ImporterForm() {
      InitializeComponent();
      problem = new Problem();
      Control problemView = (UserControl)problem.CreateView();
      problemView.Dock = DockStyle.Fill;
      problemViewPanel.Controls.Add(problemView);      
    }

    private void importButton_Click(object sender, EventArgs e) {
      Importer importer = new Importer(problem);

      OpenFileDialog dialog = new OpenFileDialog();
      DialogResult result = dialog.ShowDialog();
      while (result != DialogResult.Cancel) {
        string fileName = dialog.FileName;
        string directoryName = Path.GetDirectoryName(fileName);
        importer.Import(fileName, directoryName);
      }
    }
  }
}
