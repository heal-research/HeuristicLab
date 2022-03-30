﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using HEAL.Attic;

namespace HeuristicLab.JsonInterface {
  public abstract class IntervalRestrictedArrayJsonItem<T> : ArrayJsonItem<T>
      where T : IComparable {
    public T Minimum { get; set; }
    public T Maximum { get; set; }

    protected override ValidationResult Validate() {
      IList<string> errors = new List<string>();
      bool success = true;
      foreach (var x in Value) {
        if (Minimum.CompareTo(x) > 0 && Maximum.CompareTo(x) < 0) {
          success = false;
          errors.Add($"[{Path}]: Value {x} is not between {Minimum} and {Maximum}.");
        }
      }
      return new ValidationResult(success, errors);
    }

    protected internal override void FromJObject(JObject jObject) {
      base.FromJObject(jObject);

      var minProp = jObject[nameof(IIntervalRestrictedJsonItem<T>.Minimum)];
      if (minProp != null) Minimum = minProp.ToObject<T>();


      var maxProp = jObject[nameof(IIntervalRestrictedJsonItem<T>.Maximum)];
      if (maxProp != null) Maximum = maxProp.ToObject<T>();
    }

    public IntervalRestrictedArrayJsonItem(string id, IJsonConvertable convertable, JsonItemConverter converter) :
      base(id, convertable, converter) { }
  }
}
