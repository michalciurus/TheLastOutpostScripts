using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;



static class RandomExtensions
{
    
    public static IEnumerable<Enum> GetFlags(this Enum e)
    {
        return Enum.GetValues(e.GetType()).Cast<Enum>().Where(e.HasFlag);
    }

    public static bool HasAnyFlag(this Enum e, Enum k)
    {
        foreach (Enum possibleFlag in k.GetFlags())
        {
            if (e.HasFlag(possibleFlag))
            {
                return true;
            }
        }

        return false;
    }
    
    public static void Shuffle<T> (this Random rng, T[] array)
    {
        int n = array.Length;
        while (n > 1) 
        {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
}