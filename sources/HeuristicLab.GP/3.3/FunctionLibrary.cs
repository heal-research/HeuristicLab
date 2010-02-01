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
using HeuristicLab.Core;
using System.Xml;
using HeuristicLab.GP.Interfaces;

namespace HeuristicLab.GP {
  public class FunctionLibrary : ItemBase, IEditable {
    private List<IFunction> functions = new List<IFunction>();
    public IEnumerable<IFunction> Functions {
      get { return functions; }
    }

    public FunctionLibrary()
      : base() {
    }

    public void AddFunction(IFunction fun) {
      if (!functions.Contains(fun)) {
        functions.Add(fun);
        fun.Changed += new EventHandler(fun_Changed);
        OnChanged();
      }
    }

    public void RemoveFunction(IFunction fun) {
      functions.Remove(fun);
      fun.Changed -= new EventHandler(fun_Changed);

      // remove the operator from the allowed sub-functions of all functions
      foreach (IFunction f in Functions) {
        for (int i = 0; i < f.MaxSubTrees; i++) {
          f.RemoveAllowedSubFunction(fun, i);
        }
      }
      OnChanged();
    }

    void fun_Changed(object sender, EventArgs e) {
      OnChanged();
    }

    #region persistence
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      foreach (IFunction f in functions) {
        node.AppendChild(PersistenceManager.Persist("Function", f, document, persistedObjects));
      }
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      foreach (XmlNode fNode in node.SelectNodes("Function")) {
        AddFunction((IFunction)PersistenceManager.Restore(fNode, restoredObjects));
      }
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      FunctionLibrary clone = (FunctionLibrary)base.Clone(clonedObjects);
      foreach (var function in functions) {
        clone.AddFunction((Function)Auxiliary.Clone(function, clonedObjects));
      }
      return clone;
    }
    #endregion

    public override IView CreateView() {
      return new FunctionLibraryEditor(this);
    }

    #region IEditable Members

    public IEditor CreateEditor() {
      return new FunctionLibraryEditor(this);
    }

    #endregion
  }
}
