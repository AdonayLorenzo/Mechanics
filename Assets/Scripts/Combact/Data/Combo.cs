// Assets/Scripts/Combat/Data/Combo.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Combo", menuName = "Combat/Combo")]
public class Combo : ScriptableObject
{
    public string comboName;
    public List<Weapon> requiredSequence; // La secuencia de armas que activa el combo
    public Attack comboAttack; // El ataque resultante del combo
    public string description;
}