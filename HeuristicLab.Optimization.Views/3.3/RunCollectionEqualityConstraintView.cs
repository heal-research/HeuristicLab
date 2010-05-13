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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimization.Views {
  [Content(typeof(RunCollectionEqualityConstraint),true)]
  public partial class RunCollectionEqualityConstraintView : RunCollectionConstraintView {
    public RunCollectionEqualityConstraintView() {
      InitializeComponent();
    }

    public new RunCollectionEqualityConstraint Content {
      get { return (RunCollectionEqualityConstraint)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null || Content.ConstraintData == null)
        this.txtConstraintData.Text = string.Empty;
      else
        this.txtConstraintData.Text = Content.ConstraintData.ToString();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      txtConstraintData.ReadOnly = Content == null || this.ReadOnly ;
    }

    private void txtConstraintData_TextChanged(object sender, EventArgs e) {
      Content.ConstraintData = txtConstraintData.Text;
    }
  }
}
