using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NVYVE.MVC
{
    public class CustomHashSet<T> : HashSet<T>
    {

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj);
        }
    }
}