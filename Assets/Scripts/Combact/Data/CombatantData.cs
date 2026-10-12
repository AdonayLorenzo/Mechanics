// Assets/Scripts/Combat/Data/CombatantData.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Combatant Data", menuName = "Combat/Combatant Data")]
public class CombatantData : ScriptableObject
{
    public string combatantName;
    public int maxHP;
    public int baseAttack;
    public int baseDefense;
    public int maxActionsPerTurn;
    public List<Weapon> weapons;
}