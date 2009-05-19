#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Core;
using System.IO;

namespace HeuristicLab.AdvancedOptimizationFrontend {
  /// <summary>
  /// The main form of the application.
  /// </summary>
  public partial class MainForm : Form, IControlManager {
    #region Inner Types
    private class Task {
      public string filename;
      public IStorable storable;
      public IEditor editor;

      private Task() { }
      public Task(string filename, IStorable storable, IEditor editor) {
        this.filename = filename;
        this.storable = storable;
        this.editor = editor;
      }
    }
    #endregion

    private object locker;
    private int runningTasks;

    /// <summary>
    /// Initializes a new instance of <see cref="MainForm"/>.
    /// </summary>
    public MainForm() {
      InitializeComponent();

      locker = new object();
      runningTasks = 0;

      AvailableOperatorsForm form = new AvailableOperatorsForm();
      form.Show(dockPanel);

      DiscoveryService discoveryService = new DiscoveryService();

      // discover creatable items
      Type[] creatables = discoveryService.GetTypes(typeof(IEditable));
      string[] names = new string[creatables.Length];
      for (int i = 0; i < creatables.Length; i++)
        names[i] = creatables[i].Name;
      Array.Sort(names, creatables);
      foreach (Type type in creatables) {
        if (!type.IsAbstract) {
          ToolStripMenuItem item = new ToolStripMenuItem();
          item.Tag = type;
          item.Text = "&" + type.Name + "...";
          item.Click += new EventHandler(newToolStripMenuItem_Click);
          newToolStripMenuItem.DropDownItems.Add(item);

          item = new ToolStripMenuItem();
          item.Tag = type;
          item.Text = "&" + type.Name + "...";
          item.Click += new EventHandler(newToolStripMenuItem_Click);
          newToolStripDropDownButton.DropDownItems.Add(item);
        }
      }
    }

    #region IControlManager Members
    /// <summary>
    /// Displays the given <paramref name="control"/>. 
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the given <paramref name="control"/>
    /// is neither a view nor an editor.</exception>
    /// <param name="control">The control to display.</param>
    public void ShowControl(IControl control) {
      DockContent content;
      if (control is IEditor)
        content = new EditorForm((IEditor)control);
      else if (control is IView)
        content = new ViewForm((IView)control);
      else
        throw new InvalidOperationException("Control is neither a view nor an editor.");

      content.TabText = content.Text;
      content.Show(dockPanel);
    }
    #endregion

    private void EnableDisableItems() {
      closeToolStripMenuItem.Enabled = false;
      closeAllToolStripMenuItem.Enabled = false;
      saveToolStripMenuItem.Enabled = false;
      saveToolStripButton.Enabled = false;
      saveAsToolStripMenuItem.Enabled = false;
      saveAllToolStripMenuItem.Enabled = false;
      saveAllToolStripButton.Enabled = false;

      if (ActiveMdiChild != null) {
        closeToolStripMenuItem.Enabled = true;
        closeAllToolStripMenuItem.Enabled = true;
        saveAllToolStripMenuItem.Enabled = true;
        saveAllToolStripButton.Enabled = true;
        EditorForm form = ActiveMdiChild as EditorForm;
        if (form != null){
          if (((Control)form.Editor).Enabled) {
            saveToolStripMenuItem.Enabled = true;
            saveToolStripButton.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;
          } else {
            closeToolStripMenuItem.Enabled = false;
            closeAllToolStripMenuItem.Enabled = false;
          }
        }
      }
    }

