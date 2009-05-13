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
using HeuristicLab.Selection;
using System.Threading;

namespace HeuristicLab.FixedOperators {
  class FixedSGAMain : FixedOperatorBase {
    public override string Description {
      get { return @"Implements the functionality of SGAMain with fixed control structures. Operators like selection, crossover, mutation and evaluation are delegated."; }
    }

    // Shared
    Sorter sorter;

    // CreateChildren
    Counter counter;
    IRandom random;
    DoubleData probability;
    ChildrenInitializer ci;
    OperatorBase crossover;
    OperatorBase mutator;
    OperatorBase evaluator;
    SubScopesRemover sr;

    // CreateReplacement
    LeftSelector ls;
    RightReducer rr;
    RightSelector rs;
    LeftReducer lr;
    MergingReducer mr;

    Thread executionThread;
    Thread cancelThread;


    //long[] timesExecuteCreateChildren;
    public FixedSGAMain()
      : base() {
      AddVariableInfo(new VariableInfo("Selector", "Selection strategy for SGA", typeof(OperatorBase), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaximumGenerations", "Maximum number of generations to create", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Generations", "Number of processed generations", typeof(IntData), VariableKind.In | VariableKind.Out));

      Name = "FixedSGAMain";

      sorter = new Sorter();
      sorter.GetVariableInfo("Descending").ActualName = "Maximization";
      sorter.GetVariableInfo("Value").ActualName = "Quality";

      InitVariablesForCreateChildren();
      InitVariablesForCreateReplacement();
    }

    private void InitVariablesForCreateReplacement() {
      ls = new LeftSelector();
      rr = new RightReducer();
      rs = new RightSelector();
      lr = new LeftReducer();
      mr = new MergingReducer();

      ls.GetVariableInfo("Selected").ActualName = "Elites";
      rs.GetVariableInfo("Selected").ActualName = "Elites";

    }

    protected void InitVariablesForCreateChildren() {
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
    }

    public override IOperation Apply(IScope scope) {
      base.Apply(scope);
      object o = (object)scope;
      WorkerMethod(o);
      //try {
      //  executionThread = new Thread(new ParameterizedThreadStart(WorkerMethod));
      //  executionThread.Name = "ExecutionThread";
      //  executionThread.Start(o);

      //  //cancelThread = new Thread(new ThreadStart(CheckCancelFlag));
      //  //cancelThread.Name = "CancelFlagCheckerThread";
      //  //cancelThread.Start();

      //  executionThread.Join();
      //}
      //catch (ThreadAbortException) {
      //  return new AtomicOperation(this, scope);
      //}

      if (Canceled) {
        return new AtomicOperation(this, scope);
      }

      return null;
    } // Apply

    private void CheckCancelFlag() {
      while (executionThread.IsAlive) {
        if (Canceled) {
          executionThread.Abort();
          return;
        }
        Thread.Sleep(500);
      } // while
    } // CheckCancelFlag

