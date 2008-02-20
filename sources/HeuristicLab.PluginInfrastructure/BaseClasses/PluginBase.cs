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
using System.Reflection;
using System.Diagnostics;

namespace HeuristicLab.PluginInfrastructure {
  /// <summary>
  /// Default implementation of the IPlugin interface.
  /// </summary>
  public class PluginBase : IPlugin {
    private string name;
    private Version version;
    private string[] files;
    private string description;

    public PluginBase() {
      ReadAttributes();
    }

    private void ReadAttributes() {
      object[] pluginAttributes = this.GetType().GetCustomAttributes(typeof(ClassInfoAttribute), false);

      // exactly one attribute of the type ClassInfoAttribute must be given
      if(pluginAttributes.Length != 1) {
        throw new InvalidPluginException();
      }

      // after the assertion we are sure that the array access will not fail
      ClassInfoAttribute pluginAttribute = (ClassInfoAttribute)pluginAttributes[0];

      // if the plugin name is not explicitly set in the attribute then the default plugin name is the FullName of the type
      if(pluginAttribute != null && pluginAttribute.Name != null) {
        this.name = pluginAttribute.Name;
      } else {
        this.name = this.GetType().FullName;
      }

      // if the version is not explicitly set in the attribute then the version of the assembly is used as default
      if(pluginAttribute != null && pluginAttribute.Version != null) {
        this.version = new Version(pluginAttribute.Version);
      } else {
        this.version = this.GetType().Assembly.GetName().Version;
      }

      // if the description is not explicitly set in the attribute then the name of name of the plugin is used as default
      if(pluginAttribute != null && pluginAttribute.Description != null) {
        this.description = pluginAttribute.Description;
      } else {
        this.description = name;
      }

      // get all attributes of type PluginFileAttribute, multiple usage is possible
      PluginFileAttribute[] fileAttributes = (PluginFileAttribute[])this.GetType().GetCustomAttributes(typeof(PluginFileAttribute), false);

      // exctract the file names from the attributes
      this.files = new string[fileAttributes.Length];
      int i = 0;
      foreach(PluginFileAttribute fileAttr in fileAttributes) {
        files[i++] = fileAttr.Filename;
      }
    }

    #region IPlugin Members
    public string Name {
      get {
        return name;
      }
    }

    public Version Version {
      get {
        return version;
      }
    }

    public string Description {
      get {
        return description;
      }
    }

    public  string[] Files {
      get {
        return files;
      }
    }

    public virtual void OnInstall() {
    }

    public virtual void OnDelete() {
    }

    public virtual void OnPreUpdate() {
    }

    public virtual void OnPostUpdate() {
    }

    #endregion

  }
}
