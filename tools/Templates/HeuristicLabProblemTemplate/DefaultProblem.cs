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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace $rootnamespace$ {
  [Item("$problemName$", "$problemDescription$")]
  [Creatable("Problems")]
  [StorableClass]
  public sealed class $safeitemname$ : $problemTypeImplementation$ {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Type; }
    }

    #region Parameter Properties
    $parameterProperties$
    #endregion

    #region Properties
    $properties$
    #endregion

    [StorableConstructor]
    private $safeitemname$(bool deserializing) : base(deserializing) { }
    private $safeitemname$($safeitemname$ original, Cloner cloner)
      : base(original, cloner) {
      // TODO: Clone your private fields here
      AttachEventHandlers();
    }
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
      return new $safeitemname$(this, cloner);
    }

    #region Events
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
      // TODO: Add custom problem analyzer to the list
      // TODO: Add operators from the representation either by direct instantiation, or by using ApplicationManager.Manger.GetInstances<T>().Cast<IOperator>()
    }
    private void ParameterizeSolutionCreator() {
      // TODO: Set the parameters of the solution creator
    }
    private void ParameterizeEvaluator() {
      // TODO: Set the parameters of the evaluator
    }
    #endregion
  }
}
