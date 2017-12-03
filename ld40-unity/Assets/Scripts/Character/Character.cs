using UnityEngine;

public enum CharacterType
{
    A,
    B,
    C
}

public enum CharacterCharge
{
    Neutral,
    Positive,
    Negative
}

public class Character : MonoBehaviour
{
    private static readonly string MATERIAL_CHARGE_COLOR_PROPERTY = "_Color";

    [Header("Character Settings")]
    public CharacterType characterType = CharacterType.A;
    public CharacterCharge characterCharge = CharacterCharge.Neutral;
    public float speed = 1f;

    [Header("Happiness")]
    public float peopleTolerance = 1f;
    public float chargeTolerance = 0.4f;

    [Header("Character Face")]
    public SpriteRenderer faceRenderer = null;
    public Sprite happyFace = null;
    public Sprite neutralFace = null;
    public Sprite sadFace = null;

    [Header("Character Charge")]
    public Renderer[] chargeRenderers = null;
    public Color neutralColor = Color.white;
    public Color positiveColor = Color.red;
    public Color negativeColor = Color.blue;

    private CharacterCharge _currentCharge = CharacterCharge.Neutral;
    private MaterialPropertyBlock[] _propertyBlocks = null;

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

    private void Start()
    {
        _propertyBlocks = new MaterialPropertyBlock[chargeRenderers.Length];
        for (int blockIdx = 0; blockIdx < chargeRenderers.Length; ++blockIdx)
        {
            _propertyBlocks[blockIdx] = new MaterialPropertyBlock();
        }
    }

    private void Update()
    {
        if (_currentCharge != characterCharge)
        {
            UpdateCharacterCharge();
        }

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
        int numA = roomSituation.numCharacters[(int)CharacterType.A];
        int numB = roomSituation.numCharacters[(int)CharacterType.B];
        int numC = roomSituation.numCharacters[(int)CharacterType.C];

        int numPositive = roomSituation.numCharges[(int)CharacterCharge.Positive];
        int numNegative = roomSituation.numCharges[(int)CharacterCharge.Negative];

        float typeHappiness = 0f;
        switch (characterType)
        {
            case CharacterType.A:
                {
                    typeHappiness = (numA - (numB + numC));
                }
                break;

            case CharacterType.B:
                {
                    typeHappiness = (numB - 2f * numC);
                }
                break;

            case CharacterType.C:
                {
                    typeHappiness = (numC > 1) ? ((numA + numB) - 5f * (numC - 1)) : 0;
                }
                break;
        }
        typeHappiness = 1 + typeHappiness * peopleTolerance;

        float chargeHappiness = -Mathf.Abs(numPositive - numNegative) * chargeTolerance;

        happiness = Mathf.Clamp(typeHappiness + chargeHappiness, -1f, 1f);
        UpdateFaceSprite();
    }

    private void UpdateFaceSprite()
    {
        HappinessState happinessState = HappinessUtils.GetHappinessStateFromValue(happiness);

        Sprite currentSprite = neutralFace;
        switch (happinessState)
        {
            case HappinessState.Happy:
                currentSprite = happyFace; 
                break;
            case HappinessState.Unhappy:
                currentSprite = sadFace;
                break;
        }

        faceRenderer.sprite = currentSprite;
    }

    private void UpdateCharacterCharge()
    {
        Color newChargeColor = neutralColor;
        switch (characterCharge)
        {
            case CharacterCharge.Positive:
                newChargeColor = positiveColor;
                break;

            case CharacterCharge.Negative:
                newChargeColor = negativeColor;
                break;
        }

        for (int rendererIdx = 0; rendererIdx < chargeRenderers.Length; ++rendererIdx)
        {
            Renderer chargeRenderer = chargeRenderers[rendererIdx];
            MaterialPropertyBlock propertyBlock = _propertyBlocks[rendererIdx];

            chargeRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(MATERIAL_CHARGE_COLOR_PROPERTY, newChargeColor);
            chargeRenderer.SetPropertyBlock(propertyBlock);
        }

        _currentCharge = characterCharge;
    }
}
