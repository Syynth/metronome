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

    public class Tuple<T1, T2>
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        internal Tuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
    }

    public static class Tuple
    {
        public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
        {
            var tuple = new Tuple<T1, T2>(item1, item2);
            return tuple;
        }
    }

}