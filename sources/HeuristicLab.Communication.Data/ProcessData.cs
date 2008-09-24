using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Communication.Data {
  public class ProcessData : ItemBase, IDataStream {
    private LocalProcessDriverConfiguration config;
    public IDriverConfiguration Configuration {
      get { return config; }
      set { config = (LocalProcessDriverConfiguration)value; }
    }
    private Process process;
    public Process Process {
      get { return process; }
      set { process = value; }
    }

    public ProcessData() {
      process = null;
      config = null;
    }

    // A process cannot be cloned
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ProcessData clone = new ProcessData();
      clonedObjects.Add(Guid, clone);
      clone.process = process;
      clone.config = (LocalProcessDriverConfiguration)Auxiliary.Clone(config, clonedObjects);
      return clone;
    }

    #region persistence
    // A process cannot be persisted
    // but information can be persisted that will allow it to be recreated
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlNode configNode = PersistenceManager.Persist("Configuration", config, document, persistedObjects);
      node.AppendChild(configNode);
      return node;
    }

    // A process cannot be persisted
    // but information can be persisted that will allow it to be recreated
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      Configuration = (LocalProcessDriverConfiguration)PersistenceManager.Restore(node.SelectSingleNode("Configuration"), restoredObjects);
      StartProcess();
    }
    #endregion

    private void StartProcess() {
      process = new Process();
      process.StartInfo.FileName = config.ExecutablePath.Data;
      process.StartInfo.Arguments = config.Arguments.Data;
      process.StartInfo.UseShellExecute = false;
      process.StartInfo.RedirectStandardInput = true;
      process.StartInfo.RedirectStandardError = true;
      process.StartInfo.RedirectStandardOutput = true;
      process.Start();
    }

    public void Initialize(IDriverConfiguration configuration) {
      Configuration = configuration;
      StartProcess();
    }

    public bool Connect() {  
      return true;
    }

    public void Close() {
      if (!process.HasExited) process.Kill();
      process.Close();
      process = null;
    }

    public void Write(string s) {
      StreamWriter writer = process.StandardInput;
      writer.WriteLine(s);
      writer.WriteLine(".");
    }

    public string Read() {
      StreamReader reader = process.StandardOutput;
      StringBuilder buffer = new StringBuilder();
      string line = "";
      do {
        line = reader.ReadLine();
        if (line.Equals(".")) break;
        buffer.AppendLine(line);
      } while (true);
      return buffer.ToString();
    }
  }
}