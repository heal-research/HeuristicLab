#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// The visual representation of a <see cref="Variable"/>.
  /// </summary>
  [View("Variable Value View")]
  [Content(typeof(Variable), false)]
  [Content(typeof(IVariable), false)]
  public partial class VariableValueView : ItemView {
    /// <summary>
    /// Gets or sets the variable to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public new IVariable Content {
      get { return (IVariable)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VariableView"/> with caption "Variable".
    /// </summary>
    public VariableValueView() {
      InitializeComponent();
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="Variable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void DeregisterContentEvents() {
      Content.ValueChanged -= new EventHandler(Content_ValueChanged);
      base.DeregisterContentEvents();
    }

    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="Variable"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ValueChanged += new EventHandler(Content_ValueChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        viewHost.Content = null;
      } else {
        viewHost.ViewType = null;
        viewHost.Content = Content.Value;
      }
    }

    protected virtual void Content_ValueChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ValueChanged), sender, e);
      else {
        viewHost.ViewType = null;
        viewHost.Content = Content.Value;
      }
    }

    protected virtual void VariableValueView_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (!ReadOnly && (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is IItem)) {
        if ((e.KeyState & 32) == 32) e.Effect = DragDropEffects.Link;  // ALT key
        else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Copy)) e.Effect = DragDropEffects.Copy;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Move)) e.Effect = DragDropEffects.Move;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Link)) e.Effect = DragDropEffects.Link;
      }
    }
    protected virtual void VariableValueView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        IItem item = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) as IItem;
        if (e.Effect.HasFlag(DragDropEffects.Copy)) item = (IItem)item.Clone();
        Content.Value = item;
      }
    }
  }
}
