using UnityEngine;
using System.Collections;

namespace Assets.Code
{

    public static class Direction
    {
        public const int Left = -1;
        public const int Right = 1;
    }

    public static class Utils
    {

        public static bool IsInLayerMask(int layer, LayerMask layermask)
        {
            return layermask == (layermask | (1 << layer));
        }

        public static Vector3 Clockwise(Vector3 input)
        {
            return new Vector3(-input.y, input.x);
        }

        public static Vector3 CounterClockwise(Vector3 input)
        {
            return Clockwise(input) * -1;
        }

        public static int Mod(int x, int m)
        {
            return (x % m + m) % m;
        }

    }

}