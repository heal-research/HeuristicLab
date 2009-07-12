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
  /// <summary>
  /// Fixed GA base
  /// </summary>
  class FixedGAMainBase : FixedOperatorBase {

    protected OperatorBase selector;
    protected Sorter sorter;

    // operators for CreateChildren
    protected Counter counter;
    protected ChildrenInitializer ci;
    protected OperatorBase crossover;
    protected OperatorBase mutator;
    protected OperatorBase evaluator;
    protected SubScopesRemover sr;
    protected StochasticBranch sb;

    // operators for CreateReplacement
    protected LeftSelector ls;
    protected RightReducer rr;
    protected RightSelector rs;
    protected LeftReducer lr;
    protected MergingReducer mr;
    
    protected QualityLogger ql;
    protected BestAverageWorstQualityCalculator bawqc;
    protected DataCollector dc;
    protected LinechartInjector lci;
    protected IntData maxGenerations;
    protected IntData nrOfGenerations;
    protected IntData subscopeNr;

    public FixedGAMainBase() : base() {
      AddVariableInfo(new VariableInfo("Selector", "Selection strategy for SGA", typeof(OperatorBase), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaximumGenerations", "Maximum number of generations to create", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Generations", "Number of processed generations", typeof(IntData), VariableKind.In | VariableKind.Out));                                               

      sorter = new Sorter();
      sorter.GetVariableInfo("Descending").ActualName = "Maximization";
      sorter.GetVariableInfo("Value").ActualName = "Quality";

      InitCreateChildren();
      InitReplacement();

      sb = new StochasticBranch();
      sb.GetVariableInfo("Probability").ActualName = "MutationRate";
      Name = "FixedGAMain";
    } // FixedGABase

    protected virtual void InitReplacement() {
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

      #region Initialization
      ql = new QualityLogger();

      bawqc = new BestAverageWorstQualityCalculator();
      dc = new DataCollector();
      ItemList<StringData> names = dc.GetVariable("VariableNames").GetValue<ItemList<StringData>>();
      names.Add(new StringData("BestQuality"));
      names.Add(new StringData("AverageQuality"));
      names.Add(new StringData("WorstQuality"));

      lci = new LinechartInjector();
      lci.GetVariableInfo("Linechart").ActualName = "Quality Linechart";
      lci.GetVariable("NumberOfLines").GetValue<IntData>().Data = 3;

      maxGenerations = GetVariableValue<IntData>("MaximumGenerations", scope, true);
      nrOfGenerations = GetVariableValue<IntData>("Generations", scope, true);

      
      try {
        subscopeNr = scope.GetVariableValue<IntData>("SubScopeNr", false);
      }
      catch (Exception) {
        subscopeNr = new IntData(0);
        scope.AddVariable(new Variable("SubScopeNr", subscopeNr));
      }

      GetOperatorsFromScope(scope);

      try {
        sb.RemoveSubOperator(0);
      }
      catch (Exception) {
      }
      sb.AddSubOperator(mutator);
      #endregion
      
      return null;
    }

    /// <summary>
    /// Fetch main operators like selector, crossover, mutator, ... from scope
    /// and store them in instance variables.
    /// </summary>
    /// <param name="scope"></param>
    protected virtual void GetOperatorsFromScope(IScope scope) {
      selector = (OperatorBase)GetVariableValue("Selector", scope, true);
      crossover = (OperatorBase)GetVariableValue("Crossover", scope, true);
      mutator = (OperatorBase)GetVariableValue("Mutator", scope, true);
      evaluator = GetVariableValue<OperatorBase>("Evaluator", scope, true);
    } // GetOperatorsFromScope

    protected virtual void DoReplacement(IScope scope) {
      //// SequentialSubScopesProcessor
      Execute(ls, scope.SubScopes[0]);
      Execute(rr, scope.SubScopes[0]);

      Execute(rs, scope.SubScopes[1]);
      Execute(lr, scope.SubScopes[1]);

      Execute(mr, scope);
      Execute(sorter, scope);

    } // DoReplacement
  
  } // class FixedGABase
} // namespace HeuristicLab.FixedOperators  
