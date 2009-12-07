using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Core.Views;

namespace HeuristicLab.Optimizer {
  internal static class FileManager {

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

    private static Dictionary<IItem, FileInfo> files = new Dictionary<IItem, FileInfo>();
    private static NewItemDialog newItemDialog = null;
    private static OpenFileDialog openFileDialog = null;
    private static SaveFileDialog saveFileDialog = null;

    public static void New() {
      if (newItemDialog == null) newItemDialog = new NewItemDialog();
      if (newItemDialog.ShowDialog() == DialogResult.OK) {
        MainFormManager.MainForm.ShowView(MainFormManager.CreateDefaultView(newItemDialog.Item));
        files.Add(newItemDialog.Item, new FileInfo());
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
        foreach (string file in openFileDialog.FileNames) {
          IItem item = (IItem)XmlParser.Deserialize(file);
          MainFormManager.MainForm.ShowView(MainFormManager.CreateDefaultView(item));
          files.Add(item, new FileInfo(file));
        }
      }
    }

    public static void Save() {
      IItemView activeView = MainFormManager.MainForm.ActiveView as IItemView;
      if ((activeView != null) && (CreatableAttribute.IsCreatable(activeView.Item.GetType()))) {
        Save(activeView.Item);
      }
    }
    private static void Save(IItem item) {
      if (files[item].Filename != string.Empty) {
        if (files[item].Compressed)
          XmlGenerator.Serialize(item, saveFileDialog.FileName, 9);
        else
          XmlGenerator.Serialize(item, saveFileDialog.FileName, 0);
      } else {
        SaveAs(item);
      }
    }

    public static void SaveAs() {
      IItemView activeView = MainFormManager.MainForm.ActiveView as IItemView;
      if ((activeView != null) && (CreatableAttribute.IsCreatable(activeView.Item.GetType()))) {
        SaveAs(activeView.Item);
      }
    }
    public static void SaveAs(IItem item) {
      if (saveFileDialog == null) {
        saveFileDialog = new SaveFileDialog();
        saveFileDialog.Title = "Save Item";
        saveFileDialog.DefaultExt = "hl";
        saveFileDialog.Filter = "Uncompressed HeuristicLab Files|*.hl|HeuristicLab Files|*.hl|All Files|*.*";
        saveFileDialog.FilterIndex = 2;
      }

      saveFileDialog.FileName = files[item].Filename;
      if (saveFileDialog.FileName == string.Empty) saveFileDialog.FileName = "Item";
      if (! files[item].Compressed)
        saveFileDialog.FilterIndex = 1;
      else
        saveFileDialog.FilterIndex = 2;

      if (saveFileDialog.ShowDialog() == DialogResult.OK) {
        if (saveFileDialog.FilterIndex == 1) {
          XmlGenerator.Serialize(item, saveFileDialog.FileName, 0);
        } else {
          XmlGenerator.Serialize(item, saveFileDialog.FileName, 9);
        }
        files[item].Filename = saveFileDialog.FileName;
        files[item].Compressed = saveFileDialog.FilterIndex != 1;
      }
    }

    public static void SaveAll() {
      var views = from v in MainFormManager.MainForm.Views
                  where v is IItemView
                  where CreatableAttribute.IsCreatable(((IItemView)v).Item.GetType())
                  select v as IItemView;

      foreach (IItemView view in views) {
        Save(view.Item);
      }
    }
  }
}
