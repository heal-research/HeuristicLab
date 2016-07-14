#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.DataPreprocessing.Filter;

namespace HeuristicLab.DataPreprocessing {
  [Item("Filter", "Represents the filter grid.")]
  public class FilterContent : Item, IViewShortcut {

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Filter; }
    }

    private ICheckedItemCollection<IFilter> filters = new CheckedItemCollection<IFilter>();

    public FilterLogic FilterLogic { get; private set; }

    public ICheckedItemCollection<IFilter> Filters {
      get {
        return this.filters;
      }
      set {
        this.filters = value;
      }
    }

    private bool isAndCombination = true;
    public bool IsAndCombination {
      get {
        return this.isAndCombination;
      }
      set {
        this.isAndCombination = value;
      }
    }

    public FilterContent(FilterLogic filterLogic) {
      FilterLogic = filterLogic;
    }

    protected FilterContent(FilterContent content, Cloner cloner)
      : base(content, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new FilterContent(this, cloner);
    }
  }
}
