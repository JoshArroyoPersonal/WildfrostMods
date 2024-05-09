using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    public class StatusEffectMoveCard : StatusEffectInstant
    {
        public enum Containers
        {
            Hand,
            Draw,
            Discard
        }

        public Containers toWhere = Containers.Discard;

        public virtual CardContainer GetContainer()
        {
            if (toWhere == Containers.Hand)
            {
                return References.Player.handContainer;
            }
            else if (toWhere == Containers.Draw)
            {
                return References.Player.handContainer;
            }
            else if (toWhere == Containers.Discard)
            {
                return References.Player.discardContainer;
            }
            throw new Exception("Did you forget to declare a container when building the StatusEffect?");
        }

        public override IEnumerator Process()
        {
            yield return Sequences.CardMove(target, new CardContainer[] { GetContainer() });
            foreach (CardContainer c in target.preContainers)
            {
                c.TweenChildPositions();
            }
            yield return base.Process();
        }
    }
}
