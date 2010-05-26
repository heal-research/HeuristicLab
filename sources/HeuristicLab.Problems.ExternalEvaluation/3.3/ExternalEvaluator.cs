#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;
using System.Collections;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("ExternalEvaluationValuesCollector", "Creates a solution message, and communicates it via the driver to receive a quality message.")]
  [StorableClass]
  public class ExternalEvaluator : ValuesCollector, IExternalEvaluationProblemEvaluator {
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public IValueLookupParameter<IExternalEvaluationDriver> DriverParameter {
      get { return (IValueLookupParameter<IExternalEvaluationDriver>)Parameters["Driver"]; }
    }

    public ExternalEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the current solution."));
      Parameters.Add(new ValueLookupParameter<IExternalEvaluationDriver>("Driver", "The driver to communicate with the external process."));
    }

    public override IOperation Apply() {
      IExternalEvaluationDriver driver = DriverParameter.ActualValue;
      SolutionMessage.Builder messageBuilder = SolutionMessage.CreateBuilder();
      messageBuilder.SolutionId = 0;
      foreach (IParameter param in CollectedValues) {
        IItem value = param.ActualValue;
        if (value != null) {
          ILookupParameter lookupParam = param as ILookupParameter;
          string name = lookupParam != null ? lookupParam.TranslatedName : param.Name;
          try {
            value.AddToSolutionMessage(name, messageBuilder);
          } catch (ArgumentException ex) {
            throw new InvalidOperationException("ERROR in " + Name + ": Parameter " + name + " cannot be added to the message.", ex);
          }
        }
      }
      QualityMessage answer = driver.Evaluate(messageBuilder.Build());
      if (QualityParameter.ActualValue == null)
        QualityParameter.ActualValue = new DoubleValue(answer.Quality);
      else QualityParameter.ActualValue.Value = answer.Quality;

      return base.Apply();
    }
  }
}
