using UnityEngine;

public class IntroController : MonoBehaviour
{
    public RoomCharacterSpawner spawner = null;

    public GameObject introGO = null;
    public GameObject titleGO = null;
    public GameObject instructions1GO = null;
    public GameObject instructions2GO = null;
    public GameObject instructions3GO = null;

    public float timeToSkipTitle = 7f;
    public float timeToSkipInstructions1 = 5f;
    public float timeToSkipInstructions2 = 5f;
    public float timeToSkipInstructions3 = 10f;

    private enum State
    {
        Title,
        Instructions1,
        Instructions2,
        Instructions3,
        Finished
    }
    private State _currentState = State.Title;
    private float _normalisedElapsedTime = 0f;
    private float _currentSkipTimeS = 0f;

    private void Start()
    {
        introGO.SetActive(true);
        titleGO.SetActive(true);
        instructions1GO.SetActive(false);
        instructions2GO.SetActive(false);
        instructions3GO.SetActive(false);

        _currentSkipTimeS = timeToSkipTitle;
    }

    private void Update()
    {
        if (_currentState == State.Finished)
        {
            return;
        }

        _normalisedElapsedTime += Time.deltaTime / _currentSkipTimeS;

        if (Input.anyKeyDown || (_normalisedElapsedTime >= 1f))
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
                    }
                    break;
            }
        }
    }
}
