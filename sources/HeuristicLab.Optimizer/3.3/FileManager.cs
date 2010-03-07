#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Persistence.Default.Xml;

namespace HeuristicLab.Optimizer {
  internal static class FileManager {

    #region Private Class FileInfo
    private class FileInfo {
      public string Filename { get; set; }
      public bool Compressed { get; set; }

      public FileInfo(string filename, bool compressed) {
        Filename = filename;
        Compressed = compressed;
      }
      public FileInfo(string filename)
        : this(filename, true) {
      }
      public FileInfo()
        : this(string.Empty, true) {
      }
    }
    #endregion

    private static Dictionary<IContentView, FileInfo> files;
    private static NewItemDialog newItemDialog;
    private static OpenFileDialog openFileDialog;
    private static SaveFileDialog saveFileDialog;
    private static int waitingCursors;
    private static int newDocumentsCounter;

    static FileManager() {
      files = new Dictionary<IContentView, FileInfo>();
      newItemDialog = null;
      openFileDialog = null;
      saveFileDialog = null;
      waitingCursors = 0;
      newDocumentsCounter = 1;
      // NOTE: Events fired by the main form are registered in HeuristicLabOptimizerApplication.
    }

    public static void New() {
      if (newItemDialog == null) newItemDialog = new NewItemDialog();
      if (newItemDialog.ShowDialog() == DialogResult.OK) {
        IView view = MainFormManager.CreateDefaultView(newItemDialog.Item);
        if (view == null) {
          MessageBox.Show("There is no view for the new item. It cannot be displayed. ", "No View Available", MessageBoxButtons.OK, MessageBoxIcon.Error);
        } else {
          if (view is IContentView) {
            view.Caption = "Item" + newDocumentsCounter.ToString() + ".hl";
            newDocumentsCounter++;
          }
          view.Show();
        }
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
        foreach (string filename in openFileDialog.FileNames)
          LoadItemAsync(filename);
      }
    }

    public static void Save() {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      if (activeView != null) {
        Save(activeView);
      }
    }
    private static void Save(IContentView view) {
      if ((!files.ContainsKey(view)) || (!File.Exists(files[view].Filename))) {
        SaveAs(view);
      } else {
        if (files[view].Compressed)
          SaveItemAsync(view, files[view].Filename, 9);
        else
          SaveItemAsync(view, files[view].Filename, 0);
      }
    }

    public static void SaveAs() {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      if (activeView != null) {
        SaveAs(activeView);
      }
    }
    public static void SaveAs(IContentView view) {
      if (saveFileDialog == null) {
        saveFileDialog = new SaveFileDialog();
        saveFileDialog.Title = "Save Item";
        saveFileDialog.DefaultExt = "hl";
        saveFileDialog.Filter = "Uncompressed HeuristicLab Files|*.hl|HeuristicLab Files|*.hl|All Files|*.*";
        saveFileDialog.FilterIndex = 2;
      }

      if (!files.ContainsKey(view)) {
        files.Add(view, new FileInfo());
        saveFileDialog.FileName = view.Caption;
      } else {
        saveFileDialog.FileName = files[view].Filename;
      }
      if (! files[view].Compressed)
        saveFileDialog.FilterIndex = 1;
      else
        saveFileDialog.FilterIndex = 2;

      if (saveFileDialog.ShowDialog() == DialogResult.OK) {
        if (saveFileDialog.FilterIndex == 1) {
          SaveItemAsync(view, saveFileDialog.FileName, 0);
        } else {
          SaveItemAsync(view, saveFileDialog.FileName, 9);
        }
      }
    }

    public static void SaveAll() {
      var views = from v in MainFormManager.MainForm.Views
                  where v is IContentView
                  select v as IContentView;

      foreach (IContentView view in views) {
        ItemView itemView = view as ItemView;
        if ((itemView == null) || (itemView.EnableFileOperations)) {
          Save(view);
        }
      }
    }

    // NOTE: This event is fired by the main form. It is registered in HeuristicLabOptimizerApplication.
    internal static void ViewClosed(object sender, ViewEventArgs e) {
      IContentView view = e.View as IContentView;
      if (view != null) files.Remove(view);
    }

    #region Asynchronous Save/Load Operations
    private static void Invoke(Action a) {
      Form form = MainFormManager.MainForm as Form;
      if (form.InvokeRequired)
        form.Invoke(a);
      else
        a.Invoke();
    }

    private static void SaveItemAsync(IContentView view, string filename, int compression) {
      ThreadPool.QueueUserWorkItem(
        new WaitCallback(
          delegate(object arg) {
            try {
              DisableView(view);
              SetWaitingCursor();
              XmlGenerator.Serialize(view.Content, filename, compression);
              Invoke(delegate() {
                view.Caption = Path.GetFileName(filename);
                files[view].Filename = filename;
                files[view].Compressed = compression > 0;
              });
            }
            catch (Exception ex) {
              Auxiliary.ShowErrorMessageBox(ex);
            } finally {
              ResetWaitingCursor();
              EnableView(view);
            }
          }
        )
      );
    }
    private static void LoadItemAsync(string filename) {
      ThreadPool.QueueUserWorkItem(
        new WaitCallback(
          delegate(object arg) {
            try {
              SetWaitingCursor();
              IItem item = (IItem)XmlParser.Deserialize(filename);
              Invoke(delegate() {
                IContentView view = MainFormManager.CreateDefaultView(item) as IContentView;
                if (view == null) {
                  MessageBox.Show("There is no view for the loaded item. It cannot be displayed. ", "No View Available", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } else {
                  view.Caption = Path.GetFileName(filename);
                  files.Add(view, new FileInfo(filename));
                  view.Show();
                }
              });
            } catch (Exception ex) {
              Auxiliary.ShowErrorMessageBox(ex);
            } finally {
              ResetWaitingCursor();
            }
          }
        )
      );
    }

    private static void SetWaitingCursor() {
      Invoke(delegate() {
        waitingCursors++;
        ((Form)MainFormManager.MainForm).Cursor = Cursors.AppStarting;
      });
    }
    private static void ResetWaitingCursor() {
      Invoke(delegate() {
        waitingCursors--;
        if (waitingCursors == 0) ((Form)MainFormManager.MainForm).Cursor = Cursors.Default;
      });
    }
    private static void DisableView(IView view) {
      Invoke(delegate() {
        ((UserControl)view).Enabled = false;
      });
    }
    private static void EnableView(IView view) {
      Invoke(delegate() {
        ((UserControl)view).Enabled = true;
      });
    }
    #endregion
  }
}
