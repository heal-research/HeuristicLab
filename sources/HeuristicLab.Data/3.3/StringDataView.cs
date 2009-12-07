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
  /// The visual representation of the class <see cref="StringData"/>, symbolizing a string value.
  /// </summary>
  [Content(typeof(StringData), true)]
  public partial class StringDataView : ItemViewBase {
    /// <summary>
    /// Gets or sets the string value to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="HeuristicLab.Core.ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public StringData StringData {
      get { return (StringData)Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="StringDataView"/>.
    /// </summary>
    public StringDataView() {
      InitializeComponent();
    }
    /// <summary>
    /// Initializes a new instance of the class <see cref="StringDataView"/> with the given
    /// <paramref name="stringData"/>.
    /// <note type="caution"> No CopyConstructor! <paramref name="stringData"/> is not copied!</note>
    /// </summary>
    /// <param name="stringData">The string value to represent visually.</param>
    public StringDataView(StringData stringData)
      : this() {
      StringData = stringData;
    }

    /// <summary>
    /// Removes the eventhandler from the underlying <see cref="StringData"/>.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void RemoveItemEvents() {
      StringData.Changed -= new EventHandler(StringData_Changed);
      base.RemoveItemEvents();
    }
    /// <summary>
    /// Adds an eventhandler to the underlying <see cref="StringData"/>.
    /// </summary>
    /// <remarks>Calls <see cref="HeuristicLab.Core.ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      StringData.Changed += new EventHandler(StringData_Changed);
    }

    /// <summary>
    /// Update the controls with the latest string value.
    /// </summary>
    protected override void UpdateControls() {
      base.UpdateControls();
      if (StringData == null) {
        dataTextBox.Enabled = false;
      } else {
        dataTextBox.Enabled = true;
        dataTextBox.Text = StringData.ToString();
      }
    }

    private void dataTextBox_Validating(object sender, CancelEventArgs e) {
      e.Cancel = false;
      StringData.Data = dataTextBox.Text;
    }

    private void StringData_Changed(object sender, EventArgs e) {
      Refresh();
    }
  }
}
