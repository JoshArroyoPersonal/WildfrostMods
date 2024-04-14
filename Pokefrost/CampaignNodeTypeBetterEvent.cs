using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine;
using UnityEngine.UI;

namespace Pokefrost
{
    internal class CampaignNodeTypeBetterEvent : CampaignNodeTypeEvent
    {
        public string key = "Trade";
        public static Dictionary<string, GameObject> Prefabs = new Dictionary<string, GameObject>();

        public int choices = 3;
        public List<CardData> force;

        public override IEnumerator SetUp(CampaignNode node)
        {
            yield return null;
            CharacterRewards component = References.Player.GetComponent<CharacterRewards>();
            List<CardData> list = force.Clone();
            if (list.Count > 0)
            {
                component.PullOut("Units", list);
            }

            int itemCount = choices - list.Count;
            list.AddRange(component.Pull<CardData>(node, "Units", itemCount));
            node.data = new Dictionary<string, object>
        {
            {
                "cards",
                list.ToSaveCollectionOfNames()
            }
        };
        }

        public override IEnumerator Populate(CampaignNode node)
        {
            EventRoutineTrade eventRoutineCompanion = FindObjectOfType<EventRoutineTrade>();
            eventRoutineCompanion.node = node;
            yield return eventRoutineCompanion.Populate();
        }

        public override IEnumerator Run(CampaignNode node)
        {
            yield return Transition.To("Event");
            GameObject gameObject = GameObject.Instantiate(Prefabs[key], GameObject.FindObjectOfType<EventManager>(true).transform);
            gameObject.SetActive(true);
            EventRoutine eventRoutine = gameObject.GetComponent<EventRoutine>();
            Events.InvokeEventStart(node, eventRoutine);
            yield return Populate(node);
            Events.InvokeEventPopulated(eventRoutine);
            Transition.End();
            yield return eventRoutine.Run();
            yield return Transition.To("MapNew");
            Transition.End();
            yield return MapNew.CheckCompanionLimit();
        }

        public override IEnumerator SetUp(CampaignNode node)
        {
            node.data = new Dictionary<string, object> {
        {
            "amount",
            3
        } };
            yield return null;
        }

        public CampaignNodeTypeBetterEvent()
        {
        }
    }
}
