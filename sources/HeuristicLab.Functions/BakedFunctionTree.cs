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
using HeuristicLab.DataAnalysis;
using HeuristicLab.Data;
using System.Xml;
using System.Globalization;

namespace HeuristicLab.Functions {

  class LightWeightFunction {
    public int arity = 0;
    public IFunction functionType;
    public List<double> data = new List<double>();

    public LightWeightFunction Clone() {
      LightWeightFunction clone = new LightWeightFunction();
      clone.arity = arity;
      clone.functionType = functionType;
      clone.data.AddRange(data);
      return clone;
    }
  }

  class BakedFunctionTree : ItemBase, IFunctionTree {
    private List<LightWeightFunction> linearRepresentation;
    private bool treesExpanded = false;
    private List<IFunctionTree> subTrees;
    private bool variablesExpanded = false;
    private List<IVariable> variables;
    private BakedTreeEvaluator evaluator = null;

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
      foreach(IVariableInfo variableInfo in function.VariableInfos) {
        if(variableInfo.Local) {
          variables.Add((IVariable)function.GetVariable(variableInfo.FormalName).Clone());
        }
      }
    }

    internal BakedFunctionTree(IFunctionTree tree)
      : this() {
      LightWeightFunction fun = new LightWeightFunction();
      fun.functionType = tree.Function;
      linearRepresentation.Add(fun);
      foreach(IVariable variable in tree.LocalVariables) {
        IItem value = variable.Value;
        fun.data.Add(GetDoubleValue(value));
      }
      foreach(IFunctionTree subTree in tree.SubTrees) {
        AddSubTree(new BakedFunctionTree(subTree));
      }
    }

    private double GetDoubleValue(IItem value) {
      if(value is DoubleData) {
        return ((DoubleData)value).Data;
      } else if(value is ConstrainedDoubleData) {
        return ((ConstrainedDoubleData)value).Data;
      } else if(value is IntData) {
        return ((IntData)value).Data;
      } else if(value is ConstrainedIntData) {
        return ((ConstrainedIntData)value).Data;
      } else throw new NotSupportedException("Invalid datatype of local variable for GP");
    }

    private int BranchLength(int branchRoot) {
      int arity = linearRepresentation[branchRoot].arity;
      int length = 1;
      for(int i = 0; i < arity; i++) {
        length += BranchLength(branchRoot + length);
      }
      return length;
    }

    private void FlattenTrees() {
      if(treesExpanded) {
        linearRepresentation[0].arity = subTrees.Count;
        foreach(BakedFunctionTree subTree in subTrees) {
          subTree.FlattenVariables();
          subTree.FlattenTrees();
          linearRepresentation.AddRange(subTree.linearRepresentation);
        }
        treesExpanded = false;
        subTrees = null;
      }
    }

    private void FlattenVariables() {
      if(variablesExpanded) {
        linearRepresentation[0].data.Clear();
        foreach(IVariable variable in variables) {
          linearRepresentation[0].data.Add(GetDoubleValue(variable.Value));
        }
        variablesExpanded = false;
        variables = null;
      }
    }

