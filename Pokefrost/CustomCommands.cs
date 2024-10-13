using Deadpan.Enums.Engine.Components.Modding;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static Console;

namespace Pokefrost
{
    internal class Commands
    {
        public static IEnumerator AddCustomCommands()
        {
            yield return new WaitUntil(() => SceneManager.Loaded.ContainsKey("MainMenu"));
            if (commands != null)
            {
                AddCommands();
            }
            
        }

        public static void AddCommands()
        {
            commands.Add(new CommandMultModifier());
        }

        public class CommandMultModifier : Command
        {
            public override string id => "poke.Modifier";

            public override string format => "poke.Modifier <name>";

            public override string desc => "Gives the corresponding modifier";

            public override bool IsRoutine => false;
            public override void Run(string args)
            {
                GameModifierData modifier = AddressableLoader.GetGroup<GameModifierData>("GameModifierData").FirstOrDefault( (a) => string.Equals(a.name, args, StringComparison.CurrentCultureIgnoreCase));

                if (modifier == null)
                {
                    Fail("Upgrade [" + args + "] does not exist!");
                }

                if (Campaign.instance == null)
                {
                    Fail("Must be in a run!");
                }

                ModifierSystem.AddModifier(Campaign.Data, modifier);

                Routine.Clump clump = new Routine.Clump();
                Script[] startScripts = modifier.startScripts;
                foreach (Script script in startScripts)
                {
                    clump.Add(script.Run());
                }

                startScripts = modifier.setupScripts;
                foreach (Script script2 in startScripts)
                {
                    clump.Add(script2.Run());
                }

                string[] systemsToAdd = modifier.systemsToAdd;
                foreach (string text in systemsToAdd)
                {
                    Debug.Log($"[{modifier}] adding system: {text}");
                    Campaign.instance.gameObject.AddComponentByName(text);
                }
            }

            public override IEnumerator GetArgOptions(string currentArgs)
            {
                yield return AddressableLoader.LoadGroup("CardUpgradeData");
                IEnumerable<GameModifierData> enumerable = from a in AddressableLoader.GetGroup<GameModifierData>("GameModifierData")
                                                          where a.name.ToLower().Contains(currentArgs.ToLower())
                                                          select a;
                List<string> list = new List<string>();
                foreach (GameModifierData item in enumerable)
                {
                    list.Add(item.name);
                }

                predictedArgs = list.ToArray();
            }
        }
    }
}
