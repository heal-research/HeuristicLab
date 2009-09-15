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

using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Selection;
using HeuristicLab.Logging;
using HeuristicLab.Data;
using HeuristicLab.GP.Operators;
using HeuristicLab.Modeling;
using System.Collections.Generic;
using System;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.GP.StructureIdentification {
  public class StandardGPRegression : HeuristicLab.GP.Algorithms.StandardGP, IAlgorithm {

    public override string Name { get { return "StandardGP - StructureIdentification"; } }

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

    public virtual double PunishmentFactor {
      get { return GetVariableInjector().GetVariable("PunishmentFactor").GetValue<DoubleData>().Data; }
      set { GetVariableInjector().GetVariable("PunishmentFactor").GetValue<DoubleData>().Data = value; }
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

    public StandardGPRegression()
      : base() {

      PunishmentFactor = 10.0;
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
      return DefaultStructureIdentificationOperators.CreateGenerationStepHook();
    }

    protected override VariableInjector CreateGlobalInjector() {
      VariableInjector injector = base.CreateGlobalInjector();
      injector.AddVariable(new HeuristicLab.Core.Variable("PunishmentFactor", new DoubleData()));
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
      IAnalyzerModel model = new AnalyzerModel();
      DefaultStructureIdentificationOperators.PopulateAnalyzerModel(bestModelScope, model);
      DefaultRegressionOperators.PopulateAnalyzerModel(bestModelScope, model);
      return model;
    }
  }
}
