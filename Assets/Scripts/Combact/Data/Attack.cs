// Assets/Scripts/Combat/Data/Attack.cs
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Combat/Attack")]
public class Attack : ScriptableObject
{
    public string attackName;
    [TextArea]
    public string description;
    public int basePower;
    public string attackType; // Ejemplo: "Physical", "Magical", "Fire", "Ice"
    // Soporte para efectos de escudo
    public bool appliesShield = false;
    public int shieldAmount = 0;
    // Si true, el escudo se aplica al atacante; si false, al objetivo
    public bool shieldToSelf = false;
    // Soporte para efectos de curación
    public bool appliesHeal = false;
    public int healAmount = 0;
    // Si true, la curación se aplica al atacante; si false, al objetivo
    public bool healToSelf = false;
}