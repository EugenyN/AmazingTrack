// Copyright 2019 Eugeny Novikov. Code under MIT license.

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
}