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

namespace HeuristicLab.MainForm {
  public static class MainFormManager {
    private static object locker;
    private static IMainForm mainform;
    private static Dictionary<Type, List<Type>> views;
    private static Dictionary<Type, Type> defaultViews;

    static MainFormManager() {
      locker = new object();
      views = new Dictionary<Type, List<Type>>();
      defaultViews = new Dictionary<Type, Type>();
    }

    public static void RegisterMainForm(IMainForm mainform) {
      lock (locker) {
        if (MainFormManager.mainform == null) {
          MainFormManager.mainform = mainform;

          DiscoveryService ds = new DiscoveryService();
          IEnumerable<Type> types =
            from t in ds.GetTypes(typeof(IView))
            where !t.IsAbstract && !t.IsInterface && !t.IsGenericType
            select t;

          foreach (Type t in types) {
            foreach (Type viewableType in GetViewableType(t)) {
              if (viewableType != null) {
                if (!views.ContainsKey(viewableType))
                  views[viewableType] = new List<Type>();
                views[viewableType].Add(t);

                if (DefaultViewAttribute.IsDefaultView(t)) {
                  if (defaultViews.ContainsKey(viewableType))
                    throw new ArgumentException("DefaultView for type " + viewableType + " is " + defaultViews[viewableType] +
                      ". Can't register additional DefaultView " + t + ".");
                  defaultViews[viewableType] = t;
                }
              }
            }
          }
        } else
          throw new ArgumentException("A mainform was already associated with the mainform manager.");
      }
    }

    private static IEnumerable<Type> GetViewableType(Type t) {
      IEnumerable<Type> interfaceTypes =
       from type in t.GetInterfaces()
       where type.Namespace == "HeuristicLab.MainForm" && type.Name.StartsWith("IView") &&
             type.IsGenericType && !type.IsGenericTypeDefinition
       select type;

      foreach (Type interfaceType in interfaceTypes) {
        yield return interfaceType.GetGenericArguments()[0];
      }
    }

    public static IMainForm MainForm {
      get { return mainform; }
    }

    public static T GetMainForm<T>() where T : IMainForm {
      return (T)mainform;
    }

    public static IEnumerable<Type> GetViewTypes(Type viewableType) {
      List<Type> viewsForType = new List<Type>();
      foreach (KeyValuePair<Type, List<Type>> v in views) {
        if (v.Key.IsAssignableFrom(viewableType))
          viewsForType.AddRange(v.Value);
      }
      return viewsForType.Distinct();
    }

    public static bool ViewCanViewObject(IView view, object o) {
      return GetViewTypes(o.GetType()).Contains(view.GetType());
    }

    public static Type GetDefaultViewType(Type viewableType) {
      //check if viewableType has a default view
      if (defaultViews.ContainsKey(viewableType))
        return defaultViews[viewableType];

      //check base classes for default view
      Type type = viewableType;
      while (type.BaseType != null && !defaultViews.ContainsKey(type)) {
        type = type.BaseType;
      }
      if (defaultViews.ContainsKey(type))
        return defaultViews[type];

      //check if exact one implemented interface has a default view
      List<Type> temp = (from t in defaultViews.Keys
                         where t.IsAssignableFrom(viewableType) && t.IsInterface
                         select t).ToList();
      if (temp.Count == 1)
        return defaultViews[temp[0]];
      //more than one default view for implemented interfaces are found
      if (temp.Count > 1)
        throw new Exception("Could not determine which is the default view for type " + viewableType.ToString() + ". Because more than one implemented interfaces have a default view.");
      return null;
    }

    public static IView<T> CreateDefaultView<T>(T objectToView) {
      Type t = GetDefaultViewType(objectToView.GetType());
      if (t == null)
        return null;
      else
        return (IView<T>)Activator.CreateInstance(t);
    }
  }
}
