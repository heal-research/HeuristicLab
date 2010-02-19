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
using System.Xml;
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.GP.Interfaces;

namespace HeuristicLab.GP {
  public abstract class FunctionLibraryInjectorBase : OperatorBase {
    private const string FUNCTIONLIBRARY = "FunctionLibrary";

    public override string Description {
      get { return @"Description is missing."; }
    }

    private FunctionLibrary functionLibrary;
    public FunctionLibrary FunctionLibrary {
      get {
        return functionLibrary;
      }
      set {
        this.functionLibrary = value;
        FireChanged();
      }
    }

    public FunctionLibraryInjectorBase()
      : base() {
      AddVariableInfo(new VariableInfo(FUNCTIONLIBRARY, "Preconfigured default function library", typeof(FunctionLibrary), VariableKind.New));
      // create the default function library
      functionLibrary = CreateFunctionLibrary();
    }

    public override IOperation Apply(IScope scope) {
      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName(FUNCTIONLIBRARY), (FunctionLibrary)functionLibrary.Clone()));
      return null;
    }

    protected abstract FunctionLibrary CreateFunctionLibrary();

    protected static void SetAllowedSubOperators(IFunction f, IEnumerable<IFunction> gs) {
      for (int i = 0; i < f.MaxSubTrees; i++) {
        SetAllowedSubOperators(f, i, gs);
      }
    }

    protected static void SetAllowedSubOperators(IFunction f, int i, IEnumerable<IFunction> gs) {
      foreach (var g in gs) {
        f.AddAllowedSubFunction(g, i);
      }
    }

    public override IView CreateView() {
      return new FunctionLibraryInjectorView(this);
    }

    #region persistence
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      FunctionLibraryInjectorBase clone = (FunctionLibraryInjectorBase)base.Clone(clonedObjects);
      clone.functionLibrary = (FunctionLibrary)Auxiliary.Clone(functionLibrary, clonedObjects);
      return clone;
    }
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      node.AppendChild(PersistenceManager.Persist("FunctionLibrary", FunctionLibrary, document, persistedObjects));
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      functionLibrary = (FunctionLibrary)PersistenceManager.Restore(node.SelectSingleNode("FunctionLibrary"), restoredObjects);
    }
    #endregion
  }
}
