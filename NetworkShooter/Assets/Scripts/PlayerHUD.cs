using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    public Image portrait;         // la imagen Retrato
    public Image marco;            // la imagen Marco
    public Image healthFill;       // la imagen BarraVida
    public TMP_Text damageText;    // el TextoDaño

    private Health player;

    public void Setup(Health target, Color color, Sprite portraitSprite)
    {
        player = target;

        if (portraitSprite != null)
            portrait.sprite = portraitSprite;

        if (color.a > 0f)
            portrait.color = color;   // tiñe el retrato

        player.OnHealthChanged += UpdateHUD;
        UpdateHUD(player.CurrentHealth, player.maxHealth);
    }

    void OnDestroy()
    {
        if (player != null)
            player.OnHealthChanged -= UpdateHUD;
    }

    void UpdateHUD(float current, float max)
    {
        healthFill.fillAmount = current / max;
        damageText.text = (max - current).ToString("0") + "%";
    }
}