// Assets/Scripts/UI/UIManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public Slider playerHPBar;
    public Slider enemyHPBar;
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI enemyHPText;
    public TextMeshProUGUI battleLogText;
    // Textos para mostrar escudo encima de la vida (el usuario dijo que los coloca, así que sólo exponemos los campos)
    public TextMeshProUGUI playerShieldText;
    public TextMeshProUGUI enemyShieldText;
    public TextMeshProUGUI actionCountText;
    public GameObject actionMenuPanel;
    public GameObject weaponButtonPrefab;
    // Tooltip prefab (raíz) y referencias a los textos dentro del prefab
    public GameObject tooltipPrefab;
    private GameObject tooltipInstance;
    private TextMeshProUGUI tooltipNameText;
    private TextMeshProUGUI tooltipDescriptionText;
    public bool turnEnded = false;

    public void UpdateUI(Combatant player, Combatant enemy)
    {
        playerHPBar.maxValue = player.combatantData.maxHP;
        playerHPBar.value = player.currentHP;
        playerHPText.text = $"{player.currentHP}/{player.combatantData.maxHP}";
        enemyHPBar.maxValue = enemy.combatantData.maxHP;
        enemyHPBar.value = enemy.currentHP;
        enemyHPText.text = $"{enemy.currentHP}/{enemy.combatantData.maxHP}";
        // Mostrar escudo si existe
        if (playerShieldText != null)
        {
            playerShieldText.text = player.currentShield > 0 ? $"Escudo: {player.currentShield}" : "";
        }
        if (enemyShieldText != null)
        {
            enemyShieldText.text = enemy.currentShield > 0 ? $"Escudo: {enemy.currentShield}" : "";
        }
    }

    public void ShowMessage(string message)
    {
        battleLogText.text = message;
    }

    // Tooltip control
    public void ShowTooltip(Weapon weapon, RectTransform buttonRect)
    {
        if (tooltipPrefab == null || weapon == null) return;
        if (tooltipInstance == null)
        {
            tooltipInstance = Instantiate(tooltipPrefab, actionMenuPanel.transform);
            // buscar textos dentro del prefab
            var texts = tooltipInstance.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 2)
            {
                tooltipNameText = texts[0];
                tooltipDescriptionText = texts[1];
            }
            else if (texts.Length == 1)
            {
                tooltipNameText = texts[0];
            }
        }
        if (tooltipInstance != null)
        {
            // asegúrate que el tooltip es hijo del mismo panel donde están los botones
            tooltipInstance.transform.SetParent(actionMenuPanel.transform, false);
            tooltipInstance.SetActive(true);
            if (tooltipNameText != null) tooltipNameText.text = weapon.weaponName;
            if (tooltipDescriptionText != null) tooltipDescriptionText.text = weapon.weaponAttack != null ? weapon.weaponAttack.description : "";

            // posicionar justo encima del botón usando anchoredPosition local
            RectTransform tooltipRect = tooltipInstance.GetComponent<RectTransform>();
            RectTransform parentRect = actionMenuPanel.GetComponent<RectTransform>();
            if (tooltipRect != null && buttonRect != null && parentRect != null)
            {
                // calcular posición relativa del botón dentro del panel
                Vector2 buttonAnchored = buttonRect.anchoredPosition;
                float yOffset = (buttonRect.rect.height * 0.5f) + (tooltipRect.rect.height * 0.5f) + 6f; // padding
                tooltipRect.anchoredPosition = buttonAnchored + new Vector2(0, yOffset);
                tooltipRect.SetAsLastSibling();
            }
        }
    }

    public void HideTooltip()
    {
        if (tooltipInstance != null) tooltipInstance.SetActive(false);
    }

    // Muestra una vista previa del combo en el log: "A + ?" o "A + B = comboName"
    public void ShowComboPreview(List<Weapon> queued)
    {
        if (queued == null || queued.Count == 0)
        {
            // Limpiar preview
            return;
        }

        if (queued.Count == 1)
        {
            battleLogText.text = $"{queued[0].weaponName} + ?";
        }
        else if (queued.Count == 2)
        {
            // Intentar encontrar combo
            var combo = BattleManager.Instance.GetComboMatch(queued);
            if (combo != null)
            {
                battleLogText.text = $"{queued[0].weaponName} + {queued[1].weaponName} = {combo.comboName}";
            }
            else
            {
                battleLogText.text = $"{queued[0].weaponName} + {queued[1].weaponName}";
            }
        }
        else
        {
            // Mostrar concatenación simple si hay más de 2
            string s = queued[0].weaponName;
            for (int i = 1; i < queued.Count; i++) s += " + " + queued[i].weaponName;
            battleLogText.text = s;
        }
    }

    public void UpdateActionCount(int count)
    {
        actionCountText.text = $"{count}";
    }

    public void ShowPlayerTurnUI(Combatant player)
    {
        actionMenuPanel.SetActive(true);
        turnEnded = false;
        DisplayWeapons(player.combatantData.weapons);
    }

    public void HidePlayerTurnUI()
    {
        actionMenuPanel.SetActive(false);
    }

    public void DisplayWeapons(List<Weapon> weapons)
    {
        ClearMenu();
        foreach (var weapon in weapons)
        {
            GameObject buttonGO = Instantiate(weaponButtonPrefab, actionMenuPanel.transform);
            buttonGO.GetComponentInChildren<TextMeshProUGUI>().text = weapon.weaponName;
            buttonGO.GetComponent<Button>().onClick.AddListener(() => OnWeaponSelected(weapon));
            // Añadir componente que maneje hover
            var wb = buttonGO.AddComponent<WeaponButton>();
            wb.weapon = weapon;
            wb.uiManager = this;
        }
    }

    private void OnWeaponSelected(Weapon weapon)
    {
        BattleManager.Instance.AddActionToQueue(weapon);
        if (BattleManager.Instance.Player.currentActions > 0)
        {
            DisplayWeapons(BattleManager.Instance.Player.combatantData.weapons);
        }
    }

    private void ClearMenu()
    {
        foreach (Transform child in actionMenuPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnEndTurnButton()
    {
        turnEnded = true;
    }
}