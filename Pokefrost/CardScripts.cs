using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    public class CardScriptForsee : CardScript
    {
        public static List<int> ids = new List<int>();
        public override void Run(CardData target)
        {
            if (target.name == "websiteofsites.wildfrost.pokefrost.natu" && Campaign.instance != null)
            {
                Debug.Log("[Pokefrost] Forseeing...");
                int id = Campaign.FindCharacterNode(References.Player).id;
                foreach (CampaignNode node in Campaign.instance.nodes)
                {
                    if (node.type.isBattle && node.id >= id)
                    {
                        id = node.id;
                        break;
                    }
                }
                foreach (CampaignNode node in Campaign.instance.nodes.InRandomOrder())
                {
                    Debug.Log($"[Pokefrost] {node.type.name}-{node.id}");
                    if (node.id <= id)
                    {
                        continue;
                    }
                    if (node.type.name == "CampaignNodeItem" || node.type.name == "CampaignNodeCurseItems")
                    {
                        string[] collection = node.data.Get<SaveCollection<string>>("cards").collection;
                        foreach(string item in collection.InRandomOrder())
                        {
                            if (Pokefrost.rotomAppliances.Contains(item)) { continue; }
                            target.SetCustomData("Future Sight", item);
                            target.TryGetCustomData("Future Sight", out string value, "");
                            Debug.Log($"[Pokefrost] Foresaw {value}");
                            target.SetCustomData("Future Sight ID", node.id);
                            ids.Add(node.id);
                            break;
                        }
                        
                    }
                    if (node.type.name == "CampaignNodeShop")
                    {
                        bool falg = false;
                        ShopRoutine.Data data2 = node.data.Get<ShopRoutine.Data>("shopData");
                        foreach (int item in data2.items.GetIndices().InRandomOrder())
                        {
                            if (data2.items[item].category == "Items")
                            {
                                target.SetCustomData("Future Sight ID", node.id);
                                target.SetCustomData("Future Sight", data2.items[item].cardDataName);
                                target.TryGetCustomData("Future Sight", out string value, "");
                                Debug.Log($"[Pokefrost] Foresaw {value}");
                                falg = true;
                                ids.Add(node.id);
                                break;
                            }
                        }
                        if (falg)
                        {
                            break;
                        }
                    }
                }

            }
        }
    }

    public abstract class EntityCardScript : ScriptableObject
    {
        public abstract IEnumerator Run(Entity entity, int stack);
    }

    public class EntityCardScriptSwapTraits : EntityCardScript
    {
        protected TraitData traitA;

        protected TraitData traitB;

        public override IEnumerator Run(Entity target, int _)
        {
            int origStackA = 0;
            int origStackB = 0;
            foreach(Entity.TraitStacks stacks in target.traits)
            {
                if (stacks.data.name == traitA.name)
                {
                    origStackA = stacks.count - stacks.tempCount;
                    stacks.count -= origStackA;
                }
                if (stacks.data.name == traitB.name)
                {
                    origStackB = stacks.count - stacks.tempCount;
                    stacks.count -= origStackB;
                }
            }

            target.GainTrait(traitA, origStackB, temporary: false);
            target.GainTrait(traitB, origStackA, temporary: false);
            yield return target.UpdateTraits();

        }

        public static EntityCardScriptSwapTraits Create(TraitData traitA, TraitData traitB)
        {
            EntityCardScriptSwapTraits script = ScriptableObject.CreateInstance<EntityCardScriptSwapTraits>();
            script.traitA = traitA;
            script.traitB = traitB;
            return script;
        }
    }
}