    public IList<IFunctionTree> SubTrees {
      get {
        if(!treesExpanded) {
          subTrees = new List<IFunctionTree>();
          int arity = linearRepresentation[0].arity;
          int branchIndex = 1;
          for(int i = 0; i < arity; i++) {
            BakedFunctionTree subTree = new BakedFunctionTree();
            int length = BranchLength(branchIndex);
            for(int j = branchIndex; j < branchIndex + length; j++) {
              subTree.linearRepresentation.Add(linearRepresentation[j].Clone());
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
        if(!variablesExpanded) {
          variables = new List<IVariable>();
          IFunction function = Function;
          int localVariableIndex = 0;
          foreach(IVariableInfo variableInfo in function.VariableInfos) {
            if(variableInfo.Local) {
              IVariable clone = (IVariable)function.GetVariable(variableInfo.FormalName).Clone();
              IItem value = clone.Value;
              if(value is ConstrainedDoubleData) {
                ((ConstrainedDoubleData)value).Data = linearRepresentation[0].data[localVariableIndex];
              } else if(value is ConstrainedIntData) {
                ((ConstrainedIntData)value).Data = (int)linearRepresentation[0].data[localVariableIndex];
              } else if(value is DoubleData) {
                ((DoubleData)value).Data = linearRepresentation[0].data[localVariableIndex];
              } else if(value is IntData) {
                ((IntData)value).Data = (int)linearRepresentation[0].data[localVariableIndex];
              } else throw new NotSupportedException("Invalid local variable type for GP.");
              variables.Add(clone);
              localVariableIndex++;
            }
          }
          variablesExpanded = true;
          linearRepresentation[0].data.Clear();
        }
        return variables;
      }
    }

    public IFunction Function {
      get { return linearRepresentation[0].functionType; }
    }

    public IVariable GetLocalVariable(string name) {
      foreach(IVariable var in LocalVariables) {
        if(var.Name == name) return var;
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
      if(!treesExpanded) throw new InvalidOperationException();
      subTrees.Add(tree);
    }

    public void InsertSubTree(int index, IFunctionTree tree) {
      if(!treesExpanded) throw new InvalidOperationException();
      subTrees.Insert(index, tree);
    }

    public void RemoveSubTree(int index) {
      // sanity check
      if(!treesExpanded) throw new InvalidOperationException();
      subTrees.RemoveAt(index);
    }

    public double Evaluate(Dataset dataset, int sampleIndex) {
      FlattenVariables();
      FlattenTrees();
      if(evaluator == null) evaluator = new BakedTreeEvaluator(linearRepresentation);
      return evaluator.Evaluate(dataset, sampleIndex);
    }


    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      FlattenVariables();
      FlattenTrees();
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      throw new NotImplementedException();
      //XmlAttribute codeAttribute = document.CreateAttribute("LinearRepresentation");
      //codeAttribute.Value = GetString<int>(code);
      //node.Attributes.Append(codeAttribute);
      //return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      throw new NotImplementedException();
      //base.Populate(node, restoredObjects);
      //XmlNode evaluatorNode = node.SelectSingleNode("Evaluator");
      //if(evaluatorNode != null) {
      //  this.evaluator = (BakedTreeEvaluator)PersistenceManager.Restore(evaluatorNode, restoredObjects);
      //}
      //code = GetList<int>(node.Attributes["Code"].Value, s => int.Parse(s, CultureInfo.InvariantCulture));
      //data = GetList<double>(node.Attributes["Data"].Value, s => double.Parse(s, CultureInfo.InvariantCulture));
      //treesExpanded = false;
      //variablesExpanded = false;
    }

    //private string GetString<T>(IEnumerable<T> xs) where T : IConvertible {
    //  StringBuilder builder = new StringBuilder();
    //  foreach(T x in xs) {
    //    builder.Append(x.ToString(CultureInfo.InvariantCulture) + "; ");
    //  }
    //  if(builder.Length > 0) builder.Remove(builder.Length - 2, 2);
    //  return builder.ToString();
    //}

    //private List<T> GetList<T>(string s, Converter<string, T> converter) {
    //  List<T> result = new List<T>();
    //  string[] tokens = s.Split(new char[] {';',' '}, StringSplitOptions.RemoveEmptyEntries);
    //  foreach(string token in tokens) {
    //    T x = converter(token.Trim());
    //    result.Add(x);
    //  }
    //  return result;
    //}

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      BakedFunctionTree clone = new BakedFunctionTree();
      // in case the user (de)serialized the tree between evaluation and selection we have to flatten the tree again.
      if(treesExpanded) FlattenTrees();
      if(variablesExpanded) FlattenVariables();
      foreach(LightWeightFunction f in linearRepresentation) {
        clone.linearRepresentation.Add(f.Clone());
      }
      return clone;
    }

    public override IView CreateView() {
      return new FunctionTreeView(this);
    }
  }
}
