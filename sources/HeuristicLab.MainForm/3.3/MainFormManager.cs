#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.MainForm {
  public static class MainFormManager {
    private static object locker;
    private static IMainForm mainform;
    private static HashSet<Type> views;
    private static Dictionary<Type, Type> defaultViews;

    static MainFormManager() {
      locker = new object();
      mainform = null;
      views = new HashSet<Type>();
      defaultViews = new Dictionary<Type, Type>();
    }

    public static void RegisterMainForm(IMainForm mainForm) {
      lock (locker) {
        if (MainFormManager.mainform != null)
          throw new ArgumentException("A mainform was already associated with the mainform manager.");
        if (mainForm == null)
          throw new ArgumentException("Could not associate null as a mainform in the mainform manager.");

        MainFormManager.mainform = mainForm;
        IEnumerable<Type> types =
          from t in ApplicationManager.Manager.GetTypes(typeof(IContentView), false)
          where !t.IsAbstract && !t.IsInterface && ContentAttribute.HasContentAttribute(t)
          select t;

        foreach (Type viewType in types) {
          views.Add(viewType);
          foreach (Type contentType in ContentAttribute.GetDefaultViewableTypes(viewType)) {
            if (defaultViews.ContainsKey(contentType))
              throw new ArgumentException("DefaultView for type " + contentType + " is " + defaultViews[contentType] +
                ". Can't register additional DefaultView " + viewType + ".");
            defaultViews[contentType] = viewType;
          }
        }
      }
    }

    public static IMainForm MainForm {
      get { return mainform; }
    }

    public static T GetMainForm<T>() where T : IMainForm {
      return (T)mainform;
    }

    public static IEnumerable<Type> GetViewTypes(Type contentType) {
      CheckForContentType(contentType);
      List<Type> viewTypes = (from v in views
                              where ContentAttribute.CanViewType(v, contentType)
                              select v).ToList();
      //transform generic type definitions to generic types
      for (int i = 0; i < viewTypes.Count; i++) {
        viewTypes[i] = TransformGenericTypeDefinition(viewTypes[i], contentType);
      }
      return viewTypes.Where(t => t != null);
    }

    public static IEnumerable<Type> GetViewTypes(Type contentType, bool returnOnlyMostSpecificViewTypes) {
      CheckForContentType(contentType);
      List<Type> viewTypes = new List<Type>(GetViewTypes(contentType));
      if (returnOnlyMostSpecificViewTypes) {
        Type defaultViewType = null;
        try {
          defaultViewType = GetDefaultViewType(contentType);
        }
        catch (InvalidOperationException) { }

        foreach (Type viewType in viewTypes.ToList()) {
          if ((viewType != defaultViewType) && viewTypes.Any(t => t.IsSubclassOf(viewType)))
            viewTypes.Remove(viewType);
        }
      }
      return viewTypes;
    }

    public static bool ViewCanViewContent(IContentView view, IContent content) {
      return ContentAttribute.CanViewType(view.GetType(), content.GetType());
    }

    public static Type GetDefaultViewType(Type contentType) {
      CheckForContentType(contentType);
      //check base classes for default view
      Type type = contentType;
      while (type != null) {
        //check classes
        foreach (Type defaultContentType in defaultViews.Keys) {
          if (type == defaultContentType || type.CheckGenericTypes(defaultContentType))
            return TransformGenericTypeDefinition(defaultViews[defaultContentType], contentType);
        }

        //check interfaces
        IEnumerable<Type> nonInheritedInterfaces = type.GetInterfaces().Where(i => !i.IsAssignableFrom(type.BaseType));
        List<Type> defaultViewList = new List<Type>();
        foreach (Type defaultContentType in defaultViews.Keys) {
          if (nonInheritedInterfaces.Contains(defaultContentType) || nonInheritedInterfaces.Any(i => i.CheckGenericTypes(defaultContentType)))
            defaultViewList.Add(defaultViews[defaultContentType]);
        }

        //return only most spefic view as default view
        foreach (Type viewType in defaultViewList.ToList()) {
          if (defaultViewList.Any(t => t.IsSubclassOf(viewType)))
            defaultViewList.Remove(viewType);
        }

        if (defaultViewList.Count == 1)
          return TransformGenericTypeDefinition(defaultViewList[0], contentType);
        else if (defaultViewList.Count > 1)
          throw new InvalidOperationException("Could not determine which is the default view for type " + contentType.ToString() + ". Because more than one implemented interfaces have a default view.");

        type = type.BaseType;
      }

      return null;
    }

    public static IContentView CreateDefaultView(Type contentType) {
      CheckForContentType(contentType);
      Type t = GetDefaultViewType(contentType);
      if (t == null)
        return null;

      return (IContentView)Activator.CreateInstance(t);
    }
    public static IContentView CreateView(Type viewType) {
      CheckForContentViewType(viewType);
      if (viewType.IsGenericTypeDefinition)
        throw new ArgumentException("View can not be created becaues given type " + viewType.ToString() + " is a generic type definition.");

      return (IContentView)Activator.CreateInstance(viewType);
    }

    private static void CheckForContentType(Type contentType) {
      if (!typeof(IContent).IsAssignableFrom(contentType))
        throw new ArgumentException("DefaultViews are only specified for types of IContent and not for " + contentType + ".");
    }
    private static void CheckForContentViewType(Type viewType) {
      if (!typeof(IContentView).IsAssignableFrom(viewType))
        throw new ArgumentException("View can not be created becaues given type " + viewType.ToString() + " is not of type IContentView.");
    }

    private static Type TransformGenericTypeDefinition(Type viewType, Type contentType) {
      if (contentType.IsGenericTypeDefinition)
        throw new ArgumentException("The content type " + contentType.ToString() + " must not be a generic type definition.");

      if (!viewType.IsGenericTypeDefinition)
        return viewType;

      Type contentTypeBaseType = null;
      foreach (Type type in ContentAttribute.GetViewableTypes(viewType)) {
        contentTypeBaseType = contentType;
        while (contentTypeBaseType != null && (!contentTypeBaseType.IsGenericType ||
              type.GetGenericTypeDefinition() != contentTypeBaseType.GetGenericTypeDefinition()))
          contentTypeBaseType = contentTypeBaseType.BaseType;

        //check interfaces for generic type arguments
        if (contentTypeBaseType == null) {
          IEnumerable<Type> implementedInterfaces = contentType.GetInterfaces().Where(t => t.IsGenericType);
          foreach (Type implementedInterface in implementedInterfaces) {
            if (implementedInterface.CheckGenericTypes(type))
              contentTypeBaseType = implementedInterface;
          }
        }
        if (contentTypeBaseType != null) break;
      }

      if (!contentTypeBaseType.IsGenericType)
        throw new ArgumentException("Neither content type itself nor any of its base classes is a generic type. Could not determine generic type argument for the view.");

      Type[] viewTypeGenericArguments = viewType.GetGenericArguments();
      Type[] contentTypeGenericArguments = contentTypeBaseType.GetGenericArguments();

      if (contentTypeGenericArguments.Length != viewTypeGenericArguments.Length)
        throw new ArgumentException("Neiter the type (" + contentType.ToString() + ") nor any of its base types specifies " +
          viewTypeGenericArguments.Length + " generic type arguments.");

      for (int i = 0; i < viewTypeGenericArguments.Length; i++) {
        foreach (Type typeConstraint in viewTypeGenericArguments[i].GetGenericParameterConstraints()) {
          if (!typeConstraint.IsAssignableFrom(contentTypeGenericArguments[i]))
            return null;
        }
      }

      Type returnType = viewType.MakeGenericType(contentTypeGenericArguments);
      return returnType;
    }
  }
}
