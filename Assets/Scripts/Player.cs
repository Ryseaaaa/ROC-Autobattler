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
    public Dictionary<int, Move> Selected = new();
    
    /// <summary>
    /// Moves that have been discarded or played
    /// </summary>
    public List<Move> DiscardPile = new();

    //called when move is selected
    public bool Select(int moveIndex)
    {
        if (Selected.ContainsKey(moveIndex))
        {
            Selected.Remove(moveIndex);
            return false;
        }
        else if (Selected.Count < MaxSelectedSize)
        {
            Selected.Add(moveIndex, Hand[moveIndex]);
            return true;
        }
        return false;
    }

    //called when player takes damage
    public void TakeDamage(float _damage) 
    {
        CurHp -= _damage;
        GameUIManager.Instance.UpdateStat("Hp",CurHp + "/" + MaxHp);
        GameUIManager.Instance.Gravity(_damage);
        if(CurHp <= 0) 
        {
            gameManager.Die();
        }
    }
    //called when player heals
    public void Heal(float _amount)
    {
        CurHp = MathF.Min(MaxHp, (CurHp + _amount));
        GameUIManager.Instance.UpdateStat("Hp", CurHp + "/" + MaxHp);
    }
    //called when player plays selected moves
    public void Play()
    {
        List<Move> playedMoves = new List<Move>();
        foreach (Move move in Selected.Values)
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
        if(DrawPile.Count == 0 && Hand.Count < MaxHandSize)
        {
            ResetDrawPile();
        }
        Draw();
    }

    // called when draw pile needs to be reset
    public void ResetDrawPile()
    {
        foreach ( Move move in Moves)
        {
            DrawPile.Add(move);
        }
        foreach (Move move in Hand)
        {
            DrawPile.Remove(move);
        }
        DrawPile.Shuffle();
    }

    //called when player draws new moves
    public void Draw()
    {
        int toDraw = MaxHandSize - Hand.Count;
        if (Moves.Count < toDraw)
        {
            toDraw = DrawPile.Count;
        }
        
        if (DrawPile.Count < toDraw)
        {
            for (int i = 0; i < DrawPile.Count; i++)
            {
                DrawPile.MoveTo(0, Hand);
            }
            ResetDrawPile();
            toDraw = Math.Min(MaxHandSize - Hand.Count, DrawPile.Count);
        } 
        
        
        for (int i = 0; i < toDraw; i++)
        {
            DrawPile.MoveTo(0, Hand);
        }

        GameUIManager.Instance.SetMovesInHand(Hand);
    }
}