    private void WorkerMethod(object o) {
      try {
        IScope scope = o as IScope;
        Stopwatch swApply = new Stopwatch();
        swApply.Start();
        for (int i = 0; i < SubOperators.Count; i++) {
          if (scope.GetVariable(SubOperators[i].Name) != null)
            scope.RemoveVariable(SubOperators[i].Name);
          scope.AddVariable(new Variable(SubOperators[i].Name, SubOperators[i]));
        }

        OperatorBase selector = (OperatorBase)GetVariableValue("Selector", scope, true);
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

        LessThanComparator ltc = new LessThanComparator();
        ltc.GetVariableInfo("LeftSide").ActualName = "Generations";
        ltc.GetVariableInfo("RightSide").ActualName = "MaximumGenerations";
        ltc.GetVariableInfo("Result").ActualName = "GenerationsCondition";

        IntData maxGenerations = GetVariableValue<IntData>("MaximumGenerations", scope, true);
        IntData nrOfGenerations = GetVariableValue<IntData>("Generations", scope, true);
        nrOfGenerations.Data = 0;

        threaded = false;
        // fetch variables from scope for create children
        InitializeExecuteCreateChildren(scope);
        try {
          //for (int i = nrOfGenerations.Data; i < maxGenerations.Data; i++) {
          //  Execute(selector, scope, false);
          //  ExecuteCreateChildrenWithFixedControlStructures(scope.SubScopes[1]);
          //  ExecuteCreateReplacementWithFixedConstrolStructures(scope);
          //  ql.Execute(scope);
          //  bawqc.Execute(scope);
          //  dc.Execute(scope);
          //  lci.Execute(scope);
          //  nrOfGenerations.Data++;
          //} // for i
          for (int i = 0; i < maxGenerations.Data; i++) {
            Execute(selector, scope, false);
            ExecuteCreateChildrenWithFixedControlStructures(scope.SubScopes[1]);
            ExecuteCreateReplacementWithFixedConstrolStructures(scope);
            Execute(ql, scope, false);
            Execute(bawqc, scope, false);
            Execute(dc, scope, false);
            Execute(lci, scope, false);
            nrOfGenerations.Data++;
          } // for i
        } // try
        catch (CancelException) {
          Console.WriteLine("Micro engine aborted by cancel flag.");
        }

        //for (int i = nrOfGenerations.Data; i < maxGenerations.Data && !Canceled; i++) {
        //  if (Canceled) {
        //    executionPointer.Data = -1;
        //    continue;
        //  }
        //  if (executionPointer.Data < 0)
        //    Execute(selector, scope);

        //  if (Canceled) {
        //    executionPointer.Data = 0;
        //    continue;
        //  }
        //  if (executionPointer.Data < 1)
        //    ExecuteCreateChildrenWithFixedControlStructures(scope.SubScopes[1]);

        //  if (Canceled) {
        //    executionPointer.Data = 1;
        //    continue;
        //  }
        //  if (executionPointer.Data < 2)
        //    ExecuteCreateReplacementWithFixedConstrolStructures(scope);

        //  if (Canceled) {
        //    executionPointer.Data = 2;
        //    continue;
        //  }
        //  if (executionPointer.Data < 3) {
        //    ql.Execute(scope);
        //    bawqc.Execute(scope);
        //    dc.Execute(scope);
        //    lci.Execute(scope);
        //    nrOfGenerations.Data++;
        //  }
        //  executionPointer.Data = -1;
        //} // for i


        //Stopwatch watch = new Stopwatch();
        //long[] times = new long[10];
        //timesExecuteCreateChildren = new long[10];
        //for (int i = nrOfGenerations.Data; i < maxGenerations.Data && !Canceled; i++) {
        //  watch.Start();
        //  selector.Execute(scope);
        //  watch.Stop();
        //  times[0] += watch.ElapsedTicks;
        //  watch.Reset();
        //  watch.Start();
        //  ExecuteCreateChildrenWithFixedControlStructures(scope.SubScopes[1]);
        //  watch.Stop();
        //  times[1] += watch.ElapsedTicks;
        //  watch.Reset();
        //  watch.Start();
        //  ExecuteCreateReplacementWithFixedConstrolStructures(scope);
        //  watch.Stop();
        //  times[2] += watch.ElapsedTicks;
        //  watch.Reset();
        //  watch.Start();
        //  ql.Execute(scope);
        //  watch.Stop();
        //  times[3] += watch.ElapsedTicks;
        //  watch.Reset();
        //  watch.Start();
        //  bawqc.Execute(scope);
        //  watch.Stop();
        //  times[4] += watch.ElapsedTicks;
        //  watch.Reset();
        //  watch.Start();
        //  dc.Execute(scope);
        //  watch.Stop();
        //  times[5] += watch.ElapsedTicks;
        //  watch.Reset();
        //  watch.Start();
        //  lci.Execute(scope);
        //  watch.Stop();
        //  times[6] += watch.ElapsedTicks;
        //  watch.Reset();
        //  watch.Start();
        //  nrOfGenerations.Data++;
        //}

        swApply.Stop();
        Console.WriteLine("SGAMain.Apply(): {0}", swApply.Elapsed);

        //if (Canceled) {
        //  return new AtomicOperation(this, scope);
        //}

        //return null;

      }
      catch (ThreadAbortException) {

        throw;
      }
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scope"></param>
    protected void ExecuteCreateChildrenWithFixedControlStructures(IScope scope) {
      //// ChildrenInitializer
      //ci.Apply(scope);
      //// UniformSequentialSubScopesProcessor
      //foreach (IScope s in scope.SubScopes) {
      //  Execute(crossover, s, false);
      //  // Stochastic Branch
      //  if (random.NextDouble() < probability.Data)
      //    Execute(mutator, s, false);
      //  Execute(evaluator, s, false);
      //  sr.Execute(s);
      //  counter.Execute(s);
      //} // foreach

      //sorter.Execute(scope);

      EmptyOperator empty = new EmptyOperator();

      // ChildrenInitializer
      Execute(ci, scope, false);
      // UniformSequentialSubScopesProcessor
      foreach (IScope s in scope.SubScopes) {
        Execute(crossover, s, false);
        // Stochastic Branch
        if (random.NextDouble() < probability.Data)
          Execute(mutator, s, false);
        else
          Execute(empty, s, false);
        Execute(evaluator, s, false);
        Execute(sr, s, false);
        Execute(counter, s, false);
      } // foreach

      Execute(sorter, scope, false);
    } // ExecuteCreateChildrenHWCS

    private void ExecuteCreateReplacementWithFixedConstrolStructures(IScope scope) {
      //// SequentialSubScopesProcessor
      //ls.Execute(scope.SubScopes[0]);
      //rr.Execute(scope.SubScopes[0]);

      //rs.Execute(scope.SubScopes[1]);
      //lr.Execute(scope.SubScopes[1]);

      //mr.Execute(scope);
      //sorter.Execute(scope);

      Execute(ls, scope.SubScopes[0], false);
      Execute(rr, scope.SubScopes[0], false);

      Execute(rs, scope.SubScopes[1], false);
      Execute(lr, scope.SubScopes[1], false);

      Execute(mr, scope, false);
      Execute(sorter, scope, false);

    } // ExecuteCreateReplacementWithFixedConstrolStructures

    //public override void Abort() {
    //  base.Abort();
    //  executionThread.Abort();
    //} // Abort

  } // class FixedSGAMain
} // namespace HeuristicLab.FixedOperators
