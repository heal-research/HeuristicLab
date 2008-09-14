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

namespace HeuristicLab.CEDMA.Charting {
  public class Record {
    public const string MAPE_TRAINING = "MAPE (training)";
    public const string MAPE_VALIDATION = "MAPE (validation)";
    public const string MAPE_TEST = "MAPE (test)";
    public const string R2_TRAINING = "R2 (training)";
    public const string R2_VALIDATION = "R2 (validation)";
    public const string R2_TEST = "R2 (test)";
    public const string TARGET_VARIABLE = "Target variable";
    public const string TREE_SIZE = "Tree size";
    public const string TREE_HEIGHT = "Tree height";

    public const string X_JITTER = "__X_JITTER";
    public const string Y_JITTER = "__Y_JITTER";

    public event EventHandler OnSelectionChanged;

    private Dictionary<string, double> values = new Dictionary<string, double>();
    private ResultList resultList;

    private bool selected = false;
    public bool Selected { get { return selected; } }

    private string uri;
    public string Uri { get { return uri; } }
    public Record(ResultList resultList, string uri) {
      this.uri = uri;
      this.resultList = resultList;
    }

    public void Set(string name, double value) {
      values.Add(name, value);
    }

    public double Get(string name) {
      if(name == null || !values.ContainsKey(name)) return double.NaN;
      return values[name];
    }

    public void ToggleSelected() {
      selected = !selected;
      if(OnSelectionChanged != null) OnSelectionChanged(this, new EventArgs());      
    }

    internal string GetToolTipText() {
      StringBuilder b = new StringBuilder();
      foreach(KeyValuePair<string, double> v in values) {
        if(v.Key != X_JITTER && v.Key != Y_JITTER) {
          b.Append(v.Key).Append(" = ").Append(v.Value).AppendLine();
        }
      }
      return b.ToString();
    }

    internal void OpenModel() {
      resultList.OpenModel(this);
    }
  }
}