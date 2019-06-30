// Copyright 2019 Eugeny Novikov. Code under MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AmazingTrack
{
    public static class Extensions
    {
        public static GameObject ParentObject(this GameObject child)
        {
            if (child.transform.parent == null)
                return null;
            return child.transform.parent.gameObject;
        }

        public static bool IsSiblingObject(this GameObject objA, GameObject objB)
        {
            if (objB == null)
                return false;
            return objA.transform.parent == objB.transform.parent;
        }
    }

    public static class ObjectFinder
    {
        public static T Find<T>(string tag) where T : Component
        {
            var obj = GameObject.FindGameObjectWithTag(tag);
            if (obj == null)
                return null;
            return obj.GetComponent<T>();
        }
    }
}