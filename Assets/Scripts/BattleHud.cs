using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    public Slider hpSlider;
    public Text charName;

    public void SetHud(Unit unit)
    {
        hpSlider.maxValue = unit.maxHp;
        hpSlider.value = unit.currentHp;
        charName.text = unit.unitName;
    }

    public void SetHP(int hp)
    {
        hpSlider.value = hp;
    }
}
