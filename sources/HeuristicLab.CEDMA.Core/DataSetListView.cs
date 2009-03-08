#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;

namespace HeuristicLab.CEDMA.Core {
  public partial class DataSetListView : ViewBase {
    public DataSetList DataSetList {
      get { return (DataSetList)Item; }
      set { base.Item = value; }
    }

    public DataSetListView() {
      InitializeComponent();
      Caption = "DataSet View";
    }
    public DataSetListView(DataSetList dataSetList)
      : this() {
      DataSetList = dataSetList;
      dataSetList.Changed += new EventHandler(dataSetList_Changed);
    }

    void dataSetList_Changed(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((EventHandler)dataSetList_Changed, sender, e);
      else UpdateControls();
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      detailsGroupBox.Controls.Clear();
      detailsGroupBox.Enabled = false;
      if(DataSetList == null) {
        Caption = "Data Sets View";
        dataSetsListView.Enabled = false;
      } else {
        dataSetsListView.Enabled = true;
        dataSetsListView.Items.Clear();
        foreach(DataSet dataSet in DataSetList) {
          ListViewItem node = new ListViewItem();
          node.Text = dataSet.Name;
          node.Tag = dataSet;
          dataSetsListView.Items.Add(node);
        }
      }
    }

    #region Button Events
    private void addButton_Click(object sender, EventArgs e) {
      DataSet dataSet = new DataSet();
      dataSet.Store = DataSetList.Store;
      DataSetList.Add(dataSet);
      UpdateControls();
    }
    private void dataSetsListView_SelectedIndexChanged(object sender, EventArgs e) {
      if(dataSetsListView.SelectedItems.Count > 0) {
        if(detailsGroupBox.Controls.Count > 0)
          detailsGroupBox.Controls[0].Dispose();
        detailsGroupBox.Controls.Clear();
        detailsGroupBox.Enabled = false;
        IViewable viewable = (IViewable)dataSetsListView.SelectedItems[0].Tag;
        Control control = (Control)viewable.CreateView();
        detailsGroupBox.Controls.Add(control);
        control.Dock = DockStyle.Fill;
        detailsGroupBox.Enabled = true;
      }
    }
    #endregion


    private void refreshButton_Click(object sender, EventArgs e) {
      UpdateControls();
    }
  }
}

