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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.Data.Views;
using HeuristicLab.Collections;

namespace HeuristicLab.Optimization.Views {
  [View("RunCollection Tabular View")]
  [Content(typeof(RunCollection), false)]
  public partial class RunCollectionTabularView : StringConvertibleMatrixView {
    public RunCollectionTabularView() {
      InitializeComponent();
      Caption = "Run Collection";
      this.dataGridView.RowHeaderMouseDoubleClick += new DataGridViewCellMouseEventHandler(dataGridView_RowHeaderMouseDoubleClick);
    }

    public RunCollectionTabularView(RunCollection content)
      : this() {
      Content = content;
    }

    public new RunCollection Content {
      get { return (RunCollection) base.Content; }
      set { base.Content = value; }
    }

    private void dataGridView_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
      if (e.RowIndex > 0)
        MainFormManager.CreateDefaultView(Content.ElementAt(e.RowIndex)).Show();
    }
  }
}
