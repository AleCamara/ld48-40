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
    [Header("UI")]
    public Image[] uiCharacterImages = null;
    public Color uiHappyColor = Color.green;
    public Color uiNeutralColor = Color.yellow;
    public Color uiUnhappyColor = Color.red;

    private List<Character> _characters = new List<Character>();
    private RoomSituation _roomSituation = new RoomSituation();
    private RoomHappiness _roomHappiness = new RoomHappiness();

    private void Update()
    {
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

    public void AddCharacter(Character character)
    {
        if (!_characters.Contains(character))
        {
            _characters.Add(character);
        }
    }

    public void RemoveCharacter(Character character)
    {
        if (_characters.Contains(character))
        {
            _characters.Remove(character);
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
