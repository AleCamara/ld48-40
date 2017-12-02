using UnityEngine;

public enum CharacterType
{
    A,
    B,
    C
}

public class Character : MonoBehaviour
{
    [Header("Character Settings")]
    public CharacterType characterType = CharacterType.A;
    public float speed = 1f;

    [Header("Character Face")]
    public SpriteRenderer faceRenderer = null;
    public Sprite happyFace = null;
    public Sprite neutralFace = null;
    public Sprite sadFace = null;

    public float happiness
    {
        get;
        private set;
    }

    public Transform targetPosition
    {
        get;
        set;
    }

    public bool isDragged
    {
        get;
        set;
    }

    public Room currentRoom
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

        Vector3 displacementDirection = targetPosition.position - transform.position;
        displacementDirection.z = 0f;
        float distance = displacementDirection.magnitude;
        displacementDirection.Normalize();

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
                    happiness = 1f;
                }
                break;

            case CharacterType.C:
                int numNonC = roomSituation.numCharacters[(int)CharacterType.A] + roomSituation.numCharacters[(int)CharacterType.B];
                int numC = roomSituation.numCharacters[(int)CharacterType.C];
                happiness = numNonC - numC;
                break;
        }
        happiness = Mathf.Clamp(happiness, -1f, 1f);
        UpdateFaceSprite();
    }

    private void UpdateFaceSprite()
    {
        Sprite currentSprite = neutralFace;
        if (happiness > 0.5f)
        {
            currentSprite = happyFace;
        }
        else if (happiness < -0.5f)
        {
            currentSprite = sadFace;
        }

        faceRenderer.sprite = currentSprite;
    }
}
