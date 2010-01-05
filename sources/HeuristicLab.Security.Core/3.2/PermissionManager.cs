using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using HeuristicLab.Security.Contracts.Interfaces;
using HeuristicLab.Security.Contracts.BusinessObjects;
using HeuristicLab.Security.DataAccess;
using HeuristicLab.DataAccess.Interfaces;
using HeuristicLab.PluginInfrastructure;
using System.Security.Cryptography;
using System.ServiceModel;

namespace HeuristicLab.Security.Core {
  public class PermissionManager : IPermissionManager{

    private static ISessionFactory factory;
    private static ISessionFactory Factory {
      get {
        // lazy initialization
        if(factory==null) 
          factory = ServiceLocator.GetSessionFactory();
        return factory;
      }      
    }

    private static ISession session;
    
    private static IDictionary<Guid,string> currentSessions = new Dictionary<Guid, string>();
    Object locker = new Object();

    private static string getMd5Hash(string input) {
      // Create a new instance of the MD5CryptoServiceProvider object.
      MD5 md5Hasher = MD5.Create();

      // Convert the input string to a byte array and compute the hash.
      byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

      // Create a new Stringbuilder to collect the bytes
      // and create a string.
      StringBuilder sBuilder = new StringBuilder();

      // Loop through each byte of the hashed data 
      // and format each one as a hexadecimal string.
      for (int i = 0; i < data.Length; i++) {
        sBuilder.Append(data[i].ToString("x2"));
      }

      // Return the hexadecimal string.
      return sBuilder.ToString();
    }

   /// <summary>
   /// If a session exists for this userName then it is returned, otherwise the given password
   /// is checked and a new session is created.
   /// </summary>
   /// <param name="userName"></param>
   /// <param name="password"></param>
   /// <returns></returns>
    public Guid Authenticate(String userName, String password) {
      try {
        session = Factory.GetSessionForCurrentThread();

        password = getMd5Hash(password);

        IUserAdapter userAdapter = session.GetDataAdapter<User, IUserAdapter>();
        User user = userAdapter.GetByLogin(userName);

        if (user != null &&
            user.Password.Equals(password)) {
          Guid sessionId;

          lock (locker) {
            if (currentSessions.Values.Contains(userName)) {
              sessionId = GetGuid(userName);
            } else {
              sessionId = Guid.NewGuid();
              currentSessions.Add(sessionId, userName);
            }
          }

          return sessionId;
        } else 
          return Guid.Empty;
      }
      catch (Exception ex) { throw new FaultException("Server: " + ex.Message); }
      finally {
        if (session != null)
          session.EndSession();
      }
    }

    /// <summary>
    /// Checks if the owner of the given session has the given permission.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="permissionId"></param>
    /// <param name="entityId"></param>
    /// <returns></returns>
    public bool CheckPermission(Guid sessionId, Guid permissionId, Guid entityId) {
      string userName;
      bool existsSession;
      lock (locker)
        existsSession = currentSessions.TryGetValue(sessionId, out userName);
      if (existsSession) {
        try {
          session = Factory.GetSessionForCurrentThread();
          
          IPermissionOwnerAdapter permOwnerAdapter = session.GetDataAdapter<PermissionOwner, IPermissionOwnerAdapter>();
          PermissionOwner permOwner = permOwnerAdapter.GetByName(userName);

          IPermissionAdapter permissionAdapter = session.GetDataAdapter<Permission, IPermissionAdapter>();
          Permission permission = permissionAdapter.GetById(permissionId);
          
          if ((permission != null) && (permOwner != null))
            return (permissionAdapter.getPermission(permOwner.Id, permission.Id, entityId) != null);
          else return false;
        }
        catch (Exception ex) { throw new FaultException("Server: " + ex.Message); }
        finally {
          if (session != null)
            session.EndSession();
        }
      } else return false;
    }

    /// <summary>
    /// Removes the given session.
    /// </summary>
    /// <param name="sessionId"></param>
    public void EndSession(Guid sessionId) {
      lock (locker) {
        if (currentSessions.Keys.Contains(sessionId))
          currentSessions.Remove(sessionId);
      }
    }

    /// <summary>
    /// Gets the sessionId for a user.
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    public Guid GetGuid(string userName) {
      foreach (Guid guid in currentSessions.Keys)
        if (currentSessions[guid].CompareTo(userName) == 0)
          return guid;
      return Guid.Empty;
    }
  } 
}
