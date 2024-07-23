[System.Serializable]
public class GoalAttempt
{
    public int roundNumber;
    public int attemptNumber;
    public GoalPosition goalPosition;
    public float reflexTime;
    public bool isSaved;
    public float errorDistance;
    public string bodyArea;

    public GoalAttempt(int roundNumber, int attemptNumber, GoalPosition goalPosition, float reflexTime, bool isSaved, float errorDistance, string bodyArea)
    {
        this.roundNumber = roundNumber;
        this.attemptNumber = attemptNumber;
        this.goalPosition = goalPosition;
        this.reflexTime = reflexTime;
        this.isSaved = isSaved;
        this.errorDistance = errorDistance;
        this.bodyArea = bodyArea;
    }

    public string ToCsvString()
    {
        return $"{roundNumber},{attemptNumber},{goalPosition},{reflexTime},{isSaved},{errorDistance},{bodyArea}";
    }
}