using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokefrost
{
    internal class StatusEffectApplyXWhenCertainCardPlayed : StatusEffectApplyXOnTurn
    {
        public string cardName = "";
        public bool useCardName = true;
        public string customDataKey = "";
        public bool useCustomData = false;
        public override bool RunCardPlayedEvent(Entity entity, Entity[] targets)
        {
            if (!turnTaken && target.enabled && DesiredCard(entity) && Battle.IsOnBoard(target))
            {
                if (trueOnTurn)
                {
                    turnTaken = true;
                    return false;
                }

                return true;
            }

            return false;
        }

        protected virtual bool DesiredCard(Entity entity)
        {
            if (useCardName)
            {
                return (entity?.data?.name == cardName);
            }
            if (useCustomData)
            {
                target.data.TryGetCustomData<string>(customDataKey, out string value, "");
                return (entity?.data?.name == value);
            }
            return false;
        }
    }
}
