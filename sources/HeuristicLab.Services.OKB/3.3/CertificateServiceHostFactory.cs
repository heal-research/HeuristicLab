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
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace HeuristicLab.Services.OKB {

  /// <summary>
  /// A simple factory that knows how to parameterize and create a <see cref="CertificateServiceHost"/>.
  /// </summary>
  public class CertificateServiceHostFactory : ServiceHostFactory {
    /// <summary>
    /// Creates a <see cref="T:System.ServiceModel.ServiceHost"/> for a specified type of service with a specific base address.
    /// </summary>
    /// <param name="serviceType">Specifies the type of service to host.</param>
    /// <param name="baseAddresses">The <see cref="T:System.Array"/> of type <see cref="T:System.Uri"/> that contains the base addresses for the service hosted.</param>
    /// <returns>
    /// A <see cref="T:System.ServiceModel.ServiceHost"/> for the type of service specified with a specific base address.
    /// </returns>
    protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses) {
      return new CertificateServiceHost(serviceType, baseAddresses);
    }
  }

}
