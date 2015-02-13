#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Threading;
using Google.ProtocolBuffers;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("External Evaluation Problem", "A problem that is evaluated in a different process.")]
  [Creatable("Problems")]
  [StorableClass]
  public sealed class ExternalEvaluationProblem : SingleObjectiveBasicProblem<IEncoding> {

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Type; }
    }

    #region Parameters
    public OptionalValueParameter<EvaluationCache> CacheParameter {
      get { return (OptionalValueParameter<EvaluationCache>)Parameters["Cache"]; }
    }
    public IValueParameter<CheckedItemCollection<IEvaluationServiceClient>> ClientsParameter {
      get { return (IValueParameter<CheckedItemCollection<IEvaluationServiceClient>>)Parameters["Clients"]; }
    }
    public IValueParameter<SolutionMessageBuilder> MessageBuilderParameter {
      get { return (IValueParameter<SolutionMessageBuilder>)Parameters["MessageBuilder"]; }
    }
    public IFixedValueParameter<SingleObjectiveOptimizationSupportScript> SupportScriptParameter {
      get { return (IFixedValueParameter<SingleObjectiveOptimizationSupportScript>)Parameters["SupportScript"]; }
    }
    #endregion

    #region Properties
    public EvaluationCache Cache {
      get { return CacheParameter.Value; }
    }
    public CheckedItemCollection<IEvaluationServiceClient> Clients {
      get { return ClientsParameter.Value; }
    }
    public SolutionMessageBuilder MessageBuilder {
      get { return MessageBuilderParameter.Value; }
    }
    public SingleObjectiveOptimizationSupportScript OptimizationSupportScript {
      get { return SupportScriptParameter.Value; }
    }
    private ISingleObjectiveOptimizationSupport OptimizationSupport {
      get { return SupportScriptParameter.Value; }
    }
    #endregion

    [StorableConstructor]
    private ExternalEvaluationProblem(bool deserializing) : base(deserializing) { }
    private ExternalEvaluationProblem(ExternalEvaluationProblem original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExternalEvaluationProblem(this, cloner);
    }
    public ExternalEvaluationProblem()
      : base() {
      Parameters.Remove("Maximization"); // readonly in base class
      Parameters.Add(new FixedValueParameter<BoolValue>("Maximization", "Set to false if the problem should be minimized.", new BoolValue()));
      Parameters.Add(new OptionalValueParameter<EvaluationCache>("Cache", "Cache of previously evaluated solutions."));
      Parameters.Add(new ValueParameter<CheckedItemCollection<IEvaluationServiceClient>>("Clients", "The clients that are used to communicate with the external application.", new CheckedItemCollection<IEvaluationServiceClient>() { new EvaluationServiceClient() }));
      Parameters.Add(new ValueParameter<SolutionMessageBuilder>("MessageBuilder", "The message builder that converts from HeuristicLab objects to SolutionMessage representation.", new SolutionMessageBuilder()));
      Parameters.Add(new FixedValueParameter<SingleObjectiveOptimizationSupportScript>("SupportScript", "A script that can provide neighborhood and analyze the results of the optimization.", new SingleObjectiveOptimizationSupportScript()));

      Operators.Add(new BestScopeSolutionAnalyzer());
    }

    #region Single Objective Problem Overrides
    public override bool Maximization {
      get { return Parameters.ContainsKey("Maximization") && ((IValueParameter<BoolValue>)Parameters["Maximization"]).Value.Value; }
    }

    public override double Evaluate(Individual individual, IRandom random) {
      return Cache == null ? EvaluateOnNextAvailableClient(BuildSolutionMessage(individual)).Quality
        : Cache.GetValue(BuildSolutionMessage(individual), m => EvaluateOnNextAvailableClient(m).Quality);
    }

    public override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      OptimizationSupport.Analyze(individuals, qualities, results, random);
    }

    public override IEnumerable<Individual> GetNeighbors(Individual individual, IRandom random) {
      return OptimizationSupport.GetNeighbors(individual, random);
    }
    #endregion

    #region Evaluation Helpers
    private HashSet<IEvaluationServiceClient> activeClients = new HashSet<IEvaluationServiceClient>();
    private object clientLock = new object();
    private QualityMessage EvaluateOnNextAvailableClient(SolutionMessage message) {
      IEvaluationServiceClient client = null;
      lock (clientLock) {
        client = Clients.CheckedItems.FirstOrDefault(c => !activeClients.Contains(c));
        while (client == null && Clients.CheckedItems.Any()) {
          Monitor.Wait(clientLock);
          client = Clients.CheckedItems.FirstOrDefault(c => !activeClients.Contains(c));
        }
        if (client != null)
          activeClients.Add(client);
      }
      try {
        return client.Evaluate(message, GetQualityMessageExtensions());
      } finally {
        lock (clientLock) {
          activeClients.Remove(client);
          Monitor.PulseAll(clientLock);
        }
      }
    }

    private ExtensionRegistry GetQualityMessageExtensions() {
      return ExtensionRegistry.CreateInstance();
    }

    private SolutionMessage BuildSolutionMessage(Individual individual) {
      lock (clientLock) {
        SolutionMessage.Builder protobufBuilder = SolutionMessage.CreateBuilder();
        protobufBuilder.SolutionId = 0;
        var scope = new Scope();
        individual.CopyToScope(scope);
        foreach (var variable in scope.Variables) {
          try {
            MessageBuilder.AddToMessage(variable.Value, variable.Name, protobufBuilder);
          } catch (ArgumentException ex) {
            throw new InvalidOperationException(string.Format("ERROR while building solution message: Parameter {0} cannot be added to the message", name), ex);
          }
        }
        return protobufBuilder.Build();
      }
    }
    #endregion
  }
}
