#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.GeneticProgramming.Robocode;

namespace HeuristicLab.Problems.GeneticProgramming.Views.Robocode {
  [View("CodeSymbol View")]
  [Content(typeof(CodeSymbol), IsDefaultView = true)]
  public partial class CodeNodeView : ItemView {
    public new CodeSymbol Content {
      get { return (CodeSymbol)base.Content; }
      set { base.Content = value; }
    }

    public CodeNodeView()
      : base() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        prefixCode.Text = string.Empty;
        suffixCode.Text = string.Empty;
      } else {
        prefixCode.Text = Content.Prefix;
        suffixCode.Text = Content.Suffix;
      }
    }

    private void suffixCode_Validated(object sender, EventArgs e) {
      Content.Suffix = suffixCode.Text;
    }

    private void prefixCode_Validated(object sender, EventArgs e) {
      Content.Prefix = prefixCode.Text;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();

      prefixCode.Enabled = Content != null && !Locked && !ReadOnly;
      suffixCode.Enabled = Content != null && !Locked && !ReadOnly;
    }
  }
}
