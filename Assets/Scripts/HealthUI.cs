using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Image healthBarImage;

    public void UpdateHealth(int health, int maxHealth)
    {
        healthBarImage.fillAmount = (float)health / (float)maxHealth;
    }
}