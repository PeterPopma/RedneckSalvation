using UnityEngine;

public class Mission
{
    private string description;
    private bool finished;
    private Vector2 startLocation;
    private Vector2 destinationLocation;
    private Time startingTime;

    public bool Finished { get => finished; set => finished = value; }
    public string Description { get => description; set => description = value; }
    public Vector2 StartLocation { get => startLocation; set => startLocation = value; }
    public Vector2 DestinationLocation { get => destinationLocation; set => destinationLocation = value; }
    public Time StartingTime { get => startingTime; set => startingTime = value; }

    public Mission(string description, Vector2 startLocation, Vector2 destinationLocation)
    {
        this.description = description;
        this.startLocation = startLocation;
        this.destinationLocation = destinationLocation;
    }
}
