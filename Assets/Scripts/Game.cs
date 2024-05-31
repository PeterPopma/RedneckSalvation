using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance;
    List<GameObject> npcs = new List<GameObject>();

    public List<GameObject> NPCs { get => npcs; set => npcs = value; }

    public void Awake()
    {
        Instance = this;
    }

}
