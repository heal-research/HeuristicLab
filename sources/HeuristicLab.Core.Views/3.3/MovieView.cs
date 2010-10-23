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

using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Core.Views {
  [View("Movie View")]
  public partial class MovieView<T> : ItemView where T : class, IItem {
    public new IItemCollection<T> Content {
      get { return (IItemCollection<T>)base.Content; }
      set { base.Content = value; }
    }

    public MovieView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.ItemsAdded -= new CollectionItemsChangedEventHandler<T>(Content_ItemsAdded);
      Content.ItemsRemoved -= new CollectionItemsChangedEventHandler<T>(Content_ItemsRemoved);
      Content.CollectionReset -= new CollectionItemsChangedEventHandler<T>(Content_CollectionReset);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemsAdded += new CollectionItemsChangedEventHandler<T>(Content_ItemsAdded);
      Content.ItemsRemoved += new CollectionItemsChangedEventHandler<T>(Content_ItemsRemoved);
      Content.CollectionReset += new CollectionItemsChangedEventHandler<T>(Content_CollectionReset);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (backgroundWorker.IsBusy) backgroundWorker.CancelAsync();
      if (Content == null) {
        trackBar.Maximum = 10;
        viewHost.Content = null;
      } else {
        Caption += " (" + Content.GetType().Name + ")";
        trackBar.Maximum = Content.Count - 1;
        viewHost.Content = Content.FirstOrDefault();
      }
      trackBar.Value = 0;
      UpdateLables();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      firstButton.Enabled = (Content != null) && (Content.Count > 0) && (!backgroundWorker.IsBusy);
      trackBar.Enabled = (Content != null) && (Content.Count > 0) && (!backgroundWorker.IsBusy);
      lastButton.Enabled = (Content != null) && (Content.Count > 0) && (!backgroundWorker.IsBusy);
      playButton.Enabled = (Content != null) && (Content.Count > 0) && (!backgroundWorker.IsBusy);
      stopButton.Enabled = (Content != null) && (backgroundWorker.IsBusy);
    }

    protected override void OnClosed(FormClosedEventArgs e) {
      base.OnClosed(e);
      if (backgroundWorker.IsBusy) backgroundWorker.CancelAsync();
    }

    #region Content Events
    protected virtual void Content_ItemsAdded(object sender, CollectionItemsChangedEventArgs<T> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<T>(Content_ItemsAdded), sender, e);
      else {
        trackBar.Maximum = Content.Count - 1;
        maximumLabel.Text = trackBar.Maximum.ToString();
        if (viewHost.Content == null) {
          viewHost.Content = Content.FirstOrDefault();
          SetEnabledStateOfControls();
        }
      }
    }
    protected virtual void Content_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<T> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<T>(Content_ItemsRemoved), sender, e);
      else {
        trackBar.Maximum = Content.Count - 1;
        maximumLabel.Text = trackBar.Maximum.ToString();
        if (e.Items.Contains((T)viewHost.Content)) {
          trackBar.Value = 0;
          viewHost.Content = Content.FirstOrDefault();
          UpdateLables();
          SetEnabledStateOfControls();
        }
      }
    }
    protected virtual void Content_CollectionReset(object sender, CollectionItemsChangedEventArgs<T> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<T>(Content_CollectionReset), sender, e);
      else {
        trackBar.Maximum = Content.Count - 1;
        trackBar.Value = 0;
        viewHost.Content = Content.FirstOrDefault();
        UpdateLables();
        SetEnabledStateOfControls();
      }
    }
    #endregion

    #region Control Events
    protected virtual void trackBar_ValueChanged(object sender, EventArgs e) {
      indexLabel.Text = trackBar.Value.ToString();
      viewHost.Content = Content == null ? null : Content.ElementAtOrDefault(trackBar.Value);
    }
    protected virtual void firstButton_Click(object sender, EventArgs e) {
      trackBar.Value = trackBar.Minimum;
    }
    protected virtual void lastButton_Click(object sender, EventArgs e) {
      trackBar.Value = trackBar.Maximum;
    }
    protected virtual void playButton_Click(object sender, EventArgs e) {
      firstButton.Enabled = false;
      trackBar.Enabled = false;
      lastButton.Enabled = false;
      playButton.Enabled = false;
      stopButton.Enabled = true;
      backgroundWorker.RunWorkerAsync();
    }
    protected virtual void stopButton_Click(object sender, EventArgs e) {
      backgroundWorker.CancelAsync();
    }
    #endregion

    #region BackgroundWorker Events
    protected virtual void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
      bool terminate = false;
      while (!backgroundWorker.CancellationPending && !terminate) {
        Invoke((Action)delegate() {
          if (trackBar.Value < trackBar.Maximum) trackBar.Value++;
          terminate = trackBar.Value == trackBar.Maximum;
        });
        Thread.Sleep(100);
      }
    }
    protected virtual void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
      firstButton.Enabled = true;
      trackBar.Enabled = true;
      lastButton.Enabled = true;
      playButton.Enabled = true;
      stopButton.Enabled = false;
    }
    #endregion

    #region Helpers
    protected virtual void UpdateLables() {
      minimumLabel.Text = trackBar.Minimum.ToString();
      indexLabel.Text = trackBar.Value.ToString();
      maximumLabel.Text = trackBar.Maximum.ToString();
    }
    #endregion
  }
}
