public enum HappinessState
{
    Happy,
    Neutral,
    Unhappy
}

public static class HappinessUtils
{
    public static HappinessState GetHappinessStateFromValue(float happiness)
    {
        HappinessState result = HappinessState.Neutral;
        if (happiness > 0.25f)
        {
            result = HappinessState.Happy;
        }
        else if (happiness < -0.25f)
        {
            result = HappinessState.Unhappy;
        }
        return result;
    }
}
