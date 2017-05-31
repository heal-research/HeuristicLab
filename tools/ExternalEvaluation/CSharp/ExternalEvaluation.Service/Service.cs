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
using System.Threading;
using Google.ProtocolBuffers;
using HeuristicLab.Problems.ExternalEvaluation;

namespace HeuristicLab.Problems.ExternalEvaluation.Service {
  public abstract class Service {
    private static readonly int maxConnections = 10;
    protected IListener listener;
    private Dictionary<SolutionMessage, IChannel> recipientMemory;
    private List<IChannel> channels;

    public Service(IListenerFactory listenerFactory) {
      channels = new List<IChannel>();
      recipientMemory = new Dictionary<SolutionMessage, IChannel>();
      listener = listenerFactory.CreateListener();
      listener.Discovered += new EventHandler<EventArgs<IChannel>>(listener_Discovered);
    }

    public virtual void Start() {
      listener.Listen();
    }

    public virtual void Stop() {
      listener.Stop();
      lock (channels) {
        foreach (IChannel channel in channels)
          channel.Close();
        channels.Clear();
      }
      lock (recipientMemory) {
        recipientMemory.Clear();
      }
    }

    private void listener_Discovered(object sender, EventArgs<IChannel> e) {
      lock (channels) {
        if (channels.Count < maxConnections) {
          channels.Add(e.Value);
          SolutionProducer producer = new SolutionProducer(this, e.Value);
          Thread tmp = new Thread(producer.Produce);
          producer.Finished += new EventHandler<EventArgs<IChannel>>(producer_Finished);
          tmp.Start();
        } else e.Value.Close();
      }
    }

    private void producer_Finished(object sender, EventArgs<IChannel> e) {
      lock (channels) {
        channels.Remove(e.Value);
      }
      lock (recipientMemory) {
        var solutions = recipientMemory.Where(x => x.Value == e.Value).Select(x => x.Key).ToList();
        foreach (SolutionMessage msg in solutions)
          recipientMemory.Remove(msg);
      }
    }

    protected virtual void Send(QualityMessage msg, SolutionMessage original) {
      lock (recipientMemory) {
        if (recipientMemory.ContainsKey(original)) {
          recipientMemory[original].Send(msg);
          recipientMemory.Remove(original);
        }
      }
    }

    protected virtual void OnSolutionProduced(SolutionMessage msg, IChannel channel) {
      lock (recipientMemory) {
        recipientMemory.Add(msg, channel);
      }
    }

    private class SolutionProducer {
      private IChannel channel;
      private Service service;

      public SolutionProducer(Service service, IChannel channel) {
        this.service = service;
        this.channel = channel;
      }

      public void Produce() {
        while (true) {
          SolutionMessage.Builder builder = SolutionMessage.CreateBuilder();
          try {
            SolutionMessage msg = (SolutionMessage)channel.Receive(builder);
            MessageNotifier notifier = new MessageNotifier(msg, channel);
            ThreadPool.QueueUserWorkItem(new WaitCallback(notifier.Notify), service);
          } catch (Exception) {
            break;
          }
        }
        OnFinished();
      }

      public event EventHandler<EventArgs<IChannel>> Finished;
      private void OnFinished() {
        var handler = Finished;
        if (handler != null) handler(this, new EventArgs<IChannel>(channel));
      }

      private class MessageNotifier {
        SolutionMessage msg;
        IChannel channel;

        public MessageNotifier(SolutionMessage msg, IChannel channel) {
          this.msg = msg;
          this.channel = channel;
        }

        public void Notify(object state) {
          Service service = (Service)state;
          service.OnSolutionProduced(msg, channel);
        }
      }
    }
  }
}
