// Assets/Scripts/Combat/BattleManager.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    public Combatant playerPrefab;
    public Combatant enemyPrefab;

    private Combatant player;
    private Combatant enemy;
    public Combatant Player => player;
    
    public UIManager uiManager;
    private List<Weapon> playerTurnActions = new List<Weapon>();
    public List<Weapon> GetPlayerTurnActions() => new List<Weapon>(playerTurnActions);
    public List<Combo> allCombos;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeBattle();
    }

    private void InitializeBattle()
    {
        player = Instantiate(playerPrefab);
        enemy = Instantiate(enemyPrefab);
        
        player.Init();
        enemy.Init();

        uiManager.UpdateUI(player, enemy);
        uiManager.ShowMessage("¡El combate ha comenzado!");

        StartCoroutine(PlayerTurnRoutine());
    }

    private IEnumerator PlayerTurnRoutine()
    {
        Debug.Log("Turno del Jugador");
        playerTurnActions.Clear();
        // Actualizar preview de combo en UI
        uiManager.ShowComboPreview(playerTurnActions);
        player.currentActions = player.combatantData.maxActionsPerTurn;
        uiManager.UpdateActionCount(player.currentActions);
        uiManager.ShowPlayerTurnUI(player);
        while (player.currentActions > 0 && !uiManager.turnEnded)
        {
            yield return null;
        }
        uiManager.HidePlayerTurnUI();
        yield return StartCoroutine(ProcessPlayerActions());
    }

    public void AddActionToQueue(Weapon weapon)
    {
        if (player.currentActions > 0)
        {
            playerTurnActions.Add(weapon);
            player.currentActions--;
            uiManager.UpdateActionCount(player.currentActions);
            // Actualizar preview de combo en UI
            uiManager.ShowComboPreview(playerTurnActions);
            if (player.currentActions <= 0)
            {
                uiManager.turnEnded = true;
            }
        }
    }

    private IEnumerator ProcessPlayerActions()
    {
        Debug.Log("Procesando acciones del jugador...");
        Combo matchedCombo = GetComboMatch(playerTurnActions);
        if (matchedCombo != null)
        {
            yield return StartCoroutine(ExecuteSingleAttack(matchedCombo.comboAttack, player, enemy));
            uiManager.ShowMessage($"¡Combo activado! {matchedCombo.comboName}!");
        }
        else
        {
            foreach (var weapon in playerTurnActions)
            {
                yield return StartCoroutine(ExecuteSingleAttack(weapon.weaponAttack, player, enemy));
            }
        }
        if (enemy.currentHP <= 0)
        {
            EndBattle(true);
            yield break;
        }
        yield return new WaitForSeconds(1.0f);
        yield return StartCoroutine(EnemyTurnRoutine());
    }

    public Combo GetComboMatch(List<Weapon> sequence)
    {
        if (sequence == null) return null;
        foreach (var combo in allCombos)
        {
            if (sequence.Count != combo.requiredSequence.Count) continue;

            // Comprobación order-insensitive: comparar multisets (permitir repetidos)
            var requiredCopy = new List<Weapon>(combo.requiredSequence);
            bool match = true;
            foreach (var w in sequence)
            {
                if (requiredCopy.Contains(w))
                {
                    requiredCopy.Remove(w);
                }
                else
                {
                    match = false;
                    break;
                }
            }
            if (match) return combo;
        }
        return null;
    }

    private IEnumerator ExecuteSingleAttack(Attack attack, Combatant attacker, Combatant target)
    {
        uiManager.ShowMessage($"{attacker.combatantData.combatantName} usa {attack.attackName}!");
        // Aplicar escudo si corresponde
        if (attack.appliesShield && attack.shieldAmount > 0)
        {
            if (attack.shieldToSelf)
            {
                attacker.ApplyShield(attack.shieldAmount);
                uiManager.ShowMessage($"{attacker.combatantData.combatantName} recibe un escudo de {attack.shieldAmount}!");
                uiManager.UpdateUI(player, enemy);
            }
            else
            {
                target.ApplyShield(attack.shieldAmount);
                uiManager.ShowMessage($"{target.combatantData.combatantName} recibe un escudo de {attack.shieldAmount}!");
                uiManager.UpdateUI(player, enemy);
            }
        }

        // Aplicar curación si corresponde
        if (attack.appliesHeal && attack.healAmount > 0)
        {
            if (attack.healToSelf)
            {
                attacker.Heal(attack.healAmount);
                uiManager.ShowMessage($"{attacker.combatantData.combatantName} se cura {attack.healAmount}!");
                uiManager.UpdateUI(player, enemy);
            }
            else
            {
                target.Heal(attack.healAmount);
                uiManager.ShowMessage($"{target.combatantData.combatantName} se cura {attack.healAmount}!");
                uiManager.UpdateUI(player, enemy);
            }
        }

        // Aplicar daño si el ataque tiene potencia
        if (attack.basePower > 0)
        {
            int damage = attack.basePower + attacker.combatantData.baseAttack;
            target.TakeDamage(damage);
            uiManager.UpdateUI(player, enemy);
            Debug.Log($"{target.combatantData.combatantName} recibe {damage} de daño.");
        }
        yield return new WaitForSeconds(1.0f);
    }

    private IEnumerator EnemyTurnRoutine()
    {
        Debug.Log("Turno del Enemigo");
        if (enemy.combatantData.weapons.Count > 0)
        {
            Weapon enemyWeapon = enemy.combatantData.weapons[Random.Range(0, enemy.combatantData.weapons.Count)];
            yield return StartCoroutine(ExecuteSingleAttack(enemyWeapon.weaponAttack, enemy, player));
        }
        if (player.currentHP <= 0)
        {
            EndBattle(false);
            yield break;
        }
        yield return new WaitForSeconds(1.0f);
        uiManager.turnEnded = false;
        StartCoroutine(PlayerTurnRoutine());
    }

    public void EndBattle(bool playerWon)
    {
        Debug.Log(playerWon ? "¡Has ganado!" : "Has sido derrotado...");
        uiManager.ShowMessage(playerWon ? "¡Has ganado!" : "Has sido derrotado...");
    }
}