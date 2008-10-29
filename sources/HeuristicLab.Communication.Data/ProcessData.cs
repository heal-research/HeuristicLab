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
    }

    public bool Connect() {
      StartProcess();
      return process != null && !process.HasExited;
    }

    public void Close() {
      if (!process.HasExited) process.Kill();
      process.Close();
      process = null;
    }

    public void Write(string s) {
      StreamWriter writer = process.StandardInput;
      writer.WriteLine(s);
      writer.WriteLine(((char)4).ToString());
    }

    public string Read() {
      StreamReader reader = process.StandardOutput;
      StringBuilder buffer = new StringBuilder();
      string line = "";
      do {
        line = reader.ReadLine();
        if (line.Equals(((char)4).ToString())) break;
        buffer.AppendLine(line);
      } while (true);
      return buffer.ToString();
    }
  }
}