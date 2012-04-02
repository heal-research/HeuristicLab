#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Security.Cryptography;
using System.Text;

namespace HeuristicLab.Clients.Common {
  /// <summary>
  /// Provides functionality for encrypting and decrypting strings. 
  /// Based on the code from http://weblogs.asp.net/jgalloway/archive/2008/04/13/encrypting-passwords-in-a-net-app-config-file.aspx
  /// </summary>
  public static class CryptoService {
    private static byte[] entropy = System.Text.Encoding.Unicode.GetBytes("Salt Is Not A Password");

    public static string EncryptString(string input) {
      byte[] encryptedData = ProtectedData.Protect(
        Encoding.Unicode.GetBytes(input),
        entropy,
        DataProtectionScope.CurrentUser);
      return Convert.ToBase64String(encryptedData);
    }

    public static string DecryptString(string encryptedData) {
      try {
        byte[] decryptedData = System.Security.Cryptography.ProtectedData.Unprotect(
          Convert.FromBase64String(encryptedData),
          entropy,
          System.Security.Cryptography.DataProtectionScope.CurrentUser);
        return System.Text.Encoding.Unicode.GetString(decryptedData);
      }
      catch (CryptographicException) {
        // we assume here that the password was saved in clear text in a previous HL version
        return encryptedData;
      }
      catch (FormatException) {
        return encryptedData;
      }
      catch (Exception ex) {
        throw ex;
      }
    }
  }
}
