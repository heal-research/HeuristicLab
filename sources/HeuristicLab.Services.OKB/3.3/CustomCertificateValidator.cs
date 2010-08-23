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

using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace HeuristicLab.Services.OKB {

  /// <summary>
  /// A certificate validator that uses a list of certificates to validate.
  /// </summary>
  public class CustomCertificateValidator : X509CertificateValidator {

    private IList<X509Certificate2> validCertificates;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomCertificateValidator"/> class.
    /// </summary>
    /// <param name="validCertificates">The valid certificates.</param>
    public CustomCertificateValidator(IEnumerable<X509Certificate2> validCertificates) {
      this.validCertificates = new List<X509Certificate2>(validCertificates);
    }

    /// <summary>
    /// Validates the X.509 certificate using an internal list of valid certifiates.
    /// </summary>
    /// <param name="certificate">The <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2"/> that represents the X.509 certificate to validate.</param>
    public override void Validate(X509Certificate2 certificate) {
      if (!validCertificates.Contains(certificate))
        throw new SecurityTokenValidationException("certificate has not been registered");
    }
  }
}
