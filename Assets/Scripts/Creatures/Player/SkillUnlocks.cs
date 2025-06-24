using System;
using System.Collections.Generic;
using UnityEngine;

namespace Creatures.Player
{

[Serializable]
public class SkillUnlocks
{
    public List<Skill> unlockedSkills = new();
    private Dictionary<Skill, bool> _skills = new();

    public void Init()
    {
        // Initialize all skills as locked
        foreach (Skill skill in Enum.GetValues(typeof(Skill)))
        {
            _skills[skill] = unlockedSkills.Contains(skill);
        }
    }

    public bool IsUnlocked(Skill skill) =>
        _skills.TryGetValue(skill, out var unlocked) && unlocked;

    public void Unlock(Skill skill) =>
        _skills[skill] = true;

    public void Lock(Skill skill) =>
        _skills[skill] = false;

    public List<Skill> AllSkills()
    {
        List<Skill> skills = new();
        foreach (Skill skill in Enum.GetValues(typeof(Skill))) skills.Add(skill);
        return skills;
    }
}

}