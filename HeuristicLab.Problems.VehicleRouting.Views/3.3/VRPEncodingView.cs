#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.VehicleRouting.Encodings;

namespace HeuristicLab.Problems.VehicleRouting.Views {
  /// <summary>
  /// The base class for visual representations of items.
  /// </summary>
  [View("VRPEncoding View")]
  [Content(typeof(IVRPEncoding), true)]
  public sealed partial class VRPEncodingView : ItemView {
    public new IVRPEncoding Content {
      get { return (IVRPEncoding)base.Content; }
      set { base.Content = value; }
    }

    public VRPEncodingView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();

      if (Content != null) {
        typeTextBox.Text = Content.GetType().Name;
        valueTextBox.Text = Content.ToString();
      } else {
        typeTextBox.Text = string.Empty;
        valueTextBox.Text = string.Empty;
      }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();

      Content.ToStringChanged += new EventHandler(Content_ToStringChanged);
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();

      Content.ToStringChanged -= new EventHandler(Content_ToStringChanged);
    }

    void Content_ToStringChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ToStringChanged), sender, e);
      else {
        valueTextBox.Text = Content.ToString();
      }
    }

  }
}
