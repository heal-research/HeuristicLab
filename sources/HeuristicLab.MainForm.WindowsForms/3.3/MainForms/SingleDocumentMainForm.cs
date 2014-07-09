#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;

namespace HeuristicLab.MainForm.WindowsForms {
  public partial class SingleDocumentMainForm : MainForm {
    public SingleDocumentMainForm()
      : base() {
      InitializeComponent();
    }
    public SingleDocumentMainForm(Type userInterfaceItemType)
      : base(userInterfaceItemType) {
      InitializeComponent();
    }
    public SingleDocumentMainForm(Type userInterfaceItemType, bool showContentInViewHost)
      : this(userInterfaceItemType) {
      this.ShowContentInViewHost = showContentInViewHost;
    }

    protected override void ShowView(IView view, bool firstTimeShown) {
      if (InvokeRequired) Invoke((Action<IView, bool>)ShowView, view, firstTimeShown);
      else {
        base.ShowView(view, firstTimeShown);
        if (firstTimeShown)
          this.GetForm(view).Show(this);
        else {
          this.GetForm(view).Visible = true;
          this.GetForm(view).Activate();
        }
      }
    }

    protected override void Hide(IView view) {
      if (InvokeRequired) Invoke((Action<IView>)Hide, view);
      else {
        base.Hide(view);
        Form form = this.GetForm(view);
        if (form != null) {
          form.Hide();
        }
      }
    }

    protected override Form CreateForm(IView view) {
      Form form = new DocumentForm(view);
      form.ShowInTaskbar = true;
      return form;
    }
  }
}
