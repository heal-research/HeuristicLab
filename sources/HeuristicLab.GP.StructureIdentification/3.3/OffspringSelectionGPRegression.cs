﻿#region License Information
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
using System.Linq;
using System.Collections.Generic;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Evolutionary;
using HeuristicLab.Logging;
using HeuristicLab.Operators;
using HeuristicLab.Selection;
using HeuristicLab.Selection.OffspringSelection;
using HeuristicLab.Modeling;
using HeuristicLab.DataAnalysis;
using HeuristicLab.Operators.Programmable;
using HeuristicLab.GP.Algorithms;

namespace HeuristicLab.GP.StructureIdentification {
  public class OffspringSelectionGPRegression : HeuristicLab.GP.Algorithms.OffspringSelectionGP, IStochasticAlgorithm {
    public override string Name { get { return "OffspringSelectionGP - StructureIdentification"; } }

    public virtual int TargetVariable {
      get { return ProblemInjector.GetVariableValue<IntData>("TargetVariable", null, false).Data; }
      set { ProblemInjector.GetVariableValue<IntData>("TargetVariable", null, false).Data = value; }
    }

    public virtual Dataset Dataset {
      get { return ProblemInjector.GetVariableValue<Dataset>("Dataset", null, false); }
      set { ProblemInjector.GetVariable("Dataset").Value = value; }
    }

    public virtual IAnalyzerModel Model {
      get {
        if (!Engine.Terminated)
          throw new InvalidOperationException("The algorithm is still running. Wait until the algorithm is terminated to retrieve the result.");
        else
          return CreateGPModel();
      }
    }
    public IEnumerable<int> AllowedVariables {
      get {
        ItemList<IntData> allowedVariables = ProblemInjector.GetVariableValue<ItemList<IntData>>("AllowedFeatures", null, false);
        return allowedVariables.Select(x => x.Data);
      }
      set {
        ItemList<IntData> allowedVariables = ProblemInjector.GetVariableValue<ItemList<IntData>>("AllowedFeatures", null, false);
        foreach (int x in value) allowedVariables.Add(new IntData(x));
      }
    }

    public int TrainingSamplesStart {
      get { return ProblemInjector.GetVariableValue<IntData>("TrainingSamplesStart", null, false).Data; }
      set { ProblemInjector.GetVariableValue<IntData>("TrainingSamplesStart", null, false).Data = value; }
    }

    public int TrainingSamplesEnd {
      get { return ProblemInjector.GetVariableValue<IntData>("TrainingSamplesEnd", null, false).Data; }
      set { ProblemInjector.GetVariableValue<IntData>("TrainingSamplesEnd", null, false).Data = value; }
    }

    public int ValidationSamplesStart {
      get { return ProblemInjector.GetVariableValue<IntData>("ValidationSamplesStart", null, false).Data; }
      set { ProblemInjector.GetVariableValue<IntData>("ValidationSamplesStart", null, false).Data = value; }
    }

    public int ValidationSamplesEnd {
      get { return ProblemInjector.GetVariableValue<IntData>("ValidationSamplesEnd", null, false).Data; }
      set { ProblemInjector.GetVariableValue<IntData>("ValidationSamplesEnd", null, false).Data = value; }
    }

    public int TestSamplesStart {
      get { return ProblemInjector.GetVariableValue<IntData>("TestSamplesStart", null, false).Data; }
      set { ProblemInjector.GetVariableValue<IntData>("TestSamplesStart", null, false).Data = value; }
    }

    public int TestSamplesEnd {
      get { return ProblemInjector.GetVariableValue<IntData>("TestSamplesEnd", null, false).Data; }
      set { ProblemInjector.GetVariableValue<IntData>("TestSamplesEnd", null, false).Data = value; }
    }

    public virtual double PunishmentFactor {
      get { return GetVariableInjector().GetVariable("PunishmentFactor").GetValue<DoubleData>().Data; }
      set { GetVariableInjector().GetVariable("PunishmentFactor").GetValue<DoubleData>().Data = value; }
    }

    public virtual int MaxBestValidationSolutionAge {
      get { return GetVariableInjector().GetVariable("MaxBestValidationSolutionAge").GetValue<IntData>().Data; }
      set { GetVariableInjector().GetVariable("MaxBestValidationSolutionAge").GetValue<IntData>().Data = value; }
    }

    public override IOperator ProblemInjector {
      get { return GetProblemInjector().OperatorGraph.InitialOperator.SubOperators[0]; }
      set {
        value.Name = "ProblemInjector";
        CombinedOperator problemInjector = GetProblemInjector();
        problemInjector.OperatorGraph.RemoveOperator(ProblemInjector.Guid);
        problemInjector.OperatorGraph.AddOperator(value);
        problemInjector.OperatorGraph.InitialOperator.AddSubOperator(value, 0);
      }
    }

    public OffspringSelectionGPRegression()
      : base() {
      PunishmentFactor = 10.0;
      MaxBestValidationSolutionAge = 20;
    }

    protected override IOperator CreateFunctionLibraryInjector() {
      return DefaultStructureIdentificationOperators.CreateFunctionLibraryInjector();
    }

    protected override IOperator CreateProblemInjector() {
      return DefaultRegressionOperators.CreateProblemInjector();
    }

    protected override IOperator CreateInitialPopulationEvaluator() {
      return DefaultStructureIdentificationOperators.CreateInitialPopulationEvaluator();
    }

    protected override IOperator CreateEvaluationOperator() {
      return DefaultStructureIdentificationOperators.CreateEvaluator();
    }


    protected override IOperator CreateGenerationStepHook() {
      IOperator hook = DefaultStructureIdentificationOperators.CreateGenerationStepHook();
      hook.AddSubOperator(CreateBestSolutionProcessor());
      return hook;
    }

