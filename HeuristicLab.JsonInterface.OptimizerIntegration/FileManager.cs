using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  internal  static class FileManager {
    private static SaveFileDialog saveFileDialog;
    private static OpenFileDialog openFileDialog;

    public static void ExportJsonTemplate() {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      if (activeView != null) {
        ExportJsonTemplate(activeView);
      }
    }
    public static void ExportJsonTemplate(IContentView view) { 
      // TODO: view to select free params, warning if no results are generated
      IStorableContent content = view.Content as IStorableContent;
      if (!view.Locked && content != null) {
        if (saveFileDialog == null) {
          saveFileDialog = new SaveFileDialog();
          saveFileDialog.Title = "Export .json-Template";
          saveFileDialog.DefaultExt = "json";
          saveFileDialog.Filter = ".json-Template|*.json|All Files|*.*";
          saveFileDialog.FilterIndex = 1;
        }

        INamedItem namedItem = content as INamedItem;
        string suggestedFileName = string.Empty;
        if (!string.IsNullOrEmpty(content.Filename)) suggestedFileName = content.Filename;
        else if (namedItem != null) suggestedFileName = namedItem.Name;
        else suggestedFileName = "Item";

        saveFileDialog.FileName = suggestedFileName + " " + namedItem.GetType().Name;

        if (saveFileDialog.ShowDialog() == DialogResult.OK) {
          IAlgorithm alg = namedItem as IAlgorithm;
          File.WriteAllText(saveFileDialog.FileName, JCGenerator.GenerateTemplate(alg));
        }
      }
    }


    public static void ImportJsonTemplate() {
      if (openFileDialog == null) {
        openFileDialog = new OpenFileDialog();
        openFileDialog.Title = "Import .json-Template";
        openFileDialog.FileName = "Item";
        openFileDialog.Multiselect = false;
        openFileDialog.DefaultExt = "json";
        openFileDialog.Filter = ".json-Template|*.json|All Files|*.*";
      }

      if (openFileDialog.ShowDialog() == DialogResult.OK) {
        try {
          var content = JCInstantiator.Instantiate(openFileDialog.FileName);
          IView view = MainFormManager.MainForm.ShowContent(content);
          if (view == null)
            ErrorHandling.ShowErrorDialog("There is no view for the loaded item. It cannot be displayed.", new InvalidOperationException("No View Available"));
        } catch (Exception ex) {
          ErrorHandling.ShowErrorDialog((Control)MainFormManager.MainForm, "Cannot open file.", ex);
        } finally {
          ((MainForm.WindowsForms.MainForm)MainFormManager.MainForm).ResetAppStartingCursor();
        }
      }
    }
  }
}
