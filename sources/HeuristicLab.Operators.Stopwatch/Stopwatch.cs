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
using System.Xml;
using HeuristicLab.Core;
using System.Diagnostics;
using HeuristicLab.Data;

namespace HeuristicLab.Operators.Stopwatch {
  public class Stopwatch : ItemBase {
    private System.Diagnostics.Stopwatch stopwatch;
    private long elapsedTicks;
    private bool running;

    public Stopwatch() {
      stopwatch = new System.Diagnostics.Stopwatch();
      elapsedTicks = 0;
      running = false;
    }

    public void Start() {
      stopwatch.Start();
      running = true;
    }

    public void Stop() {
      stopwatch.Stop();
      elapsedTicks = stopwatch.ElapsedTicks;
      stopwatch.Reset();
      running = false;
    }

    public long ElapsedTicks {
      get {
        return elapsedTicks + stopwatch.ElapsedTicks;
      }
    }

    public TimeSpan Elapsed {
      get {
        return TimeSpan.FromTicks(elapsedTicks).Add(stopwatch.Elapsed);
      }
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      Stopwatch clone = (Stopwatch)base.Clone(clonedObjects);
      if(running) clone.Start();
      clone.elapsedTicks = elapsedTicks;
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute runningAttr = document.CreateAttribute("Running");
      runningAttr.Value = running.ToString();
      node.Attributes.Append(runningAttr);
      XmlAttribute elapsedTicksAttr = document.CreateAttribute("ElapsedTicks");
      elapsedTicksAttr.Value = elapsedTicks.ToString();
      node.Attributes.Append(elapsedTicksAttr);
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid,IStorable> restoredObjects) {
      elapsedTicks = long.Parse(node.Attributes["ElapsedTicks"].Value);
      running = bool.Parse(node.Attributes["Running"].Value);
      if(running) stopwatch.Start();
      base.Populate(node, restoredObjects);
    }
  }
}
