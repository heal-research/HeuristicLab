using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Core.Views;

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

    private static Dictionary<IItemView, FileInfo> files;
    private static NewItemDialog newItemDialog;
    private static OpenFileDialog openFileDialog;
    private static SaveFileDialog saveFileDialog;
    private static int waitingCursors;
    private static int newDocumentsCounter;

    static FileManager() {
      files = new Dictionary<IItemView, FileInfo>();
      newItemDialog = null;
      openFileDialog = null;
      saveFileDialog = null;
      waitingCursors = 0;
      newDocumentsCounter = 1;
    }

    public static void New() {
      if (newItemDialog == null) newItemDialog = new NewItemDialog();
      if (newItemDialog.ShowDialog() == DialogResult.OK) {
        IItemView view = MainFormManager.CreateDefaultView(newItemDialog.Item) as IItemView;
        if (view != null) {
          view.Closed += new EventHandler(ViewClosed);
          view.Caption = "Item" + newDocumentsCounter.ToString() + ".hl";
          newDocumentsCounter++;
          MainFormManager.MainForm.ShowView(view);
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
      IItemView activeView = MainFormManager.MainForm.ActiveView as IItemView;
      if ((activeView != null) && (CreatableAttribute.IsCreatable(activeView.Item.GetType()))) {
        Save(activeView);
      }
    }
    private static void Save(IItemView view) {
      if (!files.ContainsKey(view)) {
        SaveAs(view);
      } else {
        if (files[view].Compressed)
          SaveItemAsync(view, files[view].Filename, 9);
        else
          SaveItemAsync(view, files[view].Filename, 0);
      }
    }

    public static void SaveAs() {
      IItemView activeView = MainFormManager.MainForm.ActiveView as IItemView;
      if ((activeView != null) && (CreatableAttribute.IsCreatable(activeView.Item.GetType()))) {
        SaveAs(activeView);
      }
    }
    public static void SaveAs(IItemView view) {
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
                  where v is IItemView
                  where CreatableAttribute.IsCreatable(((IItemView)v).Item.GetType())
                  select v as IItemView;

      foreach (IItemView view in views) {
        Save(view);
      }
    }

    private static void ViewClosed(object sender, EventArgs e) {
      IItemView view = (IItemView)sender;
      view.Closed -= new EventHandler(ViewClosed);
      files.Remove(view);
    }

    #region Asynchronous Save/Load Operations
    private static void Invoke(Action a) {
      Form form = MainFormManager.MainForm as Form;
      if (form.InvokeRequired)
        form.Invoke(a);
      else
        a.Invoke();
    }

    private static void SaveItemAsync(IItemView view, string filename, int compression) {
      ThreadPool.QueueUserWorkItem(
        new WaitCallback(
          delegate(object arg) {
            try {
              SetWaitingCursor();
              XmlGenerator.Serialize(view.Item, filename, compression);
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
                IItemView view = MainFormManager.CreateDefaultView(item) as IItemView;
                if (view != null) {
                  view.Closed += new EventHandler(ViewClosed);
                  view.Caption = Path.GetFileName(filename);
                  files.Add(view, new FileInfo(filename));
                  MainFormManager.MainForm.ShowView(view);
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
    #endregion
  }
}
