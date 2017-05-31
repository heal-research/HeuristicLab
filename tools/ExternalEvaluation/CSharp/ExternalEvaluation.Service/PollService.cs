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

namespace HeuristicLab.Problems.ExternalEvaluation.Service {
  public class PollService : Service {
    private BlockingQueue<SolutionMessage> messageQueue;

    public PollService(IListenerFactory driverFactory)
      : base(driverFactory) {
      messageQueue = new BlockingQueue<SolutionMessage>(int.MaxValue);
    }

    public override void Stop() {
      messageQueue.Close();
      base.Stop();
    }

    public SolutionMessage GetSolution() {
      try {
        return messageQueue.Dequeue();
      } catch (QueueClosedException) { }
      return null;
    }

    public QualityMessage.Builder PrepareQualityMessage(SolutionMessage msg, double quality) {
      QualityMessage.Builder qualityMessageBuilder = QualityMessage.CreateBuilder();
      qualityMessageBuilder.SetSolutionId(msg.SolutionId)
                           .SetQuality(quality);
      return qualityMessageBuilder;
    }

    public void SendQualityMessage(SolutionMessage sMsg, QualityMessage qMsg) {
      Send(qMsg, sMsg);
    }

    public void SendQuality(SolutionMessage msg, double quality) {
      QualityMessage.Builder qualityMessageBuilder = QualityMessage.CreateBuilder();
      qualityMessageBuilder.SetSolutionId(msg.SolutionId)
                           .SetQuality(quality);
      Send(qualityMessageBuilder.Build(), msg);
    }

    public event EventHandler SolutionProduced;

    protected override void OnSolutionProduced(SolutionMessage msg, IChannel channel) {
      base.OnSolutionProduced(msg, channel);
      try {
        messageQueue.Enqueue(msg);
      } catch (QueueClosedException) {
        return;
      }
      EventHandler handler = SolutionProduced;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
