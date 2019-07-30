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

using System.Windows.Forms;
using HeuristicLab.Data.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Encodings.PermutationEncoding.Views {
  [View("Permutation View")]
  [Content(typeof(Permutation), IsDefaultView = true)]
  public partial class PermutationView : StringConvertibleArrayView {
    public new Permutation Content {
      get { return (Permutation)base.Content; }
      set { base.Content = value; }
    }

    public PermutationView() {
      InitializeComponent();
      dataGridView.Top = permutationTypeView.Bottom + permutationTypeView.Margin.Bottom + dataGridView.Margin.Top;
      dataGridView.Height = Bottom - dataGridView.Top;
    }

    #region Register Content Events
    protected override void DeregisterContentEvents() {
      Content.PermutationTypeChanged -= new System.EventHandler(Content_PermutationTypeChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.PermutationTypeChanged += new System.EventHandler(Content_PermutationTypeChanged);
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        if (permutationTypeView.Content != null)
          permutationTypeView.Content.ValueChanged -= new System.EventHandler(PermutationTypeView_ValueChanged);
        permutationTypeView.Content = null;
      } else {
        permutationTypeView.Content = new PermutationType(Content.PermutationType);
        permutationTypeView.Content.ValueChanged += new System.EventHandler(PermutationTypeView_ValueChanged);
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
    }

    #region Event Handlers

    private void Content_PermutationTypeChanged(object sender, System.EventArgs e) {
      if (permutationTypeView.Content.Value != Content.PermutationType)
        permutationTypeView.Content.Value = Content.PermutationType;
    }

    private void PermutationTypeView_ValueChanged(object sender, System.EventArgs e) {
      if (permutationTypeView.Content.Value != Content.PermutationType)
        Content.PermutationType = permutationTypeView.Content.Value;
    }
    #endregion
  }
}
