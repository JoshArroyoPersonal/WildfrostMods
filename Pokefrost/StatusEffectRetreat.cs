using Deadpan.Enums.Engine.Components.Modding;
using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Tables;

namespace Pokefrost
{
    internal class StatusEffectRetreat : StatusEffectInstant
    {

        public static readonly string Key_TooBig = "websiteofsites.wildfrost.pokefrost.toobig";

        [PokeLocalizer]
        public static void DefineStrings()
        {
            StringTable tooltips = LocalizationHelper.GetCollection("Tooltips", SystemLanguage.English);
            tooltips.SetString(Key_TooBig, "Target Too Big!");
        }

        public virtual void PopupText(Entity entity, string s)
        {
            NoTargetTextSystem noText = GameSystem.FindObjectOfType<NoTargetTextSystem>();
            if (noText != null)
            {
                TMP_Text textElement = noText.textElement;
                StringTable tooltips = LocalizationHelper.GetCollection("Tooltips", SystemLanguage.English);
                textElement.text = tooltips.GetString(s).GetLocalizedString();
                noText.PopText(entity.transform.position);
            }
        }

        public static CardStack FindEnemyReserveStack() 
        {
            foreach (CardStack cardStack in GameObject.FindObjectsOfType<CardStack>(true)) 
            {
                if (cardStack.name == "Enemy Reserve Stack")
                {
                    return cardStack;
                }
            }
            return null;
        }

        public static WaveDeploySystem FindWaveSystem()
        {

            foreach (WaveDeploySystem waveSys in GameObject.FindObjectsOfType<WaveDeploySystem>(true))
            {
                if (waveSys.name == "WaveDeployer")
                {
                    return waveSys;
                }
            }
            return null;
        }

        public static WaveDeploySystemOverflow FindWaveSystemOverflow()
        {

            foreach (WaveDeploySystemOverflow waveSys in GameObject.FindObjectsOfType<WaveDeploySystemOverflow>(true))
            {
                if (waveSys.name == "WaveDeployerOverflow")
                {
                    return waveSys;
                }
            }
            return null;
        }

        public static void FailSafe(int _)
        {
            List<Entity> list = Battle.GetCardsOnBoard(Battle.GetOpponent(References.Player));
            if (list == null || list.Count > 0) { return; }

            CardStack stack = FindEnemyReserveStack();
            if (stack == null) { return; }

            if (stack.Count > 0)
            {
                WaveDeploySystemOverflow overSys = FindWaveSystemOverflow();
                if (overSys == null || overSys.currentWave < overSys.waves.Count) { return; }

                FixWaves(stack.entities.Clone().ToArray());
                return;
            }

            Battle.instance.winner = References.Player;
            Battle.instance.phase = Battle.Phase.End;
        }

        public static bool FixWaves(params Entity[] entities)
        {
            
            if (entities.Count() == 0)
            {
                return false;
            }

            WaveDeploySystem waveSys = FindWaveSystem();
            WaveDeploySystemOverflow overSys = FindWaveSystemOverflow();

            if (waveSys == null || overSys == null)
            {
                return false;
            }

            bool flag1 = false;
            bool flag2 = false;
            if (overSys.currentWave >= overSys.waves.Count)
            {
                flag1 = true;
            }

            if (!flag1)
            {
                BattleWaveManager.Wave nextWave = overSys.waves[overSys.currentWave];
                if (nextWave.units.Count() > 5) 
                {
                    flag2 = true;
                }
                else
                {
                    overSys.deployed.Remove(entities[0].data.id);
                    nextWave.units.Add(entities[0].data);
                }
            }

            if (flag1 || flag2)
            {
                overSys.Overflow(entities);
                overSys.deployed.Remove(entities[0].data.id);
                if (flag1)
                {
                    overSys.SetCounter(overSys.waves[overSys.currentWave].counter);
                    overSys.Show();
                }
            }
            

            return true;

        }

        public override IEnumerator Process()
        {

            if (target.height > 1)
            {
                PopupText(target, Key_TooBig);
            }

            else
            {
                CardStack cardStack = FindEnemyReserveStack();

                if (cardStack != null)
                {
                    if (FixWaves(target))
                    {
                        yield return Sequences.CardMove(target, new CardContainer[] { cardStack });
                    }

                }
            }

            yield return base.Process();
        }

    }
}
