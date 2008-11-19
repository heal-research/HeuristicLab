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
using HeuristicLab.Hive.Client.Common;

namespace HeuristicLab.Hive.Client.Common {
  [Serializable]
  abstract public class JobBase : IJob {

    public long JobId { get; set; }

    private Thread thread = null;
    
    public int Progress { get; set; }
    
    protected bool abort = false;
    public bool Running { get; set; }

    public event EventHandler JobStopped;

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

    public XmlNode GetXmlNode() {
      XmlDocument doc = PersistenceManager.CreateXmlDocument();
      return doc.CreateNode(XmlNodeType.Element, "testnode", null);      
    }

    public JobBase() {    
    }
  }
}
