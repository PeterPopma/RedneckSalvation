using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] GameObject panelMission;
    [SerializeField] TextMeshProUGUI textMission;
    [SerializeField] List<Mission> missions = new List<Mission>();
    public static Game Instance;
    List<GameObject> npcs = new List<GameObject>();
    Mission activeMission = null;
    float timeLeftDisplayMission;
    float timeLeftDelay;
    string missionText;

    public List<GameObject> NPCs { get => npcs; set => npcs = value; }
    public Mission ActiveMission { get => activeMission; set => activeMission = value; }
    public List<Mission> Missions { get => missions; set => missions = value; }

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        panelMission.SetActive(false);
        Cursor.visible = false;
    }

    public void FixedUpdate()
    {
        if (timeLeftDisplayMission > 0)
        {
            timeLeftDisplayMission -= Time.deltaTime;
            if (timeLeftDisplayMission <= 0)
            {
                panelMission.SetActive(false);
            }
        }
        if (timeLeftDelay > 0)
        {
            timeLeftDelay -= Time.deltaTime;
            if (timeLeftDelay <= 0)
            {
                ShowMission();
            }
        }
    }

    private void ShowMission()
    {
        textMission.text = missionText;
        panelMission.SetActive(true);
        timeLeftDisplayMission = 8;
    }

    public void DisplayMission(string missionText, float delay = 0.01f)
    {
        this.missionText = missionText;
        timeLeftDelay = delay;
    }

}
