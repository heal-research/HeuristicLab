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
using System.Threading;
using System.Xml;
using HeuristicLab.Core;

namespace HeuristicLab.Hive.JobBase {
  [Serializable]
  abstract public class JobBase : StorableBase, IJob {

    private Thread thread = null;
    public event EventHandler JobStopped;
    
    public long JobId { get; set; }    
    public double Progress { get; set; }        
    public bool Running { get; set; }
    
    protected bool abort = false;    

    abstract public void Run();

    public void Start() {
      thread = new Thread(new ThreadStart(Run));
      thread.Start();
      Running = true;
    }

    public void Stop() {
      abort = true;        
    }

    protected void OnJobStopped() {
      Console.WriteLine("Job has finished");
      Running = false;
      if (JobStopped != null)
        JobStopped(this, new EventArgs());
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);

      XmlNode progr = document.CreateNode(XmlNodeType.Element, "Progress", null);
      progr.InnerText = Convert.ToString(Progress);

      node.AppendChild(progr);
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);

      XmlNode progr = node.SelectSingleNode("Progress");
      Progress = Convert.ToDouble(progr.InnerText);
    }

    public JobBase() {    
    }
  }
}
