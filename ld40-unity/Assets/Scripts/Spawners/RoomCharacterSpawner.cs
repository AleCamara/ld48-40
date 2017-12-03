using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomSpawnPositions
{
    public Room room = null;
    public Transform[] spawnPositions = new Transform[0];

    [HideInInspector]
    public RandomHand<Transform> positionHand = null;
    [HideInInspector]
    public bool stopSpawning = false;
}

public class RoomCharacterSpawner : MonoBehaviour
{
    [Header("Visitor Generator")]
    public GameObject[] visitorPrefabs = new GameObject[0];
    public Transform visitorParent = null;
    public CharacterType[] visitorHand = new CharacterType[0];
    public RoomSpawnPositions[] spawnPositions = new RoomSpawnPositions[0];
    public int numTimesRoomInHand = 4;

    [Header("Spawn Rate")]
    public float startRate = 1f / 10f;
    public float endRate = 1f / 2f;
    public float timeToReachEndRateS = 5 * 60;
    public AnimationCurve rateCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private float _normalizedElapsedTime = 0f;
    private float _timeSinceLastSpawnS = 0f;

    private RandomHand<CharacterType> _characterHand = null;
    private RandomHand<int> _roomHand = null;

    private void Start()
    {
        _characterHand = new RandomHand<CharacterType>(visitorHand);

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

    private float GetCurrentSpawnRate()
    {
        float curveRate = rateCurve.Evaluate(_normalizedElapsedTime);
        return (curveRate * (endRate - startRate)) + startRate;
    }

    private void SpawnCharacterInRandomRoom()
    {
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

        Room room = spawnPositions[roomIndex].room;
        if (!room.HasSpawnCapacity())
        {
            spawnPositions[roomIndex].stopSpawning = true;
        }

        if (spawnPositions[roomIndex].stopSpawning)
        {
            return;
        }
        
        GameObject characterGO = Instantiate(visitorPrefabs[characterTypeIdx], visitorParent);
        Character character = characterGO.GetComponent<Character>();

        Vector3 characterPosition = spawnPositions[roomIndex].positionHand.GetRandomElement().position;
        characterPosition.z = characterGO.transform.position.z;
        characterGO.transform.position = characterPosition;
        character.targetPosition = room.GetRandomFreeDancePosition();

        room.AddCharacter(character);
    }
}
