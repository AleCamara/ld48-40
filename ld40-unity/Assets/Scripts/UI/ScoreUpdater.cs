using UnityEngine;
using UnityEngine.UI;

public class ScoreUpdater : MonoBehaviour
{
    private static readonly string SCORE_PREFIX = "Happy Dancers: ";

    public Text scoreText = null;
    public Room[] rooms = new Room[0];

    private void Update()
    {
        int numHappyDancers = 0;
        foreach (Room room in rooms)
        {
            numHappyDancers += room.NumHappyDancers();
        }
        scoreText.text = SCORE_PREFIX + numHappyDancers.ToString();
    }
}
