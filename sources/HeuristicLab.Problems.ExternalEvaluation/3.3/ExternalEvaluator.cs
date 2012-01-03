#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("ExternalEvaluationValuesCollector", "Creates a solution message, and communicates it via the driver to receive a quality message.")]
  [StorableClass]
  public class ExternalEvaluator : ValuesCollector, IExternalEvaluationProblemEvaluator {

    #region Parameters
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public IValueLookupParameter<CheckedItemCollection<IEvaluationServiceClient>> ClientsParameter {
      get { return (ValueLookupParameter<CheckedItemCollection<IEvaluationServiceClient>>)Parameters["Clients"]; }
    }
    public IValueParameter<SolutionMessageBuilder> MessageBuilderParameter {
      get { return (IValueParameter<SolutionMessageBuilder>)Parameters["MessageBuilder"]; }
    }
    #endregion

    #region Parameter Values
    protected SolutionMessageBuilder MessageBuilder {
      get { return MessageBuilderParameter.Value; }
    }
    protected CheckedItemCollection<IEvaluationServiceClient> Clients {
      get { return ClientsParameter.ActualValue; }
    }
    #endregion

    #region Fields
    protected HashSet<IEvaluationServiceClient> activeClients = new HashSet<IEvaluationServiceClient>();
    protected object clientLock = new object();
    #endregion

    #region Construction & Cloning
    [StorableConstructor]
    protected ExternalEvaluator(bool deserializing) : base(deserializing) { }
    protected ExternalEvaluator(ExternalEvaluator original, Cloner cloner) : base(original, cloner) { }
    public ExternalEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the current solution."));
      Parameters.Add(new ValueLookupParameter<CheckedItemCollection<IEvaluationServiceClient>>("Clients", "Collection of clients which communicate the the external process. These clients my be contacted in parallel."));
      Parameters.Add(new ValueParameter<SolutionMessageBuilder>("MessageBuilder", "The message builder that converts from HeuristicLab objects to SolutionMessage representation.", new SolutionMessageBuilder()));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExternalEvaluator(this, cloner);
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey("Clients")) {
        Parameters.Add(new ValueLookupParameter<CheckedItemCollection<IEvaluationServiceClient>>("Clients", "Collection of clients which communicate the the external process. These clients my be contacted in parallel."));
        if (Parameters.ContainsKey("Client")) {
          var client = ((IValueLookupParameter<IEvaluationServiceClient>)Parameters["Client"]).Value;
          if (client != null)
            ClientsParameter.Value = new CheckedItemCollection<IEvaluationServiceClient>() { client };
          Parameters.Remove("Client");
        }
      }
      #endregion
    }
    #endregion

    public override IOperation Apply() {

      QualityMessage answer = EvaluateOnNextAvailableClient(BuildSolutionMessage());

      if (QualityParameter.ActualValue == null)
        QualityParameter.ActualValue = new DoubleValue(answer.Quality);
      else QualityParameter.ActualValue.Value = answer.Quality;

      return base.Apply();
    }

    protected QualityMessage EvaluateOnNextAvailableClient(SolutionMessage message) {
      IEvaluationServiceClient client = null;
      lock (clientLock) {
        client = Clients.CheckedItems.FirstOrDefault(c => !activeClients.Contains(c));
        while (client == null && Clients.CheckedItems.Count() > 0) {
          Monitor.Wait(clientLock);
          client = Clients.CheckedItems.FirstOrDefault(c => !activeClients.Contains(c));
        }
        if (client != null)
          activeClients.Add(client);
      }
      try {
        return client.Evaluate(message);
      } finally {
        lock (clientLock) {
          activeClients.Remove(client);
          Monitor.PulseAll(clientLock);
        }
      }
    }

    protected SolutionMessage BuildSolutionMessage() {
      lock (clientLock) {
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
              throw new InvalidOperationException(string.Format("ERROR while building solution message: Parameter {0} cannot be added to the message", name), ex);
            }
          }
        }
        return protobufBuilder.Build();
      }
    }

  }
}
