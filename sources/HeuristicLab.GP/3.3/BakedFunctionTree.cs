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
using HeuristicLab.Data;
using System.Xml;
using System.Globalization;

namespace HeuristicLab.GP {

  public class LightWeightFunction {
    public byte arity = 0;
    public IFunction functionType;
    public List<object> localData = new List<object>();

    public LightWeightFunction Clone() {
      LightWeightFunction clone = new LightWeightFunction();
      clone.arity = arity;
      clone.functionType = functionType;
      clone.localData.AddRange(localData);
      return clone;
    }
  }

  public class BakedFunctionTree : ItemBase, IFunctionTree {
    private List<LightWeightFunction> linearRepresentation;
    public List<LightWeightFunction> LinearRepresentation {
      get {
        FlattenVariables();
        FlattenTrees();
        return linearRepresentation;
      }
    }
    private bool treesExpanded = false;
    private List<IFunctionTree> subTrees;
    private bool variablesExpanded = false;
    private List<IVariable> variables;

    public BakedFunctionTree() {
      linearRepresentation = new List<LightWeightFunction>();
    }

    internal BakedFunctionTree(IFunction function)
      : this() {
      LightWeightFunction fun = new LightWeightFunction();
      fun.functionType = function;
      linearRepresentation.Add(fun);
      treesExpanded = true;
      subTrees = new List<IFunctionTree>();
      variables = new List<IVariable>();
      variablesExpanded = true;
      foreach (IVariableInfo variableInfo in function.VariableInfos) {
        if (variableInfo.Local) {
          variables.Add((IVariable)function.GetVariable(variableInfo.FormalName).Clone());
        }
      }
    }

    internal BakedFunctionTree(IFunctionTree tree)
      : this() {
      LightWeightFunction fun = new LightWeightFunction();
      fun.functionType = tree.Function;
      linearRepresentation.Add(fun);
      foreach (IVariable variable in tree.LocalVariables) {
        IObjectData value = (IObjectData)variable.Value;
        fun.localData.Add(value.Data);
      }
      foreach (IFunctionTree subTree in tree.SubTrees) {
        AddSubTree(new BakedFunctionTree(subTree));
      }
    }

    private int BranchLength(int branchRoot) {
      int arity = linearRepresentation[branchRoot].arity;
      int length = 1;
      for (int i = 0; i < arity; i++) {
        length += BranchLength(branchRoot + length);
      }
      return length;
    }

    private void FlattenTrees() {
      if (treesExpanded) {
        linearRepresentation[0].arity = (byte)subTrees.Count;
        foreach (BakedFunctionTree subTree in subTrees) {
          subTree.FlattenVariables();
          subTree.FlattenTrees();
          linearRepresentation.AddRange(subTree.linearRepresentation);
        }
        treesExpanded = false;
        subTrees = null;
      }
    }

    private void FlattenVariables() {
      if (variablesExpanded) {
        linearRepresentation[0].localData.Clear();
        foreach (IVariable variable in variables) {
          object objData = variable.Value;
          while (objData is IObjectData) objData = ((IObjectData)objData).Data;
          linearRepresentation[0].localData.Add(objData);
        }
        variablesExpanded = false;
        variables = null;
      }
    }

    public int Size {
      get {
        if (treesExpanded) {
          int size = 1;
          foreach (BakedFunctionTree tree in subTrees) {
            size += tree.Size;
          }
          return size;
        } else
          return linearRepresentation.Count;
      }
    }

    public int Height {
      get {
        if (treesExpanded) {
          int height = 0;
          foreach (IFunctionTree subTree in subTrees) {
            int curHeight = subTree.Height;
            if (curHeight > height) height = curHeight;
          }
          return height + 1;
        } else {
          int nextBranchStart;
          return BranchHeight(0, out nextBranchStart);
        }
      }
    }

    private int BranchHeight(int branchStart, out int nextBranchStart) {
      LightWeightFunction f = linearRepresentation[branchStart];
      int height = 0;
      branchStart++;
      for (int i = 0; i < f.arity; i++) {
        int curHeight = BranchHeight(branchStart, out nextBranchStart);
        if (curHeight > height) height = curHeight;
        branchStart = nextBranchStart;
      }
      nextBranchStart = branchStart;
      return height + 1;
    }

    public IList<IFunctionTree> SubTrees {
      get {
        if (!treesExpanded) {
          subTrees = new List<IFunctionTree>();
          int arity = linearRepresentation[0].arity;
          int branchIndex = 1;
          for (int i = 0; i < arity; i++) {
            BakedFunctionTree subTree = new BakedFunctionTree();
            int length = BranchLength(branchIndex);
            for (int j = branchIndex; j < branchIndex + length; j++) {
              subTree.linearRepresentation.Add(linearRepresentation[j]);
            }
            branchIndex += length;
            subTrees.Add(subTree);
          }
          treesExpanded = true;
          linearRepresentation.RemoveRange(1, linearRepresentation.Count - 1);
          linearRepresentation[0].arity = 0;
        }
        return subTrees;
      }
    }

