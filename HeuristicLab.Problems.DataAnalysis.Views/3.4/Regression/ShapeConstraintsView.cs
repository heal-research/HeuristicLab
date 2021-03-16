#region License Information

/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("ShapeConstraints View")]
  [Content(typeof(ShapeConstraints), true)]
  public partial class ShapeConstraintsView : AsynchronousContentView {
    public new ShapeConstraints Content {
      get => (ShapeConstraints)base.Content;
      set => base.Content = value;
    }

    public bool suspendUpdates = false;

    public ShapeConstraintsView() {
      InitializeComponent();
      errorOutput.Text = "";
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      this.shapeConstraintsView.Content = Content;
      UpdateControl();
      errorOutput.Text = "";
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemsAdded += constraints_Added;
      Content.ItemsRemoved += constraint_Removed;
      Content.Changed += Content_Changed;
      Content.CollectionReset += constraints_Reset;
      Content.ItemsMoved += constraints_Moved;
      Content.ItemsReplaced += Content_ItemsReplaced;
      Content.CheckedItemsChanged += Content_CheckedItemsChanged;
    }


    protected override void DeregisterContentEvents() {
      Content.ItemsAdded -= constraints_Added;
      Content.ItemsRemoved -= constraint_Removed;
      Content.Changed -= Content_Changed;
      Content.CollectionReset -= constraints_Reset;
      Content.ItemsMoved -= constraints_Moved;
      Content.ItemsReplaced -= Content_ItemsReplaced;
      Content.CheckedItemsChanged -= Content_CheckedItemsChanged;
      base.DeregisterContentEvents();
    }

    protected override void SetEnabledStateOfControls() {
      constraintsInput.Enabled = Content != null && !Locked && !ReadOnly;
      parseBtn.Enabled = Content != null && !Locked && !ReadOnly;
    }


    private void parseBtn_Click(object sender, EventArgs e) {
      if (constraintsInput.Text != null) {
        suspendUpdates = true;
        Content.Clear();
        try {
          var parsedConstraints = ShapeConstraintsParser.ParseConstraints(constraintsInput.Text);
          Content.AddRange(parsedConstraints);
          errorOutput.Text = "Constraints successfully parsed.";
          errorOutput.ForeColor = Color.DarkGreen;
        } catch (ArgumentException ex) {
          errorOutput.Text = ex.Message;
          errorOutput.ForeColor = Color.DarkRed;
        } finally {
          suspendUpdates = false;
        }
      } else {
        errorOutput.Text = "No constraints were found!";
      }
    }

    private void UpdateControl() {
      if (suspendUpdates) return;
      if (Content == null) {
        constraintsInput.Text = string.Empty;
      } else {
        var newText = ToString(Content);
        if (newText != constraintsInput.Text)
          constraintsInput.Text = newText;
      }
    }

    private string ToString(ShapeConstraints constraints) {
      var sb = new StringBuilder();
      foreach (var constraint in constraints) {
        if (!constraints.ItemChecked(constraint)) {
          sb.Append("# ").AppendLine(constraint.ToString());
        } else {
          sb.AppendLine(constraint.ToString());
        }
      }
      return sb.ToString();
    }

    private void constraint_Changed(object sender, EventArgs e) {
      UpdateControl();
    }

    private void constraints_Added(object sender,
                                     CollectionItemsChangedEventArgs<IndexedItem<ShapeConstraint>> e) {
      foreach (var addedItem in e.Items) addedItem.Value.Changed += constraint_Changed;
      UpdateControl();
    }

    private void constraint_Removed(object sender, CollectionItemsChangedEventArgs<IndexedItem<ShapeConstraint>> e) {
      foreach (var removedItem in e.Items) removedItem.Value.Changed -= constraint_Changed;
      UpdateControl();
    }

    private void constraints_Moved(object sender, CollectionItemsChangedEventArgs<IndexedItem<ShapeConstraint>> e) {
      UpdateControl();
    }

    private void constraints_Reset(object sender, CollectionItemsChangedEventArgs<IndexedItem<ShapeConstraint>> e) {
      foreach (var addedItem in e.Items) addedItem.Value.Changed += constraint_Changed;
      foreach (var removedItem in e.OldItems) removedItem.Value.Changed -= constraint_Changed;
      UpdateControl();
    }

    private void Content_CheckedItemsChanged(object sender, CollectionItemsChangedEventArgs<IndexedItem<ShapeConstraint>> e) {
      UpdateControl();
    }

    private void Content_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<ShapeConstraint>> e) {
      foreach (var addedItem in e.Items) addedItem.Value.Changed += constraint_Changed;
      foreach (var removedItem in e.OldItems) removedItem.Value.Changed -= constraint_Changed;
      UpdateControl();
    }

    private void constraintsInput_TextChanged(object sender, EventArgs e) {
      errorOutput.Text = "Unparsed changes! Press parse button to save changes.";
      errorOutput.ForeColor = Color.DarkOrange;
    }

    private void Content_Changed(object sender, EventArgs e) {
      UpdateControl();
    }

    private void helpButton_DoubleClick(object sender, EventArgs e) {
      using (InfoBox dialog = new InfoBox("Help for shape constraints",
        "HeuristicLab.Problems.DataAnalysis.Views.Resources.shapeConstraintsHelp.rtf",
        this)) {
        dialog.ShowDialog(this);
      }
    }
  }
}