    private IOperator CreateBestSolutionProcessor() {
      CombinedOperator op = new CombinedOperator();
      op.Name = "BestSolutionProcessor";
      SequentialProcessor seq = new SequentialProcessor();

      ProgrammableOperator variableStorer = new ProgrammableOperator();
      variableStorer.RemoveVariableInfo("Result");
      variableStorer.AddVariableInfo(new VariableInfo("Input", "Value to copy", typeof(ObjectData), VariableKind.In));
      variableStorer.AddVariableInfo(new VariableInfo("Output", "Value to write", typeof(ObjectData), VariableKind.Out));
      variableStorer.Code = "scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName(\"Output\"), (ObjectData)Input.Clone()));";

      IOperator evaluatedSolutionsStorer = (IOperator)variableStorer.Clone();
      evaluatedSolutionsStorer.GetVariableInfo("Input").ActualName = "EvaluatedSolutions";
      evaluatedSolutionsStorer.GetVariableInfo("Output").ActualName = "EvaluatedSolutions";

      IOperator selectionPressureStorer = (IOperator)variableStorer.Clone();
      selectionPressureStorer.GetVariableInfo("Input").ActualName = "SelectionPressure";
      selectionPressureStorer.GetVariableInfo("Output").ActualName = "SelectionPressure";

      ProgrammableOperator resetBestSolutionAge = new ProgrammableOperator();
      resetBestSolutionAge.RemoveVariable("Result");
      resetBestSolutionAge.AddVariableInfo(
        new VariableInfo("BestValidationSolutionAge", "Age of best validation solution", typeof(IntData), VariableKind.In | VariableKind.Out));
      resetBestSolutionAge.Code = "BestValidationSolutionAge.Data = 0;";

      seq.AddSubOperator(evaluatedSolutionsStorer);
      seq.AddSubOperator(selectionPressureStorer);
      seq.AddSubOperator(resetBestSolutionAge);

      op.OperatorGraph.AddOperator(seq);
      op.OperatorGraph.InitialOperator = seq;
      return op;
    }

    protected override VariableInjector CreateGlobalInjector() {
      VariableInjector injector = base.CreateGlobalInjector();
      injector.AddVariable(new HeuristicLab.Core.Variable("PunishmentFactor", new DoubleData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("BestValidationSolutionAge", new IntData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("MaxBestValidationSolutionAge", new IntData()));
      injector.AddVariable(new HeuristicLab.Core.Variable("MaxNumberOfTrainingSamples", new IntData(4000)));
      return injector;
    }


    protected override IOperator CreateLoggingOperator() {
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
      validationQualityLogger.GetVariableInfo("Quality").ActualName = "ValidationQuality";
      validationQualityLogger.GetVariableInfo("QualityLog").ActualName = "ValidationQualityLog";
      seq.AddSubOperator(collector);
      seq.AddSubOperator(qualityLogger);
      seq.AddSubOperator(validationQualityLogger);

      loggingOperator.OperatorGraph.AddOperator(seq);
      loggingOperator.OperatorGraph.InitialOperator = seq;
      return loggingOperator;
    }

    protected override IOperator CreateTerminationCondition() {
      CombinedOperator terminationCritertion = new CombinedOperator();
      terminationCritertion.Name = "TerminationCondition";
      GreaterThanComparator bestSolutionAge = new GreaterThanComparator();
      bestSolutionAge.GetVariableInfo("LeftSide").ActualName = "BestValidationSolutionAge";
      bestSolutionAge.GetVariableInfo("RightSide").ActualName = "MaxBestValidationSolutionAge";
      bestSolutionAge.GetVariableInfo("Result").ActualName = "TerminationCriterion";

      IOperator combinedTerminationCriterion = AlgorithmBase.CombineTerminationCriterions(base.CreateTerminationCondition(), bestSolutionAge);

      terminationCritertion.OperatorGraph.AddOperator(combinedTerminationCriterion);
      terminationCritertion.OperatorGraph.InitialOperator = combinedTerminationCriterion;
      return terminationCritertion;
    }

    protected override IOperator CreatePostProcessingOperator() {
      CombinedOperator op = new CombinedOperator();
      op.Name = "ModelAnalyser";
      SequentialProcessor seq = new SequentialProcessor();
      seq.AddSubOperator(DefaultStructureIdentificationOperators.CreatePreparationForPostProcessingOperator());

      UniformSequentialSubScopesProcessor subScopesProc = new UniformSequentialSubScopesProcessor();
      SequentialProcessor solutionProc = new SequentialProcessor();
      solutionProc.AddSubOperator(CreateModelAnalyzerOperator());

      subScopesProc.AddSubOperator(solutionProc);
      seq.AddSubOperator(subScopesProc);

      op.OperatorGraph.AddOperator(seq);
      op.OperatorGraph.InitialOperator = seq;
      return op;
    }

    protected virtual IOperator CreateModelAnalyzerOperator() {
      return DefaultRegressionOperators.CreatePostProcessingOperator();
    }

    protected CombinedOperator GetProblemInjector() {
      return (CombinedOperator)GetInitializationOperator().SubOperators[0];
    }

    protected virtual IAnalyzerModel CreateGPModel() {
      IScope bestModelScope = Engine.GlobalScope.SubScopes[0];
      var model = new AnalyzerModel();

      model.SetMetaData("SelectionPressure", bestModelScope.GetVariableValue<DoubleData>("SelectionPressure", false).Data);
      DefaultStructureIdentificationOperators.PopulateAnalyzerModel(bestModelScope, model);
      DefaultRegressionOperators.PopulateAnalyzerModel(bestModelScope, model);

      return model;
    }
  }
}
