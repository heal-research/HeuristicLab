﻿#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
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
          ErrorHandlingUI.ShowErrorDialog("There is no view for the new item. It cannot be displayed.", new InvalidOperationException("No View Available"));
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

    private static void LoadingCompleted(IStorableContent content, Exception error, ContentManager.Info info) {
      try {
        if (error != null) throw error;
        if (info!=null && info.UnknownTypeGuids.Any()) {
          var message = "Unknown type guids: " + string.Join(Environment.NewLine, info.UnknownTypeGuids);
          MessageBox.Show((Control)MainFormManager.MainForm, message, $"File {info.Filename} not restored completely", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        IView view = MainFormManager.MainForm.ShowContent(content);
        if (view == null)
          ErrorHandlingUI.ShowErrorDialog("There is no view for the loaded item. It cannot be displayed.", new InvalidOperationException("No View Available"));
      } catch (Exception ex) {
        ErrorHandlingUI.ShowErrorDialog((Control)MainFormManager.MainForm, "Cannot open file.", ex);
      } finally {
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
          var cancellationTokenSource = new CancellationTokenSource();
          AddProgressInContentViews(content, cancellationTokenSource);
          ContentManager.SaveAsync(content, content.Filename, true, SavingCompleted, cancellationTokenSource.Token);
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

        INamedItem namedItem = content as INamedItem;
        string suggestedFileName = string.Empty;
        if (!string.IsNullOrEmpty(content.Filename)) suggestedFileName = content.Filename;
        else if (namedItem != null) suggestedFileName = namedItem.Name;
        else suggestedFileName = "Item";

        saveFileDialog.FileName = suggestedFileName;

        if (saveFileDialog.ShowDialog() == DialogResult.OK) {
          MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>().SetAppStartingCursor();
          bool compressed = saveFileDialog.FilterIndex != 1;
          var cancellationTokenSource = new CancellationTokenSource();
          AddProgressInContentViews(content, cancellationTokenSource, saveFileDialog.FileName);

          ContentManager.SaveAsync(content, saveFileDialog.FileName, compressed, SavingCompleted, cancellationTokenSource.Token);
        }
      }
    }
    private static void SavingCompleted(IStorableContent content, Exception error) {
      try {
        if (error != null) throw error;
        MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>().UpdateTitle();
      } catch (OperationCanceledException) { // do nothing if canceled
      } catch (Exception ex) {
        ErrorHandlingUI.ShowErrorDialog((Control)MainFormManager.MainForm, "Cannot save file.", ex);
      } finally {
        Progress.Hide(content);
        MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>().ResetAppStartingCursor();
      }
    }

    private static void AddProgressInContentViews(IStorableContent content, CancellationTokenSource cancellationTokenSource, string fileName = null) {
      string message = string.Format("Saving to file \"{0}\"...", Path.GetFileName(fileName ?? content.Filename));
      Progress.Show(content, message, ProgressMode.Indeterminate, cancelRequestHandler: () => cancellationTokenSource.Cancel());
    }
  }
}
