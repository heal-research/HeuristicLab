#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimization.Views {
  /// <summary>
  /// The base class for visual representations of items.
  /// </summary>
  [View("UserDefinedAlgorithm View")]
  [Content(typeof(UserDefinedAlgorithm), true)]
  public sealed partial class UserDefinedAlgorithmView : EngineAlgorithmView {
    public new UserDefinedAlgorithm Content {
      get { return (UserDefinedAlgorithm)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemBaseView"/>.
    /// </summary>
    public UserDefinedAlgorithmView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null)
        globalScopeView.Content = null;
      else
        globalScopeView.Content = Content.GlobalScope;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      globalScopeView.Enabled = Content != null;
      operatorGraphViewHost.ReadOnly = Content == null || ReadOnly;
    }
  }
}
