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
