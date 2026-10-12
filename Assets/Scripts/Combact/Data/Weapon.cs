// Assets/Scripts/Combat/Data/Weapon.cs
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Combat/Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public Attack weaponAttack;
}