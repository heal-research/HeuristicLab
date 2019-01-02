#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using Google.ProtocolBuffers;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("EvaluationServiceClient", "An RPC client that evaluates a solution.")]
  [StorableType("97F11C34-F32B-4AD2-92FB-F4A12311C962")]
  public class EvaluationServiceClient : ParameterizedNamedItem, IEvaluationServiceClient {

    public override bool CanChangeName { get { return false; } }
    public override bool CanChangeDescription { get { return false; } }

    #region Parameters
    public IValueParameter<IEvaluationChannel> ChannelParameter {
      get { return (IValueParameter<IEvaluationChannel>)Parameters["Channel"]; }
    }
    public IValueParameter<IntValue> RetryParameter {
      get { return (IValueParameter<IntValue>)Parameters["Retry"]; }
    }

    private IEvaluationChannel Channel {
      get { return ChannelParameter.Value; }
    }
    #endregion


    #region Construction & Cloning
    [StorableConstructor]
    protected EvaluationServiceClient(StorableConstructorFlag _) : base(_) { }
    protected EvaluationServiceClient(EvaluationServiceClient original, Cloner cloner)
      : base(original, cloner) {
      RegisterEvents();
    }
    public EvaluationServiceClient()
      : base() {
      Parameters.Add(new ValueParameter<IEvaluationChannel>("Channel", "The channel over which to call the remote function."));
      Parameters.Add(new ValueParameter<IntValue>("Retry", "How many times the client should retry obtaining a quality in case there is an exception. Note that it immediately aborts when the channel cannot be opened.", new IntValue(10)));
      RegisterEvents();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new EvaluationServiceClient(this, cloner);
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      ChannelParameter_ValueChanged(this, EventArgs.Empty);
      RegisterEvents();
    }
    #endregion

    #region IEvaluationServiceClient Members
    public QualityMessage Evaluate(SolutionMessage solution, ExtensionRegistry qualityExtensions) {
      int tries = 0, maxTries = RetryParameter.Value.Value;
      bool success = false;
      QualityMessage result = null;
      while (!success) {
        try {
          tries++;
          CheckAndOpenChannel();
          Channel.Send(solution);
          result = (QualityMessage)Channel.Receive(QualityMessage.CreateBuilder(), qualityExtensions);
          success = true;
        } catch (InvalidOperationException) {
          throw;
        } catch {
          if (tries >= maxTries)
            throw;
        }
      }
      if (result != null && result.SolutionId != solution.SolutionId) throw new InvalidDataException(Name + ": Received a quality for a different solution.");
      return result;
    }

    public void EvaluateAsync(SolutionMessage solution, ExtensionRegistry qualityExtensions, Action<QualityMessage> callback) {
      int tries = 0, maxTries = RetryParameter.Value.Value;
      bool success = false;
      while (!success) {
        try {
          tries++;
          CheckAndOpenChannel();
          Channel.Send(solution);
          success = true;
        } catch (InvalidOperationException) {
          throw;
        } catch {
          if (tries >= maxTries)
            throw;
        }
      }
      System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ReceiveAsync), new ReceiveAsyncInfo(solution, callback, qualityExtensions));
    }
    #endregion

    #region Auxiliary Methods
    private void CheckAndOpenChannel() {
      if (Channel == null) throw new InvalidOperationException(Name + ": The channel is not defined.");
      if (!Channel.IsInitialized) {
        try {
          Channel.Open();
        } catch (Exception e) {
          throw new InvalidOperationException(Name + ": The channel could not be opened.", e);
        }
      }
    }

    private void ReceiveAsync(object callbackInfo) {
      var info = (ReceiveAsyncInfo)callbackInfo;
      QualityMessage message = null;
      try {
        message = (QualityMessage)Channel.Receive(QualityMessage.CreateBuilder(), info.QualityExtensions);
      } catch { }
      if (message != null && message.SolutionId != info.SolutionMessage.SolutionId) throw new InvalidDataException(Name + ": Received a quality for a different solution.");
      info.CallbackDelegate.Invoke(message);
    }

    private void RegisterEvents() {
      ChannelParameter.ValueChanged += new EventHandler(ChannelParameter_ValueChanged);
    }

    void ChannelParameter_ValueChanged(object sender, EventArgs e) {
      if (ChannelParameter.Value == null)
        name = "Empty EvaluationServiceClient";
      else
        name = String.Format("{0} ServiceClient", ChannelParameter.Value.Name);
      OnNameChanged();
    }

    [StorableType("9f1ab6e1-fe6e-4624-afbd-9bfdfdb75d44")]
    #endregion

    private class ReceiveAsyncInfo {
      public SolutionMessage SolutionMessage { get; set; }
      public Action<QualityMessage> CallbackDelegate { get; set; }
      public ExtensionRegistry QualityExtensions { get; set; }
      public ReceiveAsyncInfo(SolutionMessage solutionMessage, Action<QualityMessage> callbackDelegate, ExtensionRegistry qualityExtensions) {
        SolutionMessage = solutionMessage;
        CallbackDelegate = callbackDelegate;
        QualityExtensions = qualityExtensions;
      }
    }
  }
}
