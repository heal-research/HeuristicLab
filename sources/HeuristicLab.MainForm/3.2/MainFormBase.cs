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
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.MainForm {
  public partial class MainFormBase : Form, IMainForm {
    public MainFormBase()
      : base() {
      InitializeComponent();
      openViews = new List<IView>();
    }

    public MainFormBase(Type userInterfaceItemType)
      : this() {
      this.userInterfaceItemType = userInterfaceItemType;
    }

    #region IMainForm Members
    public string Title {
      get { return this.Title; }
      set { this.Title = value; }
    }

    public string StatusStripText {
      get { return this.statusStripLabel.Text; }
      set { this.statusStripLabel.Text = value; }
    }

    protected Type userInterfaceItemType;
    public Type UserInterfaceItemType {
      get { return this.userInterfaceItemType; }
    }

    protected IModel model;
    public IModel Model {
      get { return this.model; }
      set { this.model = value; }
    }

    protected IView activeView;
    public IView ActiveView {
      get { return this.activeView; }
    }

    protected List<IView> openViews;
    public IEnumerable<IView> OpenViews {
      get { return openViews; }
    }

    public virtual void ShowView(IView view) {
      view.MainForm = this;
      activeView = view;
      openViews.Add(view);
    }

    #endregion
  }
}
