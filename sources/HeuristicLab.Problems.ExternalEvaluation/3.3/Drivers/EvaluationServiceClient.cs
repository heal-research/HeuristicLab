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
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("EvaluationServiceClient", "An RPC client that evaluates a solution.")]
  [StorableClass]
  public class EvaluationServiceClient : ParameterizedNamedItem, IEvaluationServiceClient {
    public override bool CanChangeName { get { return false; } }
    public override bool CanChangeDescription { get { return false; } }

    public IValueParameter<IEvaluationChannel> ChannelParameter {
      get { return (IValueParameter<IEvaluationChannel>)Parameters["Channel"]; }
    }

    private IEvaluationChannel Channel {
      get { return ChannelParameter.Value; }
    }

    public EvaluationServiceClient()
      : base() {
      Parameters.Add(new ValueParameter<IEvaluationChannel>("Channel", "The channel over which to call the remote function."));
    }

    #region IEvaluationServiceClient Members
    
    public QualityMessage Evaluate(SolutionMessage solution) {
      CheckAndOpenChannel();
      Channel.Send(solution);
      return (QualityMessage)Channel.Receive(QualityMessage.CreateBuilder());
    }

    public void EvaluateAsync(SolutionMessage solution, Action<QualityMessage> callback) {
      CheckAndOpenChannel();
      Channel.Send(solution);
      System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ReceiveAsync), callback);
    }

    #endregion

    private void CheckAndOpenChannel() {
      if (Channel == null) throw new InvalidOperationException(Name + ": The channel is not defined.");
      if (!Channel.IsInitialized) {
        try {
          Channel.Open();
        } catch (Exception e) { // TODO: Change to specific exception
          throw new InvalidOperationException(Name + ": The channel could not be opened.", e);
        }
      }
    }

    private void ReceiveAsync(object callback) {
      QualityMessage message = (QualityMessage)Channel.Receive(QualityMessage.CreateBuilder());
      ((Action<QualityMessage>)callback).Invoke(message);
    }
  }
}
