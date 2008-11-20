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
using System.Text;
using System.Configuration;

namespace HeuristicLab.Settings {
  public class PluginSettingsProvider : SettingsProvider, IApplicationSettingsProvider {
    public override string ApplicationName {
      get {
        return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
      }
      set {
        // do nothing (cf. MSDN article "Application Settings Architecture")
      }
    }

    public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config) {
      base.Initialize(name, config);
    }

    public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection) {
      throw new NotImplementedException();
    }

    public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection) {
      throw new NotImplementedException();
    }

    #region IApplicationSettingsProvider Members

    public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property) {
      throw new NotImplementedException();
    }

    public void Reset(SettingsContext context) {
      throw new NotImplementedException();
    }

    public void Upgrade(SettingsContext context, SettingsPropertyCollection properties) {
      throw new NotImplementedException();
    }

    #endregion
  }
}
