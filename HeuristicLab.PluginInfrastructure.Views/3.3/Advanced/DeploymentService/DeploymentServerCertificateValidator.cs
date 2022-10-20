#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace HeuristicLab.PluginInfrastructure.Advanced.DeploymentService {
  internal class DeploymentServerCertificateValidator : X509CertificateValidator {
    private X509Certificate2 allowedCertificate;
    public DeploymentServerCertificateValidator(X509Certificate2 allowedCertificate) {
      if (allowedCertificate == null) {
        throw new ArgumentNullException("allowedCertificate");
      }

      this.allowedCertificate = allowedCertificate;
    }
    public override void Validate(X509Certificate2 certificate) {
      // Check that there is a certificate.
      if (certificate == null) {
        throw new ArgumentNullException("certificate");
      }

      // Check that the certificate issuer matches the configured issuer
      if (!allowedCertificate.Equals(certificate)) {
        throw new SecurityTokenValidationException("Server certificate doesn't match.");
      }
    }
  }
}
