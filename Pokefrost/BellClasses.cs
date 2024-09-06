using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace Pokefrost
{
    //This class contains scripts and modifier systems for bells
    public class BounceJuiceModifierSystem : GameSystem
    {
        public void OnEnable()
        {
            StatusEffectSpicune.OnJuiceCleared += BounceJuice;
        }

        public void OnDisable()
        {
            StatusEffectSpicune.OnJuiceCleared -= BounceJuice;
        }

        private void BounceJuice(Entity entity, int amount)
        {
            if (Battle.instance == null) { return; }

            List<Entity> list = Battle.GetCards(References.Player);
            TargetConstraintCanBeBoosted canBeBoosted = ScriptableObject.CreateInstance<TargetConstraintCanBeBoosted>();
            foreach(Entity e in list.InRandomOrder())
            {
                if (canBeBoosted.Check(e) && e != entity)
                {
                    ActionQueue.Stack(new ActionApplyStatus(e, null, Pokefrost.instance.Get<StatusEffectData>("Spicune"), 1));
                    break;
                }
            }
        }
    }

    public class ScriptRunScriptsOnDeckAlt : Script
    {
        [SerializeField]
        public CardScript[] scripts;

        [SerializeField]
        public TargetConstraint[] constraints;

        [SerializeField]
        public Vector2Int countRange;

        [SerializeField]
        public bool includeReserve;

        public override IEnumerator Run()
        {
            List<CardData> list = new List<CardData>();
            AddRangeIfConstraints(list, References.PlayerData.inventory.deck, constraints);
            if (includeReserve)
            {
                AddRangeIfConstraints(list, References.PlayerData.inventory.reserve, constraints);
            }

            if (list.Count > 0)
            {
                Affect(list);
            }

            yield break;
        }

        public static void AddRangeIfConstraints(ICollection<CardData> collection, CardDataList toAdd, TargetConstraint[] constraints)
        {
            foreach (CardData item in toAdd)
            {
                AddIfConstraints(collection, item, constraints);
            }
        }

        public static void AddIfConstraints(ICollection<CardData> collection, CardData item, TargetConstraint[] constraints)
        {
            if (!constraints.Any((TargetConstraint c) => !c.Check(item)))
            {
                collection.Add(item);
            }
        }

        public void Affect(IReadOnlyCollection<CardData> cards)
        {
            int num = countRange.Random();
            Debug.Log("[" + base.name + "] Affecting [" + string.Join(", ", cards) + "]");
            foreach (CardData item in cards.InRandomOrder())
            {
                foreach (CardScript item2 in scripts)
                {
                    item2.Run(item);
                }

                num--;
                if (num <= 0)
                {
                    break;
                }
            }
        }
    }
}
