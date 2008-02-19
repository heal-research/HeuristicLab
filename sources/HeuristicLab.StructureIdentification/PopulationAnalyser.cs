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
using HeuristicLab.Operators;
using HeuristicLab.Functions;
using HeuristicLab.Data;

namespace HeuristicLab.StructureIdentification {
  public class PopulationAnalyser : OperatorBase {
    public override string Description {
      get { return @"Generates statistics about the population. (for debugging)"; }
    }

    public PopulationAnalyser()
      : base() {
    }

    public override IOperation Apply(IScope topScope) {

      Scope resultsScope = new Scope("Pop. analyzer results");
      IScope scope = topScope.SubScopes[0];

      var functionTrees = scope.SubScopes.Select(s => s.GetVariableValue<IFunction>("OperatorTree", false));
      var treeSizes = scope.SubScopes.Select(s => s.GetVariableValue<IntData>("TreeSize", false).Data);
      var treeHeights = scope.SubScopes.Select(s => s.GetVariableValue<IntData>("TreeHeight", false).Data);
      var qualities = scope.SubScopes.Select(s => s.GetVariableValue<DoubleData>("Quality", false).Data);

      var allFunctions = functionTrees.Select(t => AllNodes(t)).Aggregate((x, y) => { x.AddRange(y); return x; });

      //var results = scope.SubScopes.Select(s => (DoubleArrayData)s.GetVariableValue<DoubleArrayData>("Results", false));
      //var resultSums = results.Select(a => a.Data.Aggregate(0.0, (x, y) => x + y));

      var functionsHistogram = from f in allFunctions
                               group f by ((StringData)f.GetVariable("TypeId").Value).Data into g
                               select new { FunctionType = g.Key, Functions = g };

      resultsScope.AddVariable(new HeuristicLab.Core.Variable("AvgTreeSize", new DoubleData(treeSizes.Average())));
      resultsScope.AddVariable(new HeuristicLab.Core.Variable("MinTreeSize", new DoubleData(treeSizes.Min())));
      resultsScope.AddVariable(new HeuristicLab.Core.Variable("MaxTreeSize", new DoubleData(treeSizes.Max())));

      resultsScope.AddVariable(new HeuristicLab.Core.Variable("AvgTreeHeight", new DoubleData(treeHeights.Average())));
      resultsScope.AddVariable(new HeuristicLab.Core.Variable("MinTreeHeight", new DoubleData(treeHeights.Min())));
      resultsScope.AddVariable(new HeuristicLab.Core.Variable("MaxTreeHeight", new DoubleData(treeHeights.Max())));

      resultsScope.AddVariable(new HeuristicLab.Core.Variable("AvgQuality", new DoubleData(qualities.Average())));
      resultsScope.AddVariable(new HeuristicLab.Core.Variable("MinQuality", new DoubleData(qualities.Min())));
      resultsScope.AddVariable(new HeuristicLab.Core.Variable("MaxQuality", new DoubleData(qualities.Max())));

      ItemList list = new ItemList();
      PrefixVisitor visitor = new PrefixVisitor();
      foreach(IFunction function in functionTrees) {
        visitor.Reset();
        function.Accept(visitor);
        list.Add(new StringData(visitor.Representation));
      }

      resultsScope.AddVariable(new HeuristicLab.Core.Variable("Functions", list));

      topScope.SubScopes[1].AddSubScope(resultsScope);
      return null;
    }

    private List<IFunction> AllNodes(IFunction f) {
      List<IFunction> result = new List<IFunction>();
      result.Add(f);
      if(f.SubFunctions.Count == 0) {
        return result;
      } else return f.SubFunctions.Select(subFunction => AllNodes(subFunction)).Aggregate(result, (x, y) => { x.AddRange(y); return x; });
    }

    private class PrefixVisitor : IFunctionVisitor {
      private string representation;
      public string Representation {
        get { return representation; }
      }

      public void Reset() {
        representation = "";
      }

      private void VisitFunction(string name, IFunction f) {
        representation += "(" + name;
        foreach(IFunction subFunction in f.SubFunctions) {
          representation += " ";
          subFunction.Accept(this);
        }
        representation += ")";
      }

      #region IFunctionVisitor Members

      public void Visit(IFunction function) {
        representation += function.Name;
      }

      public void Visit(Addition addition) {
        VisitFunction("+", addition);
      }

      public void Visit(Constant constant) {
        representation += constant.Value.Data.ToString("F2");
      }

      public void Visit(Cosinus cosinus) {
        VisitFunction("cos", cosinus);
      }

      public void Visit(Division division) {
        VisitFunction("/", division);
      }

      public void Visit(Exponential exponential) {
        VisitFunction("expt", exponential);
      }

      public void Visit(Logarithm logarithm) {
        VisitFunction("log", logarithm);
      }

      public void Visit(Multiplication multiplication) {
        VisitFunction("*", multiplication);
      }

      public void Visit(Power power) {
        VisitFunction("pow", power);
      }

      public void Visit(Signum signum) {
        VisitFunction("signum", signum);
      }

      public void Visit(Sinus sinus) {
        VisitFunction("sinus", sinus);
      }

      public void Visit(Sqrt sqrt) {
        VisitFunction("sqrt", sqrt);
      }

      public void Visit(Substraction substraction) {
        VisitFunction("-", substraction);
      }

      public void Visit(Tangens tangens) {
        VisitFunction("tan", tangens);
      }

      public void Visit(HeuristicLab.Functions.Variable variable) {
        string offsetStr;

        if(variable.SampleOffset > 0) {
          offsetStr = "(+ t " + variable.SampleOffset + ")";
        } else if(variable.SampleOffset < 0) {
          offsetStr = "(- t " + -variable.SampleOffset  + ")";
        } else {
          offsetStr = "(t)";
        }
        int v = variable.VariableIndex;
        representation+= "(* " + variable.Weight.ToString("F2") + " " + variable.Name + v + offsetStr + ")";
      }

      public void Visit(And and) {
        VisitFunction("and", and);
      }

      public void Visit(Average average) {
        VisitFunction("average", average);
      }

      public void Visit(IfThenElse ifThenElse) {
        VisitFunction("if-then-else", ifThenElse);
      }

      public void Visit(Not not) {
        VisitFunction("not", not);
      }

      public void Visit(Or or) {
        VisitFunction("or", or);
      }

      public void Visit(Xor xor) {
        VisitFunction("xor", xor);
      }

      public void Visit(Equal equal) {
        VisitFunction("eq?", equal);
      }

      public void Visit(LessThan lessThan) {
        VisitFunction("<", lessThan);
      }

      #endregion
    }
  }
}
