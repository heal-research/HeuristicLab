#region License Information
/* HeuristicLab
 * Copyright (C) 2002-$year$ Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.IO;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.TravelingSalesman {
  [Item("$problemName$", "$problemDescription$")]
  [Creatable("Problems")]
  [StorableClass]
  public sealed class $safeitemname$ : ParameterizedNamedItem, I$problemType$ObjectiveProblem {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Type; }
    }

    #region Parameter Properties
    $maximizationParameterProperty$
    IParameter I$problemType$ObjectiveProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
    $solutionCreatorParameterProperty$
    IParameter IProblem.SolutionCreatorParameter {
      get { return SolutionCreatorParameter; }
    }
    $evaluatorParameterProperty$
    IParameter IProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }
    $parameterProperties$
    #endregion

    #region Properties
    $properties$
    public IEnumerable<IOperator> Operators {
      get { return operators; }
    }
    ISolutionCreator IProblem.SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
    }
    I$problemType$ObjectiveEvaluator I$problemType$ObjectiveProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    IEvaluator IProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    #endregion

    [Storable]
    private List<IOperator> operators;

    [StorableConstructor]
    private $safeitemname$(bool deserializing) : base(deserializing) { }
    public $safeitemname$()
      : base() {
      // TODO: Create a new instance of evaluator and solution creator

      $parameterInitializers$

      ParameterizeSolutionCreator();
      ParameterizeEvaluator();

      InitializeOperators();
      AttachEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      $safeitemname$ clone = ($safeitemname$)base.Clone(cloner);
      clone.operators = operators.Select(x => (IOperator)cloner.Clone(x)).ToList();
      // TODO: Clone private fields here
      clone.AttachEventHandlers();
      return clone;
    }

    #region Events
    public event EventHandler SolutionCreatorChanged;
    private void OnSolutionCreatorChanged() {
      EventHandler handler = SolutionCreatorChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler EvaluatorChanged;
    private void OnEvaluatorChanged() {
      EventHandler handler = EvaluatorChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler OperatorsChanged;
    private void OnOperatorsChanged() {
      EventHandler handler = OperatorsChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Reset;
    private void OnReset() {
      EventHandler handler = Reset;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    // TODO: Add your event handlers here
    #endregion

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserializationHook() {
      AttachEventHandlers();
    }

    private void AttachEventHandlers() {
      // TODO: Add event handlers to the parameters here
    }

    private void InitializeOperators() {
      operators = new List<IOperator>();
      // TODO: Add custom problem analyzer to the list
      // TODO: Add operators from the representation either by direct instantiation, or by using ApplicationManager.Manger.GetInstances<T>().Cast<IOperator>()
    }
    #endregion
  }
}
