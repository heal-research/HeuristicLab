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
using System.Diagnostics;
using System.Xml;

namespace HeuristicLab.Hive.Client.Common {
  [Serializable]
  public class TestJob: JobBase {

    private int runValue = 0;

    public override void Run() {
      int max = 10;
      while(runValue < max && abort == false) {
        for (int y = 0; y < Int32.MaxValue; y++) ;
          if (abort == true) {            
            Debug.WriteLine("Job Abort Processing");
           break;
          }
        runValue++;
        Progress = (double)runValue / max;        
        Debug.WriteLine("Iteration " + runValue + " done");
        Debug.WriteLine("Progress " + Progress*100 + " Percent");
      }      
      OnJobStopped();
    }
    public override System.Xml.XmlNode GetXmlNode(string name, System.Xml.XmlDocument document, IDictionary<Guid, HeuristicLab.Core.IStorable> persistedObjects) {      
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
    
      XmlNode startValue = document.CreateNode(XmlNodeType.Element, "StartValue", null);
      startValue.InnerText = Convert.ToString(runValue);
      
      node.AppendChild(startValue);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, HeuristicLab.Core.IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);

      XmlNode startValue = node.SelectSingleNode("StartValue");
      runValue = Convert.ToInt32(startValue.InnerText);
    }


  }
}
