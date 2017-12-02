using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomSituation
{
    public int[] numCharacters = null;

    public RoomSituation()
    {
        numCharacters = new int[Enum.GetValues(typeof(CharacterType)).Length];
        Clear();
    }

    public void Clear()
    {
        for (int charIdx = 0; charIdx < numCharacters.Length; ++charIdx)
        {
            numCharacters[charIdx] = 0;
        }
    }
}

public class RoomHappiness
{
    public float[] characterHappiness = null;

    private int[] _numCharacters = null;

    public RoomHappiness()
    {
        characterHappiness = new float[Enum.GetValues(typeof(CharacterType)).Length];
        _numCharacters = new int[Enum.GetValues(typeof(CharacterType)).Length];
        Clear();
    }

    public void AddHappinessOfCharacter(Character character)
    {
        int charTypeIdx = (int)character.characterType;
        int numCharsOfType = _numCharacters[charTypeIdx];
        if (numCharsOfType > 0)
        {
            characterHappiness[charTypeIdx] = (characterHappiness[charTypeIdx] * (float)numCharsOfType + character.happiness) / (float)(numCharsOfType + 1);
        }
        else
        {
            characterHappiness[charTypeIdx] = character.happiness;
        }
        _numCharacters[charTypeIdx]++;
    }

    public void Clear()
    {
        for (int charIdx = 0; charIdx < characterHappiness.Length; ++charIdx)
        {
            characterHappiness[charIdx] = 0f;
            _numCharacters[charIdx] = 0;
        }
    }
}

public class Room : MonoBehaviour
{
    public Transform[] roomDancePositions = new Transform[0];
    public float spawnCapacityFractionFromTotal = 3f / 4f;
    public float dropCapacityFractionFromTotal = 4f / 5f;

    [Header("UI")]
    public Image[] uiCharacterImages = null;
    public Color uiHappyColor = Color.green;
    public Color uiNeutralColor = Color.yellow;
    public Color uiUnhappyColor = Color.red;

    private List<Character> _characters = new List<Character>();
    private RoomSituation _roomSituation = new RoomSituation();
    private RoomHappiness _roomHappiness = new RoomHappiness();
    private Character[] _characterInDancePosition = null;
    private int _spawnCapacity = 0;
    private int _dropCapacity = 0;

    private void Start()
    {
        int numDancePositions = roomDancePositions.Length;
        _characterInDancePosition = new Character[numDancePositions];
        _spawnCapacity = Mathf.FloorToInt(spawnCapacityFractionFromTotal * numDancePositions);
        _dropCapacity = Mathf.FloorToInt(dropCapacityFractionFromTotal * numDancePositions);
    }

    private void Update()
    {
        UpdateCharacterDancePositions();

        _roomSituation.Clear();
        _roomHappiness.Clear();

        // Calculate room situation
        foreach (Character character in _characters)
        {
            _roomSituation.numCharacters[(int)character.characterType]++;
        }

        // Tell situation to all characters and gather their happiness
        foreach (Character character in _characters)
        {
            character.EvaluateRoomSituation(_roomSituation);
            _roomHappiness.AddHappinessOfCharacter(character);
        }

        // Update UI
        int numCharTypes = Enum.GetValues(typeof(CharacterType)).Length;
        for (int charTypeIdx = 0; charTypeIdx < numCharTypes; ++charTypeIdx)
        {
            uiCharacterImages[charTypeIdx].color = GetHappinessColor(_roomHappiness.characterHappiness[charTypeIdx]);
        }
    }

    public bool HasSpawnCapacity()
    {
        return _characters.Count < _spawnCapacity;
    }

    public bool HasDropCapacity()
    {
        return _characters.Count < _dropCapacity;
    }

    public Transform GetRandomFreeDancePosition()
    {
        Transform freePosition = null;

        int numPositions = roomDancePositions.Length;
        int startIndex = UnityEngine.Random.Range(0, numPositions);
        int currentIndex = (startIndex + 1) % numPositions;
        while ((_characterInDancePosition[currentIndex] != null) && (currentIndex != startIndex))
        {
            currentIndex = (currentIndex + 1) % numPositions;
        }

        if (currentIndex != startIndex)
        {
            freePosition = roomDancePositions[currentIndex];
        }

        return freePosition;
    }

    public void AddCharacter(Character character)
    {
        if (!_characters.Contains(character))
        {
            _characters.Add(character);
            character.currentRoom = this;
        }
    }

    public void RemoveCharacter(Character character)
    {
        if (_characters.Contains(character))
        {
            _characters.Remove(character);
            character.currentRoom = null;
        }
    }

    private void UpdateCharacterDancePositions()
    {
        for (int characterIdx = 0; characterIdx < _characterInDancePosition.Length; ++characterIdx)
        {
            _characterInDancePosition[characterIdx] = null;
        }

        foreach (Character character in _characters)
        {
            int index = Array.IndexOf(roomDancePositions, character.targetPosition);
            if (index >= 0)
            {
                _characterInDancePosition[index] = character;
            }
        }
    }

    private Color GetHappinessColor(float happiness)
    {
        Color colorA = uiNeutralColor;
        Color colorB = uiHappyColor;
        float t = Mathf.Clamp(happiness, -1f, 1f);

        if (t < 0)
        {
            colorA = uiUnhappyColor;
            colorB = uiNeutralColor;
            t += 1f;
        }

        return Color.Lerp(colorA, colorB, t);
    }
}
