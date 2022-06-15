using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManaBar : Instancable<ManaBar>
{
    public Image manaBar;
    public TextMeshProUGUI manaText;

    public void UpdateValue(int mana)
    {
        manaBar.fillAmount = (float)mana / 10;
        manaText.text = mana.ToString();
    }
}
