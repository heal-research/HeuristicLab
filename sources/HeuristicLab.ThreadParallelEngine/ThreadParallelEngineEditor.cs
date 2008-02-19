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

namespace HeuristicLab.ThreadParallelEngine {
  public partial class ThreadParallelEngineEditor : EngineBaseEditor {
    public ThreadParallelEngine ThreadParallelEngine {
      get { return (ThreadParallelEngine)Engine; }
      set { base.Engine = value; }
    }

    public ThreadParallelEngineEditor() {
      InitializeComponent();
    }
    public ThreadParallelEngineEditor(ThreadParallelEngine threadParallelEngine)
      : this() {
      ThreadParallelEngine = threadParallelEngine;
    }

    protected override void RemoveItemEvents() {
      ThreadParallelEngine.WorkersChanged -= new EventHandler(ThreadParallelEngine_WorkersChanged);
      base.RemoveItemEvents();
    }

    protected override void AddItemEvents() {
      base.AddItemEvents();
      ThreadParallelEngine.WorkersChanged += new EventHandler(ThreadParallelEngine_WorkersChanged);
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (ThreadParallelEngine == null) {
        workersNumericUpDown.Value = 1;
        workersNumericUpDown.Enabled = false;
      } else {
        workersNumericUpDown.Value = ThreadParallelEngine.Workers;
        workersNumericUpDown.Enabled = true;
      }
    }

    #region ThreadParallelEngine Events
    private void ThreadParallelEngine_WorkersChanged(object sender, EventArgs e) {
      workersNumericUpDown.Value = ThreadParallelEngine.Workers;
    }
    #endregion

    #region NumericUpDown Events
    private void workersNumericUpDown_ValueChanged(object sender, EventArgs e) {
      ThreadParallelEngine.Workers = (int)workersNumericUpDown.Value;
    }
    #endregion
  }
}
