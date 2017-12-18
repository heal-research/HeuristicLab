#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Core.Views;
using HeuristicLab.DataPreprocessing.Filter;
using HeuristicLab.MainForm;

namespace HeuristicLab.DataPreprocessing.Views {
  [View("CheckedFilterCollection View")]
  [Content(typeof(FilterContent), true)]
  public partial class FilterView : ItemView {
    public new FilterContent Content {
      get { return (FilterContent)base.Content; }
      set { base.Content = value; }
    }

    public FilterView() {
      InitializeComponent();
      tbTotal.Text = "0";
      tbRemaining.Text = "0";
      tbPercentage.Text = "0%";
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        checkedFilterView.Content = Content.Filters;
        rBtnAnd.Checked = Content.IsAndCombination;
        rBtnOr.Checked = !Content.IsAndCombination;
        UpdateFilter();
      } else {
        checkedFilterView.Content = null;
      }
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Filters.ItemsAdded += Content_ItemsAdded;
      Content.Filters.ItemsRemoved += Content_ItemsRemoved;
      Content.Filters.CheckedItemsChanged += Content_CheckedItemsChanged;
    }
    protected override void DeregisterContentEvents() {
      Content.Filters.ItemsAdded -= Content_ItemsAdded;
      Content.Filters.ItemsRemoved -= Content_ItemsRemoved;
      Content.Filters.CheckedItemsChanged -= Content_CheckedItemsChanged;
      base.DeregisterContentEvents();
    }

    private void UpdateFilter() {
      bool activeFilters = Content.ActiveFilters.Any();
      applyFilterButton.Enabled = activeFilters;

      Content.PreprocessingData.ResetFilter();

      int numTotal = Content.PreprocessingData.Rows;
      int numRemaining = numTotal;

      if (activeFilters) {
        var remainingRows = Content.GetRemainingRows();
        numRemaining = remainingRows.Count(x => x);

        if (numRemaining < numTotal) {
          Content.PreprocessingData.SetFilter(remainingRows);
        }
      }

      tbRemaining.Text = numRemaining.ToString();
      double ratio = numTotal > 0 ? numRemaining / (double)numTotal : 0.0;
      tbPercentage.Text = ratio.ToString("P4");
      tbTotal.Text = numTotal.ToString();
    }


    #region Content Events
    //whenever a new filter is added the preprocessing data is set to the filter
    private void Content_ItemsAdded(object sender, Collections.CollectionItemsChangedEventArgs<IFilter> e) {
      if (Content != null) {
        foreach (IFilter filter in e.Items) {
          filter.ConstrainedValue = Content.PreprocessingData;
        }
      }
    }
    private void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IFilter> e) {
      if (Content != null) {
        UpdateFilter();
      }
    }
    private void Content_CheckedItemsChanged(object sender, Collections.CollectionItemsChangedEventArgs<IFilter> e) {
      if (Content != null) {
        foreach (IFilter filter in e.Items) {
          filter.Active = checkedFilterView.Content.ItemChecked(filter);
        }
        UpdateFilter();
      }
    }
    #endregion

    #region Controls Events
    private void rBtnAnd_CheckedChanged(object sender, EventArgs e) {
      if (Content != null) {
        Content.IsAndCombination = rBtnAnd.Checked;
        UpdateFilter();
      }
    }
    private void applyFilterButton_Click(object sender, EventArgs e) {
      if (Content != null) {
        //apply filters
        Content.PreprocessingData.PersistFilter();
        Content.PreprocessingData.ResetFilter();
        //deactivate checked filters
        foreach (var filter in Content.Filters.CheckedItems.ToList()) {
          checkedFilterView.Content.SetItemCheckedState(filter, false);
          filter.Active = false;
        }
        UpdateFilter();
      }
    }
    #endregion
  }
}
