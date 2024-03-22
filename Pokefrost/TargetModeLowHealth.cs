using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    public class TargetModeLowHealth : TargetMode
    {
        public override Entity[] GetPotentialTargets(Entity entity, Entity target, CardContainer targetContainer)
        {
            HashSet<Entity> hashSet = new HashSet<Entity>();
            if ((bool)targetContainer)
            {
                if (targetContainer.Count > 0)
                {
                    hashSet.Add(GetTarget(targetContainer));
                }
            }
            else if ((bool)target)
            {
                if (target.containers.Length != 0)
                {
                    CardContainer cardContainer = target.containers.RandomItem();
                    if (cardContainer.Count > 0)
                    {
                        hashSet.Add(GetTarget(cardContainer));
                    }
                }
            }
            else
            {
                int[] rowIndices = Battle.instance.GetRowIndices(entity);
                int[] array = rowIndices;
                foreach (int rowIndex in array)
                {
                    AddTargets(entity, hashSet, rowIndex);
                }

                if (hashSet.Count == 0)
                {
                    int rowCount = Battle.instance.rowCount;
                    for (int j = 0; j < rowCount; j++)
                    {
                        if (!rowIndices.Contains(j))
                        {
                            AddTargets(entity, hashSet, j);
                        }
                    }
                }
            }

            if (hashSet.Count <= 0)
            {
                return null;
            }

            return hashSet.ToArray();
        }

        public override CardSlot[] GetTargetSlots(CardSlotLane row)
        {
            return new CardSlot[1] { row.slots[row.max - 1] };
        }

        public override bool CanTarget(Entity entity)
        {
            bool flag = false;
            CardContainer[] containers = entity.containers;
            foreach (CardContainer cardContainer in containers)
            {
                flag = true;
                for (int j = cardContainer.IndexOf(entity) + 1; j < cardContainer.max; j++)
                {
                    if ((bool)cardContainer[j] && cardContainer[j].canBeHit)
                    {
                        flag = false;
                        break;
                    }
                }

                if (flag)
                {
                    break;
                }
            }

            return flag;
        }

        public void AddTargets(Entity entity, HashSet<Entity> targets, int rowIndex)
        {
            List<Entity> enemiesInRow = entity.GetEnemiesInRow(rowIndex);
            Entity target = GetTarget(enemiesInRow);
            if ((bool)target)
            {
                targets.Add(target);
                return;
            }

            target = GetEnemyCharacter(entity);
            if ((bool)target)
            {
                targets.Add(target);
            }
        }

        public Entity GetTarget(IList<Entity> targets)
        {
            int lowest = 1000000000;
            Entity truetarget = null;
            for (int num = 0; num < targets.Count; num++)
            {
                Entity entity = targets[num];
                if ((bool)entity && entity.enabled && entity.alive && entity.canBeHit)
                {
                    if(lowest > entity.hp.current)
                    {
                        lowest = entity.hp.current;
                        truetarget = entity;
                    }
                }
            }

            return truetarget;
        }
    }
}
