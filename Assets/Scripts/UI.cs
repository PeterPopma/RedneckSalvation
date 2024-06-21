using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] Texture2D textureMap;
    [SerializeField] Texture2D textureFrame;
    [SerializeField] Texture2D texturePlayerPosition;
    [SerializeField] Texture2D textureMission;
    [SerializeField] Texture2D textureDestination;
    [SerializeField] GameObject imageMKey;
    [SerializeField] private RectTransform minimapDotBlue;
    [SerializeField] private Shader blendShader;
//    [SerializeField] private RectTransform minimapPlayer;
    private Rect rectFrame;
    private Player player;
    private int mapTextureHeight, mapTextureWidth;
    private int minimapWidth, minimapHeight;
    private int minimapPositionX, minimapPositionY;
    private float ratioTextureToMinimapX, ratioTextureToMinimapY;
    private bool showMap;

    public bool ShowMap { get => showMap; set => showMap = value; }

    void Start()
    {
        mapTextureWidth = textureMap.width;
        mapTextureHeight = textureMap.height;
        minimapWidth = (int)(Screen.width * 0.15);
        minimapHeight = (int)(Screen.height * 0.25);
        minimapPositionX = Screen.width - minimapWidth - 10;
        minimapPositionY = Screen.height - minimapHeight - 10;
        ratioTextureToMinimapX = minimapWidth / (float)textureMap.width;
        ratioTextureToMinimapY = minimapHeight / (float)textureMap.height;
        rectFrame = new Rect(minimapPositionX - 8, minimapPositionY - 8, minimapWidth + 16, minimapHeight + 16);
        player = GameObject.Find("Player").GetComponent<Player>();
        imageMKey.transform.position = new Vector3(minimapPositionX - 45, imageMKey.transform.position.y, imageMKey.transform.position.z);
    }

    private Vector2 WorldToMapPosition(float x, float z)
    {
        // 0,0..1022,833 <->  240,1677..2357,4217
        // note that the map uses world Z as X-axis and world X as Y-axis
        return new Vector2((float)(1022f * (z - 1677) / 2540.0), (float)(833f * (1-((x-240)/2117.0))));
    }

    void OnGUI()
    {
        if (showMap)
        {
            UpdateMap();
        }
        else
        {
            UpdateMiniMap();
        }
    }
    private void UpdateMap()
    {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), textureMap);
        Vector2 position = WorldToMapPosition(player.transform.position.x, player.transform.position.z);
        GUI.DrawTexture(new Rect(Screen.width * (position.x / mapTextureWidth), Screen.height * (position.y / mapTextureHeight), 32, 32), texturePlayerPosition);
        foreach (Mission mission in Game.Instance.Missions)
        {
            if (!mission.Finished)
            {
                position = WorldToMapPosition(mission.StartLocation.x, mission.StartLocation.y);
                GUI.DrawTexture(new Rect(Screen.width * (position.x / mapTextureWidth), Screen.height * (position.y / mapTextureHeight), 32, 32), textureMission);
            }
        }
    }

    private void UpdateMiniMap()
    {
        Vector2 position = WorldToMapPosition(player.transform.position.x, player.transform.position.z);
        float lowerleftCornerMinimapInMapTextureX = position.x - (minimapWidth / 2);
        float lowerleftCornerMinimapInMapTextureY = position.y + (minimapHeight / 2);
        if (lowerleftCornerMinimapInMapTextureX < 0)
            lowerleftCornerMinimapInMapTextureX = 0;
        if (lowerleftCornerMinimapInMapTextureX > mapTextureWidth - minimapWidth)
            lowerleftCornerMinimapInMapTextureX = mapTextureWidth - minimapWidth;
        if (lowerleftCornerMinimapInMapTextureY < minimapHeight)
            lowerleftCornerMinimapInMapTextureY = minimapHeight;
        if (lowerleftCornerMinimapInMapTextureY > mapTextureHeight)
            lowerleftCornerMinimapInMapTextureY = mapTextureHeight;
        float minimapPercentageInTextureMapX = lowerleftCornerMinimapInMapTextureX / mapTextureWidth;
        float minimapPercentageInTextureMapY = 1 - (lowerleftCornerMinimapInMapTextureY / mapTextureHeight);

        GUI.DrawTexture(rectFrame, textureFrame);
        GUI.DrawTextureWithTexCoords(new Rect(minimapPositionX, minimapPositionY, minimapWidth, minimapHeight), textureMap,
            new Rect(minimapPercentageInTextureMapX, minimapPercentageInTextureMapY,
                    ratioTextureToMinimapX, ratioTextureToMinimapY), false);

        if (position.x > 0 && position.y > 0 && position.x < mapTextureWidth && position.y < mapTextureHeight)
        {
//            minimapPlayer.position = new Vector3(minimapPositionX + position.x - lowerleftCornerMinimapInMapTextureX, position.y - lowerleftCornerMinimapInMapTextureY + minimapHeight, 0);
//            minimapPlayer.rotation = Quaternion.Euler(0, 0, -player.transform.rotation.eulerAngles.y);
            Matrix4x4 guiRotationMatrix = GUI.matrix; // set up for GUI rotation
            GUIUtility.RotateAroundPivot(-player.transform.eulerAngles.y + 90, new Vector2(12 + minimapPositionX + position.x - lowerleftCornerMinimapInMapTextureX, 12 + minimapPositionY + position.y - lowerleftCornerMinimapInMapTextureY + minimapHeight));
            GUI.DrawTexture(new Rect(minimapPositionX + position.x - lowerleftCornerMinimapInMapTextureX, minimapPositionY + position.y - lowerleftCornerMinimapInMapTextureY + minimapHeight, 24, 24), texturePlayerPosition);
            GUI.matrix = guiRotationMatrix; //end GUI rotation    
        }

        foreach (Mission mission in Game.Instance.Missions)
        {
            if (!mission.Finished)
            {
                position = WorldToMapPosition(mission.StartLocation.x, mission.StartLocation.y);
                if (position.x > lowerleftCornerMinimapInMapTextureX && position.y > lowerleftCornerMinimapInMapTextureY - minimapHeight && position.x < lowerleftCornerMinimapInMapTextureX + minimapWidth - 20 && position.y < lowerleftCornerMinimapInMapTextureY - 20)
                {
                    GUI.DrawTexture(new Rect(minimapPositionX + position.x - lowerleftCornerMinimapInMapTextureX, minimapPositionY + position.y - lowerleftCornerMinimapInMapTextureY + minimapHeight, 20, 20), textureMission);
                }
            }
        }
    }
}