using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//Class that handels all UI
public class GameUIManager : MonoBehaviour
{

    private static GameUIManager _instance;
    // get instance
    public static GameUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<GameUIManager>();
            }
            return _instance;
        }
    }

    private GameManager gameManager
    {
        get { return GameManager.Instance; }
    }

    [SerializeField]
    private GameObject battleUI;
    [SerializeField]
    private GameObject choiceUI;
    [SerializeField]
    private GameObject rerollButton;
    [SerializeField]
    private List<GameObject> stats;
    [SerializeField]
    private GameObject RoundNumber;
    [SerializeField]
    private GameObject enemyHealth;
    [SerializeField]
    private GameObject enemyDamage;

    [SerializeField]
    private GameObject hand;
    [SerializeField]
    private GameObject movePrefab;
    private GameObject[] moves;


    [SerializeField]
    private Color SelectedColor;
    [SerializeField]
    private Color UnselectedColor;

    //update the UI to enter the game
    public void EnterGame()
    {
        UnselectAll();
        GameManager.Instance.Player.Hand.Clear();
        GameManager.Instance.Player.ResetDrawPile();
        GameManager.Instance.Player.Draw();
        battleUI.SetActive(true);
        choiceUI.SetActive(false);
    }

    //update the UI to enter choice reward
    public void EnterChoice()
    {
        rerolls = maxRerolls;
        SetUpgrades();
        rerollButton.GetComponent<Image>().color = Color.white;
        rerollButton.GetComponentInChildren<TextMeshProUGUI>().text = "Reroll! (" + rerolls + "/" + maxRerolls + ")";
        battleUI.SetActive(false);
        choiceUI.SetActive(true);
    }

    




    void Start()
    {
        EnterGame();
    }



    // Selects a single card
    public void SelectCard(int index)
    {
        if (gameManager.Player.Select(index))
            moves[index].gameObject.GetComponent<Image>().color = SelectedColor;

        else moves[index].gameObject.GetComponent<Image>().color = UnselectedColor;
    }

    // Check if move is selected
    public bool isSelected(int index)
    {
        if (gameManager.Player.Selected.ContainsKey(index))
        {
            return true;
        }
        return false;
    }

    // Unselects all moves
    public void UnselectAll()
    {
        for (int i = 0; i < moves.Length; i++)
        {
            if (isSelected(i))
            {
                SelectCard(i);
            }
        }
    }

    // Plays the currently selected moves, if possible (Called by button)
    public void PlayHand()
    {
        if(gameManager.Player.Selected.Count != 0)
        {
            gameManager.PlayHand();
            UpdateEnemy();
        }
    }


    // Makes the moves buttons that can be clicked to select them
    public void SetMovesInHand(List<Move> _hand)
    {
        /*if ((lastHandSize != gameManager.Player.MaxHandSize || moves == null) || true) //case handsize changed
        {*/
        // Gettin rid of this if statement makes it work but i cant be bothered to figure out why yet
        if (moves != null) {
            foreach (GameObject move in moves)
            {
                Destroy(move);
            }
        }

        //create new moves
        int index = 0;
        moves = new GameObject[_hand.Count];
        //determine placement offset
        float maxX = hand.GetComponent<RectTransform>().rect.width * 0.5f - movePrefab.GetComponent<RectTransform>().rect.width - 20;
        float minX = -maxX;
        foreach (Move move in _hand)
        {
            
            int count = _hand.Count;
            float thisX = Mathf.Lerp(minX, maxX, (float)index / (count - 1));
            thisX *= (float)Screen.width / 1920;
            if(count == 1)
            {
                thisX = (minX + maxX) / 2;
            }

            //instantiate
            moves[index] = Instantiate(movePrefab, hand.transform);
            moves[index].transform.Translate(new Vector3(thisX, 0, 0));

            //set correct color
            moves[index].gameObject.GetComponent<Image>().color = UnselectedColor;

            //set function
            var i = index;
            moves[index].GetComponent<Button>().onClick.AddListener(delegate { SelectCard(i); });



            //set text
            moves[index].transform.Find("MoveTitle").GetComponent<TextMeshProUGUI>().SetText(move.Title);
            moves[index].transform.Find("MoveDescription").GetComponent<TextMeshProUGUI>().SetText(move.GetDisplayText().ToString());
            index++;
        }
    }
    /*
        else //case handsize same
        {
            int index = 0;
            foreach (Move move in _hand)
            {
                moves[index].transform.Find("MoveTitle").GetComponent<TextMeshProUGUI>().SetText(move.Title);
                moves[index].transform.Find("MoveDescription").GetComponent<TextMeshProUGUI>().SetText(move.GetDisplayText().ToString());
            }
        
        }
    */


    // Update enemy's displayed stats
    public void UpdateEnemy()
    {
        enemyHealth.GetComponent<TextMeshProUGUI>().SetText("Health: " + gameManager.EnemyHealth + "/" + gameManager.EnemyMaxHealth);
        enemyDamage.GetComponent<TextMeshProUGUI>().SetText("Damage: " + gameManager.EnemyDamage);
    }


    //Makes a text popup whenever a move is played
    public void PopupAtMove(string text, int movePos, bool isCrit)
    {

        // Debug.Log("Move popup with text: " + text);
        // Debug.Log("Pos: " + movePos + " & Crit: " + isCrit.ToString());
    }


    // Updates a single displayed stat
    public void UpdateStat(string _name, string _newvalue)
    {
        foreach (GameObject stat in stats)
        {
            if (stat.name == _name)
            {
                stat.GetComponent<TextMeshProUGUI>().text = _name + " = " + _newvalue;
                break;
            }

        }

    }

    public void UpdateAllStats() //Updates all the displayed Stats
    {
        UpdateStat("Hp", gameManager.Player.CurHp + "/" + gameManager.Player.MaxHp);
        UpdateStat("HpRegen", gameManager.Player.HpRegen.ToString());

        UpdateStat("CritChance", gameManager.Player.CritChance.ToString());
        UpdateStat("DodgeChance", gameManager.Player.DodgeChance.ToString());
        UpdateStat("Luck", gameManager.Player.Luck.ToString());
    }



    [SerializeField] private GameObject mnmPrefab;
    [SerializeField] private GameObject bananaPrefab;
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private Transform eminemeny;

    // Enemy animation when enemy hits you
    public void Gravity(float damage)
    {
        GameObject mnm = Instantiate(
            mnmPrefab,
            eminemeny.position,
            Quaternion.identity,
            transform.GetChild(0)
        );

        GameObject textpopupobject = Instantiate(
            textPrefab,
            new Vector2(mnm.transform.position.x, mnm.transform.position.y + 120f),
            Quaternion.identity,
            transform.GetChild(0)
        );
        TextPopup textpopup = textpopupobject.GetComponent<TextPopup>();
        textpopup.Text = "Took " + damage + " damage";
        textpopup.Lifespan = 3f;
        textpopup.Color = Color.white;

        if (GameManager.Instance.audioManager != null) {
            if (HelperFunctions.ReturnRandomBool(0.5f, 0))
            {
                mnm.GetComponent<Rigidbody2D>().gravityScale *= -1;
                GameManager.Instance.audioManager.PlaySound("Gravity");
            }
            else
            {
                GameManager.Instance.audioManager.PlaySound("ArmsAreHeavy");
            }
        }
    }
    // Enemy animation when enemy misses you
    public void Miss()
    {
        GameObject banana = Instantiate(
            bananaPrefab,
            eminemeny.position,
            Quaternion.identity,
            transform.GetChild(0)
        );
        GameObject textpopupobject = Instantiate(
            textPrefab,
            new Vector2(banana.transform.position.x, banana.transform.position.y + 120f),
            Quaternion.identity,
            transform.GetChild(0)
        );
        TextPopup textpopup = textpopupobject.GetComponent<TextPopup>();
        textpopup.Text = "Dodged!";
        textpopup.Lifespan = 3f;
        textpopup.Color = Color.yellow;
        GameManager.Instance.audioManager.PlaySound("Miss");
    }

    private string statUpgrade;

    private int statUpgradeAmount = 0;

    private Move moveToAdd = new BasicAttack();
    private Move moveToRemove;

    // Sets upgrade options
    public void SetUpgrades()
    {
        //Determine stats to select from
        List<string> statsUpgradeable = new List<string>()
        {
            "MaxHp",
            "HpRegen",
            "Luck"
        };
        if(GameManager.Instance.Player.CritChance < 100)
        {
            statsUpgradeable.Add("CritChance");
        }
        if(GameManager.Instance.Player.DodgeChance < 50)
        {
            statsUpgradeable.Add("DodgeChance");
        }
        
        // Add Move
        int i = HelperFunctions.ReturnRandomIntInRange(0, 3,0);
        switch (i)
        {
            case 0:
                moveToAdd = new StrongAttack();
                break;
            case 1:
                moveToAdd = new DiceRoll();
                break; 
            case 2:
                moveToAdd = new BloodleachMove();
                break;
            default: 
                moveToAdd = new BasicAttack();
                break;
        }
        choiceUI.transform.Find("Choice1").transform.Find("Description").GetComponentInChildren<TextMeshProUGUI>().text = "Add " + moveToAdd.Title;

        // Upgrade Stat
        statUpgrade = statsUpgradeable[HelperFunctions.ReturnRandomIntInRange(0, statsUpgradeable.Count,0)];
        statUpgradeAmount = HelperFunctions.ReturnRandomIntInRange(10, 20 + gameManager.Player.Luck, 0);
        switch (statUpgrade)
        {
            case "CritChance":
            case "DodgeChance":
                statUpgradeAmount /= 2;
                choiceUI.transform.Find("Choice2").transform.Find("Description").GetComponentInChildren<TextMeshProUGUI>().text = "Upgrade " + statUpgrade + " by " + statUpgradeAmount + "%";
                break;
            case "Luck":
                statUpgradeAmount /= 2;
                choiceUI.transform.Find("Choice2").transform.Find("Description").GetComponentInChildren<TextMeshProUGUI>().text = "Upgrade " + statUpgrade + " by " + statUpgradeAmount;
                break;
            case "HpRegen":
                Debug.Log("Upgrade landed on HpRegen. Initial amount = " + statUpgradeAmount);
                statUpgradeAmount /= 4;
                Debug.Log("After division = " + statUpgradeAmount);
                choiceUI.transform.Find("Choice2").transform.Find("Description").GetComponentInChildren<TextMeshProUGUI>().text = "Upgrade " + statUpgrade + " by " + statUpgradeAmount;
                break;
            default:
                choiceUI.transform.Find("Choice2").transform.Find("Description").GetComponentInChildren<TextMeshProUGUI>().text = "Upgrade " + statUpgrade + " by " + statUpgradeAmount + "%";
                break;
        }

        // Remove Move
        moveToRemove = GameManager.Instance.Player.Moves[UnityEngine.Random.Range(0, GameManager.Instance.Player.Moves.Count)];
        choiceUI.transform.Find("Choice3").transform.Find("Description").GetComponentInChildren<TextMeshProUGUI>().text = "Remove " + moveToRemove.Title;
    }

    private int maxRerolls = 3;
    private int rerolls = 3;

    // Rerolls your upgrade options
    public void RerollUpgrades()
    {
        if (rerolls > 0) {
            rerolls--;
            rerollButton.GetComponentInChildren<TextMeshProUGUI>().text = "Reroll! (" + rerolls + "/" + maxRerolls+")";
            SetUpgrades();
            
            
        }
        if(rerolls<= 0)
        {
            rerollButton.GetComponent<Image>().color = Color.gray;
        }
    }
    
    // On the buttons to take the upgradeas
    public void TakeUpgrade(int index)
    {
        switch(index) //Switch statement for determining what upgrade the player chose
        {
            case 0: // If player chooses to add move, add move
                gameManager.Player.Moves.Add(moveToAdd); 
                break;
            case 1: // If player takes upgrade, update accordingly
                Func<int, float> upgrade = x => 1 + x * 0.01f; 
                int _ = 0;
                switch (statUpgrade)
                {
                    case "MaxHp":
                        _ = gameManager.Player.MaxHp;
                        gameManager.Player.MaxHp = Mathf.CeilToInt(gameManager.Player.MaxHp + upgrade(statUpgradeAmount));
                        gameManager.Player.CurHp += gameManager.Player.MaxHp - _;
                        break;
                    case "HpRegen":
                        Debug.Log("Decided to take hp regen. Cur amt = " + statUpgradeAmount);
                        Debug.Log("Player amt = " + gameManager.Player.HpRegen);
                        gameManager.Player.HpRegen = Mathf.CeilToInt(gameManager.Player.HpRegen + statUpgradeAmount);
                        break;
                    case "CritChance":
                        gameManager.Player.CritChance = Math.Min(100,gameManager.Player.CritChance + statUpgradeAmount);
                        break;
                    case "DodgeChance":
                        gameManager.Player.DodgeChance = Mathf.Min(50,Mathf.CeilToInt(gameManager.Player.DodgeChance + statUpgradeAmount));
                        break;
                    case "Luck":
                        gameManager.Player.Luck = Mathf.CeilToInt(gameManager.Player.Luck + statUpgradeAmount);
                        break;
                    default:
                        Debug.Log("Stat " + statUpgrade + " upgraded by " + statUpgradeAmount + "%");
                        break;
                }
                UpdateAllStats();
                break;
            case 2: // If player chooses to remove move, remove it, making sure to reset the drawpile too to avoid bugs.
                if (gameManager.Player.Hand.Contains(moveToRemove))
                {
                    gameManager.Player.Hand.Remove(moveToRemove);
                    SetMovesInHand(gameManager.Player.Hand);
                }
                gameManager.Player.Moves.Remove(moveToRemove);
                gameManager.Player.ResetDrawPile();
                break;

            default:
                
            break;
        }
        // After taking an upgrade, enter the battle again
        EnterGame();
    }
    public void UpdateRound(int curRound, int maxRound)
    {
        RoundNumber.GetComponentInChildren<TextMeshProUGUI>().text = "Round: " + curRound + "/" + maxRound;
    }
}
