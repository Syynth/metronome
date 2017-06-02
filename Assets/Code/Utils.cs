using UnityEngine;
using System.Collections;

namespace Assets.Code
{

    public static class Direction
    {
        public const int Left = -1;
        public const int Right = 1;
    }

    public static class Extensions
    {

        public static bool IsValid(this RaycastHit hit)
        {
            return hit.collider != null;
        }

    }

}