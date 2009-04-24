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

namespace HeuristicLab.FixedOperators {
  class FixedAllSGAMain : CombinedOperator {
    public override string Description {
      get { return @"Implements the functionality of SGAMain with fixed control structures. Operators like selection, crossover, mutation and evaluation are delegated."; }
    }

    ChildrenInitializer ci;
    OperatorBase crossover;
    OperatorBase mutator;
    OperatorBase evaluator;
    IRandom random;
    DoubleData probability;

    long[] timesExecuteCreateChildren;

    public FixedAllSGAMain()
      : base() {
      AddVariableInfo(new VariableInfo("Selector", "Selection strategy for SGA", typeof(OperatorBase), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaximumGenerations", "Maximum number of generations to create", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Generations", "Number of processed generations", typeof(IntData), VariableKind.In | VariableKind.Out));
      Name = "FixedAllSGAMain";
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

    public override IOperation Apply(IScope scope) {
      Stopwatch swApply = new Stopwatch();
      swApply.Start();
      //base.Apply(scope); // noch nachfragen ob das auch in ordnung wäre
      for (int i = 0; i < SubOperators.Count; i++) {
        if (scope.GetVariable(SubOperators[i].Name) != null)
          scope.RemoveVariable(SubOperators[i].Name);
        scope.AddVariable(new Variable(SubOperators[i].Name, SubOperators[i]));
      }

      OperatorBase selector = (OperatorBase)GetVariableValue("Selector", scope, true);
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
      long[] times = new long[10];
      timesExecuteCreateChildren = new long[10];
      for (int i = 0; i < maxGenerations.Data; i++) {
        watch.Start();
        selector.Execute(scope);
        watch.Stop();
        times[0] += watch.ElapsedTicks;
        watch.Reset();
        watch.Start();
        ExecuteCreateChildren(scope.SubScopes[1], evaluatedSolutions);
        watch.Stop();
        times[1] += watch.ElapsedTicks;
        watch.Reset();
        watch.Start();
        createReplacement.Execute(scope);
        watch.Stop();
        times[2] += watch.ElapsedTicks;
        watch.Reset();
        watch.Start();
        ql.Execute(scope);
        watch.Stop();
        times[3] += watch.ElapsedTicks;
        watch.Reset();
        watch.Start();
        bawqc.Execute(scope);
        watch.Stop();
        times[4] += watch.ElapsedTicks;
        watch.Reset();
        watch.Start();
        dc.Execute(scope);
        watch.Stop();
        times[5] += watch.ElapsedTicks;
        watch.Reset();
        watch.Start();
        lci.Execute(scope);
        watch.Stop();
        times[6] += watch.ElapsedTicks;
        watch.Reset();
        watch.Start();
        nrOfGenerations.Data++;
      }

      swApply.Stop();
      Console.WriteLine("SGAMain.Apply(): {0}", swApply.Elapsed);
      return null;
    } // Apply

    /// <summary>
    /// Initializes some variables needed before the execution of create children
    /// </summary>
    /// <param name="scope"></param>
    private void InitializeExecuteCreateChildren(IScope scope) {
      crossover = (OperatorBase)GetVariableValue("Crossover", scope, true);
      mutator = (OperatorBase)GetVariableValue("Mutator", scope, true);
      evaluator = GetVariableValue<OperatorBase>("Evaluator", scope, true);

      random = GetVariableValue<IRandom>("Random", scope, true);
      probability = GetVariableValue<DoubleData>("MutationRate", scope, true);

      ci = new ChildrenInitializer();
    }

    private void ExecuteCreateChildren(IScope scope, IntData evaluatedSolutions) {
      //Stopwatch watch = new Stopwatch();
      //long[] times = new long[10];
      // ChildrenInitializer
      //watch.Start();
      ci.Apply(scope);
      //watch.Stop();
      //timesExecuteCreateChildren[0] += watch.ElapsedTicks;
      //watch.Reset();
      // UniformSequentialSubScopesProcessor
      foreach (IScope s in scope.SubScopes) {
        //watch.Start();
        if (crossover.Execute(s) != null)
          throw new InvalidOperationException("ERROR: no support for combined operators!");
        //watch.Stop();
        //timesExecuteCreateChildren[1] += watch.ElapsedTicks;
        //watch.Reset();


        // Stochastic Branch
        if (random.NextDouble() < probability.Data) {
          //watch.Start();
          if (mutator.Execute(s) != null)
            throw new InvalidOperationException("ERROR: no support for combined operators!");
          //watch.Stop();
          //timesExecuteCreateChildren[2] += watch.ElapsedTicks;
          //watch.Reset();

        }

        //watch.Start();
        if (evaluator.Execute(s) != null)
          throw new InvalidOperationException("ERROR: no support for combined operators!");
        //watch.Stop();
        //timesExecuteCreateChildren[3] += watch.ElapsedTicks;
        //watch.Reset();

        // subscopes remover
        //watch.Start();
        while (s.SubScopes.Count > 0)
          s.RemoveSubScope(s.SubScopes[0]);
        //watch.Stop();
        //timesExecuteCreateChildren[4] += watch.ElapsedTicks;
        //watch.Reset();

        evaluatedSolutions.Data++;
      } // foreach

      // sort scopes
      bool descending = GetVariableValue<BoolData>("Maximization", scope, true).Data;

      double[] keys = new double[scope.SubScopes.Count];
      int[] sequence = new int[keys.Length];

      for (int i = 0; i < keys.Length; i++) {
        keys[i] = scope.SubScopes[i].GetVariableValue<DoubleData>("Quality", false).Data;
        sequence[i] = i;
      }

      Array.Sort<double, int>(keys, sequence);

      if (descending) {
        int temp;
        for (int i = 0; i < sequence.Length / 2; i++) {
          temp = sequence[i];
          sequence[i] = sequence[sequence.Length - 1 - i];
          sequence[sequence.Length - 1 - i] = temp;
        }
      }
      scope.ReorderSubScopes(sequence);
      //watch.Stop();
      //timesExecuteCreateChildren[5] += watch.ElapsedTicks;
      //watch.Reset();

      return;
    } // ExecuteCreateChildrenHW

  } // class AllFixedSGAMain
} // namespace HeuristicLab.FixedOperators
