using UnityEngine;
using System.Collections.Generic;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    public PlayerHUD hudPrefab;
    public Transform container;            // HUDContainer
    public Color[] playerColors;           // un color por jugador
    public Sprite[] portraits;             // un retrato por jugador (opcional)

    private List<PlayerHUD> huds = new List<PlayerHUD>();

    void Awake()
    {
        Instance = this;
    }

    public void RegisterPlayer(Health player)
    {
        int index = huds.Count;

        PlayerHUD hud = Instantiate(hudPrefab, container);

        Color color = index < playerColors.Length ? playerColors[index] : Color.white;
        Sprite portrait = index < portraits.Length ? portraits[index] : null;

        hud.Setup(player, color, portrait);

        // Tiñe al jugador con el mismo color que su retrato
        if (player.TryGetComponent(out PlayerControl control))
            control.SetColor(color);

        huds.Add(hud);
    }
}