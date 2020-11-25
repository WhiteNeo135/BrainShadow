using UnityEngine;
using System.Collections.Generic;

namespace NewRandSys
{
    public class RandomizeInt
    {
        public static int RandomFrom(params int[] value)
        {
            if (value.Length < 1) return 0;
            return (value[Random.Range(0, value.Length)]);
        }
        public static int RandomEven(int min, int max)
        {
            List<int> intsArr = new List<int>();

            for (int i = min; i <= max; i++)
            {
                if (i % 2 == 0) intsArr.Add(i);
            }

            return RandomFrom(intsArr.ToArray());
        }
        public static int RandomOdd(int min, int max)
        {
            List<int> intsArr = new List<int>();

            for (int i = min; i <= max; i++)
            {
                if (i % 2 == 1) intsArr.Add(i);
            }

            return RandomFrom(intsArr.ToArray());
        }
    }
    public class RandomizeIntWhithException
    {
        private static int RandomFrom(params int[] value)
        {
            if (value.Length < 1) return 0;
            return (value[Random.Range(0, value.Length)]);
        }
        private static bool HaveInt(int i, int[] minus)
        {
            foreach (int j in minus)
            {
                if (j == i)
                {
                    return false;
                }
            }
            return true;
        }
        public static int RandomEvenWE(int min, int max, params int[] minus)
        {
            List<int> intsArr = new List<int>();

            for (int i = min; i <= max; i++)
            {
                if (i % 2 == 0 && HaveInt(i, minus)) intsArr.Add(i);
            }

            return RandomFrom(intsArr.ToArray());
        }
        public static int RandomOddWE(int min, int max)
        {
            List<int> intsArr = new List<int>();

            for (int i = min; i <= max; i++)
            {
                if (i % 2 == 1) intsArr.Add(i);
            }

            return RandomFrom(intsArr.ToArray());
        }
    }
}