using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomSpawnPositions
{
    public Room room = null;
    public Transform[] spawnPositions = new Transform[0];

    [HideInInspector]
    public RandomHand<Transform> positionHand = null;
}

public class RoomCharacterSpawner : MonoBehaviour
{
    [Header("Visitor Generator")]
    public GameObject[] visitorPrefabs = new GameObject[0];
    public Transform visitorParent = null;
    public CharacterType[] visitorHand = new CharacterType[0];
    public CharacterCharge[] chargeHand = new CharacterCharge[0];
    public RoomSpawnPositions[] spawnPositions = new RoomSpawnPositions[0];
    public int numTimesRoomInHand = 4;
    public int numVisitorHandsBeforeSplit = 7;
    public int numVisitorHandsAfterSplit = 7;

    [Header("Spawn Rate")]
    public float startRate = 1f / 10f;
    public float endRate = 1f / 2f;
    public float timeToReachEndRateS = 5 * 60;
    public AnimationCurve rateCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private float _normalizedElapsedTime = 0f;
    private float _timeSinceLastSpawnS = 0f;

    private RandomHand<CharacterType> _characterHand = null;
    private RandomHand<CharacterCharge> _chargeHand = null;
    private RandomHand<int> _roomHand = null;
    private bool _hasStarted = false;
    private bool _hasSplitted = false;
    private bool _hasStoppedSpawning = false;

    private void Start()
    {
        _characterHand = new RandomHand<CharacterType>(visitorHand);
        _chargeHand = new RandomHand<CharacterCharge>(chargeHand);

        int numRooms = spawnPositions.Length;
        int numRoomsInHand = numTimesRoomInHand * numRooms;
        int[] defaultRoomHand = new int[numRoomsInHand];
        for (int index = 0; index < numRoomsInHand; ++index)
        {
            defaultRoomHand[index] = index % numRooms;
        }
        _roomHand = new RandomHand<int>(defaultRoomHand);

        foreach (RoomSpawnPositions rsp in spawnPositions)
        {
            rsp.positionHand = new RandomHand<Transform>(rsp.spawnPositions);
        }
    }

    private void Update()
    {
        if (!_hasStarted || _hasStoppedSpawning)
        {
            return;
        }

        _normalizedElapsedTime += Time.deltaTime / timeToReachEndRateS;
        _timeSinceLastSpawnS += Time.deltaTime;
        float currentSpawnRate = GetCurrentSpawnRate();
        float timeToSpawn = (currentSpawnRate > 0.001f) ? 1f / currentSpawnRate : 1000f;
        while (_timeSinceLastSpawnS > timeToSpawn)
        {
            _timeSinceLastSpawnS -= timeToSpawn;
            SpawnCharacterInRandomRoom();
        }
    }

    public void BeginSpawning()
    {
        _hasStarted = true;
    }

    private float GetCurrentSpawnRate()
    {
        float curveRate = rateCurve.Evaluate(_normalizedElapsedTime);
        return (curveRate * (endRate - startRate)) + startRate;
    }

    private void SpawnCharacterInRandomRoom()
    {
        if (_hasStoppedSpawning)
        {
            return;
        }

        CharacterType characterType = _characterHand.GetRandomElement();
        int characterTypeIdx = (int)characterType;
        if (characterTypeIdx >= visitorPrefabs.Length)
        {
            Debug.LogError("Trying to spawn character of type (" + characterType.ToString() + ") but no prefab has been defined for it.");
            return;
        }

        int roomIndex = _roomHand.GetRandomElement();
        if (roomIndex >= spawnPositions.Length)
        {
            Debug.LogError("Trying to spawn character of type (" + characterType.ToString() + ") in room (" + roomIndex.ToString() + ") but the room doesn't exist.");
            return;
        }

        CharacterCharge characterCharge = CharacterCharge.Neutral;
        if (_hasSplitted)
        {
            characterCharge = _chargeHand.GetRandomElement();
        }

        Room room = spawnPositions[roomIndex].room;        
        GameObject characterGO = Instantiate(visitorPrefabs[characterTypeIdx], visitorParent);
        Character character = characterGO.GetComponent<Character>();
        character.characterCharge = characterCharge;

        Vector3 characterPosition = spawnPositions[roomIndex].positionHand.GetRandomElement().position;
        characterPosition.z = characterGO.transform.position.z;
        characterGO.transform.position = characterPosition;
        character.targetPosition = room.GetRandomFreeDancePosition();

        room.AddCharacter(character);

        if (!_hasSplitted && (_characterHand.numHandsDelivered > numVisitorHandsBeforeSplit))
        {
            _hasSplitted = true;
        }
        else if (_characterHand.numHandsDelivered > (numVisitorHandsBeforeSplit + numVisitorHandsAfterSplit))
        {
            _hasStoppedSpawning = true;
        }
    }
}
