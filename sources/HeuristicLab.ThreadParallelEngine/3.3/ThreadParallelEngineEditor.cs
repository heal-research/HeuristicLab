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
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.ThreadParallelEngine {
  /// <summary>
  /// Visual representation of a <see cref="ThreadParallelEngine"/>.
  /// </summary>
  [Content(typeof(ThreadParallelEngine), true)]
  public partial class ThreadParallelEngineEditor : EngineBaseEditor {
    /// <summary>
    /// Gets or sets the ThreadParallelEngine to display.
    /// </summary>
    /// <remarks>Uses property <see cref="EngineBaseEditor.Engine"/> of base class 
    /// <see cref="EngineBaseEditor"/>. No own data storage present.</remarks>
    public ThreadParallelEngine ThreadParallelEngine {
      get { return (ThreadParallelEngine)Engine; }
      set { base.Engine = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ThreadParallelEngineEditor"/>.
    /// </summary>
    public ThreadParallelEngineEditor() {
      InitializeComponent();
    }
    /// <summary>
    /// Initializes a new instance of <see cref="ThreadParallelEngineEditor"/> with the given
    /// <paramref name="threadParallelEngine"/> to display.
    /// </summary>
    /// <param name="threadParallelEngine">The engine to represent visually.</param>
    public ThreadParallelEngineEditor(ThreadParallelEngine threadParallelEngine)
      : this() {
      ThreadParallelEngine = threadParallelEngine;
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="ThreadParallelEngine"/>.
    /// </summary>
    /// <remarks>Calls <see cref="EngineBaseEditor.RemoveItemEvents"/> of base class 
    /// <see cref="EngineBaseEditor"/>.
    /// </remarks>
    protected override void RemoveItemEvents() {
      ThreadParallelEngine.WorkersChanged -= new EventHandler(ThreadParallelEngine_WorkersChanged);
      base.RemoveItemEvents();
    }

    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="ThreadParallelEngine"/>.
    /// </summary>
    /// <remarks>Calls <see cref="EngineBaseEditor.AddItemEvents"/> of base class 
    /// <see cref="EngineBaseEditor"/>.
    /// </remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      ThreadParallelEngine.WorkersChanged += new EventHandler(ThreadParallelEngine_WorkersChanged);
    }

    /// <summary>
    /// Updates the controls with the latest data.
    /// </summary>
    /// <remarks>Calls <see cref="EngineBaseEditor.UpdateControls"/> of base class 
    /// <see cref="EngineBaseEditor"/>.</remarks>
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
