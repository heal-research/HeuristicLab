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
using System.Diagnostics;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Operators;
using HeuristicLab.Random;
using HeuristicLab.Selection;
using HeuristicLab.Logging;
using HeuristicLab.Data;
using HeuristicLab.Operators.Programmable;
using HeuristicLab.Selection.OffspringSelection;
using HeuristicLab.Evolutionary;

namespace HeuristicLab.GP.StructureIdentification {
  public class OffspringSelectionGP : StandardGP {
    public override string Name { get { return "OffspringSelectionGP"; } }

    public virtual int MaxEvaluatedSolutions {
      get { return GetVariableInjector().GetVariable("MaxEvaluatedSolutions").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("MaxEvaluatedSolutions").GetValue<IntData>().Data = value; }
    }

    public virtual double SelectionPressureLimit {
      get { return GetVariableInjector().GetVariable("SelectionPressureLimit").GetValue<DoubleData>().Data; }
      set { GetVariableInjector().GetVariable("SelectionPressureLimit").GetValue<DoubleData>().Data = value; }
    }

    public virtual double ComparisonFactor {
      get { return GetVariableInjector().GetVariable("ComparisonFactor").GetValue<DoubleData>().Data; }
      set { GetVariableInjector().GetVariable("ComparisonFactor").GetValue<DoubleData>().Data = value; }
    }

    public virtual double SuccessRatioLimit {
      get { return GetVariableInjector().GetVariable("SuccessRatioLimit").GetValue<DoubleData>().Data; }
      set { GetVariableInjector().GetVariable("SuccessRatioLimit").GetValue<DoubleData>().Data = value; }
    }

    public override int MaxGenerations {
      get { throw new NotSupportedException(); }
      set { /* ignore */ }
    }

    public override int TournamentSize {
      get { throw new NotSupportedException(); }
      set { /* ignore */ }
    }

    public OffspringSelectionGP()
      : base() {
      PopulationSize = 1000;
      Parents = 20;
      MaxEvaluatedSolutions = 5000000;
      SelectionPressureLimit = 400;
      ComparisonFactor = 1.0;
      SuccessRatioLimit = 1.0;
    }

