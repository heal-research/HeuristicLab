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
using HeuristicLab.Tracing;

namespace HeuristicLab.Hive.Engine {
  /// <summary>
  /// Visual representation of a <see cref="HiveEngine"/>.
  /// </summary>
  public partial class HiveEngineEditor : EngineBaseEditor {
    /// <summary>
    /// Gets or set the engine to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="EngineBaseEditor.Engine"/> of base class 
    /// <see cref="EngineBaseEditor"/>. No own data storage present.</remarks>
    public HiveEngine HiveEngine {
      get { return (HiveEngine)Engine; }
      set {
        if (base.Engine != null) RemoveItemEvents();
        base.Engine = value;
        AddItemEvents();
        SetDataBinding();
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="HiveEngineEditor"/>.
    /// </summary>
    public HiveEngineEditor() {
      InitializeComponent();
    }
    /// <summary>
    /// Initializes a new instance of <see cref="HiveEngineEditor"/> with the given
    /// <paramref name="hiveEngine"/>.
    /// </summary>
    /// <param name="hiveEngine">The engine to display.</param>
    public HiveEngineEditor(HiveEngine hiveEngine)
      : this() {
      HiveEngine = hiveEngine;
      base.executeButton.Click += new EventHandler(executeButton_Click);
      base.abortButton.Click += new EventHandler(abortButton_Click);
    }

    void abortButton_Click(object sender, EventArgs e) {
      snapshotButton.Enabled = false;
    }

    void executeButton_Click(object sender, EventArgs e) {
      abortButton.Enabled = true;
      snapshotButton.Enabled = true;
    }

    private void SetDataBinding() {
      urlTextBox.DataBindings.Add("Text", HiveEngine, "HiveServerUrl");
      multiSubmitTextbox.DataBindings.Add("Text", HiveEngine, "MultiSubmitCount");
      assignedRessourceTextBox.DataBindings.Add("Text", HiveEngine, "RessourceIds");         
    }

    protected override void RemoveItemEvents() {
      Engine.Initialized -= new EventHandler(Engine_Initialized);
      Engine.Finished -= new EventHandler(Engine_Finished);
      base.RemoveItemEvents();
    }

    protected override void AddItemEvents() {
      base.AddItemEvents();
      Engine.Finished += new EventHandler(Engine_Finished);
      Engine.Initialized += new EventHandler(Engine_Initialized);
    }

    void Engine_Initialized(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke((EventHandler)Engine_Initialized, sender, e);
      } else {
        abortButton.Enabled = false;
        snapshotButton.Enabled = false;
      }
    }

    void Engine_Finished(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke((EventHandler)Engine_Initialized, sender, e);
      } else {
        abortButton.Enabled = false;
        snapshotButton.Enabled = false;
      }
    }

    private void snapshotButton_Click(object sender, EventArgs e) {
      BackgroundWorker worker = new BackgroundWorker();
      worker.DoWork += (s, args) => {
        HiveEngine.RequestSnapshot();
      };
      worker.RunWorkerCompleted += (s, args) => {
        Logger.Debug("HiveEngineEditor: RunWorkerCompleted");
        this.Cursor = Cursors.Default;
        abortButton.Enabled = true;
        snapshotButton.Enabled = true;
      };
      this.Cursor = Cursors.WaitCursor;
      abortButton.Enabled = false;
      snapshotButton.Enabled = false;
      Logger.Debug("HiveEngineEditor: RunWorkerAsync");
      worker.RunWorkerAsync();
    }
  }
}
