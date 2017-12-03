using UnityEngine;

public class IntroController : MonoBehaviour
{
    public RoomCharacterSpawner spawner = null;

    [Header("Intro Objects")]
    public GameObject introGO = null;
    public GameObject titleGO = null;
    public GameObject instructions1GO = null;
    public GameObject instructions2GO = null;
    public GameObject instructions3GO = null;

    [Header("Intro Times")]
    public float timeToSkipTitle = 7f;
    public float timeToSkipInstructions1 = 5f;
    public float timeToSkipInstructions2 = 5f;
    public float timeToSkipInstructions3 = 10f;

    [Header("Charge Objects")]
    public GameObject chargeIntroGO = null;
    public GameObject chargeInstructions1GO = null;
    public GameObject chargeInstructions2GO = null;

    [Header("Charge Times")]
    public float timeToSkipChargeInstructions1 = 3f;
    public float timeToSkipChargeInstructions2 = 7f;

    public bool showChargeIntro
    {
        get;
        set;
    }

    private enum State
    {
        Title,
        Instructions1,
        Instructions2,
        Instructions3,
        FinishedIntro,
        ChargeInstructions1,
        ChargeInstructions2,
        FinalFinished
    }
    private State _currentState = State.Title;
    private float _normalisedElapsedTime = 0f;
    private float _currentSkipTimeS = 0f;

    private void Start()
    {
        Time.timeScale = 1f;

        introGO.SetActive(true);
        titleGO.SetActive(true);
        instructions1GO.SetActive(false);
        instructions2GO.SetActive(false);
        instructions3GO.SetActive(false);

        chargeIntroGO.SetActive(false);
        chargeInstructions1GO.SetActive(false);
        chargeInstructions2GO.SetActive(false);

        showChargeIntro = false;
        _currentSkipTimeS = timeToSkipTitle;
    }

    private void Update()
    {
        bool hasFinished = _currentState == State.FinalFinished;
        if (hasFinished)
        {
            return;
        }

        bool keepWaitingForStartShowingChargeIntro = (_currentState == State.FinishedIntro) && !showChargeIntro;
        if (keepWaitingForStartShowingChargeIntro)
        {
            return;
        }

        _normalisedElapsedTime += (Time.timeScale > 0f) ? Time.deltaTime / _currentSkipTimeS : 1f / 30f / _currentSkipTimeS;
        
        if (Input.anyKeyDown || (_normalisedElapsedTime >= 1f) || (_currentState == State.FinishedIntro))
        {
            _normalisedElapsedTime = 0;
            switch (_currentState)
            {
                case State.Title:
                    {
                        instructions1GO.SetActive(true);
                        titleGO.SetActive(false);
                        _currentSkipTimeS = timeToSkipInstructions1;
                        _currentState = State.Instructions1;
                    }
                    break;

                case State.Instructions1:
                    {
                        instructions2GO.SetActive(true);
                        _currentSkipTimeS = timeToSkipInstructions2;
                        _currentState = State.Instructions2;
                    }
                    break;

                case State.Instructions2:
                    {
                        instructions3GO.SetActive(true);
                        _currentSkipTimeS = timeToSkipInstructions3;
                        _currentState = State.Instructions3;
                    }
                    break;

                case State.Instructions3:
                    {
                        introGO.SetActive(false);
                        spawner.BeginSpawning();
                        _currentState = State.FinishedIntro;
                    }
                    break;

                case State.FinishedIntro:
                    {
                        if (showChargeIntro)
                        {
                            Time.timeScale = 0f;
                            chargeInstructions1GO.SetActive(true);
                            chargeIntroGO.SetActive(true);
                            _currentSkipTimeS = timeToSkipChargeInstructions1;
                            _currentState = State.ChargeInstructions1;
                        }
                    }
                    break;

                case State.ChargeInstructions1:
                    {
                        chargeInstructions2GO.SetActive(true);
                        _currentSkipTimeS = timeToSkipChargeInstructions2;
                        _currentState = State.ChargeInstructions2;
                    }
                    break;

                case State.ChargeInstructions2:
                    {
                        chargeIntroGO.SetActive(false);
                        _currentState = State.FinalFinished;
                        Time.timeScale = 1f;
                    }
                    break;
            }
        }
    }
}
