using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnLane
{
    public Transform standByPosition = null;
    public Transform readyPosition = null;
    [HideInInspector]
    public Character standByCharacter = null;
    [HideInInspector]
    public Character readyCharacter = null;
}

public class VisitorSpawner : MonoBehaviour
{
    [Header("Visitor Generator")]
    public GameObject[] visitorPrefabs = new GameObject[0];
    public Transform visitorParent = null;
    public CharacterType[] visitorHand = new CharacterType[0];
    public SpawnLane[] spawnLanes = new SpawnLane[0];

    private List<CharacterType> _currentVisitorHand = new List<CharacterType>();
    private Character[] _standByCharacters = new Character[0];

    private void Start()
    {
        ReplenishVisitorHand();

        int numLanes = spawnLanes.Length;
        for (int laneIdx = 0; laneIdx < numLanes; ++laneIdx)
        {
            // This character will be pushed to the ready position when the next character gets spawned
            SpawnCharacterInLane(laneIdx);
            SpawnCharacterInLane(laneIdx);
        }
    }

    public void SpawnReplacementForCharacter(Character character)
    {
        for (int laneIdx = 0; laneIdx < spawnLanes.Length; ++laneIdx)
        {
            SpawnLane spawnLane = spawnLanes[laneIdx];
            if (spawnLane.readyCharacter == character)
            {
                SpawnCharacterInLane(laneIdx);
                break;
            }
        }
    }

    private void SpawnCharacterInLane(int laneIdx)
    {
        CharacterType characterType = GetRandomCharacterTypeFromHand();
        int characterTypeIdx = (int)characterType;
        if (characterTypeIdx >= visitorPrefabs.Length)
        {
            Debug.LogError("Trying to spawn character of type (" + characterType.ToString() + ") but no prefab has been defined for it.");
            return;
        }

        if (spawnLanes[laneIdx].standByCharacter != null)
        {
            spawnLanes[laneIdx].standByCharacter.targetPosition = spawnLanes[laneIdx].readyPosition.position;
            spawnLanes[laneIdx].readyCharacter = spawnLanes[laneIdx].standByCharacter;
        }

        GameObject characterGO = Instantiate(visitorPrefabs[characterTypeIdx], visitorParent);
        Character character = characterGO.GetComponent<Character>();
        spawnLanes[laneIdx].standByCharacter = character;

        Vector3 targetPosition = spawnLanes[laneIdx].standByPosition.position;
        character.targetPosition = targetPosition;
        characterGO.transform.position = targetPosition;
    }

    private CharacterType GetRandomCharacterTypeFromHand()
    {
        if (_currentVisitorHand.Count < 1)
        {
            ReplenishVisitorHand();
        }

        int handIdx = UnityEngine.Random.Range(0, _currentVisitorHand.Count);
        CharacterType result = _currentVisitorHand[handIdx];
        _currentVisitorHand.RemoveAt(handIdx);
        return result;
    }

    private void ReplenishVisitorHand()
    {
        _currentVisitorHand.Clear();
        _currentVisitorHand.AddRange(visitorHand);
    }
}