    public ICollection<IVariable> LocalVariables {
      get {
        if (!variablesExpanded) {
          variables = new List<IVariable>();
          IFunction function = Function;
          int localVariableIndex = 0;
          foreach (IVariableInfo variableInfo in function.VariableInfos) {
            if (variableInfo.Local) {
              IVariable clone = (IVariable)function.GetVariable(variableInfo.FormalName).Clone();
              IObjectData objData = (IObjectData)clone.Value;
              if (objData is ConstrainedDoubleData) {
                ((ConstrainedDoubleData)objData).Data = (double)linearRepresentation[0].localData[localVariableIndex];
              } else if (objData is ConstrainedIntData) {
                ((ConstrainedIntData)objData).Data = (int)linearRepresentation[0].localData[localVariableIndex];
              } else {
                objData.Data = linearRepresentation[0].localData[localVariableIndex];
              }
              variables.Add(clone);
              localVariableIndex++;
            }
          }
          variablesExpanded = true;
          linearRepresentation[0].localData.Clear();
        }
        return variables;
      }
    }

    public IFunction Function {
      get { return linearRepresentation[0].functionType; }
    }

    public IVariable GetLocalVariable(string name) {
      foreach (IVariable var in LocalVariables) {
        if (var.Name == name) return var;
      }
      return null;
    }

    public void AddVariable(IVariable variable) {
      throw new NotSupportedException();
    }

    public void RemoveVariable(string name) {
      throw new NotSupportedException();
    }

    public void AddSubTree(IFunctionTree tree) {
      if (!treesExpanded) throw new InvalidOperationException();
      subTrees.Add(tree);
    }

    public void InsertSubTree(int index, IFunctionTree tree) {
      if (!treesExpanded) throw new InvalidOperationException();
      subTrees.Insert(index, tree);
    }

    public void RemoveSubTree(int index) {
      // sanity check
      if (!treesExpanded) throw new InvalidOperationException();
      subTrees.RemoveAt(index);
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      FlattenVariables();
      FlattenTrees();
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlNode linearRepresentationNode = document.CreateElement("LinearRepresentation");
      foreach (LightWeightFunction f in linearRepresentation) {
        XmlNode entryNode = PersistenceManager.Persist("Function", f.functionType, document, persistedObjects);
        XmlAttribute arityAttribute = document.CreateAttribute("Arity");
        arityAttribute.Value = XmlConvert.ToString(f.arity);
        entryNode.Attributes.Append(arityAttribute);
        foreach (object o in f.localData) {
          if (f.localData.Count > 0) {
            entryNode.AppendChild(CreateDataNode(document, o));
          }
        }
        linearRepresentationNode.AppendChild(entryNode);
      }

      node.AppendChild(linearRepresentationNode);
      return node;
    }

    private XmlNode CreateDataNode(XmlDocument doc, object o) {
      XmlNode node = doc.CreateElement("Data");
      XmlAttribute typeAttr = doc.CreateAttribute("Type");
      node.Attributes.Append(typeAttr);
      if (o is double) {
        node.Value = XmlConvert.ToString((double)o);
        typeAttr.Value = "d";
      } else if (o is int) {
        node.Value = XmlConvert.ToString((int)o);
        typeAttr.Value = "i";
      } else if (o is string) {
        node.Value = (string)o;
        typeAttr.Value = "s";
      } else throw new ArgumentException("Invalid type for local data element: " + o);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      XmlNode linearRepresentationNode = node.SelectSingleNode("LinearRepresentation");
      foreach (XmlNode entryNode in linearRepresentationNode.ChildNodes) {
        LightWeightFunction f = new LightWeightFunction();
        f.arity = XmlConvert.ToByte(entryNode.Attributes["Arity"].Value);
        foreach (XmlNode dataNode in entryNode.ChildNodes) {
          f.localData.Add(ParseDataNode(dataNode));
        }
        f.functionType = (IFunction)PersistenceManager.Restore(entryNode, restoredObjects);
        linearRepresentation.Add(f);
      }
      treesExpanded = false;
      variablesExpanded = false;
    }

    private object ParseDataNode(XmlNode dataNode) {
      string type = dataNode.Attributes["Type"].Value;
      if (type == "d") {
        return XmlConvert.ToDouble(dataNode.Value);
      } else if (type == "i") {
        return XmlConvert.ToInt32(dataNode.Value);
      } else if (type == "s") {
        return dataNode.Value;
      } else throw new FormatException("Can't parse type \"" + type + "\" \"" + dataNode.Value + "\" as local data for GP trees");
    }

    //private string GetString(IEnumerable<double> xs) {
    //  StringBuilder builder = new StringBuilder();
    //  foreach (double x in xs) {
    //    builder.Append(x.ToString("r", CultureInfo.InvariantCulture) + "; ");
    //  }
    //  if (builder.Length > 0) builder.Remove(builder.Length - 2, 2);
    //  return builder.ToString();
    //}

    //private List<T> GetList<T>(string s, Converter<string, T> converter) {
    //  List<T> result = new List<T>();
    //  string[] tokens = s.Split(new char[] { ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
    //  foreach (string token in tokens) {
    //    T x = converter(token.Trim());
    //    result.Add(x);
    //  }
    //  return result;
    //}

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      BakedFunctionTree clone = new BakedFunctionTree();
      // in case the user (de)serialized the tree between evaluation and selection we have to flatten the tree again.
      if (treesExpanded) FlattenTrees();
      if (variablesExpanded) FlattenVariables();
      foreach (LightWeightFunction f in linearRepresentation) {
        clone.linearRepresentation.Add(f.Clone());
      }
      return clone;
    }

    public override IView CreateView() {
      return new FunctionTreeView(this);
    }

    //public override string ToString() {
    //  SymbolicExpressionExporter exporter = new SymbolicExpressionExporter();
    //  exporter.Visit(this);
    //  return exporter.GetStringRepresentation();
    //}
  }
}
