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
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Core.Views {
  [View("Movie View")]
  public partial class MovieView<T> : ItemView where T : class, IItem {
    #region Delay
    protected class Delay {
      public string Text { get; private set; }
      public int Milliseconds { get; private set; }

      public Delay(string text, int milliseconds) {
        Text = text;
        Milliseconds = milliseconds;
      }

      public override string ToString() {
        return Text;
      }
    }
    #endregion

    protected int delay;

    public new IItemCollection<T> Content {
      get { return (IItemCollection<T>)base.Content; }
      set { base.Content = value; }
    }

    public MovieView() {
      InitializeComponent();

      delayComboBox.Items.Add(new Delay("5s", 5000));
      delayComboBox.Items.Add(new Delay("2s", 2000));
      delayComboBox.Items.Add(new Delay("1s", 1000));
      delayComboBox.Items.Add(new Delay("0.5s", 500));
      delayComboBox.Items.Add(new Delay("0.1s", 100));
      delayComboBox.Items.Add(new Delay("0.05s", 50));
      delayComboBox.Items.Add(new Delay("0.01s", 10));
      delayComboBox.SelectedIndex = 4;
      delay = 100;
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
      firstButton.Enabled = (Content != null) && (Content.Count > 0) && (trackBar.Value != trackBar.Minimum) && (!backgroundWorker.IsBusy);
      previousButton.Enabled = (Content != null) && (Content.Count > 0) && (trackBar.Value != trackBar.Minimum) && (!backgroundWorker.IsBusy);
      trackBar.Enabled = (Content != null) && (Content.Count > 0) && (!backgroundWorker.IsBusy);
      nextButton.Enabled = (Content != null) && (Content.Count > 0) && (trackBar.Value != trackBar.Maximum) && (!backgroundWorker.IsBusy);
      lastButton.Enabled = (Content != null) && (Content.Count > 0) && (trackBar.Value != trackBar.Maximum) && (!backgroundWorker.IsBusy);
      playButton.Enabled = (Content != null) && (Content.Count > 0) && (!backgroundWorker.IsBusy);
      stopButton.Enabled = (Content != null) && (backgroundWorker.IsBusy);
      delayComboBox.Enabled = (Content != null) && (Content.Count > 0);
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
        if (viewHost.Content == null) viewHost.Content = Content.FirstOrDefault();
        SetEnabledStateOfControls();
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
        }
        SetEnabledStateOfControls();
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
      viewHost.Content = Content == null ? null : Content.ElementAtOrDefault(trackBar.Value);
      indexLabel.Text = trackBar.Value.ToString();
      SetEnabledStateOfControls();
    }
    protected virtual void firstButton_Click(object sender, EventArgs e) {
      if (trackBar.Value != trackBar.Minimum) trackBar.Value = trackBar.Minimum;
    }
    protected virtual void previousButton_Click(object sender, EventArgs e) {
      if (trackBar.Value != trackBar.Minimum) trackBar.Value--;
    }
    protected virtual void nextButton_Click(object sender, EventArgs e) {
      if (trackBar.Value != trackBar.Maximum) trackBar.Value++;
    }
    protected virtual void lastButton_Click(object sender, EventArgs e) {
      if (trackBar.Value != trackBar.Maximum) trackBar.Value = trackBar.Maximum;
    }
    protected virtual void playButton_Click(object sender, EventArgs e) {
      firstButton.Enabled = false;
      previousButton.Enabled = false;
      trackBar.Enabled = false;
      nextButton.Enabled = false;
      lastButton.Enabled = false;
      playButton.Enabled = false;
      stopButton.Enabled = true;
      backgroundWorker.RunWorkerAsync();
    }
    protected virtual void stopButton_Click(object sender, EventArgs e) {
      stopButton.Enabled = false;
      backgroundWorker.CancelAsync();
    }
    protected virtual void delayComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      Delay selected = delayComboBox.SelectedItem as Delay;
      if (selected != null) delay = selected.Milliseconds;
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
        Thread.Sleep(delay);
      }
    }
    protected virtual void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
      firstButton.Enabled = true;
      previousButton.Enabled = true;
      trackBar.Enabled = true;
      nextButton.Enabled = true;
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
