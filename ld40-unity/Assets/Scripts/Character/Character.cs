using UnityEngine;

public enum CharacterType
{
    A,
    B,
    C
}

public class Character : MonoBehaviour
{
    public CharacterType characterType = CharacterType.A;
    public float speed = 1f;

    public float happiness
    {
        get;
        private set;
    }

    public Vector3 targetPosition
    {
        get;
        set;
    }

    public bool isDragged
    {
        get;
        set;
    }

    private void Update()
    {
        if (isDragged)
        {
            return;
        }

        Vector3 displacementDirection = targetPosition - transform.position;
        float distance = displacementDirection.magnitude;
        float frameMaxDisplacement = speed * Time.deltaTime;
        distance = Mathf.Min(distance, frameMaxDisplacement);
        Vector3 newPosition = transform.position + displacementDirection * distance;
        transform.position = newPosition;
    }

    public void EvaluateRoomSituation(RoomSituation roomSituation)
    {
        switch (characterType)
        {
            case CharacterType.A:
                {
                    int numNonA = roomSituation.numCharacters[(int)CharacterType.B] + roomSituation.numCharacters[(int)CharacterType.C];
                    int numA = roomSituation.numCharacters[(int)CharacterType.A];
                    happiness = numA - numNonA;
                }
                break;

            case CharacterType.B:
                {
                    int numNonB = roomSituation.numCharacters[(int)CharacterType.A] + roomSituation.numCharacters[(int)CharacterType.C];
                    int numB = roomSituation.numCharacters[(int)CharacterType.B];
                    happiness = numNonB - numB;
                }
                break;

            case CharacterType.C:
                happiness = 1f;
                break;
        }
        happiness = Mathf.Clamp(happiness, -1f, 1f);
    }
}
