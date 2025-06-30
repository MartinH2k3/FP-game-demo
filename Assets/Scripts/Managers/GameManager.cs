using System.IO;
using Characters.Player;
using Helpers;
using UnityEngine;

namespace Managers
{

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public SkillUnlocks SkillData { get; private set; }
    private string _skillDataPath;

    private void Awake() {
        // Singleton enforcement
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _skillDataPath = Config.GetSkillUnlocksPath();
        LoadSkillData();
    }

    public void LoadSkillData() {
        if (File.Exists(_skillDataPath))
        {
            var json = File.ReadAllText(_skillDataPath);
            SkillData = JsonUtility.FromJson<SkillUnlocks>(json);
        }
        else
        {
            SkillData = new SkillUnlocks();
            SaveSkillData();
        }
        SkillData.Init();
    }


    public void SaveSkillData() {
        var json = JsonUtility.ToJson(SkillData, true);
        File.WriteAllText(_skillDataPath, json);
    }
}

}