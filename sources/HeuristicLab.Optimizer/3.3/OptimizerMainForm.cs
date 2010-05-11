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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using System.IO;

namespace HeuristicLab.Optimizer {
  internal partial class OptimizerMainForm : DockingMainForm {
    private string title;
    private int appStartingCursors;
    private int waitingCursors;

    private Clipboard<IItem> clipboard;
    public Clipboard<IItem> Clipboard {
      get { return clipboard; }
    }

    public OptimizerMainForm()
      : base() {
      InitializeComponent();
      appStartingCursors = 0;
      waitingCursors = 0;
    }
    public OptimizerMainForm(Type userInterfaceItemType)
      : base(userInterfaceItemType) {
      InitializeComponent();
      appStartingCursors = 0;
      waitingCursors = 0;
    }
    public OptimizerMainForm(Type userInterfaceItemType, bool showContentInViewHost)
      : this(userInterfaceItemType) {
      this.ShowContentInViewHost = showContentInViewHost;
    }

    protected override void OnInitialized(EventArgs e) {
      base.OnInitialized(e);
      AssemblyFileVersionAttribute version = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).
                                             Cast<AssemblyFileVersionAttribute>().FirstOrDefault();
      title = "HeuristicLab Optimizer";
      if (version != null) title += " " + version.Version;
      Title = title;

      ContentManager.Initialize(new PersistenceContentManager());

      WindowState = Properties.Settings.Default.ShowMaximized ? FormWindowState.Maximized : FormWindowState.Normal;

      clipboard = new Clipboard<IItem>();
      clipboard.Dock = DockStyle.Left;
      clipboard.Collapsed = Properties.Settings.Default.CollapseClipboard;
      if (Properties.Settings.Default.ShowClipboard) {
        clipboard.Show();
      }
      if (Properties.Settings.Default.ShowOperatorsSidebar) {
        OperatorsSidebar operatorsSidebar = new OperatorsSidebar();
        operatorsSidebar.Dock = DockStyle.Left;
        operatorsSidebar.Show();
        operatorsSidebar.Collapsed = Properties.Settings.Default.CollapseOperatorsSidebar;
      }
      if (Properties.Settings.Default.ShowStartPage) {
        StartPage startPage = new StartPage();
        startPage.Show();
      }
    }

    private static string CHARTCONTROLASSEMBLY = "System.Windows.Forsms.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
    private bool CheckChartControls() {
      try {
        Assembly chartControlsAssembly = Assembly.Load(CHARTCONTROLASSEMBLY);
        if (chartControlsAssembly != null)
          return true;
      }
      catch (FileNotFoundException) {
      }
      catch (FileLoadException) {
      }
      catch (BadImageFormatException) {
      }
      return false;
    }

    protected override void OnShown(EventArgs e) {
      base.OnShown(e);
      if (!CheckChartControls()) {
        ChartControlsWarning dlg = new ChartControlsWarning();
        dlg.ShowDialog(this);
      }
    }

    protected override void OnClosing(CancelEventArgs e) {
      base.OnClosing(e);
      if (MainFormManager.MainForm.Views.OfType<IContentView>().FirstOrDefault() != null) {
        if (MessageBox.Show(this, "Some views are still opened. If their content has not been saved, it will be lost after closing. Do you really want to close HeuristicLab Optimizer?", "Close Optimizer", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
          e.Cancel = true;
      }
    }
    protected override void OnClosed(EventArgs e) {
      base.OnClosed(e);
      Properties.Settings.Default.ShowMaximized = WindowState == FormWindowState.Maximized;
      Properties.Settings.Default.CollapseClipboard = clipboard.Collapsed;
      OperatorsSidebar operatorsSidebar = MainFormManager.MainForm.Views.OfType<OperatorsSidebar>().FirstOrDefault();
      if (operatorsSidebar != null) Properties.Settings.Default.CollapseOperatorsSidebar = operatorsSidebar.Collapsed;
      Properties.Settings.Default.Save();
    }

    protected override void OnActiveViewChanged() {
      base.OnActiveViewChanged();
      UpdateTitle();
    }

    public void UpdateTitle() {
      if (InvokeRequired)
        Invoke(new Action(UpdateTitle));
      else {
        IContentView activeView = ActiveView as IContentView;
        if ((activeView != null) && (activeView.Content != null) && (activeView.Content is IStorableContent)) {
          IStorableContent content = (IStorableContent)activeView.Content;
          Title = title + " [" + (string.IsNullOrEmpty(content.Filename) ? "Unsaved" : content.Filename) + "]";
        } else {
          Title = title;
        }
      }
    }

    #region Cursor Handling
    public void SetAppStartingCursor() {
      if (InvokeRequired)
        Invoke(new Action(SetAppStartingCursor));
      else {
        appStartingCursors++;
        SetCursor();
      }
    }
    public void ResetAppStartingCursor() {
      if (InvokeRequired)
        Invoke(new Action(ResetAppStartingCursor));
      else {
        appStartingCursors--;
        SetCursor();
      }
    }
    public void SetWaitCursor() {
      if (InvokeRequired)
        Invoke(new Action(SetWaitCursor));
      else {
        waitingCursors++;
        SetCursor();
      }
    }
    public void ResetWaitCursor() {
      if (InvokeRequired)
        Invoke(new Action(ResetWaitCursor));
      else {
        waitingCursors--;
        SetCursor();
      }
    }
    private void SetCursor() {
      if (waitingCursors > 0) Cursor = Cursors.WaitCursor;
      else if (appStartingCursors > 0) Cursor = Cursors.AppStarting;
      else Cursor = Cursors.Default;
    }
    #endregion
  }
}
