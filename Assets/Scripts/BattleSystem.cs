using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, BATTLETURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public GameObject playerCombatButtons;
    public GameObject enemyCombatButtons;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    Unit playerUnit;
    Unit enemyUnit;

    public TextMeshProUGUI announcementText;

    public int selectedPlayerDmg;
    public int selectedEnemyDmg;

    public BattleHud playerHud;
    public BattleHud enemyHud;

    public BattleState state;

    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        GameObject playerObject = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerObject.GetComponent<Unit>();

        GameObject enemyObject = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit =  enemyObject.GetComponent<Unit>();
        Transform outline = enemyObject.GetComponent<Transform>();

        playerHud.SetHud(playerUnit);
        enemyHud.SetHud(enemyUnit);

        announcementText.text = "FIGHT!";
        announcementText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        StartCoroutine(PlayerTurn());

    }

    IEnumerator PlayerAttack()
    {
        playerCombatButtons.SetActive(false);

        yield return new WaitForSeconds(1f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
         
    }

    IEnumerator EnemyTurn()
    {
        announcementText.text = enemyUnit.unitName + "'s Turn!";
        announcementText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        announcementText.gameObject.SetActive(false);

        enemyUnit.setStanceDamage(enemyUnit.midDamage);
        enemyUnit.setStance(PlayerAttackStance.MID);

        state = BattleState.BATTLETURN;
        StartCoroutine(BattleTurn());

    }

    IEnumerator PlayerTurn()
    {
        announcementText.text = playerUnit.unitName + "'s Turn!";
        announcementText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        announcementText.gameObject.SetActive(false);

        playerCombatButtons.SetActive(true);

        announcementText.gameObject.SetActive(false);
    }

    IEnumerator BattleTurn()
    {
        announcementText.text = "Time to deal damage!";
        announcementText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        announcementText.gameObject.SetActive(false);

        if (playerUnit.isCountering || enemyUnit.isCountering)
        {
            handleCounter();
        } else
        {
            int playerDmg = CalculateDamage(playerUnit, enemyUnit);
            int enemyDmg = CalculateDamage(enemyUnit, playerUnit);
            
            int dmgResult = playerDmg - enemyDmg;
            
            DealDamage(dmgResult);
        }

    }

    private void DealDamage(int dmgResult)
    {
        if (dmgResult > 0)
        {
            bool isDead = enemyUnit.TakeDamageAndCheckIfDead(dmgResult);
            enemyHud.SetHP(enemyUnit.currentHp);
            if (isDead)
            {
                state = BattleState.WON;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;
                StartCoroutine(PlayerTurn());
            }
        }
        else
        {
            bool isDead = playerUnit.TakeDamageAndCheckIfDead(Mathf.Abs(dmgResult));
            playerHud.SetHP(playerUnit.currentHp);
            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;
                StartCoroutine(PlayerTurn());
            }
        }
    }

    private void handleCounter()
    {
        if (playerUnit.isCountering && enemyUnit.isCountering) StartCoroutine(PlayerTurn());

        if (playerUnit.isCountering)
        {
            PlayerCounter();
        } else
        {
            EnemyCounter();
        }
    }

    private void EnemyCounter()
    {
        // Implement enemy counter logic
    }

    private void PlayerCounter()
    {
        if (isSuperEffective(playerUnit.getStance(), enemyUnit.getStance()))
        {
            // Must add attack modifiers calcs
            int totalAtk = enemyUnit.getCurrentDamage();
            int totalDef = enemyUnit.defense * enemyUnit.defenseMultiplier;
            // Need to figure out where this comes from
            int damageMultiplier = 2;
            double randomizedMultiplier = Random.Range(230, 255);
            double formula = (((totalAtk / totalDef) * damageMultiplier) * randomizedMultiplier) / 2.55;
            DealDamage(Mathf.RoundToInt((float)formula));
        }
        else
        {
            // Must add attack modifiers calcs
            int totalAtk = enemyUnit.getCurrentDamage();
            int totalDef = playerUnit.defense * playerUnit.defenseMultiplier;
            // Need to figure out where this comes from
            int damageMultiplier = 2;
            double randomizedMultiplier = Random.Range(2, 5);
            double formula = (((totalAtk / totalDef) * damageMultiplier) * randomizedMultiplier) / 2.55;
            DealDamage(Mathf.RoundToInt((float)formula) * -1);
        }
    }

    public void OnHighAttack()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        playerUnit.setStanceDamage(playerUnit.highDamage);
        playerUnit.setStance(PlayerAttackStance.HIGH);
        StartCoroutine(PlayerAttack());
    }

    public void OnMidAttack()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        playerUnit.setStanceDamage(playerUnit.midDamage);
        playerUnit.setStance(PlayerAttackStance.MID);
        StartCoroutine(PlayerAttack());
    }

    public void OnLowAttack()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }

        playerUnit.setStanceDamage(playerUnit.lowDamage);
        playerUnit.setStance(PlayerAttackStance.LOW);
        StartCoroutine(PlayerAttack());
    }

    public void OnHighCounter()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        playerUnit.setStanceDamage(playerUnit.highDamage);
        playerUnit.setStanceWithCounter(PlayerAttackStance.COUNTER_HIGH);
        StartCoroutine(PlayerAttack());
    }

    public void OnMidCounter()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }
        playerUnit.setStanceDamage(playerUnit.midDamage);
        playerUnit.setStanceWithCounter(PlayerAttackStance.COUNTER_MID);
        StartCoroutine(PlayerAttack());
    }

    public void OnLowCounter()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }

        playerUnit.setStanceDamage(playerUnit.lowDamage);
        playerUnit.setStanceWithCounter(PlayerAttackStance.COUNTER_LOW);
        StartCoroutine(PlayerAttack());
    }

    public void OnDefend()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        } 
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            announcementText.text = "You won!";
            announcementText.gameObject.SetActive(true);
            enemyUnit.gameObject.SetActive(false);
        } else if (state == BattleState.LOST)
        {
            announcementText.text = "You lost :(";
            announcementText.gameObject.SetActive(true);
            playerUnit.gameObject.SetActive(false);
        }
    }

    public int CalculateDamage(Unit attackingUnit, Unit defendingUnit)
    {
        float totalAtk = attackingUnit.getCurrentDamage() * defendingUnit.getResistanceValue(attackingUnit.getStance());
        int defendingUnitDef = defendingUnit.defense;
        // Flesh out multiplier
        int multiplier = isSuperEffective(attackingUnit.getStance(), enemyUnit.getStance()) ? 2 : 1;
        return Mathf.RoundToInt((float)(totalAtk * multiplier) - defendingUnitDef);
    }

    public bool isSuperEffective(PlayerAttackStance attackerStance, PlayerAttackStance defenderStance)
    {
        // High beats mid, mid beats low, low beats high. Counters are super effective against same level stances.
        switch(attackerStance)
        {
            case PlayerAttackStance.HIGH:
            case PlayerAttackStance.COUNTER_MID:
                if (defenderStance == PlayerAttackStance.MID) return true;
                return false;
            case PlayerAttackStance.MID:
            case PlayerAttackStance.COUNTER_LOW:
                if (defenderStance == PlayerAttackStance.LOW) return true;
                return false;
            case PlayerAttackStance.LOW:
            case PlayerAttackStance.COUNTER_HIGH:
                if (defenderStance == PlayerAttackStance.HIGH) return true;
                return false;
            default:
                return false;
        }
    }
}
