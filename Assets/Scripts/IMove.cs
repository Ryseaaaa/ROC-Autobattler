using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HandContext
{
    public HandContext(GameManager gameManager, int cardPlaceInHand)
    {
        GameManager = gameManager;
        CardPlaceInHand = cardPlaceInHand;
    }

    public GameManager GameManager { private set; get; }
    public int CardPlaceInHand { private set; get; }
}

#region BaseMove
public abstract class Move
{
    public void TakeUpgrade()
    {
    }

    public abstract int BaseDamage { get; set; }
    public abstract string Title { get; }

    public virtual void OnPlay(HandContext context)
    {
        bool isCrit = HelperFunctions.ReturnRandomBool(context.GameManager.Player.CritChance*0.01f, context.GameManager.Player.Luck*0.01f);
        int calculatedDamage = BaseDamage * (isCrit ? 2 : 1);
        context.GameManager.DealDamage(calculatedDamage);
        GameUIManager.Instance.PopupAtMove(calculatedDamage + " damage" + (isCrit ? "" : "!"), context.CardPlaceInHand, isCrit);
    }

    public virtual void OnDiscard(HandContext context)
    {

    }

    public virtual DisplayText GetDisplayText()
    {
        DisplayText text = new();
        text.AddText(BaseDamage + " damage");

        return text;
    }
}
#endregion

#region BasicAttack Move
public class BasicAttack : Move
{
    public override int BaseDamage { get { return 5; } set { } }
    public override string Title { get { return "Basic Attack"; } }

    public override void OnPlay(HandContext context)
    {
        bool isCrit = HelperFunctions.ReturnRandomBool(context.GameManager.Player.CritChance * 0.01f, context.GameManager.Player.Luck * 0.01f);
        int calculatedDamage = BaseDamage * (isCrit ? 2 : 1);
        context.GameManager.DealDamage(calculatedDamage);
        GameUIManager.Instance.PopupAtMove(calculatedDamage + " damage" + (isCrit ? "" : "!"), context.CardPlaceInHand, isCrit);
    }
    public override DisplayText GetDisplayText()
    {
        DisplayText text = new();
        text.AddText("Deals "+BaseDamage+" Damage");
        return text;
    }
}
#endregion

#region
public class StrongAttack : Move
{
    public override int BaseDamage { get { return 10; } set { } }
    public override string Title { get { return "Strong Attack"; } }
    public override void OnPlay(HandContext context)
    {
        bool isCrit = HelperFunctions.ReturnRandomBool(context.GameManager.Player.CritChance * 0.01f, context.GameManager.Player.Luck * 0.01f);
        int calculatedDamage = BaseDamage * (isCrit ? 2 : 1);
        context.GameManager.DealDamage(calculatedDamage);
        GameUIManager.Instance.PopupAtMove(calculatedDamage + " damage" + (isCrit ? "" : "!"), context.CardPlaceInHand, isCrit);
    }
    public override DisplayText GetDisplayText()
    {
        DisplayText text = new();
        text.AddText("Deals " + BaseDamage + " Damage");
        return text;
    }
}
    #endregion

    #region DiceRoll Move
    public class DiceRoll : Move
{
    private int baseDamage = 3;
    public override int BaseDamage { get { return baseDamage; } set { baseDamage = value; } }
    public override string Title { get { return "Dice Roll"; } }
    public override void OnPlay(HandContext context)
    {
        bool isCrit = HelperFunctions.ReturnRandomBool(context.GameManager.Player.CritChance * 0.01f, context.GameManager.Player.Luck * 0.01f);        
        int calculatedDamage = HelperFunctions.ReturnRandomIntInRange(0, BaseDamage + 1, (context.GameManager.Player.Luck) * (isCrit ? 1 + context.GameManager.Player.CritChance*0.01f : 1) * 0.01f);

        string maxRollText = "";
        if(calculatedDamage == BaseDamage)
        {
            BaseDamage++;
            maxRollText = " +1 max!";
        }
        context.GameManager.DealDamage(calculatedDamage);
        GameUIManager.Instance.PopupAtMove(calculatedDamage + " damage" + (isCrit ? "" : "!") + maxRollText, context.CardPlaceInHand, isCrit);
    }

    public override DisplayText GetDisplayText()
    {
        DisplayText text = new();
        text.AddText("0 to " + BaseDamage + " damage");
        text.NewLine();
        text.AddText("+1 dmg on max roll");
        text.NewLine();
        text.AddText("Crits increase only luck");
        

        
        return text;
    }

}
#endregion

#region Bloodleach Move
public class BloodleachMove : Move
{
    public override int BaseDamage { get { return 2; } set { } }
    public override string Title { get { return "Bloodleach"; } }
    public override void OnPlay(HandContext context)
    {
        bool isCrit = HelperFunctions.ReturnRandomBool(context.GameManager.Player.CritChance * 0.01f, context.GameManager.Player.Luck * 0.01f);
        int calculatedDamage = BaseDamage * 2;
        int calculatedHeal = BaseDamage * (isCrit ? 1 : 2);
        context.GameManager.DealDamage(calculatedDamage);
        context.GameManager.Player.Heal(calculatedHeal);
        GameUIManager.Instance.PopupAtMove("Dealt " + calculatedDamage + " & Healed " + calculatedHeal + (isCrit ? "" : "!") , context.CardPlaceInHand, isCrit);
    }
    public override void OnDiscard(HandContext context)
    {
        bool isCrit = HelperFunctions.ReturnRandomBool(context.GameManager.Player.CritChance * 0.01f, context.GameManager.Player.Luck * 0.01f);
        int calculatedHeal = isCrit ? 1 : BaseDamage;
        context.GameManager.Player.Heal(calculatedHeal);
        GameUIManager.Instance.PopupAtMove("Healed " + calculatedHeal + (isCrit ? "" : "!"), context.CardPlaceInHand, isCrit);
    }
    public override DisplayText GetDisplayText() 
    {
        DisplayText text = new();
        text.AddText("Deals " + 2 * BaseDamage + " damage");
        text.NewLine();
        text.AddText("Heals " + BaseDamage + " hp, or 2x on crit");

        return text;
    }
}
#endregion