    #region Open and Save Methods
    private void Open() {
      if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
        lock (locker) runningTasks++;
        Cursor = Cursors.AppStarting;
        Task task = new Task(openFileDialog.FileName, null, null);
        ThreadPool.QueueUserWorkItem(new WaitCallback(AsynchronousLoad), task);
      }
    }
    private void AsynchronousLoad(object state) {
      Task task = (Task)state;
      try {
        task.storable = PersistenceManager.Load(task.filename);
      } catch (FileNotFoundException fileNotFoundEx) {
        MessageBox.Show("Sorry couldn't open file \"" + task.filename + "\".\nThe file or plugin \"" + fileNotFoundEx.FileName + "\" is not available.\nPlease make sure you have all necessary plugins installed.",
          "Reader Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      } catch (TypeLoadException typeLoadEx) {
        MessageBox.Show("Sorry couldn't open file \"" + task.filename + "\".\nThe type \"" + typeLoadEx.TypeName + "\" is not available.\nPlease make sure that you have the correct version the plugin installed.",
          "Reader Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      } catch (Exception e) {
        MessageBox.Show(String.Format(
          "Sorry couldn't open file \"{0}\".\n The following exception occurred: {1}",
          task.filename, e.ToString()),
          "Reader Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      LoadFinished(task);
    }
    private delegate void TaskFinishedDelegate(Task task);
    private void LoadFinished(Task task) {
      if (InvokeRequired)
        Invoke(new TaskFinishedDelegate(LoadFinished), task);
      else {
        IEditor editor = null;
        if (task.storable != null) {
          IEditable editable = task.storable as IEditable;
          if (editable != null)
            editor = editable.CreateEditor();
        }
        if (editor == null)
          MessageBox.Show("Could not open item. The selected item doesn't provide an editor.", "Editor Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        else {
          editor.Filename = task.filename;
          PluginManager.ControlManager.ShowControl(editor);
        }
        lock (locker) {
          runningTasks--;
          if (runningTasks == 0)
            Cursor = Cursors.Default;
        }
      }
    }
    private void Save(EditorForm form) {
      if (form.Editor.Filename == null)
        SaveAs(form);
      else {
        lock (locker) runningTasks++;
        Cursor = Cursors.AppStarting;
        ((Control)form.Editor).Enabled = false;
        EnableDisableItems();
        Task task = new Task(form.Editor.Filename, form.Editor.Item, form.Editor);
        ThreadPool.QueueUserWorkItem(new WaitCallback(AsynchronousSave), task);
      }
    }
    private void SaveAs(EditorForm form) {
      if (saveFileDialog.ShowDialog(this) == DialogResult.OK) {
        form.Editor.Filename = saveFileDialog.FileName;
        Save(form);
      }

    }
    private void AsynchronousSave(object state) {
      Task task = (Task)state;
      try {
        PersistenceManager.Save(task.storable, task.filename);
      } catch (Exception e) {
        MessageBox.Show(String.Format(
          "Sorry couldn't save file \"{0}\".\n The following exception occurred: {1}",
          task.filename, e.ToString()),
          "Reader Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      SaveFinished(task);
    }
    private void SaveFinished(Task task) {
      if (InvokeRequired)
        Invoke(new TaskFinishedDelegate(SaveFinished), task);
      else {
        ((Control)task.editor).Enabled = true;
        EnableDisableItems();
        lock (locker) {
          runningTasks--;
          if (runningTasks == 0)
            Cursor = Cursors.Default;
        }
      }
    }
    #endregion

    private void MainForm_MdiChildActivate(object sender, EventArgs e) {
      EnableDisableItems();
    }

    #region Menu Events
    private void newToolStripMenuItem_Click(object sender, EventArgs e) {
      ToolStripItem item = (ToolStripItem)sender;
      Type type = (Type)item.Tag;
      IEditable editable = (IEditable)Activator.CreateInstance(type);
      if (editable == null) {
        MessageBox.Show("The selected item is not editable.", "Editable Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      } else {
        IEditor editor = editable.CreateEditor();
        if (editor == null) {
          MessageBox.Show("The selected item doesn't provide an editor.", "Editor Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        } else {
          PluginManager.ControlManager.ShowControl(editor);
          EnableDisableItems();
        }
      }
    }
    private void openToolStripMenuItem_Click(object sender, EventArgs e) {
      Open();
    }
    private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
      EditorForm form = ActiveMdiChild as EditorForm;
      Save(form);
    }
    private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
      EditorForm form = ActiveMdiChild as EditorForm;
      SaveAs(form);
    }
    private void saveAllToolStripMenuItem_Click(object sender, EventArgs e) {
      for (int i = 0; i < MdiChildren.Length; i++) {
        EditorForm form = MdiChildren[i] as EditorForm;
        if (((Control)form.Editor).Enabled) Save(form);
      }
    }
    private void closeToolStripMenuItem_Click(object sender, EventArgs e) {
      ActiveMdiChild.Close();
      EnableDisableItems();
    }
    private void closeAllToolStripMenuItem_Click(object sender, EventArgs e) {
      while (MdiChildren.Length > 0)
        MdiChildren[0].Close();
      EnableDisableItems();
    }
    private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
      Application.Exit();
    }
    private void availableOperatorsToolStripMenuItem_Click(object sender, EventArgs e) {
      AvailableOperatorsForm form = new AvailableOperatorsForm();
      form.Show(dockPanel);
    }
    private void collectGarbageToolStripMenuItem_Click(object sender, EventArgs e) {
      GC.Collect();
    }
    private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
      AboutDialog dialog = new AboutDialog();
      dialog.ShowDialog(this);
      dialog.Dispose();
    }
    #endregion

    #region ToolStrip Events
    private void openToolStripButton_Click(object sender, EventArgs e) {
      Open();
    }
    private void saveToolStripButton_Click(object sender, EventArgs e) {
      EditorForm form = ActiveMdiChild as EditorForm;
      Save(form);
    }
    private void saveAllToolStripButton_Click(object sender, EventArgs e) {
      for (int i = 0; i < MdiChildren.Length; i++) {
        EditorForm form = MdiChildren[i] as EditorForm;
        if (form!=null && ((Control)form.Editor).Enabled) Save(form);
      }
    }
    #endregion
  }
}
