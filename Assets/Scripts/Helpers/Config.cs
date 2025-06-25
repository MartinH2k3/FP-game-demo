using System.IO;
using UnityEngine;

namespace Helpers
{
    public class Config {
        public static string GetSkillUnlocksPath()
        {
            return Path.Combine(Application.persistentDataPath, "skill_unlocks.json");
        }
    }
}