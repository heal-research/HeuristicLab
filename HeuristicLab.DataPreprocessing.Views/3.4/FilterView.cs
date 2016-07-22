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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Core.Views;
using HeuristicLab.DataPreprocessing.Filter;
using HeuristicLab.MainForm;

namespace HeuristicLab.DataPreprocessing.Views {
  [View("CheckedFilterCollection View")]
  [Content(typeof(FilterContent), true)]
  public partial class FilterView : ItemView {

    public FilterView() {
      InitializeComponent();
      tbTotal.Text = "0";
      tbRemaining.Text = "0";
      tbPercentage.Text = "0%";
    }

    private void InitData() {
      checkedFilterView.Content = Content.Filters;
      checkedFilterView.Content.ItemsAdded += Content_ItemsAdded;
      checkedFilterView.Content.ItemsRemoved += Content_ItemsRemoved;
      checkedFilterView.Content.CheckedItemsChanged += Content_CheckedItemsChanged;
    }

    public new FilterContent Content {
      get { return (FilterContent)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        InitData();
        UpdateFilterInfo();
      }
    }

    private void Content_CheckedItemsChanged(object sender, Collections.CollectionItemsChangedEventArgs<IFilter> e) {
      if (Content != null) {
        foreach (IFilter filter in e.Items) {
          filter.Active = checkedFilterView.Content.ItemChecked(filter);
        }
        UpdateFilterInfo();
      }
    }

    private void UpdateFilterInfo() {
      List<IFilter> filters = Content.Filters.ToList();
      int activeFilters = filters.Count(c => c.Active);
      applyFilterButton.Enabled = (activeFilters > 0);
      rBtnAnd.Enabled = (activeFilters > 0);
      rBtnOr.Enabled = (activeFilters > 0);
      Content.FilterLogic.Reset();
      bool[] result = Content.FilterLogic.Preview(filters, rBtnAnd.Checked);

      int filteredCnt = result.Count(c => !c);

      tbRemaining.Text = filteredCnt.ToString();
      double percentage = result.Length == 0 ? 0.0 : filteredCnt * 100 / (double)result.Length;
      tbPercentage.Text = String.Format("{0:0.0000}%", percentage);
      tbTotal.Text = result.Length.ToString();
    }

    private void applyFilterButton_Click(object sender, EventArgs e) {
      if (Content != null) {
        List<IFilter> filters = Content.Filters.ToList();
        //apply filters
        Content.FilterLogic.Apply(filters, rBtnAnd.Checked);
        //deactivate checked filters
        filters = checkedFilterView.Content.CheckedItems.ToList();
        foreach (IFilter filter in filters) {
          checkedFilterView.Content.SetItemCheckedState(filter, false);
          filter.Active = false;
        }
        UpdateFilterInfo();
      }
    }

    //whenever a new filter is added the preprocessing data is set to the filter
    private void Content_ItemsAdded(object sender, Collections.CollectionItemsChangedEventArgs<IFilter> e) {
      if (Content != null) {
        foreach (IFilter filter in e.Items) {
          filter.ConstrainedValue = Content.FilterLogic.PreprocessingData;
        }
      }
    }

    private void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IFilter> e) {
      if (Content != null) {
        UpdateFilterInfo();
      }
    }

    private void rBtnAnd_CheckedChanged(object sender, EventArgs e) {
      if (Content != null) {
        UpdateFilterInfo();
        Content.IsAndCombination = rBtnAnd.Checked;
      }
    }

  }
}
