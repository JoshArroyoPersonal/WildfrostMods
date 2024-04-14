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
    internal class CampaignNodeTypeBetterEvent : CampaignNodeType
    {
        [SerializeField]
        public GameObject Prefab;

        public override IEnumerator Run(CampaignNode node)
        {
            yield return Transition.To("Event");
            GameObject gameObject = GameObject.Instantiate(Prefab, GameObject.FindObjectOfType<EventManager>(true).transform);
            Transition.End();
            /*yield return eventRoutine.Run();
            yield return Transition.To("MapNew");
            Transition.End();
            yield return MapNew.CheckCompanionLimit();*/
        }

        public virtual IEnumerator Populate(CampaignNode node)
        {
            return null;
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
