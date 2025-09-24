using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject battleUI;
    [SerializeField]
    private GameObject choiceUI;
    [SerializeField]
    private List<GameObject> stats;

    public void EnterGame()
    {
        UnselectAll();
        GameManager.Instance.Player.Hand.Clear();
        GameManager.Instance.Player.ResetDrawPile();
        GameManager.Instance.Player.Draw();
        battleUI.SetActive(true);
        choiceUI.SetActive(false);
    }

    public void EnterRest()
    {
        SetUpgrades();
        battleUI.SetActive(false);
        choiceUI.SetActive(true);
    }

    private static GameUIManager _instance;
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
    private GameObject enemyHealth;
    [SerializeField]
    private GameObject enemyDamage;

    [SerializeField]
    private GameObject hand;
    [SerializeField]
    private GameObject movePrefab;
    private GameObject[] moves;
    private int lastHandSize = 0;

    [SerializeField]
    private Color SelectedColor;
    [SerializeField]
    private Color UnselectedColor;

    void Start()
    {
        EnterGame();
    }



    public bool SelectCard(int index)
    {

        if (gameManager.Player.Select(index))
        {
            moves[index].gameObject.GetComponent<Image>().color = SelectedColor;
            return true;
        }
        else
        {
            moves[index].gameObject.GetComponent<Image>().color = UnselectedColor;
            return false;
        }

    }

    public void UnselectAll()
    {
        for (int i = 0; i < moves.Length; i++)
        {
            if (SelectCard(i))
            {
                SelectCard(i);
            }
        }
    }

    public void PlayHand()
    {
        if(gameManager.Player.Selected.Count != 0)
        {
            gameManager.PlayHand();
            UpdateEnemy();
            UnselectAll();
        }
    }
    public void DiscardHand()
    {
        if(gameManager.Player.Discards > 0 && gameManager.Player.Selected.Count > 0)
        {
            gameManager.DiscardHand();
            UnselectAll();
        }
    }

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
        lastHandSize = gameManager.Player.MaxHandSize;
        if (lastHandSize != _hand.Count)
        {
            Debug.Log("Hand size (player stat) doesnt match hand size (moves in hand)????");
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





    public void UpdateEnemy()
    {
        enemyHealth.GetComponent<TextMeshProUGUI>().SetText("Health: " + gameManager.EnemyHealth + "/" + gameManager.EnemyMaxHealth);
        enemyDamage.GetComponent<TextMeshProUGUI>().SetText("Damage: " + gameManager.EnemyDamage);
    }

    public void PopupAtMove(string text, int movePos, bool isCrit)
    {
        Debug.Log("Move popup with text: " + text);
        Debug.Log("Pos: " + movePos + " & Crit: " + isCrit.ToString());
    }

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

    public void UpdateAllStats()
    {
        UpdateStat("Hp", gameManager.Player.CurHp + "/" + gameManager.Player.MaxHp);
        UpdateStat("HpRegen", gameManager.Player.HpRegen.ToString());

        UpdateStat("CritChance", gameManager.Player.CritChance.ToString());
        UpdateStat("DodgeChance", gameManager.Player.DodgeChance.ToString());
        UpdateStat("Luck", gameManager.Player.Luck.ToString());
    }



    [SerializeField] private GameObject prefab;
    public void Gravity()
    {
        GameObject mnm = Instantiate(prefab);
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

    private string statUpgrade;

    private int statUpgradeAmount = 0;

    private Move moveToAdd = new BasicAttack();
    private Move moveToRemove = null;
    public void SetUpgrades()
    {
        //Determine stats to select from
        List<string> statsUpgradeable = new List<string>()
        {
            "MaxHp",
            "HpRegen",
            "DodgeChance",
            "Luck"
        };
        if(GameManager.Instance.Player.CritChance < 100)
        {
            statsUpgradeable.Add("CritChance");
        }
        
        // Add Move
        int i = HelperFunctions.ReturnRandomIntInRange(0, 3,0);
        switch (i)
        {
            case 0:
                moveToAdd = new BasicAttack();
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
        statUpgrade = statsUpgradeable[HelperFunctions.ReturnRandomIntInRange(0, statsUpgradeable.Count - 1,0)];
        statUpgradeAmount = HelperFunctions.ReturnRandomIntInRange(10, 20 + gameManager.Player.Luck, 0);
        switch (statUpgrade)
        {
            case "CritChance":
            case "DodgeChance":
                statUpgradeAmount /= 5;
                choiceUI.transform.Find("Choice2").transform.Find("Description").GetComponentInChildren<TextMeshProUGUI>().text = "Upgrade " + statUpgrade + " by " + statUpgradeAmount + "%";
                break;
            case "Luck":
                statUpgradeAmount /= 2;
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
    public void TakeUpgrade(int index)
    {
        switch(index)
        {
            case 0:
                gameManager.Player.Moves.Add(moveToAdd); 
                break;
            case 1:
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
                        gameManager.Player.HpRegen = Mathf.CeilToInt(gameManager.Player.HpRegen + upgrade(statUpgradeAmount));
                        break;
                    case "CritChance":
                        gameManager.Player.CritChance = Math.Max(100,gameManager.Player.CritChance + statUpgradeAmount);
                        break;
                    case "DodgeChance":
                        gameManager.Player.DodgeChance = Mathf.Max(50,Mathf.CeilToInt(gameManager.Player.DodgeChance + statUpgradeAmount));
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
            case 2:
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
        EnterGame();
    }
}
