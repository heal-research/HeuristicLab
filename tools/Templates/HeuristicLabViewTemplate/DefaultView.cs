#region License Information
/* HeuristicLab
 * Copyright (C) 2002-$year$ Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace $rootnamespace$ {
  [View("$viewName$")]
  [Content(typeof($contentType$), IsDefaultView = $defaultView$)]
  public partial class $safeitemname$ : ItemView {
    public new $contentType$ Content {
      get { return ($contentType$)base.Content; }
      set { base.Content = value; }
    }

    public $safeitemname$() {
      InitializeComponent();
    }

    #region Register Content Events
    protected override void DeregisterContentEvents() {
      // TODO: Deregister your event handlers on the Content here
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      // TODO: Register your event handlers on the Content here
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        // TODO: Put code here when content is null
      } else {
        // TODO: Put code here when content has been changed and is not null
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      // TODO: Put code here to enable or disable controls based on whether the Content is/not null or the view is ReadOnly
    }
    
    #region Event Handlers
    // TODO: Put event handlers here
    #endregion
  }
}
