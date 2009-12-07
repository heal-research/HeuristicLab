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

namespace HeuristicLab.Data {
  /// <summary>
  /// The visual representation of the class <see cref="BoolData"/>, symbolizing a boolean value. 
  /// </summary>
  [Content(typeof(BoolData), true)]
  public partial class BoolDataView : ItemViewBase {
    /// <summary>
    /// Gets or sets the boolean value to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="HeuristicLab.Core.ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public BoolData BoolData {
      get { return (BoolData)Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="BoolDataView"/>.
    /// </summary>
    public BoolDataView() {
      InitializeComponent();
    }
    /// <summary>
    /// Initializes a new instance of the class <see cref="BoolDataView"/> with the given 
    /// <paramref name="boolData"/>.
    /// <note type="caution"> No CopyConstructor! <paramref name="boolData"/> is not copied!</note>
    /// </summary>
    /// <param name="boolData">The boolean value to represent visually.</param>
    public BoolDataView(BoolData boolData)
      : this() {
      BoolData = boolData;
    }

    /// <summary>
    /// Removes the eventhandler from the underlying <see cref="BoolData"/>.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void RemoveItemEvents() {
      BoolData.Changed -= new EventHandler(BoolData_Changed);
      base.RemoveItemEvents();
    }
    /// <summary>
    /// Adds an eventhandler to the underlying <see cref="BoolData"/>.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      BoolData.Changed += new EventHandler(BoolData_Changed);
    }

    /// <summary>
    /// Sets the <c>dataCheckBox</c> checked or unchecked according to the latest value.
    /// </summary>
    protected override void UpdateControls() {
      base.UpdateControls();
      if (BoolData == null) {
        dataCheckBox.Enabled = false;
      } else {
        dataCheckBox.Enabled = true;
        dataCheckBox.Checked = BoolData.Data;
      }
    }

    private void dataCheckBox_CheckedChanged(object sender, EventArgs e) {
      BoolData.Data = dataCheckBox.Checked;
    }

    private void BoolData_Changed(object sender, EventArgs e) {
      Refresh();
    }
  }
}
