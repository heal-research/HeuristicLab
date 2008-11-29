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
using System.Xml;

namespace HeuristicLab.Tools.ConfigMerger {
  public class ConfigMerger {
    public static void Main(string[] args) {
      try {
        Merge(args[0], args[1]);
      }
      catch (Exception ex) {
        Console.Out.WriteLine(ex.Message);
      }
    }

    public static void Merge(string sourceFile, string destinationFile) {
      XmlDocument source = new XmlDocument();
      source.Load(sourceFile);
      XmlDocument destination = new XmlDocument();
      destination.Load(destinationFile);

      XmlNode sourceNode;
      XmlNode destinationNode;

      sourceNode = source.SelectSingleNode("/configuration/configSections/sectionGroup[@name='applicationSettings']");
      destinationNode = destination.SelectSingleNode("/configuration/configSections/sectionGroup[@name='applicationSettings']");
      Merge(sourceNode, destinationNode, destination, "/configuration/configSections");

      sourceNode = source.SelectSingleNode("/configuration/configSections/sectionGroup[@name='userSettings']");
      destinationNode = destination.SelectSingleNode("/configuration/configSections/sectionGroup[@name='userSettings']");
      Merge(sourceNode, destinationNode, destination, "/configuration/configSections");

      sourceNode = source.SelectSingleNode("/configuration/applicationSettings");
      destinationNode = destination.SelectSingleNode("/configuration/applicationSettings");
      Merge(sourceNode, destinationNode, destination, "/configuration");

      sourceNode = source.SelectSingleNode("/configuration/userSettings");
      destinationNode = destination.SelectSingleNode("/configuration/userSettings");
      Merge(sourceNode, destinationNode, destination, "/configuration");

      destination.Save(destinationFile);
    }

    private static void Merge(XmlNode source, XmlNode destination, XmlDocument document, string root) {
      if (source != null) {
        if (destination == null) {
          XmlNode newNode = document.ImportNode(source, true);
          document.SelectSingleNode(root).AppendChild(newNode);
        } else {
          foreach (XmlNode node in source.ChildNodes) {
            XmlNode newNode = document.ImportNode(node, true);
            XmlNode oldNode = destination.SelectSingleNode(BuildXPathString(newNode));
            if (oldNode != null)
              destination.ReplaceChild(newNode, oldNode);
            else
              destination.AppendChild(newNode);
          }
        }
      }
    }

    private static string BuildXPathString(XmlNode node) {
      StringBuilder builder = new StringBuilder();
      builder.Append(node.Name);
      if (node.Attributes.Count > 0) {
        XmlAttribute attrib = node.Attributes[0];
        builder.Append("[");
        builder.Append("@" + attrib.Name + "='" + attrib.Value + "'");
        for (int i = 1; i < node.Attributes.Count; i++) {
          attrib = node.Attributes[i];
          builder.Append(" and @" + attrib.Name + "='" + attrib.Value + "'");
        }
        builder.Append("]");
      }
      return builder.ToString();
    }
  }
}
