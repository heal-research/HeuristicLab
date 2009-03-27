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
using HeuristicLab.Core;
using System.Threading;
using HeuristicLab.Hive.JobBase;
using System.Xml;

namespace HeuristicLab.Hive.Engine {
  /// <summary>
  /// Represents an job that wraps an engine that should be executed in the hive.
  /// </summary>
  public class Job : StorableBase, IJob {
    private IEngine engine;
    public IEngine Engine {
      get { return engine; }
    }

    public Job()
      : base() {
      engine = new SequentialEngine.SequentialEngine();
      RegisterEvents();
    }

    private void RegisterEvents() {
      engine.Finished += new EventHandler(engine_Finished);
    }

    private void DeregisterEvents() {
      engine.Finished -= new EventHandler(engine_Finished);
    }

    void engine_Finished(object sender, EventArgs e) {
      if (JobStopped != null)
        JobStopped(this, new EventArgs());
    }

    #region IJob Members

    public event EventHandler JobStopped;

    public long JobId {
      get;
      set;
    }

    public double Progress {
      get { return 0.0; }
      set { throw new NotSupportedException(); }
    }

    public bool Running {
      get {
        return Engine.Running;
      }
      set {
        throw new NotSupportedException();
      }
    }

    public void Run() {
      throw new NotSupportedException();
    }

    public void Start() {
      Engine.Execute();
    }

    public void Stop() {
      Engine.Abort();
    }

    #endregion

    public override System.Xml.XmlNode GetXmlNode(string name, System.Xml.XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("Engine", Engine, document, persistedObjects));
      return node;
    }
    public override void Populate(System.Xml.XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      DeregisterEvents();
      engine = (SequentialEngine.SequentialEngine)PersistenceManager.Restore(node.SelectSingleNode("Engine"), restoredObjects);
      RegisterEvents();
    }
  }
}
