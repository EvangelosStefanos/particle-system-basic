using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyClasses
{
    public class Functions
    {        
        public static void applyFunction<T>(List<T> list, Action<T> func)
        {
            foreach(var item in list)
            {
                func(item);
            }
        }

    }
}
