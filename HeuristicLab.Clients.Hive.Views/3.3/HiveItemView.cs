#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.Hive.Views {
  [View("HiveItem View")]
  [Content(typeof(HiveItem), IsDefaultView = false)]
  [Content(typeof(IHiveItem), IsDefaultView = false)]
  public partial class HiveItemView : ItemView {
    public new IHiveItem Content {
      get { return (IHiveItem)base.Content; }
      set { base.Content = value; }
    }

    public HiveItemView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.PropertyChanged -= new PropertyChangedEventHandler(Content_PropertyChanged);
      Content.ModifiedChanged -= new EventHandler(Content_ModifiedChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.PropertyChanged += new PropertyChangedEventHandler(Content_PropertyChanged);
      Content.ModifiedChanged += new EventHandler(Content_ModifiedChanged);
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      storeButton.Enabled = (Content != null) && !ReadOnly && Content.Modified;
    }

    private void Content_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (InvokeRequired)
        Invoke(new PropertyChangedEventHandler(Content_PropertyChanged), sender, e);
      else {
        OnContentPropertyChanged(e.PropertyName);
      }
    }
    protected virtual void OnContentPropertyChanged(string propertyName) { }
    protected virtual void Content_ModifiedChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ModifiedChanged), sender, e);
      else
        SetEnabledStateOfControls();
    }

    protected virtual void storeButton_Click(object sender, EventArgs e) {
      try {
        Content.Store();
      }
      catch (Exception ex) {
        ErrorHandling.ShowErrorDialog(this, "Store failed.", ex);
      }
    }
  }
}