#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.PluginInfrastructure;
using System.Diagnostics;

namespace HeuristicLab.MainForm {
  class StringDict<T> : Dictionary<string, T> {
  }
  public static class MainFormManager {
    private static object locker;
    private static IMainForm mainform;
    private static HashSet<Type> views;
    private static Dictionary<Type, Type> defaultViews;

    static MainFormManager() {
      locker = new object();
      views = new HashSet<Type>();
      defaultViews = new Dictionary<Type, Type>();

      Type listString = typeof(List<string>);
      Type list = typeof(List<>);
      Type ilist = typeof(IList<>);
      Type stringStringDict = typeof(Dictionary<string, string>);
      Type dict = typeof(Dictionary<,>);
      Type stringDict = typeof(StringDict<>);
      Type stringIntDict = typeof(StringDict<int>);
      Type stringStringDict2 = typeof(StringDict<string>);

      bool result;
      result = listString.IsAssignableTo(list);
      Debug.Assert(result);

      Debug.Assert(stringStringDict2.IsAssignableTo(stringDict));
      Debug.Assert(stringStringDict2.IsAssignableTo(stringStringDict));
      Debug.Assert(stringStringDict2.IsAssignableTo(dict));
      Debug.Assert(!stringStringDict2.IsAssignableTo(stringIntDict));
      Debug.Assert(!stringIntDict.IsAssignableTo(stringStringDict));

      result = list.IsAssignableTo(ilist);
      Debug.Assert(result);
      result = listString.IsAssignableTo(ilist);
      Debug.Assert(result);
      result = list.IsAssignableTo(listString);
      Debug.Assert(!result);
      result = ilist.IsAssignableTo(listString);
      Debug.Assert(!result);
      result = ilist.IsAssignableTo(list);
      Debug.Assert(!result);

      Type stackedListList = list.MakeGenericType(typeof(List<>));
      Type stackedListListint = typeof(List<List<int>>);
      Type istackedListListint = typeof(IList<List<int>>);
      Type stackedListIListint = typeof(List<IList<int>>);
      Debug.Assert(stackedListListint.IsAssignableTo(list));
      Debug.Assert(stackedListListint.IsAssignableTo(ilist));
      Debug.Assert(!stackedListListint.IsAssignableTo(listString));
      Debug.Assert(stackedListListint.IsAssignableTo(istackedListListint));
      Debug.Assert(!stackedListListint.IsAssignableTo(stackedListIListint));
    }

    public static void RegisterMainForm(IMainForm mainform) {
      lock (locker) {
        if (MainFormManager.mainform != null)
          throw new ArgumentException("A mainform was already associated with the mainform manager.");
        if (mainform == null)
          throw new ArgumentException("Could not associate null as a mainform in the mainform manager.");

        MainFormManager.mainform = mainform;
        IEnumerable<Type> types =
          from t in ApplicationManager.Manager.GetTypes(typeof(IView))
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
      return from v in views
             where ContentAttribute.CanViewType(v, contentType)
             select v;
    }

    public static bool ViewCanViewObject(IView view, object o) {
      return ContentAttribute.CanViewType(view.GetType(), o.GetType());
    }

    public static Type GetDefaultViewType(Type contentType) {
      //check if viewableType has a default view
      if (defaultViews.ContainsKey(contentType))
        return defaultViews[contentType];

      //check base classes for default view
      Type type = contentType;
      while (type != null) {
        foreach (Type defaultViewType in defaultViews.Keys) {
          if (type == defaultViewType || type.CheckGenericTypes(defaultViewType))
            return defaultViews[defaultViewType];
        }
        type = type.BaseType;
      }

      //check if exactly one implemented interface has a default view
      List<Type> temp = (from t in defaultViews.Keys
                         where t.IsInterface && contentType.IsAssignableTo(t)
                         select t).ToList();
      if (temp.Count == 1)
        return defaultViews[temp[0]];
      //more than one default view for implemented interfaces are found
      if (temp.Count > 1)
        throw new Exception("Could not determine which is the default view for type " + contentType.ToString() + ". Because more than one implemented interfaces have a default view.");
      return null;
    }

    public static IView CreateDefaultView(object objectToView) {
      Type t = GetDefaultViewType(objectToView.GetType());
      if (t == null)
        return null;

      Type viewType = TransformGenericTypeDefinition(t, objectToView);
      if (viewType == null)
        return null;

      return (IView)Activator.CreateInstance(viewType, objectToView);
    }

    public static IView CreateView(Type viewType) {
      if (!typeof(IView).IsAssignableFrom(viewType))
        throw new ArgumentException("View can not be created becaues given type " + viewType.ToString() + " is not of type IView.");
      if (viewType.IsGenericTypeDefinition)
        throw new ArgumentException("View can not be created becaues given type " + viewType.ToString() + " is a generic type definition.");

      return (IView)Activator.CreateInstance(viewType);
    }

    public static IView CreateView(Type viewType, object objectToView) {
      if (!typeof(IView).IsAssignableFrom(viewType))
        throw new ArgumentException("View can not be created becaues given type " + viewType.ToString() + " is not of type IView.");

      Type t = TransformGenericTypeDefinition(viewType, objectToView);
      if (t == null)
        return null;

      return (IView)Activator.CreateInstance(t, objectToView);
    }

    private static Type TransformGenericTypeDefinition(Type type, object objectToView) {
      if (!type.IsGenericTypeDefinition)
        return type;

      Type[] typeGenericArguments = type.GetGenericArguments();
      Type[] objectGenericArguments = objectToView.GetType().GetGenericArguments();

      for (int i = 0; i < typeGenericArguments.Length; i++) {
        foreach (Type typeConstraint in typeGenericArguments[i].GetGenericParameterConstraints()) {
          if (!typeConstraint.IsAssignableFrom(objectGenericArguments[i]))
            return null;
        }
      }

      Type t = type.MakeGenericType(objectToView.GetType().GetGenericArguments());
      return t;
    }
  }
}
