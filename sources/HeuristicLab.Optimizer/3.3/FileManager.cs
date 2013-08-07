#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimizer {
  internal static class FileManager {
    private static NewItemDialog newItemDialog;
    private static OpenFileDialog openFileDialog;
    private static SaveFileDialog saveFileDialog;

    static FileManager() {
      newItemDialog = null;
      openFileDialog = null;
      saveFileDialog = null;
    }

    public static void New() {
      if (newItemDialog == null) newItemDialog = new NewItemDialog();
      if (newItemDialog.ShowDialog() == DialogResult.OK) {
        IView view = MainFormManager.MainForm.ShowContent(newItemDialog.Item);
        if (view == null)
          ErrorHandling.ShowErrorDialog("There is no view for the new item. It cannot be displayed.", new InvalidOperationException("No View Available"));
      }
    }

    public static void Open() {
      if (openFileDialog == null) {
        openFileDialog = new OpenFileDialog();
        openFileDialog.Title = "Open Item";
        openFileDialog.FileName = "Item";
        openFileDialog.Multiselect = true;
        openFileDialog.DefaultExt = "hl";
        openFileDialog.Filter = "HeuristicLab Files|*.hl|All Files|*.*";
      }

      if (openFileDialog.ShowDialog() == DialogResult.OK) {
        OpenFiles(openFileDialog.FileNames);
      }
    }

    public static void OpenFiles(IEnumerable<string> fileNames) {
      foreach (string filename in fileNames) {
        ((MainForm.WindowsForms.MainForm)MainFormManager.MainForm).SetAppStartingCursor();
        ContentManager.LoadAsync(filename, LoadingCompleted);
      }
    }

    private static void LoadingCompleted(IStorableContent content, Exception error) {
      try {
        if (error != null) throw error;
        IView view = MainFormManager.MainForm.ShowContent(content);
        if (view == null)
          ErrorHandling.ShowErrorDialog("There is no view for the loaded item. It cannot be displayed.", new InvalidOperationException("No View Available"));
      }
      catch (Exception ex) {
        ErrorHandling.ShowErrorDialog((Control)MainFormManager.MainForm, "Cannot open file.", ex);
      }
      finally {
        ((MainForm.WindowsForms.MainForm)MainFormManager.MainForm).ResetAppStartingCursor();
      }
    }

    public static void Save() {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      if (activeView != null) {
        Save(activeView);
      }
    }
    private static void Save(IContentView view) {
      IStorableContent content = view.Content as IStorableContent;
      if (!view.Locked && content != null) {
        if (string.IsNullOrEmpty(content.Filename))
          SaveAs(view);
        else {
          MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>().SetAppStartingCursor();
          SetSaveOperationProgressInContentViews(content, true);
          ContentManager.SaveAsync(content, content.Filename, true, SavingCompleted);
        }
      }
    }
    public static void SaveAs() {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      if (activeView != null) {
        SaveAs(activeView);
      }
    }
    public static void SaveAs(IContentView view) {
      IStorableContent content = view.Content as IStorableContent;
      if (!view.Locked && content != null) {
        if (saveFileDialog == null) {
          saveFileDialog = new SaveFileDialog();
          saveFileDialog.Title = "Save Item";
          saveFileDialog.DefaultExt = "hl";
          saveFileDialog.Filter = "Uncompressed HeuristicLab Files|*.hl|HeuristicLab Files|*.hl|All Files|*.*";
          saveFileDialog.FilterIndex = 2;
        }
        saveFileDialog.FileName = string.IsNullOrEmpty(content.Filename) ? "Item" : content.Filename;

        if (saveFileDialog.ShowDialog() == DialogResult.OK) {
          MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>().SetAppStartingCursor();
          SetSaveOperationProgressInContentViews(content, true, saveFileDialog.FileName);
          if (saveFileDialog.FilterIndex == 1) {
            ContentManager.SaveAsync(content, saveFileDialog.FileName, false, SavingCompleted);
          } else {
            ContentManager.SaveAsync(content, saveFileDialog.FileName, true, SavingCompleted);
          }
        }
      }
    }
    private static void SavingCompleted(IStorableContent content, Exception error) {
      try {
        if (error != null) throw error;
        MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>().UpdateTitle();
      }
      catch (Exception ex) {
        ErrorHandling.ShowErrorDialog((Control)MainFormManager.MainForm, "Cannot save file.", ex);
      }
      finally {
        SetSaveOperationProgressInContentViews(content, false);
        MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>().ResetAppStartingCursor();
      }
    }

    private static void SetSaveOperationProgressInContentViews(IStorableContent content, bool showProgress, string fileName = null) {
      HeuristicLab.MainForm.WindowsForms.MainForm mainForm = MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>();
      #region Mono Compatibility
      // removed the InvokeRequired check because of Mono
      mainForm.Invoke((Action)delegate {
        if (showProgress) {
          mainForm.AddOperationProgressToContent(content, string.Format("Saving file \"{0}\"...", Path.GetFileName(fileName ?? content.Filename)));
        } else
          mainForm.RemoveOperationProgressFromContent(content);
      });
      #endregion
    }
  }
}
