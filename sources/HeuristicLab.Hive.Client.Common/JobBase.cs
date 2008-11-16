using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using HeuristicLab.Core;

namespace HeuristicLab.Hive.Client.Common {

  abstract public class JobBase {

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