    protected internal override IOperator CreateGlobalInjector() {
      VariableInjector injector = (VariableInjector)base.CreateGlobalInjector();
      injector.RemoveVariable("TournamentSize");
      injector.RemoveVariable("MaxGenerations");
      injector.AddVariable(new HeuristicLab.Core.Variable("MaxEvaluatedSolutions", new IntData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("ComparisonFactor", new DoubleData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("SelectionPressureLimit", new DoubleData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("SuccessRatioLimit", new DoubleData()));
      return injector;
    }

    protected internal override IOperator CreateSelector() {
      CombinedOperator selector = new CombinedOperator();
      selector.Name = "Selector";
      SequentialProcessor seq = new SequentialProcessor();
      seq.Name = "Selector";
      EmptyOperator emptyOp = new EmptyOperator();
      ProportionalSelector femaleSelector = new ProportionalSelector();
      femaleSelector.GetVariableInfo("Selected").ActualName = "Parents";
      femaleSelector.GetVariableValue<BoolData>("CopySelected", null, false).Data = true;

      RandomSelector maleSelector = new RandomSelector();
      maleSelector.GetVariableInfo("Selected").ActualName = "Parents";
      maleSelector.GetVariableValue<BoolData>("CopySelected", null, false).Data = true;
      SequentialSubScopesProcessor seqSubScopesProc = new SequentialSubScopesProcessor();
      RightChildReducer rightChildReducer = new RightChildReducer();
      SubScopesMixer mixer = new SubScopesMixer();

      seqSubScopesProc.AddSubOperator(femaleSelector);
      seqSubScopesProc.AddSubOperator(emptyOp);

      seq.AddSubOperator(maleSelector);
      seq.AddSubOperator(seqSubScopesProc);
      seq.AddSubOperator(rightChildReducer);
      seq.AddSubOperator(mixer);

      selector.OperatorGraph.AddOperator(seq);
      selector.OperatorGraph.InitialOperator = seq;
      return selector;
    }

    protected internal override IOperator CreateChildCreater() {
      CombinedOperator childCreater = new CombinedOperator();
      childCreater.Name = "Create children";
      SequentialProcessor main = new SequentialProcessor();
      SequentialProcessor seq = new SequentialProcessor();
      SequentialProcessor offspringSelectionSeq = new SequentialProcessor();
      OperatorExtractor selector = new OperatorExtractor();
      selector.Name = "Selector (extr.)";
      selector.GetVariableInfo("Operator").ActualName = "Selector";
      SequentialSubScopesProcessor seqSubScopesProc = new SequentialSubScopesProcessor();
      EmptyOperator emptyOp = new EmptyOperator();
      OffspringSelector offspringSelector = new OffspringSelector();
      ChildrenInitializer childInitializer = new ChildrenInitializer();
      UniformSequentialSubScopesProcessor individualProc = new UniformSequentialSubScopesProcessor();
      SequentialProcessor individualSeqProc = new SequentialProcessor();
      OperatorExtractor crossover = new OperatorExtractor();
      crossover.Name = "Crossover (extr.)";
      crossover.GetVariableInfo("Operator").ActualName = "Crossover";
      StochasticBranch cond = new StochasticBranch();
      cond.GetVariableInfo("Probability").ActualName = "MutationRate";
      OperatorExtractor manipulator = new OperatorExtractor();
      manipulator.Name = "Manipulator (extr.)";
      manipulator.GetVariableInfo("Operator").ActualName = "Manipulator";
      OperatorExtractor evaluator = new OperatorExtractor();
      evaluator.Name = "Evaluator (extr.)";
      evaluator.GetVariableInfo("Operator").ActualName = "Evaluator";
      Counter evalCounter = new Counter();
      evalCounter.GetVariableInfo("Value").ActualName = "EvaluatedSolutions";
      WeightedOffspringFitnessComparer offspringFitnessComparer = new WeightedOffspringFitnessComparer();
      SubScopesRemover parentScopesRemover = new SubScopesRemover();

      Sorter sorter = new Sorter();
      sorter.GetVariableInfo("Descending").ActualName = "Maximization";
      sorter.GetVariableInfo("Value").ActualName = "Quality";

      UniformSequentialSubScopesProcessor validationEvaluator = new UniformSequentialSubScopesProcessor();
      MeanSquaredErrorEvaluator validationQualityEvaluator = new MeanSquaredErrorEvaluator();
      validationQualityEvaluator.Name = "ValidationMeanSquaredErrorEvaluator";
      validationQualityEvaluator.GetVariableInfo("MSE").ActualName = "ValidationQuality";
      validationQualityEvaluator.GetVariableInfo("SamplesStart").ActualName = "ValidationSamplesStart";
      validationQualityEvaluator.GetVariableInfo("SamplesEnd").ActualName = "ValidationSamplesEnd";

      main.AddSubOperator(seq);
      seq.AddSubOperator(selector);
      seq.AddSubOperator(seqSubScopesProc);
      seqSubScopesProc.AddSubOperator(emptyOp);
      seqSubScopesProc.AddSubOperator(offspringSelectionSeq);
      seq.AddSubOperator(offspringSelector);
      offspringSelector.AddSubOperator(seq);

      offspringSelectionSeq.AddSubOperator(childInitializer);
      offspringSelectionSeq.AddSubOperator(individualProc);
      offspringSelectionSeq.AddSubOperator(sorter);

      individualProc.AddSubOperator(individualSeqProc);
      individualSeqProc.AddSubOperator(crossover);
      individualSeqProc.AddSubOperator(cond);
      cond.AddSubOperator(manipulator);
      individualSeqProc.AddSubOperator(evaluator);
      individualSeqProc.AddSubOperator(evalCounter);
      individualSeqProc.AddSubOperator(offspringFitnessComparer);
      individualSeqProc.AddSubOperator(parentScopesRemover);

      SequentialSubScopesProcessor seqSubScopesProc2 = new SequentialSubScopesProcessor();
      main.AddSubOperator(seqSubScopesProc2);
      seqSubScopesProc2.AddSubOperator(emptyOp);

      SequentialProcessor newGenProc = new SequentialProcessor();
      newGenProc.AddSubOperator(sorter);
      newGenProc.AddSubOperator(validationEvaluator);
      seqSubScopesProc2.AddSubOperator(newGenProc);

      validationEvaluator.AddSubOperator(validationQualityEvaluator);

      childCreater.OperatorGraph.AddOperator(main);
      childCreater.OperatorGraph.InitialOperator = main;
      return childCreater;
    }

    protected internal override IOperator CreateLoopCondition(IOperator loop) {
      SequentialProcessor seq = new SequentialProcessor();
      seq.Name = "Loop Condition";
      LessThanComparator generationsComparator = new LessThanComparator();
      generationsComparator.GetVariableInfo("LeftSide").ActualName = "EvaluatedSolutions";
      generationsComparator.GetVariableInfo("RightSide").ActualName = "MaxEvaluatedSolutions";
      generationsComparator.GetVariableInfo("Result").ActualName = "EvaluatedSolutionsCondition";

      LessThanComparator selPresComparator = new LessThanComparator();
      selPresComparator.GetVariableInfo("LeftSide").ActualName = "SelectionPressure";
      selPresComparator.GetVariableInfo("RightSide").ActualName = "SelectionPressureLimit";
      selPresComparator.GetVariableInfo("Result").ActualName = "SelectionPressureCondition";

      ConditionalBranch generationsCond = new ConditionalBranch();
      generationsCond.GetVariableInfo("Condition").ActualName = "EvaluatedSolutionsCondition";

      ConditionalBranch selPresCond = new ConditionalBranch();
      selPresCond.GetVariableInfo("Condition").ActualName = "SelectionPressureCondition";

      seq.AddSubOperator(generationsComparator);
      seq.AddSubOperator(selPresComparator);
      seq.AddSubOperator(generationsCond);
      generationsCond.AddSubOperator(selPresCond);
      selPresCond.AddSubOperator(loop);

      return seq;
    }

    protected internal override IOperator CreateLoggingOperator() {
      CombinedOperator loggingOperator = new CombinedOperator();
      loggingOperator.Name = "Logging";
      SequentialProcessor seq = new SequentialProcessor();

      DataCollector collector = new DataCollector();
      ItemList<StringData> names = collector.GetVariable("VariableNames").GetValue<ItemList<StringData>>();
      names.Add(new StringData("BestQuality"));
      names.Add(new StringData("AverageQuality"));
      names.Add(new StringData("WorstQuality"));
      names.Add(new StringData("BestValidationQuality"));
      names.Add(new StringData("AverageValidationQuality"));
      names.Add(new StringData("WorstValidationQuality"));
      names.Add(new StringData("EvaluatedSolutions"));
      names.Add(new StringData("SelectionPressure"));
      QualityLogger qualityLogger = new QualityLogger();
      QualityLogger validationQualityLogger = new QualityLogger();
      validationQualityLogger.Name = "ValidationQualityLogger";
      validationQualityLogger.GetVariableInfo("Quality").ActualName = "ValidationQuality";
      validationQualityLogger.GetVariableInfo("QualityLog").ActualName = "ValidationQualityLog";

      seq.AddSubOperator(collector);
      seq.AddSubOperator(qualityLogger);
      seq.AddSubOperator(validationQualityLogger);

      loggingOperator.OperatorGraph.AddOperator(seq);
      loggingOperator.OperatorGraph.InitialOperator = seq;
      return loggingOperator;
    }


    public override IEditor CreateEditor() {
      return new OffspringSelectionGpEditor(this);
    }

    public override IView CreateView() {
      return new OffspringSelectionGpEditor(this);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      OffspringSelectionGP clone = (OffspringSelectionGP)base.Clone(clonedObjects);
      clone.SelectionPressureLimit = SelectionPressureLimit;
      clone.SuccessRatioLimit = SuccessRatioLimit;
      clone.ComparisonFactor = ComparisonFactor;
      return clone;
    }
  }
}
