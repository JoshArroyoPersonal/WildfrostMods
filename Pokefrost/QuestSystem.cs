using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Tables;

namespace Pokefrost
{
    public abstract class QuestSystem : GameSystem
    {
        public int progress = 0;

        public static string Key_GeneralFail = "websiteofsites.wildfrost.pokefrost.generalquestfailed";
        public virtual string ProgressName => "Quest";

        public virtual string GetFailureText(string key)
        {
            StringTable ui = LocalizationHelper.GetCollection("UI Text", SystemLanguage.English);
            return ui.GetString(key).GetLocalizedString(); 
        }

        [PokeLocalizer]
        public static void DefineStrings()
        {
            LocalizationHelper.GetCollection("UI Text", SystemLanguage.English).SetString(Key_GeneralFail, "Conditions not met...");
        }

        public virtual void UpdateProgress(int value)
        {
            progress = value;
            EventSaveSystem.Add(ProgressName, value);
        }

        public virtual void FindProgress()
        {
            int value = EventSaveSystem.Get(ProgressName);
            if (value != -1)
            {
                progress = value;
            }
            else
            {
                UpdateProgress(progress);
            }
        }

        public abstract bool CheckConditions(out string failureText);

        public virtual void QuestBattleStart() { }

        public virtual void QuestBattleFinish() { }
    }

    public class SpawnCressliaModifierSystem : QuestSystem
    {
        public override string ProgressName => "Dreams";
        public static string Key_Cresselia => "websiteofsites.wildfrost.pokefrost.cresseliaHurt";

        [PokeLocalizer]
        public static new void DefineStrings()
        {
            LocalizationHelper.GetCollection("UI Text", SystemLanguage.English).SetString(Key_Cresselia, "Cresselia is too injured...");
        }

        //Progress KEY
        //0 -> Cresselia is alive. Darkrai battle has not occurred.
        //1 -> Cresselia is dead. Darkrai battle will be skipped.
        //2 -> Darkrai battle has been finished.
        public void OnEnable()
        {
            Events.OnBattleStart += Spawn;
            Events.OnEntityKilled += CheckCresseliaAlive;
            FindProgress();
        }

        public void OnDisable()
        {
            Events.OnBattleStart -= Spawn;
            Events.OnEntityKilled -= CheckCresseliaAlive;
        }

        private void CheckCresseliaAlive(Entity entity, DeathType deathType)
        {
            if (entity?.data?.name != "websiteofsites.wildfrost.pokefrost.quest_cresselia")
            {
                return;
            }
            foreach (Entity card in References.Battle.cards)
            {
                if (card?.data?.name == "websiteofsites.wildfrost.pokefrost.quest_cresselia" && card.IsAliveAndExists())
                {
                    return;
                }
            }
            UpdateProgress(1);
        }

        private void Spawn()
        {
            if (progress == 0)
            {
                StartCoroutine(TrueSpawn());
            }
        }

        private IEnumerator TrueSpawn()
        {
            CardContainer slot = Battle.instance.GetRows(References.Player).RandomItem();
            Debug.Log("[Pokefrost] Got Slot");
            CardData data = Pokefrost.instance.Get<CardData>("quest_cresselia").Clone();
            Debug.Log("[Pokefrost] Got Data");
            Card card = CardManager.Get(data, References.Battle.playerCardController, References.Player, inPlay: true, isPlayerCard: true);
            Debug.Log("[Pokefrost] Got Card");
            card.entity.flipper.FlipDownInstant();
            card.transform.localPosition = new Vector3(-15f, 0f, 0f);
            yield return card.UpdateData();
            slot.Add(card.entity);
            Debug.Log("[Pokefrost] Added to Slot");
            slot.TweenChildPositions();
            ActionQueue.Add(new ActionReveal(card.entity));
            ActionQueue.Add(new ActionRunEnableEvent(card.entity));
            yield return ActionQueue.Wait();
        }

        public override bool CheckConditions(out string failureText)
        {
            failureText = GetFailureText(Key_Cresselia);
            Debug.Log($"[Pokefrost] Checking Progress... {progress}");
            return (progress == 0);
        }

        public override void QuestBattleFinish()
        {
            UpdateProgress(2);
        }

    }

    public class TicketTimerModifierSystem : QuestSystem
    {
        public override string ProgressName => "TickTock";

        public static string Key_Slow = "websiteofsites.wildfrost.pokefrost.tooslow";

        [PokeLocalizer]
        public static new void DefineStrings()
        {
            LocalizationHelper.GetCollection("UI Text", SystemLanguage.English).SetString(Key_Slow, "The ship has departed...");
        }

        Timer timer;

        //Progress KEY
        //0 -> The ship has sailed
        //ELSE -> Time flies like an arrow...
        public void OnEnable()
        {
            Events.OnBattlePreTurnStart += StartTimer;
            Events.OnBattleTurnEnd += ReadTime;
            Events.OnBattleEnd += PauseTimer;
            progress = 15 * 60;
            FindProgress();
            if (progress > 0)
            {
                MakeTimer();
            }
        }

        public void OnDisable()
        {
            Events.OnBattlePreTurnStart -= StartTimer;
            Events.OnBattleTurnEnd -= ReadTime;
            Events.OnBattleEnd -= PauseTimer;
            if (timer)
            {
                UpdateProgress((int)timer.Time);
                timer.End();
            }
            else
            {
                UpdateProgress(0);
            }
        }

        private void PauseTimer()
        {
            if (!timer) { progress = 0; return; }
            timer.Stop();
            ReadTime(420);
        }

        private void StartTimer(int _)
        {
            if (!timer) { progress = 0; return; }
            timer.Play();
        }

        private void ReadTime(int _)
        {
            if (!timer) { progress = 0; return; }
            UpdateProgress((int)timer.Time);
        }

        public void MakeTimer()
        {
            if (!timer)
            {
                timer = Timer.Create(progress);
            }
            timer.SetTime(progress);
        }

        public override bool CheckConditions(out string failureText)
        {
            failureText = GetFailureText(Key_Slow);
            Debug.Log($"[Pokefrost] Checking Progress... {progress}");
            return (progress > 0);
        }

        public override void QuestBattleStart()
        {
            timer.End();
        }

        public override void QuestBattleFinish()
        {
            UpdateProgress(0);
        }
    }
}
