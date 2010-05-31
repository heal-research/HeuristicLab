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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("ExternalEvaluationValuesCollector", "Creates a solution message, and communicates it via the driver to receive a quality message.")]
  [StorableClass]
  public class ExternalEvaluator : ValuesCollector, IExternalEvaluationProblemEvaluator {
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public IValueLookupParameter<IEvaluationServiceClient> ClientParameter {
      get { return (IValueLookupParameter<IEvaluationServiceClient>)Parameters["Client"]; }
    }

    public IValueParameter<SolutionMessageBuilder> MessageBuilderParameter {
      get { return (IValueParameter<SolutionMessageBuilder>)Parameters["MessageBuilder"]; }
    }

    private SolutionMessageBuilder MessageBuilder {
      get { return MessageBuilderParameter.Value; }
    }

    public ExternalEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the current solution."));
      Parameters.Add(new ValueLookupParameter<IEvaluationServiceClient>("Client", "The client that communicates with the external process."));
      Parameters.Add(new ValueParameter<SolutionMessageBuilder>("MessageBuilder", "The message builder that converts from HeuristicLab objects to SolutionMessage representation.", new SolutionMessageBuilder()));
    }

    public override IOperation Apply() {
      IEvaluationServiceClient client = ClientParameter.ActualValue;
      SolutionMessage.Builder protobufBuilder = SolutionMessage.CreateBuilder();
      protobufBuilder.SolutionId = 0;
      foreach (IParameter param in CollectedValues) {
        IItem value = param.ActualValue;
        if (value != null) {
          ILookupParameter lookupParam = param as ILookupParameter;
          string name = lookupParam != null ? lookupParam.TranslatedName : param.Name;
          try {
            MessageBuilder.AddToMessage(value, name, protobufBuilder);
          } catch (ArgumentException ex) {
            throw new InvalidOperationException("ERROR in " + Name + ": Parameter " + name + " cannot be added to the message.", ex);
          }
        }
      }
      QualityMessage answer = client.Evaluate(protobufBuilder.Build());
      if (QualityParameter.ActualValue == null)
        QualityParameter.ActualValue = new DoubleValue(answer.Quality);
      else QualityParameter.ActualValue.Value = answer.Quality;

      return base.Apply();
    }
  }
}
