using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Pokefrost
{
    internal class Timer : MonoBehaviour
    {
        private float time;
        private float scale = -1;

        public float Time => time;
        private FloatingText Text => GetComponent<FloatingText>();

        public event UnityAction OnFinished;

        internal static Timer Create(float time)
        {
            FloatingTextManager manager = GameObject.FindObjectOfType<FloatingTextManager>();
            FloatingText floatingText = manager.CreatePrefab();
            Timer timer = floatingText.gameObject.AddComponent<Timer>();
            timer.SetTime(time);
            timer.Show();
            return timer;
        }

        public void Start()
        {
            Text.textAsset.outlineWidth = 0.05f;
            Text.textAsset.outlineColor = Color.black;
        }

        public void SetTime(float time)
        {
            this.time = time;
        }

        public void SetScale(float scale)
        {
            this.scale = scale;
        }

        public const string RED = "#ff4444";
        public const string YEL = "#ffca57";
        public const string WHT = "#ffffff";
        public const string GRN = "#3AF6CB";
        public const string BLU = "#7EDAFF";
        //#ffffff, #ff4444, #ffca57
        private string format = "<color={3}><mspace=0.4>{0}</mspace>:<mspace=0.4>{1}</mspace>.<mspace=0.3><size=0.6>{2}</size></mspace></color>";
        private string failFormat = "<color=#ff4444><mspace=0.4>XX</mspace>:<mspace=0.4>XX</mspace>.<mspace=0.3><size=0.6>XX</size></mspace></color>";


        public void Show()
        {
            transform.position = new Vector3(0,5f,0);
            UpdateDisplay(YEL);
            gameObject.SetActive(true);
        }

        public void UpdateDisplay(string color = null)
        {
            if (color == null)
            {
                color = running ? WHT : YEL;
            }
            if (time <= 0)
            {
                Text.SetText(failFormat);
                time = 0;
                running = false;
                OnFinished?.Invoke();
                return;
            }
            int intTime = (int)time;
            string minutes = (intTime / 60).ToString("00");
            string seconds = (intTime % 60).ToString("00");
            string milli = Math.Floor((time % 1) * 100).ToString("00");
            Text.SetText(string.Format(format, minutes, seconds, milli, color));
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public bool running = false;
        public void Play()
        {
            if (!running && time > 0)
            {
                running = true;
                StartCoroutine(Run());
            }
        }

        public void Stop()
        {
            running = false;
        }

        public void End()
        {
            gameObject.Destroy();
        }

        public IEnumerator Run()
        {
            while (running)
            {
                time += scale * UnityEngine.Time.deltaTime;
                UpdateDisplay(WHT);
                yield return null;
            }
            UpdateDisplay(YEL);

        }
    }
}
