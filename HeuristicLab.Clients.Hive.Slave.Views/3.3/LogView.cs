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
using System.Windows.Forms;
using HeuristicLab.Clients.Hive.SlaveCore.Views.Properties;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Clients.Hive.SlaveCore.Views {
  [View("LogView: Displays logged messages from the slave core.")]
  [Content(typeof(SlaveItem), IsDefaultView = false)]
  public partial class LogView : ItemView {
    public new SlaveItem Content {
      get { return (SlaveItem)base.Content; }
      set {
        if (base.Content != value) {
          base.Content = value;
        }
      }
    }

    private ILog log;

    public LogView() {
      InitializeComponent();
      log = new ThreadSafeLog(Settings.Default.MaxLogCount);
      hlLogView.Content = log;
    }

    #region Register Content Events
    protected override void DeregisterContentEvents() {
      Content.SlaveMessageLogged -= new System.EventHandler<EventArgs<string>>(Content_SlaveMessageLogged);

      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();

      Content.SlaveMessageLogged += new System.EventHandler<EventArgs<string>>(Content_SlaveMessageLogged);
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
    }

    #region Event Handlers
    void Content_SlaveMessageLogged(object sender, EventArgs<string> e) {
      log.LogMessage(e.Value);
    }

    private void LogView_Load(object sender, EventArgs e) {
      chkShowBalloonTips.Checked = Settings.Default.ShowBalloonTips;
    }

    private void chkShowBalloonTips_CheckedChanged(object sender, EventArgs e) {
      if (Settings.Default.ShowBalloonTips != chkShowBalloonTips.Checked) {
        Settings.Default.ShowBalloonTips = chkShowBalloonTips.Checked;
        Settings.Default.Save();
      }
    }
    #endregion
  }
}
