#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimization.Views {
  [View("ResultCollection View")]
  [Content(typeof(ResultCollection), true)]
  [Content(typeof(IObservableKeyedCollection<string, IResult>), false)]
  public partial class ResultCollectionView : NamedItemCollectionView<IResult> {
    public override bool ReadOnly {
      get { return base.ReadOnly; }
      set { /*not needed because results are always readonly */}
    }
    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with caption "Variables Scope View".
    /// </summary>
    public ResultCollectionView() {
      InitializeComponent();
      Caption = "ResultCollection";
      itemsGroupBox.Text = "Results";
      base.ReadOnly = true;
    }
    /// <summary>
    /// Initializes a new instance of <see cref="VariablesScopeView"/> with 
    /// the given <paramref name="scope"/>.
    /// </summary>
    /// <remarks>Calls <see cref="VariablesScopeView()"/>.</remarks>
    /// <param name="scope">The scope whose variables should be represented visually.</param>
    public ResultCollectionView(IObservableKeyedCollection<string, IResult> content)
      : this() {
      Content = content;
    }

    protected override IResult CreateItem() {
      IResult item = new Result();
      if (Content.ContainsKey(item.Name))
        item = new Result(GetUniqueName(item.Name), typeof(IItem));
      return item;
    }
  }
}
