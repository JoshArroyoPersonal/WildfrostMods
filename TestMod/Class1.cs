using Deadpan.Enums.Engine.Components.Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TestMod
{
    public class TestMod : WildfrostMod
    {

        private List<CardUpgradeDataBuilder> charmlist;

        public TestMod(string modDirectory) : base(modDirectory)
        {

        }

        private void CreateModAssets()
        {
            charmlist = new List<CardUpgradeDataBuilder>();
            //Add our cards here
            charmlist.Add(
                new CardUpgradeDataBuilder(this)
                    .CreateCharm("CardUpgradeTest1")
                    .WithTier(0)
                    .WithImage("test1.png")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithTitle("Test")
                    .WithText("Test")
            );

            charmlist.Add(
                new CardUpgradeDataBuilder(this)
                    .CreateCharm("CardUpgradeTest2")
                    .WithTier(0)
                    .WithImage("test1.png")
                    .WithType(CardUpgradeData.Type.Charm)
                    .WithTitle("Test2")
                    .WithText("Test")
            );
        }

        public override void Load()
        {
            CreateModAssets();
            base.Load();

        }

        public override List<T> AddAssets<T, Y>()
        {
            var typeName = typeof(Y).Name;
            switch (typeName)
            {
                case "CardUpgradeData": return charmlist.Cast<T>().ToList();
            }

            return base.AddAssets<T, Y>();
        }

        public override string GUID => "websiteofsites.wildfrost.testmod";
        public override string[] Depends => new string[] { };
        public override string Title => "Charm Test";
        public override string Description => "Testing 2 charms";
    }
}
