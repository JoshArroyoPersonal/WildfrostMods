using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    internal class BattleGenerationScriptHooh : BattleGenerationScript
    {
        public override SaveCollection<BattleWaveManager.WaveData> Run(BattleData battleData, int points)
        {
            Debug.Log($"Creating Waves for [{battleData}]");
            List<BattleWavePoolData> list = new List<BattleWavePoolData>();
            int num = Mathf.RoundToInt((float)points * battleData.pointFactor);
            Debug.Log($"Points: {num}");

            WaveList waveList = new WaveList(num);
            BattleWavePoolData[] pools = battleData.pools.Select((p) => p.InstantiateKeepName()).ToArray();

            //Ho-oh
            waveList.Add(pools[0].Pull());

            //The rest
            int index = Dead.Random.Range(0, pools[1].waves.Length);
            pools = pools.Skip(1).ToArray();
            for(int i=0; i<3; i++)
            {
                waveList.Add(Concat(pools.Select(p => p.Pull()).ToArray()));
            }

            AddGoldGivers(waveList, battleData);
            AddBonusUnits(waveList, battleData);

            List<BattleWaveManager.WaveData> list2 = new List<BattleWaveManager.WaveData>();
            int count = waveList.Count;
            for (int k = 0; k < count; k++)
            {
                BattleWaveManager.WaveDataBasic waveDataBasic = new BattleWaveManager.WaveDataBasic
                {
                    counter = battleData.waveCounter
                };
                BattleWavePoolData.Wave wave = waveList.GetWave(k);
                List<string> list3 = new List<string>();
                foreach (CardData unit in wave.units)
                {
                    list3.Add(unit.name);
                    if (!waveDataBasic.isBossWave && unit.cardType.miniboss)
                    {
                        waveDataBasic.isBossWave = true;
                    }
                }

                waveDataBasic.cards = list3.Select((string a) => new BattleWaveManager.Card(a)).ToArray();
                list2.Add(waveDataBasic);
            }

            return new SaveCollection<BattleWaveManager.WaveData>(list2);
        }

        public BattleWavePoolData.Wave Concat(params BattleWavePoolData.Wave[] waves)
        {
            BattleWavePoolData.Wave theWave = new BattleWavePoolData.Wave();

            theWave.units = new List<CardData>(6);
            for (int i = 0; i < waves.Length; i++)
            {
                theWave.units.AddRange(waves[i].units);
                theWave.value += waves[i].value;
                theWave.positionPriority = Math.Max(theWave.positionPriority, waves[i].positionPriority);
                theWave.fixedOrder = true;
                theWave.maxSize += waves[i].maxSize;
            }
            return theWave;
        }
    }
}
