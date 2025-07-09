using System;
using Characters.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{

public class ActiveAttackTypeIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI meleeLabel;
    [SerializeField] private TextMeshProUGUI rangedLabel;
    [SerializeField] private TextMeshProUGUI spellLabel;
    [SerializeField] private Image meleeIcon;
    [SerializeField] private Image rangedIcon;
    [SerializeField] private Image spellIcon;

    [SerializeField] private Color activeTextColor = Color.black;
    [SerializeField] private Color inactiveTextColor = Color.gray;
    [SerializeField] private Color activeIconColor = Color.white;
    [SerializeField] private Color inactiveIconColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    public void SetActiveAttackType(AttackType attackType) {
        meleeLabel.color = inactiveTextColor;
        meleeIcon.color = inactiveIconColor;

        rangedLabel.color = inactiveTextColor;
        rangedIcon.color = inactiveIconColor;

        spellLabel.color = inactiveTextColor;
        spellIcon.color = inactiveIconColor;

        // Activate the selected one
        switch (attackType) {
            case AttackType.Melee:
                meleeLabel.color = activeTextColor;
                meleeIcon.color = activeIconColor;
                break;
            case AttackType.Ranged:
                rangedLabel.color = activeTextColor;
                rangedIcon.color = activeIconColor;
                break;
            case AttackType.Spell:
                spellLabel.color = activeTextColor;
                spellIcon.color = activeIconColor;
                break;
        }
    }

}

}