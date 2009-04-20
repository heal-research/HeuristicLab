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
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Permutation;
using HeuristicLab.Evolutionary;
using HeuristicLab.Operators;
using HeuristicLab.Routing.TSP;
using HeuristicLab.Logging;
using System.Diagnostics;

namespace HeuristicLab.SGA.Hardwired {
  class SGAMainWithHWControllStructures : CombinedOperator {
    public override string Description {
      get { return @"Implements the SGAMain with hardwired control structures. Operators are delegated."; }
    }

    ChildrenInitializer ci;
    OperatorBase crossover;
    OperatorBase mutator;
    OperatorBase evaluator;
    SubScopesRemover sr;
    Counter counter;
    Sorter sorter;
    IRandom random;
    DoubleData probability;

    public SGAMainWithHWControllStructures()
      : base() {
      AddVariableInfo(new VariableInfo("Selector", "Selection strategy for SGA", typeof(OperatorBase), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaximumGenerations", "Maximum number of generations to create", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Generations", "Number of processed generations", typeof(IntData), VariableKind.In | VariableKind.Out));
      Name = "SGAMainWithHWControllStructures";

      //InitCreateChildrenHWCS();
      InitCreateChildrenHW();

    }

    private void InitCreateChildrenHW() {
      // variables infos
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("MutationRate", "Probability to choose first branch", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Crossover", "Crossover strategy for SGA", typeof(OperatorBase), VariableKind.In));
      AddVariableInfo(new VariableInfo("Mutator", "Mutation strategy for SGA", typeof(OperatorBase), VariableKind.In));
      AddVariableInfo(new VariableInfo("Evaluator", "Evaluation strategy for SGA", typeof(OperatorBase), VariableKind.In));
      AddVariableInfo(new VariableInfo("EvaluatedSolutions", "Number of evaluated solutions", typeof(IntData), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("Maximization", "Sort in descending order", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Quality", "Sorting value", typeof(DoubleData), VariableKind.In));
    }

    private void InitCreateChildrenHWCS() {
      // variables for create children
      ci = new ChildrenInitializer();

      // variables infos
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("MutationRate", "Probability to choose first branch", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Crossover", "Crossover strategy for SGA", typeof(OperatorBase), VariableKind.In));
      AddVariableInfo(new VariableInfo("Mutator", "Mutation strategy for SGA", typeof(OperatorBase), VariableKind.In));
      AddVariableInfo(new VariableInfo("Evaluator", "Evaluation strategy for SGA", typeof(OperatorBase), VariableKind.In));

      sr = new SubScopesRemover();
      sr.GetVariableInfo("SubScopeIndex").Local = true;

      counter = new Counter();
      counter.GetVariableInfo("Value").ActualName = "EvaluatedSolutions";

      sorter = new Sorter();
      sorter.GetVariableInfo("Descending").ActualName = "Maximization";
      sorter.GetVariableInfo("Value").ActualName = "Quality";
    }

    public override IOperation Apply(IScope scope) {

      //base.Apply(scope); // noch nachfragen ob das auch in ordnung wäre
      for (int i = 0; i < SubOperators.Count; i++) {
        if (scope.GetVariable(SubOperators[i].Name) != null)
          scope.RemoveVariable(SubOperators[i].Name);
        scope.AddVariable(new Variable(SubOperators[i].Name, SubOperators[i]));
      }

      OperatorBase selector = (OperatorBase)GetVariableValue("Selector", scope, true);
      OperatorBase createChildren = new CreateChildren();
      OperatorBase createReplacement = new CreateReplacement();
      QualityLogger ql = new QualityLogger();

      BestAverageWorstQualityCalculator bawqc = new BestAverageWorstQualityCalculator();
      DataCollector dc = new DataCollector();
      ItemList<StringData> names = dc.GetVariable("VariableNames").GetValue<ItemList<StringData>>();
      names.Add(new StringData("BestQuality"));
      names.Add(new StringData("AverageQuality"));
      names.Add(new StringData("WorstQuality"));

      LinechartInjector lci = new LinechartInjector();
      lci.GetVariableInfo("Linechart").ActualName = "Quality Linechart";
      lci.GetVariable("NumberOfLines").GetValue<IntData>().Data = 3;

      Counter c = new Counter();
      c.GetVariableInfo("Value").ActualName = "Generations";

      LessThanComparator ltc = new LessThanComparator();
      ltc.GetVariableInfo("LeftSide").ActualName = "Generations";
      ltc.GetVariableInfo("RightSide").ActualName = "MaximumGenerations";
      ltc.GetVariableInfo("Result").ActualName = "GenerationsCondition";

      IntData maxGenerations = GetVariableValue<IntData>("MaximumGenerations", scope, true);
      IntData nrOfGenerations = GetVariableValue<IntData>("Generations", scope, true);

      InitializeExecuteCreateChildren(scope);
      IntData evaluatedSolutions = GetVariableValue<IntData>("EvaluatedSolutions", scope, true);
      Stopwatch watch = new Stopwatch();
      for (int i = 0; i < maxGenerations.Data; i++) {
        selector.Execute(scope);
        //createChildren.Execute(scope.SubScopes[1]);
        //ExecuteCreateChildrenHWCS(scope.SubScopes[1]);
        ExecuteCreateChildrenHW(scope.SubScopes[1], evaluatedSolutions);
        createReplacement.Execute(scope);
        ql.Execute(scope);
        bawqc.Execute(scope);
        dc.Execute(scope);
        lci.Execute(scope);
        nrOfGenerations.Data++;
      }

     
      return null;
    } // Apply

    private void InitializeExecuteCreateChildren(IScope scope) {
      crossover = (OperatorBase)GetVariableValue("Crossover", scope, true);
      mutator = (OperatorBase)GetVariableValue("Mutator", scope, true);
      evaluator = GetVariableValue<OperatorBase>("Evaluator", scope, true);

      random = GetVariableValue<IRandom>("Random", scope, true);
      probability = GetVariableValue<DoubleData>("MutationRate", scope, true);

      ci = new ChildrenInitializer();
    }

    private void ExecuteCreateChildrenHWCS(IScope scope) {
      // ChildrenInitializer
      ci.Apply(scope);
      // UniformSequentialSubScopesProcessor
      foreach (IScope s in scope.SubScopes) {
        crossover.Execute(s);
        // Stochastic Branch
        if (random.NextDouble() < probability.Data)
          mutator.Execute(s);
        evaluator.Execute(s);
        sr.Execute(s);
        counter.Execute(s);
      } // foreach

      sorter.Execute(scope);
    } // ExecuteCreateChildrenHWCS

    private void ExecuteCreateChildrenHW(IScope scope, IntData evaluatedSolutions){
      // ChildrenInitializer
      ci.Apply(scope);
      // UniformSequentialSubScopesProcessor
      foreach (IScope s in scope.SubScopes) {
        if (crossover.Execute(s) != null)
          throw new InvalidOperationException("ERROR: no support for combined operators!");

        // Stochastic Branch
        if (random.NextDouble() < probability.Data) {
          if (mutator.Execute(s) != null)
            throw new InvalidOperationException("ERROR: no support for combined operators!");
        }

        if (evaluator.Execute(s) != null)
          throw new InvalidOperationException("ERROR: no support for combined operators!");

        // subscopes remover
        while (s.SubScopes.Count > 0)
          s.RemoveSubScope(s.SubScopes[0]);

        evaluatedSolutions.Data++;
      } // foreach

      // sort scopes
      //bool descending = GetVariableValue<BoolData>("Maximization", scope, true).Data;
      //double[] keys = new double[scope.SubScopes.Count];
      //int[] sequence = new int[keys.Length];

      //for (int i = 0; i < keys.Length; i++) {
      //  keys[i] = scope.SubScopes[i].GetVariableValue<DoubleData>("Quality", false).Data;
      //  sequence[i] = i;
      //}

      //Array.Sort<double, int>(keys, sequence);

      //if (descending) {
      //  int temp;
      //  for (int i = 0; i < sequence.Length / 2; i++) {
      //    temp = sequence[i];
      //    sequence[i] = sequence[sequence.Length - 1 - i];
      //    sequence[sequence.Length - 1 - i] = temp;
      //  }
      //}
      //scope.ReorderSubScopes(sequence);

      return;
    }

  } // class SGAMainWithHWControllStructures
} // namespace HeuristicLab.SGA.Hardwired
