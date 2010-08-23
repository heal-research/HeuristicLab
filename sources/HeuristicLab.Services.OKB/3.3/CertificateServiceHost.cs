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
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;

namespace HeuristicLab.Services.OKB {

  /// <summary>
  /// An alternate implementation of service host that checks client certificates.
  /// </summary>
  public class CertificateServiceHost : ServiceHost {

    /// <summary>
    /// Initializes a new instance of the <see cref="CertificateServiceHost"/> class.
    /// </summary>
    /// <param name="serviceType">Type of the service.</param>
    public CertificateServiceHost(Type serviceType)
      : base(serviceType) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CertificateServiceHost"/> class.
    /// </summary>
    /// <param name="serviceType">Type of the service.</param>
    /// <param name="baseAddresses">The base addresses.</param>
    public CertificateServiceHost(Type serviceType, Uri[] baseAddresses)
      : base(serviceType, baseAddresses) {
    }

    /// <summary>
    /// Loads the service description information from the configuration file and applies it to the runtime being constructed.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">The description of the service hosted is null.</exception>
    protected override void ApplyConfiguration() {
      base.ApplyConfiguration();
      Credentials.ServiceCertificate.Certificate = GetCertificateResource("HeuristicLab.OKB.Server.server.pfx");
      Credentials.ClientCertificate.Authentication.CertificateValidationMode =
          System.ServiceModel.Security.X509CertificateValidationMode.Custom;
      Credentials.ClientCertificate.Authentication.CustomCertificateValidator =
        new CustomCertificateValidator(new[] { GetCertificateResource("HeuristicLab.OKB.Server.client.cer") });
    }

    private static X509Certificate2 GetCertificateResource(string name) {
      using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name)) {
        byte[] bytes;
        bytes = new byte[(int)stream.Length];
        stream.Read(bytes, 0, bytes.Length);
        return new X509Certificate2(bytes);
      }
    }
  }
}
