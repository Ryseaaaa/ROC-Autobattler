using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class HelperFunctions
{
    /// <summary>
    /// Returns a random boolean according to a base chance and luck
    /// </summary>
    /// <param name="_baseSuccessChance">Base success chance as a range from 0 to 1</param>
    /// <param name="_luck"></param>
    /// <returns></returns>
    public static bool ReturnRandomBool(float _baseSuccessChance, float _luck)
    {
        int _rolls = 1 + Mathf.FloorToInt(_luck);
        if (_luck%1 > Random.Range(0f,1f))
        {
            _rolls++;
        }
        float _rollValue = 0;
        for(int i  = 0; i < _rolls; i++)
        {
            _rollValue = Mathf.Max(_rollValue, Random.Range(0f,1f));
        }
        return (_rollValue > (1.0f - _baseSuccessChance));
    }
    public static float ReturnRandomFloatInRange(float _min, float _max, float _luck)
    {
        int _rolls = 1 + Mathf.FloorToInt(_luck);
        if (_luck % 1 > Random.Range(0f, 1f))
        {
            _rolls++;
        }

        float _rollValue = _min;
        for (int i = 0; i <= _rolls; i++)
        {
            _rollValue = Mathf.Max(_rollValue, Random.Range(_min, _max));
        }
        return _rollValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_min">Min <u>Inclusive</u></param>
    /// <param name="_max">Max <u>Exclusive</u></param>
    /// <param name="_luck">Every 1 <paramref name="_luck"/> adds 1 reroll to get closer to <paramref name="_max"/></param>
    /// <returns></returns>
    public static int ReturnRandomIntInRange(int _min, int _max, float _luck)
    {
        int _rolls = 1 + Mathf.FloorToInt(_luck);
        if (_luck % 1 > Random.Range(0f, 1f))
        {
            _rolls++;
        }

        int _rollValue = _min;
        for (int i = 0; i <= _rolls; i++)
        {
            _rollValue = Mathf.Max(_rollValue, Random.Range(_min, _max));
        }
        return _rollValue;
    }

    public static void Shuffle<T>(this List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0,n+1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// Moves an element from this list at index i to another list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="original"></param>
    /// <param name="index"></param>
    /// <param name="target"></param>
    public static void MoveTo<T>(this List<T> original, int index, List<T> target)
    {
        target.Add(original[index]);
        original.RemoveAt(index);
    }
}
