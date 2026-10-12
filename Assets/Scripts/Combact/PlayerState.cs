// Assets/Scripts/Combat/PlayerState.cs
using UnityEngine;
using System.Collections.Generic;

public class PlayerState : MonoBehaviour
{
    public CombatantData combatantData;
    public int currentHP;
    public int currentShield;
    public int currentActions;
    // Lista de armas a nivel de instancia (copia de combatantData.weapons en Init)
    private List<Weapon> instanceWeapons = new List<Weapon>();

    public virtual void Init()
    {
        currentHP = combatantData.maxHP;
        currentShield = 0;
        currentActions = combatantData.maxActionsPerTurn;
        // Inicializar lista de armas de la instancia a partir de los datos del combatant
        instanceWeapons = new List<Weapon>();
        if (combatantData != null && combatantData.weapons != null)
        {
            instanceWeapons.AddRange(combatantData.weapons);
        }
    }

    public virtual void TakeDamage(int damage)
    {
        // El escudo absorbe daño primero
        if (currentShield > 0)
        {
            int absorbed = Mathf.Min(currentShield, damage);
            currentShield -= absorbed;
            damage -= absorbed;
        }
        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            // Aquí puedes llamar a un evento de 'muerte'
        }
    }

    public void ApplyShield(int amount)
    {
        currentShield += amount;
    }

    // MÉTODOS PARA MANEJAR ARMAS EN LA INSTANCIA
    public List<Weapon> GetWeapons()
    {
        return instanceWeapons;
    }

    public void AddWeapon(Weapon weapon)
    {
        if (weapon == null) return;
        instanceWeapons.Add(weapon);
    }

    public bool RemoveWeapon(Weapon weapon)
    {
        if (weapon == null) return false;
        return instanceWeapons.Remove(weapon);
    }

    public void RemoveWeaponAt(int index)
    {
        if (index >= 0 && index < instanceWeapons.Count)
            instanceWeapons.RemoveAt(index);
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        int maxHP = combatantData != null ? combatantData.maxHP : int.MaxValue;
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
    }
}
