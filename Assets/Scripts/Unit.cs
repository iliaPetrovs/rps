using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAttackStance { HIGH, MID, LOW, COUNTER_HIGH, COUNTER_MID, COUNTER_LOW, DEFEND }

public class Unit : MonoBehaviour
{
    // Unit name
    public string unitName;
    // Unit HP
    public int maxHp;
    public int currentHp;
    // Unit damage
    public int highDamage;
    public int midDamage;
    public int lowDamage;
    // Unit defense and resistance
    public float highAtkRes;
    public float midAtkRes;
    public float lowAtkRes;
    public int counterMultiplier;
    public int defense;
    public int defenseMultiplier;

    // Dynamic stats
    private int currentRes = 1;
    private int currentDmg;
    private PlayerAttackStance stance;
    public bool isCountering = false;

    public bool TakeDamageAndCheckIfDead(int damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
        {
            return true;
        } else
        {
            return false;
        }
    }

    public int calcDamage(int dmg, bool isCounter=false)
    {
        int counterMultiplier = isCounter ? this.counterMultiplier : 1;
        return dmg * counterMultiplier;
    }

    public void setStanceDamage(int dmg)
    {
        currentDmg = dmg;
    }

    public void setStanceResist(int res)
    {
        currentRes = res;
    }

    public int getCurrentDamage()
    {
        return currentDmg;
    }

    public void setStance(PlayerAttackStance stance)
    {
        this.stance = stance;
        isCountering = false;
    }

    public void setStanceWithCounter(PlayerAttackStance stance)
    {
        this.stance = stance;
        isCountering = true;
    }

    public PlayerAttackStance getStance()
    {
        return this.stance;
    }

    public float getResistanceValue(PlayerAttackStance stance)
    {
        switch(stance)
        {
            case PlayerAttackStance.HIGH:
                return highAtkRes;
            case PlayerAttackStance.MID:
                return midAtkRes;
            case PlayerAttackStance.LOW:
                return lowAtkRes;
            default:
                return 1;
        }
    }
}
