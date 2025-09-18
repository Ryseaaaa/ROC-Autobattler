using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IOnPlayMoveEffect
{
    public void Mount(EventManager eventManager);
    public void OnTryPlayMoveEffect(Move move, HandContext context)
    {

    }
}
public class EventManager
{
    public List<IOnPlayMoveEffect> onTryPlayMoveEffects = new();
    public void OnTryPlayMove(Move move, HandContext context)
    {
        foreach (var effect in onTryPlayMoveEffects)
        {
            effect.OnTryPlayMoveEffect(move, context);
        }
    }

    public void OnTakeDamage() { }
}
