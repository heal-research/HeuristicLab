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
using System.Xml;
using HeuristicLab.CEDMA.DB.Interfaces;
using System.IO;

namespace HeuristicLab.CEDMA.Core {
  public class ResultRow {
    private Dictionary<string, string> attributes = new Dictionary<string, string>();

    public void AddAttribute(string name, string value) {
      attributes[name] = value;
    }

    public ICollection<string> AttributeNames {
      get { return attributes.Keys; }
    }

    public string AttributeValue(string name) {
      string value = "";
      attributes.TryGetValue(name, out value);
      return value;
    }
  }

  public class ResultTable {

    private List<ResultRow> rows = new List<ResultRow>();
    private List<string> allAttributeNames = new List<string>();

    public void AddRow(ResultRow r) {
      rows.Add(r);
      foreach(string name in r.AttributeNames) if(!allAttributeNames.Contains(name)) allAttributeNames.Add(name);
    }

    public void Write(Stream s) {
      using(StreamWriter w = new StreamWriter(s)) {
        foreach(string attributeName in allAttributeNames) w.Write(attributeName + ";");
        w.WriteLine();
        foreach(ResultRow r in rows) {
          foreach(string attributeName in allAttributeNames) {
            w.Write(r.AttributeValue(attributeName) + ";"); // we should probably escape the ';' if it occures in the value
          }
          w.WriteLine();
        }
      }
    }
  }
}
