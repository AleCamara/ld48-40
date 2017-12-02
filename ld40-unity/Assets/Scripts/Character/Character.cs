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

    public float happiness
    {
        get;
        private set;
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
