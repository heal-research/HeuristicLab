#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.Hive.JobManager.Views {
  [View("Refreshable Hive Job List")]
  [Content(typeof(ItemCollection<RefreshableJob>), false)]
  public partial class RefreshableHiveJobListView : HeuristicLab.Core.Views.ItemCollectionView<RefreshableJob> {
    public RefreshableHiveJobListView() {
      InitializeComponent();
      itemsGroupBox.Text = "Jobs";
      this.itemsListView.View = System.Windows.Forms.View.Details;
      this.itemsListView.Columns.Clear();
      this.itemsListView.Columns.Add(new ColumnHeader("Date") { Text = "Date" });
      this.itemsListView.Columns.Add(new ColumnHeader("Name") { Text = "Name" });

      this.itemsListView.HeaderStyle = ColumnHeaderStyle.Clickable;
      this.itemsListView.FullRowSelect = true;

      this.itemsListView.ListViewItemSorter = new ListViewItemDateComparer(0, SortOrder.Ascending);
    }

    protected override void SortItemsListView(SortOrder sortOrder) {
      if (itemsListView.Sorting == sortOrder || sortOrder == SortOrder.None) return;
      ((ListViewItemDateComparer)itemsListView.ListViewItemSorter).Order = sortOrder;
      itemsListView.Sorting = sortOrder;
      itemsListView.Sort();
      AdjustListViewColumnSizes();
    }

    protected override RefreshableJob CreateItem() {
      var refreshableJob = new RefreshableJob();
      refreshableJob.Job.Name = "New Hive Job";
      return refreshableJob;
    }

    protected override void OnLockedChanged() {
      base.OnLockedChanged();

      itemsListView.Enabled = !Locked;
      addButton.Enabled = !Locked;
      sortAscendingButton.Enabled = !Locked;
      sortDescendingButton.Enabled = !Locked;
      removeButton.Enabled = !Locked;
    }

    protected override void SetEnabledStateOfControls() {
      // if the view is locked, a job is currently beeing deleted and everything should be disabled
      if (!Locked)
        base.SetEnabledStateOfControls();
    }

    protected override void removeButton_Click(object sender, EventArgs e) {
      DialogResult result = MessageBox.Show("This action will permanently delete this job (also on the Hive server). Continue?", "HeuristicLab Hive Job Manager", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (result == DialogResult.Yes) {
        System.Windows.Forms.ListView.SelectedListViewItemCollection selectedItems = itemsListView.SelectedItems;
        bool inProgress = false;
        foreach (ListViewItem item in selectedItems) {
          RefreshableJob job = item.Tag as RefreshableJob;
          if (job != null && job.IsProgressing) {
            inProgress = true;
            break;
          }
        }

        if (inProgress) {
          MessageBox.Show("You can't delete jobs which are currently uploading or downloading." + Environment.NewLine + "Please wait for the jobs to complete and try again. ", "HeuristicLab Hive Job Manager", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          return;
        } else {
          DeleteHiveJobs(e);
        }
      }
    }

    private void DeleteHiveJobs(EventArgs e) {
      if (itemsListView.SelectedItems.Count > 0) {
        List<RefreshableJob> items = new List<RefreshableJob>();
        foreach (ListViewItem item in itemsListView.SelectedItems)
          items.Add((RefreshableJob)item.Tag);

        var task = System.Threading.Tasks.Task.Factory.StartNew(DeleteHiveJobsAsync, items);

        task.ContinueWith((t) => {
          MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>().RemoveOperationProgressFromView(this);
          ErrorHandling.ShowErrorDialog("An error occured while deleting the job. ", t.Exception);
        }, TaskContinuationOptions.OnlyOnFaulted);

        task.ContinueWith((t) => {
          itemsListView.Invoke(new Action(() => itemsListView.SelectedItems.Clear()));
        }, TaskContinuationOptions.OnlyOnRanToCompletion);
      }
    }

    private void DeleteHiveJobsAsync(object items) {
      MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>().AddOperationProgressToView(this, "Deleting job...");
      foreach (RefreshableJob item in (List<RefreshableJob>)items) {
        Content.Remove(item);
      }
      MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>().RemoveOperationProgressFromView(this);
    }

    protected override void Content_ItemsAdded(object sender, Collections.CollectionItemsChangedEventArgs<RefreshableJob> e) {
      if (InvokeRequired) {
        Invoke(new CollectionItemsChangedEventHandler<RefreshableJob>(Content_ItemsAdded), sender, e);
      } else {
        base.Content_ItemsAdded(sender, e);
        foreach (ColumnHeader c in this.itemsListView.Columns) {
          c.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
        foreach (var item in e.Items) {
          item.ItemImageChanged += new EventHandler(item_ItemImageChanged);
        }
      }
    }

    void item_ItemImageChanged(object sender, EventArgs e) {
      if (this.itemsListView.InvokeRequired) {
        Invoke(new EventHandler(item_ItemImageChanged), sender, e);
      } else {
        RefreshableJob job = sender as RefreshableJob;
        if (job != null) {
          foreach (ListViewItem item in this.itemsListView.Items) {
            if (item.Tag != null) {
              RefreshableJob cur = item.Tag as RefreshableJob;
              if (cur != null && cur == job) {
                this.UpdateListViewItemImage(item);
              }
            }
          }
        }
      }
    }

    protected override void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<RefreshableJob> e) {
      if (InvokeRequired) {
        Invoke(new CollectionItemsChangedEventHandler<RefreshableJob>(Content_ItemsRemoved), sender, e);
      } else {
        base.Content_ItemsRemoved(sender, e);
        foreach (var item in e.Items) {
          item.ItemImageChanged -= new EventHandler(item_ItemImageChanged);
        }
        if (Content != null && Content.Count == 0) {
          foreach (ColumnHeader c in this.itemsListView.Columns) {
            c.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
          }
        }
      }
    }

    protected override ListViewItem CreateListViewItem(RefreshableJob item) {
      ListViewItem listViewItem = base.CreateListViewItem(item);
      listViewItem.SubItems.Clear();
      listViewItem.SubItems.Insert(0, new ListViewItem.ListViewSubItem(listViewItem, item.Job.DateCreated.ToString()));
      listViewItem.SubItems.Insert(1, new ListViewItem.ListViewSubItem(listViewItem, item.Job.Name));
      listViewItem.Group = GetListViewGroup(item.Job.OwnerUsername);
      return listViewItem;
    }

    protected override void UpdateListViewItemText(ListViewItem listViewItem) {
      if (listViewItem == null) throw new ArgumentNullException();
      var item = listViewItem.Tag as RefreshableJob;
      listViewItem.SubItems[0].Text = item == null ? "null" : item.Job.DateCreated.ToString("dd.MM.yyyy HH:mm");
      listViewItem.SubItems[1].Text = item == null ? "null" : item.Job.Name;
      listViewItem.Group = GetListViewGroup(item.Job.OwnerUsername);
      listViewItem.ToolTipText = item == null ? string.Empty : item.ItemName + ": " + item.ItemDescription;
    }

    //drag'n'drop is not supported
    protected override void itemsListView_ItemDrag(object sender, ItemDragEventArgs e) { }
    protected override void itemsListView_DragEnter(object sender, DragEventArgs e) { }
    protected override void itemsListView_DragOver(object sender, DragEventArgs e) { }
    protected override void itemsListView_DragDrop(object sender, DragEventArgs e) { }

    private ListViewGroup GetListViewGroup(string groupName) {
      foreach (ListViewGroup group in itemsListView.Groups) {
        if (group.Name == groupName)
          return group;
      }
      var newGroup = new ListViewGroup(string.Format("Owner ({0})", groupName), HorizontalAlignment.Left) { Name = groupName };
      itemsListView.Groups.Add(newGroup);
      return newGroup;
    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }
  }
}
