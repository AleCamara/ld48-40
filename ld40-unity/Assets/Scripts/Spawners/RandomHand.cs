using System.Collections.Generic;
using UnityEngine;

public class RandomHand<T>
{
    private T[] _defaultHand = null;
    private List<T> _currentHand = new List<T>();

    public RandomHand(T[] hand)
    {
        _defaultHand = hand;
    }

    public T GetRandomElement()
    {
        if (_currentHand.Count < 1)
        {
            Replenish();
        }

        int elIndex = Random.Range(0, _currentHand.Count);
        T result = _currentHand[elIndex];
        _currentHand.RemoveAt(elIndex);
        return result;
    }

    private void Replenish()
    {
        _currentHand.AddRange(_defaultHand);
    }
}
