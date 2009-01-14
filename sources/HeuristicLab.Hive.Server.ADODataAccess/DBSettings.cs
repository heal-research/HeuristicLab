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
using System.Linq;
using System.Text;

namespace HeuristicLab.Hive.Server.ADODataAccess.Properties {

  internal sealed class Settings : global::System.Configuration.ApplicationSettingsBase {

    private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));

    public static Settings Default {
      get {
        return defaultInstance;
      }
    }

    private static String pw = "hive";
    private static String un = "hive";
    private static String address = "10.22.20.84";
    private static String catalog = "HiveServer"; 

    public static String GetConnString(String adress, String catalog, String un, String pw) {
      return "Data Source=" + adress + ";Initial Catalog=" + catalog + ";MultipleActiveResultSets=True;" +
        "Persist Security Info=True;User ID=" + un + ";Password=" + pw;
    }

    private static string connString =
      GetConnString(address, catalog, un, pw);

    [global::System.Configuration.ApplicationScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
    public string HiveServerConnectionString {
      get {
        return connString;
      }

      private set {
        connString = value;
      }
    }
  }
}