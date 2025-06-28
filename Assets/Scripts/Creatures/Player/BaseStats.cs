using System;
using UnityEngine;

namespace Creatures.Player
{
[Serializable]
public class BaseStats
{
    public int healthPoints = 100;
    public int mana = 50;
    public int strength = 0;
    public int agility = 0;
    public int intelligence = 0;
    public int speed = 0;
    public int durability = 0;
    public int fireResistance = 0;
    public int magicResistance = 0;

}
}