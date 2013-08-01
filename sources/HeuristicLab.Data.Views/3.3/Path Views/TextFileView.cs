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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using HeuristicLab.MainForm;

namespace HeuristicLab.Data.Views {
  [View("TextFileView")]
  [Content(typeof(TextFileValue), true)]
  public sealed partial class TextFileView : FileValueView {
    private bool fileContentChanged;
    private bool FileContentChanged {
      get { return fileContentChanged; }
      set {
        if (fileContentChanged != value) {
          fileContentChanged = value;
          OnFileContentChanged();
        }
      }
    }

    public new TextFileValue Content {
      get { return (TextFileValue)base.Content; }
      set { base.Content = value; }
    }

    public TextFileView() {
      InitializeComponent();
      fileContentChanged = false;
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.StringValue.ValueChanged += Content_FilePathChanged;
    }
    protected override void DeregisterContentEvents() {
      Content.StringValue.ValueChanged -= Content_FilePathChanged;
      fileSystemWatcher.EnableRaisingEvents = false;
      base.DeregisterContentEvents();
    }
    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      textBox.ReadOnly = Locked || ReadOnly || Content == null;
      saveButton.Enabled = !Locked && !ReadOnly && Content != null && FileContentChanged;

      if (Content != null && Content.Exists()) {
        fileSystemWatcher.Filter = Path.GetFileName(Content.Value);
        fileSystemWatcher.Path = Path.GetDirectoryName(Content.Value);
      }
      fileSystemWatcher.EnableRaisingEvents = Content != null && File.Exists(Content.Value) && Visible;
    }

    protected override void OnVisibleChanged(EventArgs e) {
      //mkommend: necessary to update the textbox to detect intermediate file changes.
      if (Visible && Content != null) UpdateTextBox();
      base.OnVisibleChanged(e);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      FileContentChanged = false;
      if (Content == null) {
        textBox.Text = string.Empty;
        //mkommend: other properties of the file system watcher cannot be cleared (e.g., path) as this leads to an ArgumentException
        fileSystemWatcher.EnableRaisingEvents = false;
        return;
      }
      saveFileDialog.Filter = Content.FileDialogFilter;
      UpdateTextBox();
    }

    private void Content_FilePathChanged(object sender, EventArgs e) {
      UpdateTextBox();
      SetEnabledStateOfControls();
    }

    private void textBox_TextChanged(object sender, EventArgs e) {
      FileContentChanged = fileText != textBox.Text;
    }

    private void OnFileContentChanged() {
      SetEnabledStateOfControls();
      textBox.ForeColor = FileContentChanged ? Color.Red : Color.Black;
    }

    private bool fileSystemWatcherChangedFired = false;
    private void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e) {
      //mkommend: cannot react on visible changed, because this event is not fired when the parent control gets visible set to false
      if (!Visible) return;
      //FileSystemWatcher may fire multiple events depending on how file changes are implemented in the modifying program
      //see: http://stackoverflow.com/questions/1764809/filesystemwatcher-changed-event-is-raised-twice
      if (fileSystemWatcherChangedFired) return;
      fileSystemWatcherChangedFired = true;

      string newContent = ReadFile(Content.Value);
      if (newContent == textBox.Text) {
        fileSystemWatcherChangedFired = false;
        return;
      }

      //File created
      if (e.ChangeType == WatcherChangeTypes.Created) {
        UpdateTextBox();
      }

      //File changed
      else if (e.ChangeType == WatcherChangeTypes.Changed) {
        string msg = string.Format("The content of the file \"{0}\" has changed.\nDo you want to reload the file?", Content.Value);
        var result = MessageBox.Show(this, msg, "Reload the file?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
        if (result == DialogResult.Yes) {
          UpdateTextBox();
        } else {
          fileText = newContent;
          FileContentChanged = true;
        }
      }

      //File deleted
      else if (e.ChangeType == WatcherChangeTypes.Deleted) {
        string msg = string.Format("The file \"{0}\" is not present anymore. Do you want to restore the file from the given content?", Content.Value);
        var result = MessageBox.Show(this, msg, "Restore file?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (result == DialogResult.Yes) {
          SaveFile();
        }
        UpdateTextBox();
      }
      fileSystemWatcherChangedFired = false;
    }

    private void fileSystemWatcher_Renamed(object sender, RenamedEventArgs e) {
      //mkommend: cannot react on visible changed, because this event is not fired when the parent control gets visible set to false
      if (!Visible) return;
      string msg = string.Format("The file \"{0}\" got renamed to \"{1}\". Do you want to update the file path?", e.OldName, e.Name);
      var result = MessageBox.Show(this, msg, "Update file path?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
      if (result == DialogResult.Yes) {
        Content.Value = e.FullPath;
        fileSystemWatcher.Filter = e.Name;
      }
      UpdateTextBox();
    }

    private void SaveFile() {
      //omit circular events by changing the file from HL
      fileSystemWatcher.EnableRaisingEvents = false;
      string path = Content.Value;
      if (string.IsNullOrEmpty(path)) {
        if (saveFileDialog.ShowDialog(this) != DialogResult.OK) return;
        path = saveFileDialog.FileName;
      }

      WriteFile(path, textBox.Text);
      Content.Value = path;
      FileContentChanged = false;
      fileSystemWatcher.EnableRaisingEvents = true;
    }

    private void saveButton_Click(object sender, EventArgs e) {
      SaveFile();
    }

    private string fileText;
    private void UpdateTextBox() {
      if (string.IsNullOrEmpty(Content.Value)) {
        textBox.Enabled = false;
        textBox.Text = string.Format("No file path is specified.");
        return;
      }

      fileText = ReadFile(Content.Value);
      if (fileText == null) {
        textBox.Enabled = false;
        textBox.Text = string.Format("The specified file \"{0}\" cannot be found.", Content.Value);
        return;
      }

      textBox.Enabled = true;
      textBox.Text = fileText;
    }
    private void textBox_Validated(object sender, EventArgs e) {
      if (!FileContentChanged) return;
      string msg = string.Format("You have not saved the changes in the file \"{0}\" yet. Do you want to save the changes?", Content.Value);
      var result = MessageBox.Show(this, msg, "Save changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
      if (result == DialogResult.Yes) SaveFile();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
      if (keyData == (Keys.Control | Keys.S)) {
        SaveFile();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    private static string ReadFile(string path) {
      if (!File.Exists(path)) return null;
      string fileContent = string.Empty;
      using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
        using (StreamReader streamReader = new StreamReader(fileStream)) {
          fileContent = streamReader.ReadToEnd();
        }
      }
      return fileContent;
    }

    private static void WriteFile(string path, string fileContent) {
      using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write)) {
        using (StreamWriter streamWriter = new StreamWriter(fileStream)) {
          streamWriter.Write(fileContent);
        }
      }
    }
  }
}
