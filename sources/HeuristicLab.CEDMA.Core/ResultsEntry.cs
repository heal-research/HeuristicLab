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
using HeuristicLab.Core;
using System.Collections;
using HeuristicLab.CEDMA.DB.Interfaces;
using System.Xml;
using System.Runtime.Serialization;
using System.IO;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.CEDMA.Core {
  public class ResultsEntry {
    private Dictionary<string, object> values = new Dictionary<string, object>();

    private bool selected = false;
    public bool Selected {
      get { return selected; }
      set { selected = value; }
    }

    private string uri;
    public string Uri {
      get { return uri; }
      set { uri = value; }
    }

    public void Set(string name, object value) {
      values.Add(name, value);
    }

    public object Get(string name) {
      if (name == null || !values.ContainsKey(name)) return null;
      return values[name];
    }

    public void ToggleSelected() {
      selected = !selected;
    }

    public string GetToolTipText() {
      StringBuilder b = new StringBuilder();
      foreach (KeyValuePair<string, object> v in values) {
        string val = v.Value.ToString();
        if (val.Length > 40) val = val.Substring(0, 38) + "...";
        b.Append(v.Key).Append(" = ").Append(val).AppendLine();
      }
      return b.ToString();
    }
  }
}