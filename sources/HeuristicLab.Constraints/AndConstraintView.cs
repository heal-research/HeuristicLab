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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Constraints {
  public partial class AndConstraintView : ViewBase {
    public AndConstraint AndConstraint {
      get { return (AndConstraint)Item; }
      set { base.Item = value; }
    }

    public AndConstraintView() {
      InitializeComponent();
    }
    public AndConstraintView(AndConstraint andConstraint)
      : this() {
      AndConstraint = andConstraint;
    }

    protected override void RemoveItemEvents() {
      AndConstraint.Changed -= new EventHandler(AndConstraint_Changed);
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      AndConstraint.Changed += new EventHandler(AndConstraint_Changed);
    }

    protected override void UpdateControls() {
      if (AndConstraint == null) {
        clausesItemListView.Enabled = false;
        clausesItemListView.ItemList = null;
      } else {
        clausesItemListView.ItemList = AndConstraint.Clauses;
        clausesItemListView.Enabled = true;
      }
    }

    void AndConstraint_Changed(object sender, EventArgs e) {
      Refresh();
    }
  }
}
