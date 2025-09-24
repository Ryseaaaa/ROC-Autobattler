using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Player
{
    public int MaxHp = 100;
    public float CurHp = 100;
    public float HpRegen = 1;

    public int MaxHandSize = 6;
    public int MaxSelectedSize = 3;

    public int Discards = 3;
    



    public int CritChance = 10;
    public int DodgeChance = 0;

    public int Luck = 0;

    public GameManager gameManager;
    
    /// <summary>
    /// All moves in deck
    /// </summary>
    public List<Move> Moves = new();
    
    /// <summary>
    /// Moves that have not yet been drawn
    /// </summary>
    public List<Move> DrawPile = new();
    
    /// /// <summary>
    /// Moves in current hand
    /// </summary>
    public List<Move> Hand = new();
    
    /// /// <summary>
    /// Moves that are currently selected
    /// </summary>
    public List<Move> Selected = new();
    
    /// <summary>
    /// Moves that have been discarded or played
    /// </summary>
    public List<Move> DiscardPile = new();
    

    


    public bool Select(int moveIndex)
    {
        Debug.Log("Selected Move Index = " + moveIndex);
        Debug.Log("Hand has " + Hand.Count + " items"); 
        //hand doesnt have same size??
        if (Selected.Contains(Hand[moveIndex]))
        {
            Selected.Remove(Hand[moveIndex]);
            return false;
        }
        else if (Selected.Count < MaxSelectedSize)
        {
            Selected.Add(Hand[moveIndex]);
            return true;
        }
        return false;
        
    }

    public void TakeDamage(float _damage) 
    {
        CurHp -= _damage;
        GameUIManager.Instance.UpdateStat("Hp",CurHp + "/" + MaxHp);
        GameUIManager.Instance.Gravity();
        if(CurHp <= 0) 
        {
            gameManager.Die();
        }
    }
    public void Heal(float _amount)
    {
        CurHp = MathF.Min(MaxHp, (CurHp + _amount));
        GameUIManager.Instance.UpdateStat("Hp", CurHp + "/" + MaxHp);
    }

    public void Play()
    {
        List<Move> playedMoves = new List<Move>();
        foreach (Move move in Selected)
        {
            playedMoves.Add(move);
            
        }
        Selected.Clear();
        int _moveIndex = 0;
        foreach (Move move in playedMoves)
        {
            move.OnPlay(new HandContext(gameManager, _moveIndex));

            Hand.Remove(move);

            _moveIndex++;

        }
        if(DrawPile.Count == 0 && Hand.Count == 0)
        {
            ResetDrawPile();
        }
        Draw();
        Debug.Log("Move pile size is now " + Moves.Count);
    }
    public void Discard()
    {
        if(Discards > 0) 
        {
            List<Move> discardedMoves = new List<Move>();
            foreach (Move move in Selected)
            {
                discardedMoves.Add(move);
            }
            Selected.Clear();
            int _moveIndex = 0;
            foreach (Move move in discardedMoves)
            {
                move.OnDiscard(new HandContext(gameManager, _moveIndex));
                Hand.Remove(move);
                _moveIndex++;
            }
        }
    }

    public void ResetDrawPile()
    {
        Debug.Log("Moves list has " + Moves.Count + " moves");
        foreach ( Move move in Moves)
        {
            DrawPile.Add(move);
        }
        foreach (Move move in Hand)
        {
            DrawPile.Remove(move);
        }
        DrawPile.Shuffle();
        Debug.Log("Moves list has " + Moves.Count + " moves");
        Debug.Log("Reset draw pile, now has " + DrawPile.Count + " moves");
    }

    public void Draw()
    {
        int toDraw = MaxHandSize - Hand.Count;
        Debug.Log("Attempting to draw " + toDraw + " moves");
        if (Moves.Count < toDraw)
        {
            toDraw = DrawPile.Count;
        }
        
        if (DrawPile.Count < toDraw)
        {
            Debug.Log("Not enough in draw pile");

            for (int i = 0; i < DrawPile.Count; i++)
            {
                DrawPile.MoveTo(0, Hand);
            }
            ResetDrawPile();
            toDraw = DrawPile.Count;
            Debug.Log("Moves in hand is now " + Hand.Count);
            Debug.Log("Reset Pile and drawing " + toDraw + " more moves");
            Debug.Log("Drawpile size = " + DrawPile.Count);
        } 
        
        for (int i = 0; i < toDraw; i++)
        {
            DrawPile.MoveTo(0, Hand);
        }
        


        
        
        Debug.Log("Moves in hand is now " + Hand.Count);

        GameUIManager.Instance.SetMovesInHand(Hand);
    }
}
