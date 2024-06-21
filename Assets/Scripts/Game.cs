using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance;
    List<GameObject> npcs = new List<GameObject>();
    Mission activeMission = null;
    List<Mission> missions = new List<Mission>();

    public List<GameObject> NPCs { get => npcs; set => npcs = value; }
    public Mission ActiveMission { get => activeMission; set => activeMission = value; }
    public List<Mission> Missions { get => missions; set => missions = value; }

    public void Awake()
    {
        Instance = this;
        missions.Add(new Mission("Steal the safe from the bank of Raspberry. Then bring it to the saloon.qsf", new Vector2(1627, 3182), new Vector2(756.1f, 3053.1f)));
        missions.Add(new Mission("The 'ol Biscuit gang has burned down John's place. Go there and whipe them out!", new Vector2(871, 2691), new Vector2(756.1f, 3053.1f)));
    }

}
