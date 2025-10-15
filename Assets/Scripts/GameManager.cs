using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<GameManager>();
            }
            return _instance;
        }
    }




    public AudioManager audioManager;
    [SerializeField]
    private GameObject audioManagerPrefab;

    private GameUIManager gameUIManager;
    
    public Player Player = new();

    public int Round = 1;
    public float EnemyMaxHealth = 20;
    public float EnemyHealth = 20;
    public float EnemyDamage = 1;

    void Start()
    {
        //setup audio
        var _ = FindObjectOfType<AudioManager>();
        if( _ == null)
        {
            audioManager = Instantiate(audioManagerPrefab).GetComponent<AudioManager>();
            
        }
        else
        {
            audioManager = FindObjectOfType<AudioManager>().GetComponent<AudioManager>();
        }
        
        Debug.Log(audioManager);
        audioManager.PlayMusic("MainMusic");
        


        //setup player
        Player.gameManager = Instance;
        Player.Moves = new List<Move> {
        };

        Player.Moves = new List<Move>();
        for(int i = 0; i < 10; i++)
        {
            Player.Moves.Add(new BasicAttack());
        }
        Player.ResetDrawPile();
        Player.Draw();

        //setup menu manager
        gameUIManager = GameUIManager.Instance;
        gameUIManager.UpdateAllStats();

        //setup enemy
        resetEnemy();
    }

    public void PlayHand()
    {
        Player.Play();
        Player.CurHp = MathF.Min(Player.MaxHp, Player.CurHp + Player.HpRegen);
        if (EnemyHealth <= 0)
        {
            Round++;
            resetEnemy();
            enterRest();
        }else
        {
            if (HelperFunctions.ReturnRandomIntInRange(0, 101, Player.Luck * 0.01f) > Player.DodgeChance)
            {
                Player.TakeDamage(EnemyDamage);
            }
            else
            {
                gameUIManager.Miss();
            }
        }
    }
    public void DiscardHand()   
    {
        Player.Discard();
    }

    public void DealDamage(float damage)
    {
        Enemy   Health -= damage;
    }

    
    private void resetEnemy()
    {
        EnemyMaxHealth += Round;
        EnemyHealth = EnemyMaxHealth;
        EnemyDamage = Mathf.Floor(Round * (1 + Round * 0.05f));
        gameUIManager.UpdateEnemy();
    }

    public void Die()
    {
        audioManager.PlaySound("Explosion");
        audioManager.MusicFilter(false);
        SceneManager.LoadScene("GameOver");
    }

    private void enterRest()
    {
        audioManager.PlaySound("Explosion");
        GameUIManager.Instance.EnterRest();
    }
}
