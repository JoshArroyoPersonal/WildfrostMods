using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pokefrost
{
    internal class StatusEffectWeather : StatusEffectData
    {
        SnowfallParticles snowfall;
        private bool active;

        public float intensityMultiplier = 2;
        public Color color;

        public override bool RunBeginEvent()
        {
            if(References.Battle != null && Battle.IsOnBoard(target))
            {
                Activate();
            }
            return false;
        }

        public override bool RunEndEvent()
        {
            Deactivate();
            return false;
        }

        public void Activate()
        {
            if (active) return;

            snowfall = GameObject.Find("Battle/Background/BackgroundSnowland(Clone)")?.GetComponentInChildren<SnowfallParticles>();
            if (snowfall == null) return;

            active = true;
            snowfall.frontSnow.startColor = color;
            Events.OnSetWeatherIntensity += Events_OnSetWeatherIntensity;
            Events.InvokeSetWeatherIntensity(0.2f, 1f);
        }

        private void Events_OnSetWeatherIntensity(float intensity, float duration)
        {
            if (snowfall == null)
            {
                Deactivate();
                return;
            }
            snowfall.storminessTo = intensity*intensityMultiplier;
        }

        public void Deactivate()
        {
            if (!active) return;

            if (snowfall != null)
            {
                snowfall.frontSnow.startColor = new Color(1f, 1f, 1f, 1f);
            }
            active = false;
            Events.OnSetWeatherIntensity -= Events_OnSetWeatherIntensity;
            Events.InvokeSetWeatherIntensity(0.1f, 1f);
        }
    }
}
