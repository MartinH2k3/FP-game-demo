using System;
using System.Collections.Generic;

namespace Creatures.Player
{

[Serializable]
public class SkillUnlocks
{
    public Dictionary<Skill, bool> Abilities = new();

    public bool IsUnlocked(Skill skill) =>
        Abilities.TryGetValue(skill, out var unlocked) && unlocked;

    public void Unlock(Skill skill) =>
        Abilities[skill] = true;

    public void Lock(Skill skill) =>
        Abilities[skill] = false;
}

}