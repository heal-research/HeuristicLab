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
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Optimizer {
  internal partial class OptimizerMainForm : DockingMainForm {
    private Clipboard<IItem> clipboard;
    public Clipboard<IItem> Clipboard {
      get { return clipboard; }
    }

    public OptimizerMainForm()
      : base() {
      InitializeComponent();
    }
    public OptimizerMainForm(Type userInterfaceItemType)
      : base(userInterfaceItemType) {
      InitializeComponent();
    }
    public OptimizerMainForm(Type userInterfaceItemType, bool showInViewHost)
      : this(userInterfaceItemType) {
      this.ShowInViewHost = showInViewHost;
    }

    protected override void OnInitialized(EventArgs e) {
      base.OnInitialized(e);
      AssemblyFileVersionAttribute version = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).
                                             Cast<AssemblyFileVersionAttribute>().FirstOrDefault();
      Title = "HeuristicLab Optimizer";
      if (version != null) Title += " " + version.Version;
      ViewClosed += new EventHandler<ViewEventArgs>(FileManager.ViewClosed);

      clipboard = new Clipboard<IItem>();
      clipboard.Dock = DockStyle.Left;
      if (Properties.Settings.Default.ShowClipboard) {
        clipboard.Show();
      }
      if (Properties.Settings.Default.ShowOperatorsSidebar) {
        OperatorsSidebar operatorsSidebar = new OperatorsSidebar();
        operatorsSidebar.Dock = DockStyle.Left;
        operatorsSidebar.Show();
      }
      if (Properties.Settings.Default.ShowStartPage) {
        StartPage startPage = new StartPage();
        startPage.Show();
      }
    }
  }
}
