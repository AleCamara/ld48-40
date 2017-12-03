using System.Collections.Generic;
using UnityEngine;

public class RandomHand<T>
{
    private T[] _defaultHand = null;
    private List<T> _currentHand = new List<T>();

    public int numHandsDelivered
    {
        get;
        private set;
    }

    public RandomHand(T[] hand)
    {
        _defaultHand = hand;
        Replenish();
        numHandsDelivered = 0;
    }

    public T GetRandomElement()
    {
        int elIndex = Random.Range(0, _currentHand.Count);
        T result = _currentHand[elIndex];
        _currentHand.RemoveAt(elIndex);
        
        if (_currentHand.Count < 1)
        {
            Replenish();
            numHandsDelivered++;
        }

        return result;
    }

    private void Replenish()
    {
        _currentHand.AddRange(_defaultHand);
    }
}
