#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using HeuristicLab.Services.Hive.DataAccess;

namespace HeuristicLab.Services.Hive {
  public class HiveOperationContext : IExtension<OperationContext> {

    public static HiveOperationContext Current {
      get {
        return OperationContext.Current != null
          ? OperationContext.Current.Extensions.Find<HiveOperationContext>()
          : null;
      }
    }

    private HiveDataContext dataContext;
    public HiveDataContext DataContext {
      get {
        return dataContext ?? (dataContext = new HiveDataContext(Settings.Default.HeuristicLab_Hive_LinqConnectionString));
      }
    }

    public void Attach(OperationContext owner) {
    }

    public void Detach(OperationContext owner) {
      if (dataContext != null) {
        dataContext.Dispose();
      }
    }
  }

  public class HiveOperationContextMessageInspector : IDispatchMessageInspector {
    public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext) {
      OperationContext.Current.Extensions.Add(new HiveOperationContext());
      return request.Headers.MessageId;
    }

    public void BeforeSendReply(ref Message reply, object correlationState) {
      OperationContext.Current.Extensions.Remove(HiveOperationContext.Current);
    }
  }

  public class HiveOperationContextBehaviorAttribute : Attribute, IServiceBehavior {
    public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,
      Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters) {
    }

    public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) {
      foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers) {
        foreach (EndpointDispatcher ed in cd.Endpoints) {
          ed.DispatchRuntime.MessageInspectors.Add(new HiveOperationContextMessageInspector());
        }
      }
    }

    public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) {
    }
  }
}
