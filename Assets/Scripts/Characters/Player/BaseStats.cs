using System;
using UnityEngine;

namespace Characters.Player
{
[Serializable]
public class BaseStats
{
    public int healthPoints = 100;
    public int mana = 50;
    public float attackSpeed = 0.25f;
    public int attackDamage = 10;
    public int attackDamageModifier = 1; // constant modifier, multiplied by strength and added to attackDamage
    public int magicDamage = 5;
    public int magicDamageModifier = 1; // constant modifier, multiplied by intelligence and added to magicDamage
    public int strength = 0;
    public int agility = 0;
    public int intelligence = 0;
    public int speed = 0;
    public int durability = 0;
    public int fireResistance = 0;
    public int magicResistance = 0;

}
}