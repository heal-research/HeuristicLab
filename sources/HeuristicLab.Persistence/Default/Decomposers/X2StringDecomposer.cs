using System;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Persistence.Core;
using System.Collections.Generic;

namespace HeuristicLab.Persistence.Default.Decomposers {
  
  public class Int2StringDecomposer : IDecomposer {

    public bool CanDecompose(Type type) {
      return type == typeof (int);
    }

    public IEnumerable<Tag> DeCompose(object obj) {
      yield return new Tag(obj.ToString());
    }

    public object CreateInstance(Type type) {
      return null;
    }

    public object Populate(object instance, IEnumerable<Tag> tags, Type type) {
      foreach ( Tag tag in tags ) {
        return int.Parse((string) tag.Value);
      }
      throw new ApplicationException("Not enough components to compose an integer.");
    }
    
  }

  public class Long2StringDecomposer : IDecomposer {

    public bool CanDecompose(Type type) {
      return type == typeof(long);
    }

    public IEnumerable<Tag> DeCompose(object obj) {
      yield return new Tag(obj.ToString());
    }

    public object CreateInstance(Type type) {
      return null;
    }

    public object Populate(object instance, IEnumerable<Tag> tags, Type type) {
      foreach (Tag tag in tags) {
        return long.Parse((string)tag.Value);
      }
      throw new ApplicationException("Not enough components to compose an integer.");
    }

  }

  public class Double2StringDecomposer : IDecomposer {

    public bool CanDecompose(Type type) {
      return type == typeof (double);
    }

    public IEnumerable<Tag> DeCompose(object obj) {
      yield return new Tag(((double)obj).ToString("r"));
    }

    public object CreateInstance(Type type) {
      return null;
    }

    public object Populate(object instance, IEnumerable<Tag> tags, Type type) {
      foreach (Tag tag in tags) {
        return double.Parse((string)tag.Value);
      }
      throw new ApplicationException("Not enough components to compose a double.");
    }

  }

  public class Bool2StringDecomposer : IDecomposer {

    public bool CanDecompose(Type type) {
      return type == typeof(bool);
    }

    public IEnumerable<Tag> DeCompose(object obj) {
      yield return new Tag(obj.ToString());
    }

    public object CreateInstance(Type type) {
      return null;
    }

    public object Populate(object instance, IEnumerable<Tag> tags, Type type) {
      foreach (Tag tag in tags) {
        return bool.Parse((string) tag.Value);
      }
      throw new ApplicationException("Not enough components to compose a bool.");
    }

  }

  public class DateTime2StringDecomposer : IDecomposer {

    public bool CanDecompose(Type type) {
      return type == typeof(DateTime);
    }

    public IEnumerable<Tag> DeCompose(object obj) {
      yield return new Tag(((DateTime)obj).Ticks);
    }

    public object CreateInstance(Type type) {
      return null;
    }

    public object Populate(object instance, IEnumerable<Tag> tags, Type type) {
      foreach (Tag tag in tags) {
        return new DateTime((long)tag.Value);
      }
      throw new ApplicationException("Not enough components to compose a bool.");
    }

  }  

}