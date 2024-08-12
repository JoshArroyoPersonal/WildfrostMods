using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Pokefrost
{
    public class ButtonExt : Button
    {
        internal StatusIconExt Icon => GetComponent<StatusIconExt>();

        internal static ButtonExt dragBlocker = null;

        internal Entity Entity => Icon?.target;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            dragBlocker = this;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            DisableDragBlocking();
        }

        public void DisableDragBlocking()
        {
            if (dragBlocker == this)
            {
                dragBlocker = null;
            }
        }

        public static void DisableDrag(ref Entity arg0, ref bool arg1)
        {
            if (dragBlocker == null || arg0 != dragBlocker.Entity)
            {
                return;
            }
            arg1 = false;
        }
    }

    public interface IStatusToken
    {
        void ButtonCreate(StatusIconExt icon);

        void RunButtonClicked();

        IEnumerator ButtonClicked();


    }

    public class StatusIconExt : StatusIcon
    {
        public ButtonAnimator animator;
        public ButtonExt button;
        private IStatusToken effectToken;

        public override void Assign(Entity entity)
        {
            base.Assign(entity);
            SetText();
            onValueDown.AddListener(delegate { Ping(); });
            onValueUp.AddListener(delegate { Ping(); });
            afterUpdate.AddListener(SetText);
            onValueDown.AddListener(CheckDestroy);
            onDestroy.AddListener(DisableDragBlocker);

            StatusEffectData effect = entity.FindStatus(type);
            if (effect is IStatusToken effect2)
            {
                effectToken = effect2;
                effect2.ButtonCreate(this);
                button.onClick.AddListener(effectToken.RunButtonClicked);
            }
        }

        public void DisableDragBlocker()
        {
            button.DisableDragBlocking();
        }
    }

    public class StatusTokenApplyX : StatusEffectApplyX, IStatusToken
    {
        //Standard Code I wish I can put into IStatusToken
        [Flags]
        public enum PlayFromFlags
        {
            None = 0,
            Board = 1,
            Hand = 2,
            Draw = 4,
            Discard = 8
        }

        public PlayFromFlags playFrom = PlayFromFlags.Board;
        public bool finiteUses = false;
        public bool oncePerTurn = false;
        protected bool unusedThisTurn = true;
        public bool endTurn = false;
        public float timing = 0.2f;

        public override void Init()
        {
            base.Init();
        }

        public override bool RunTurnStartEvent(Entity entity)
        {
            if (entity.data.cardType.name == "Leader")
            {
                unusedThisTurn = true;
            }
            return base.RunTurnStartEvent(entity);
        }

        public virtual void RunButtonClicked()
        {
            if ((bool)References.Battle && References.Battle.phase == Battle.Phase.Play 
                && CorrectPlace() 
                && !target.IsSnowed 
                && target.owner == References.Player
                && !target.silenced
                && (!oncePerTurn || unusedThisTurn) )
            {
                target.StartCoroutine(ButtonClicked());
                unusedThisTurn = false;
            }

            if ((bool) target.IsSnowed || target.silenced)
            {
                NoTargetTextSystem noText = GameSystem.FindObjectOfType<NoTargetTextSystem>();
                if (noText != null)
                {
                    TMP_Text textElement = noText.textElement;
                    if ((bool)target.IsSnowed)
                    {
                        textElement.text = "Snowed!";
                    }
                    if ((bool)target.silenced)
                    {
                        textElement.text = "Inked!";
                    }
                    noText.PopText(target.transform.position);
                }
            }

        }

        public bool CheckFlag(PlayFromFlags flag) => (playFrom & flag) != 0;

        public virtual bool CorrectPlace()
        {
            if (CheckFlag(PlayFromFlags.Board) && Battle.IsOnBoard(target))
            {
                return true;
            }
            if (CheckFlag(PlayFromFlags.Hand) && References.Player.handContainer.Contains(target))
            {
                return true;
            }
            if (CheckFlag(PlayFromFlags.Draw) && target.preContainers.Contains(References.Player.drawContainer))
            {
                return true;
            }
            if (CheckFlag(PlayFromFlags.Discard) && target.preContainers.Contains(References.Player.discardContainer))
            {
                return true;
            }
            return false;
        }

        //Main Code
        public int fixedAmount = 0;
        public int hitDamage = 0;

        public IEnumerator ButtonClicked()
        {
            if (hitDamage != 0)
            {
                List<Entity> enemies = GetTargets();
                int trueAmount = (hitDamage == -1) ? count : hitDamage;
                foreach (Entity enemy in enemies)
                {
                    if (enemy.IsAliveAndExists())
                    {
                        Hit hit = new Hit(target, enemy, trueAmount);
                        hit.canRetaliate = false;
                        yield return hit.Process();
                    }

                }

            }
            yield return Run(GetTargets(), fixedAmount);
            List<StatusTokenApplyXListener> listeners = FindListeners();
            foreach(StatusTokenApplyXListener listener in listeners)
            {
                yield return listener.Run();
            }
            target.display.promptUpdateDescription = true;
            yield return PostClick();
        }

        public List<StatusTokenApplyXListener> FindListeners()
        {
            List<StatusTokenApplyXListener> listeners = new List<StatusTokenApplyXListener>();
            foreach (StatusEffectData status in target.statusEffects)
            {
                if (status is StatusTokenApplyXListener status2)
                {
                    if (status2.type == type + "_listener")
                    {
                        listeners.Add(status2);
                    }
                }
            }
            return listeners;
        }

        public virtual IEnumerator PostClick()
        {
            if (finiteUses)
            {
                count--;
                if (count == 0)
                {
                    yield return Remove();
                }
                target.promptUpdate = true;
            }
            if (endTurn)
            {
                yield return Sequences.Wait(timing);
                References.Player.endTurn = true;
            }
        }

        public void ButtonCreate(StatusIconExt icon)
        {
            return;
        }
    }

    public class StatusTokenApplyXListener : StatusEffectApplyX
    {
        public IEnumerator Run()
        {
            yield return Run(GetTargets());
        }
    }
}
