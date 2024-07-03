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
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using HeuristicLab.Clients.Common.Properties;

namespace HeuristicLab.Clients.Common {
  public static class ClientFactory {

    #region CreateCoreClient Methods

    public static Binding GetBinding(WCFClientConfiguration config) {
      Binding binding = null;
      switch (config.Binding) {
        case WCFClientConfiguration.BindingType.BASICHTTP:
          binding = new BasicHttpBinding {
            MaxBufferSize = config.MaxBufferSize,
            MaxBufferPoolSize = config.MaxBufferPoolSize,
            MaxReceivedMessageSize = config.MaxReceivedMessageSize,
            ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas {
              MaxArrayLength = config.MaxArrayLength,
              MaxBytesPerRead = config.MaxBytesPerRead,
              MaxDepth = config.MaxDepth,
              MaxNameTableCharCount = config.MaxNameTableCharCount,
              MaxStringContentLength = config.MaxStringContentLength
            },
            SendTimeout = config.SendTimeout,
            ReceiveTimeout = config.ReceiveTimeout,
            Security = new BasicHttpSecurity {
              Mode = BasicHttpSecurityMode.TransportWithMessageCredential,
              Message = new BasicHttpMessageSecurity {
                ClientCredentialType = BasicHttpMessageCredentialType.UserName
              },
              Transport = new HttpTransportSecurity {
                ClientCredentialType = HttpClientCredentialType.None
              }
            }
          };
          break;
        case WCFClientConfiguration.BindingType.NETTCP:
          binding = new NetTcpBinding {
            MaxBufferSize = config.MaxBufferSize,
            MaxBufferPoolSize = config.MaxBufferPoolSize,
            MaxReceivedMessageSize = config.MaxReceivedMessageSize,
            ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas {
              MaxArrayLength = config.MaxArrayLength,
              MaxBytesPerRead = config.MaxBytesPerRead,
              MaxDepth = config.MaxDepth,
              MaxNameTableCharCount = config.MaxNameTableCharCount,
              MaxStringContentLength = config.MaxStringContentLength,
            },
            SendTimeout = config.SendTimeout,
            ReceiveTimeout = config.ReceiveTimeout,
            Security = new NetTcpSecurity {
              Mode = SecurityMode.TransportWithMessageCredential,
              Message = new MessageSecurityOverTcp {
                ClientCredentialType = MessageCredentialType.UserName
              },
              Transport = new TcpTransportSecurity {
                ClientCredentialType = TcpClientCredentialType.None
              }
            }
          };
          break;
      }
      return binding;
    }

    public static T CreateCoreClient<T, I>(WCFClientConfiguration config, string username = null, string password = null) where T : ClientBase<I>, I where I : class {
      var binding = GetBinding(config);
      var endpoint = new EndpointAddress(config.Address);
      var client = (T)Activator.CreateInstance(typeof(T), binding, endpoint);
      client.ClientCredentials.UserName.UserName = username ?? Settings.Default.UserName;
      client.ClientCredentials.UserName.Password = password ?? CryptoService.DecryptString(Settings.Default.Password);
      client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
      client.ClientCredentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
      return client;
    }

    #endregion

    #region CreateClient Methods
    public static T CreateClient<T, I>()
      where T : ClientBase<I>, I
      where I : class {
      return CreateClient<T, I>(null, null);
    }
    public static T CreateClient<T, I>(string endpointConfigurationName)
      where T : ClientBase<I>, I
      where I : class {
      return CreateClient<T, I>(endpointConfigurationName, null);
    }
    public static T CreateClient<T, I>(string endpointConfigurationName, string remoteAddress)
      where T : ClientBase<I>, I
      where I : class {
      return CreateClient<T, I>(endpointConfigurationName, remoteAddress, Settings.Default.UserName, CryptoService.DecryptString(Settings.Default.Password));
    }
    public static T CreateClient<T, I>(string endpointConfigurationName, string remoteAddress, string userName, string password)
      where T : ClientBase<I>, I
      where I : class {
      T client;
      if (string.IsNullOrEmpty(endpointConfigurationName)) {
        client = Activator.CreateInstance<T>();
      } else {
        client = (T)Activator.CreateInstance(typeof(T), endpointConfigurationName);
      }

      if (!string.IsNullOrEmpty(remoteAddress)) {
        SetEndpointAddress(client.Endpoint, remoteAddress);
      }

      client.ClientCredentials.UserName.UserName = userName;
      client.ClientCredentials.UserName.Password = password;
      client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;

      // we (jkarder + abeham) have disabled the revocation check for now
      // the certificate requires OCSP instead of CRL for revocation checks, but the OCSP check fails
      // we currently don't know why this is the case, because we observed a valid OCSP request/response using wireshark
      client.ClientCredentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
      return client;
    }
    #endregion

    #region Helpers
    private static void SetEndpointAddress(ServiceEndpoint endpoint, string remoteAddress) {
      // change the endpoint address and preserve the identity certificate defined in the config file
      EndpointAddressBuilder endpointAddressBuilder = new EndpointAddressBuilder(endpoint.Address);
      UriBuilder uriBuilder = new UriBuilder(endpointAddressBuilder.Uri);
      uriBuilder.Host = remoteAddress;
      endpointAddressBuilder.Uri = uriBuilder.Uri;
      endpoint.Address = endpointAddressBuilder.ToEndpointAddress();
    }
    #endregion
  }
}
