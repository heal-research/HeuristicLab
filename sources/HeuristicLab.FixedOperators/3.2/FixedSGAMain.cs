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
using System.IO;
using HeuristicLab.Random;

namespace HeuristicLab.FixedOperators {
  class FixedSGAMain : FixedOperatorBase {
    public override string Description {
      get { return @"Implements the functionality of SGAMain with fixed control structures. Operators like selection, crossover, mutation and evaluation are delegated."; }
    }

    // Shared
    protected Sorter sorter;

    // CreateChildren
    protected Counter counter;
    protected IRandom random;
    protected DoubleData probability;
    protected ChildrenInitializer ci;
    protected OperatorBase crossover;
    protected OperatorBase mutator;
    protected OperatorBase evaluator;
    protected SubScopesRemover sr;
    protected StochasticBranch sb;

    protected OperatorBase selector;

    // CreateReplacement
    protected LeftSelector ls;
    protected RightReducer rr;
    protected RightSelector rs;
    protected LeftReducer lr;
    protected MergingReducer mr;

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

      InitCreateChildren();
      InitReplacement();

      sb = new StochasticBranch();
      sb.GetVariableInfo("Probability").ActualName = "MutationRate";
    }

    private void InitReplacement() {
      ls = new LeftSelector();
      rr = new RightReducer();
      rs = new RightSelector();
      lr = new LeftReducer();
      mr = new MergingReducer();

      ls.GetVariableInfo("Selected").ActualName = "Elites";
      rs.GetVariableInfo("Selected").ActualName = "Elites";
    }

    private void InitCreateChildren() {
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
      Stopwatch swApply = new Stopwatch();
      swApply.Start();

      #region Initialization
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

      IntData maxGenerations = GetVariableValue<IntData>("MaximumGenerations", scope, true);
      IntData nrOfGenerations = GetVariableValue<IntData>("Generations", scope, true);

      IntData subscopeNr;
      try {
        subscopeNr = scope.GetVariableValue<IntData>("SubScopeNr", false);
      }
      catch (Exception) {
        subscopeNr = new IntData(0);
        scope.AddVariable(new Variable("SubScopeNr", subscopeNr));
      }

      ci = new ChildrenInitializer();


      GetOperatorsFromScope(scope);

      try {
        sb.RemoveSubOperator(0);
      }
      catch (Exception) {
      }
      sb.AddSubOperator(mutator);


      IScope s;
      IScope s2;
      #endregion
      try {
        for (; nrOfGenerations.Data < maxGenerations.Data; nrOfGenerations.Data++) {
          Execute(selector, scope);

          ////// Create Children //////
          // ChildrenInitializer
          s = scope.SubScopes[1];
          Execute(ci, s);

          SaveExecutionPointer();
          // UniformSequentialSubScopesProcessor
          for (; subscopeNr.Data < s.SubScopes.Count; subscopeNr.Data++) {
            SetExecutionPointerToLastSaved();

            s2 = s.SubScopes[subscopeNr.Data];
            Execute(crossover, s2);
            // Stochastic Branch
            Execute(sb, s2);

            // ganz böse!!!!!!!
            // wird nach dem stochastic branch angehalten und später fortgesetzt,
            // wird eine Zufallszahl erzeugt, die aber nicht verwendet wird.
            // Dadurch kommt der GA auf ein anderes Endergebnis
            // Lösung: Stochastic Branch Operator verwenden
            //randomNumber = random.NextDouble();
            //output.AppendLine(randomNumber.ToString());
            //if (randomNumber < probability.Data)
            //  Execute(mutator, s2);
            //else
            //  Execute(empty, s2);

            Execute(evaluator, s2);
            Execute(sr, s2);
            Execute(counter, s2);
          } // foreach

          Execute(sorter, s);
          ////// END Create Children //////

          DoReplacement(scope);
          Execute(ql, scope);
          Execute(bawqc, scope);
          Execute(dc, scope);
          Execute(lci, scope);
          subscopeNr.Data = 0;
          ResetExecutionPointer();
        } // for i

        //TextWriter tw = new StreamWriter(DateTime.Now.ToFileTime() + ".txt");
        //tw.Write(output.ToString());
        //tw.Close();
        //output = new StringBuilder();

        swApply.Stop();
        Console.WriteLine("SGAMain.Apply(): {0}", swApply.Elapsed);
      } // try
      catch (CancelException) {
        Console.WriteLine("Micro engine aborted by cancel flag.");
        return new AtomicOperation(this, scope);
      }

      return null;
    } // Apply

    /// <summary>
    /// Fetch main operators like selector, crossover, mutator, ... from scope
    /// and store them in instance variables.
    /// </summary>
    /// <param name="scope"></param>
    protected void GetOperatorsFromScope(IScope scope) {
      selector = (OperatorBase)GetVariableValue("Selector", scope, true);
      crossover = (OperatorBase)GetVariableValue("Crossover", scope, true);
      mutator = (OperatorBase)GetVariableValue("Mutator", scope, true);
      evaluator = GetVariableValue<OperatorBase>("Evaluator", scope, true);

      random = GetVariableValue<IRandom>("Random", scope, true);
      probability = GetVariableValue<DoubleData>("MutationRate", scope, true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scope"></param>
    protected void CreateChildren(IScope scope) {
      // ChildrenInitializer
      Execute(ci, scope);
      // UniformSequentialSubScopesProcessor
      foreach (IScope s in scope.SubScopes) {
        Execute(crossover, s);
        // Stochastic Branch
        if (random.NextDouble() < probability.Data)
          Execute(mutator, s);
        Execute(evaluator, s);
        Execute(sr, s);
        Execute(counter, s);
      } // foreach

      Execute(sorter, scope);
    } // CreateChildren

    protected void DoReplacement(IScope scope) {
      //// SequentialSubScopesProcessor
      Execute(ls, scope.SubScopes[0]);
      Execute(rr, scope.SubScopes[0]);

      Execute(rs, scope.SubScopes[1]);
      Execute(lr, scope.SubScopes[1]);

      Execute(mr, scope);
      Execute(sorter, scope);
    } // DoReplacement
  } // class FixedSGAMain
} // namespace HeuristicLab.FixedOperators
