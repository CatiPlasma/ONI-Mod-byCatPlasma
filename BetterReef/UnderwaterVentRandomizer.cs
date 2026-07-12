using System;
using System.Collections.Generic;
using CatUtilLib;
using Database;
using KSerialization;
using STRINGS;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BetterReef
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class UnderwaterVentRandomizer: KMonoBehaviour, IGameObjectEffectDescriptor
    {
        [Serialize] private float bubbleMultiplier = -1f;
        [Serialize] private float buildupMultiplier = -1f;
        [Serialize] private float solidMultiplier = -1f;
        [Serialize] private bool isRolled;
        [Serialize] private bool isElementRolled;
        [Serialize] private UnderwaterVent.Data randomElement;
        
        private float minBubbleMultiplier = 0.1f;
        private float maxBubbleMultiplier = 10f;
        private float minBuildupSOlidMultiplier = 0.5f;
        private float maxBuildupSolidMultiplier = 2f;
        
        private static bool isAIOLoaded = CatUtils.IsModLoaded("RonivansLegacy_ChemicalProcessing");

        [MyCmpAdd] private UserNameable nameable;
        private SchedulerHandle retryHandler;
        
        public float GetEmittingRate() => GetComponent<StateMachineController>().GetSMI<UnderwaterVent.Instance>().def.data.BubbleMassRate;
        public SimHashes GetEmittingElement() => GetComponent<StateMachineController>().GetSMI<UnderwaterVent.Instance>().def.data.BubbleElement;
        public float GetBuildupDuration() => GetComponent<StateMachineController>().GetSMI<UnderwaterVent.Instance>().def.data.BuildUpDuration;
        public float GetSolidMass() => GetComponent<StateMachineController>().GetSMI<UnderwaterVent.Instance>().def.data.SolidMass;
        public SimHashes GetSolidElement => GetComponent<StateMachineController>().GetSMI<UnderwaterVent.Instance>().def.data.SolidElement;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            ApplyRandomize();
            GenerateName();
        }

        protected override void OnCleanUp()
        {
            retryHandler.ClearScheduler();
            base.OnCleanUp();
        }

        private void InitMultiplier()
        {
            bubbleMultiplier = CatUtils.RollFloats(gameObject, minBubbleMultiplier, maxBubbleMultiplier);
            var (buildupMultiplierTemp, solidMultiplierTemp) = CatUtils.RollFloats(
                gameObject, 
                minBuildupSOlidMultiplier, 
                maxBuildupSolidMultiplier, 
                2);
            buildupMultiplier = buildupMultiplierTemp / (float)Math.Sqrt(bubbleMultiplier);
            solidMultiplier = solidMultiplierTemp * buildupMultiplier / (float)Math.Sqrt(bubbleMultiplier);

            isRolled = true;
        }

        private UnderwaterVent.Data RandomizeElement()
        {
            UnderwaterVent.Data result = CatUtils.RollInts(gameObject, 0, 1000) switch
            {
                var n when n%1000<1 => RandomElementSets.EnrichedUranium,
                var n when 1<=n%1000 && n%1000<3 => RandomElementSets.Uranium,
                var n when 3<=n%1000 && n%1000<10 => RandomElementSets.DepletedUranium,
                var n when 10<=n%1000 && n%1000<60 => RandomElementSets.LeadGas,
                var n when 60<=n%1000 && n%1000<100 => RandomElementSets.RockGas,
                var n when 100<=n%1000 && n%1000<150 => RandomElementSets.MercuryGas,
                var n when 150<=n%1000 && n%1000<200 => RandomElementSets.SourGas,
                _ => new UnderwaterVent.Data{BubbleMassRate = -1f}
            };
            isElementRolled = true;
            
            return result;
        }

        private void ApplyRandomize()
        {
            if (!isRolled) InitMultiplier();
            if (!isElementRolled) randomElement = RandomizeElement();

            if (!CatUtils.HasWorldTrait(gameObject, "expansion1::traits/RadioactiveCrust") &&
                randomElement.BubbleElement == SimHashes.Fallout) randomElement = RandomElementSets.LeadGas;
            if (!CatUtils.HasBiome(gameObject, "icecaves") && randomElement.BubbleElement == SimHashes.MercuryGas)
                randomElement = RandomElementSets.SourGas;
            if (isElementRolled && CatUtils.RollInts(gameObject, 0, 100)%2==0 && randomElement.BubbleMassRate < 0)
                randomElement = RandomElementSets.RawGas;
            
            UnderwaterVent.Instance smi = GetComponent<StateMachineController>()?.GetSMI<UnderwaterVent.Instance>();
            if (smi == null)
            {
                SchedualedRetry();
                return;
            }
            
            retryHandler.ClearScheduler();
            
            UnderwaterVent.Def def = smi.def;
            if (randomElement.BubbleMassRate > 0) def = new UnderwaterVent.Def{data = randomElement};
            UnderwaterVent.Def target = new UnderwaterVent.Def
            {
                data = new UnderwaterVent.Data
                {
                    BubbleMassRate = def.data.BubbleMassRate * bubbleMultiplier,
                    BuildUpDuration = def.data.BuildUpDuration * buildupMultiplier,
                    SolidMass = def.data.SolidMass * solidMultiplier,
                    BubbleElement = def.data.BubbleElement,
                    BubbleSpawnOffset = def.data.BubbleSpawnOffset,
                    BubbleTemp = def.data.BubbleTemp,
                    SolidElement = def.data.SolidElement,
                    SolidSpawnOffset = def.data.SolidSpawnOffset,
                    SolidTemp = def.data.SolidTemp
                }
            };
            smi.def = target;
        }
        
        private void GenerateName()
        {
            if (nameable.savedName !=
                Strings.Get(new StringKey("STRINGS.CREATURES.SPECIES.GEYSER.UNDERWATERVENT.NAME"))) return;
            string[] firstTwoAll = NAMEGEN.GEYSER_IDS.IDs.ToString().Split('\n');
            string firstTwo = firstTwoAll[Random.Range(1, firstTwoAll.Length)];
            nameable.SetName(UI.FormatAsLink(string.Concat(
                UI.StripLinkFormatting(gameObject.GetProperName()),
                " ",
                GetComponent<StateMachineController>()?.GetSMI<UnderwaterVent.Instance>().def.data.BubbleElement switch
                {
                    SimHashes n when n == SimHashes.Methane => "E",
                    SimHashes n when n == SimHashes.Fallout => "U",
                    SimHashes n when n == SimHashes.RockGas => "R",
                    SimHashes n when n == SimHashes.LeadGas => "L",
                    SimHashes n when n == SimHashes.MercuryGas => "M",
                    SimHashes n when n == SimHashes.SourGas => "S",
                    SimHashes n when n == ElementLoader.FindElementByName("STRINGS.ELEMENTS.RAWNATURALGAS.NAME")?.id => "N"
                },
                firstTwo,
                "-",
                Random.Range(0, 10).ToString()), "UNDERWATERVENT"));
        }

        public List<Descriptor> GetDescriptors(GameObject go)
        {
            return new List<Descriptor>
            {
                new Descriptor(
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.UWATERVENTEMIT,
                        ElementLoader.FindElementByHash(GetComponent<StateMachineController>().GetSMI<UnderwaterVent.Instance>().def.data.BubbleElement).name,
                        GameUtil.GetFormattedMass(GetEmittingRate())),
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.UWATERVENTEMIT,
                        ElementLoader.FindElementByHash(GetComponent<StateMachineController>().GetSMI<UnderwaterVent.Instance>().def.data.BubbleElement).name,
                        GameUtil.GetFormattedMass(GetEmittingRate()))),
                new Descriptor(
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.UWATERVENTDURATION,
                        GameUtil.GetFormattedCycles(GetBuildupDuration())),
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.UWATERVENTDURATION,
                        GameUtil.GetFormattedCycles(GetBuildupDuration()))),
                new Descriptor(
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.UNDERWATERVENT_SHEARING,
                        ElementLoader.FindElementByHash(GetComponent<StateMachineController>().GetSMI<UnderwaterVent.Instance>().def.data.SolidElement).name,
                        GameUtil.GetFormattedMass(GetSolidMass())),
                    string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.UNDERWATERVENT_SHEARING,
                        ElementLoader.FindElementByHash(GetComponent<StateMachineController>().GetSMI<UnderwaterVent.Instance>().def.data.SolidElement).name))
            };
        }

        public static void RegisterUnderwaterVentString()
        {
            Strings.Add("STRINGS.UI.BUILDINGEFFECTS.UWATERVENTEMIT", STRINGS.UI.BUILDINGEFFECTS.UWATERVENTEMIT);
            Strings.Add("STRINGS.UI.BUILDINGEFFECTS.UWATERVENTDURATION", STRINGS.UI.BUILDINGEFFECTS.UWATERVENTDURATION);
        }

        public static void ModifyVanillaStaticStrings()
        {
            Db.Get().MiscStatusItems.UnderwaterVentBuildUpProgress.Name = STRINGS.MISC.STATUSITEMS.UNDERWATERVENTBUILDUPPROGRESS.NAME;
            Db.Get().MiscStatusItems.UnderwaterVentBuildUpProgress.tooltipText =
                STRINGS.MISC.STATUSITEMS.UNDERWATERVENTBUILDUPPROGRESS.TOOLTIP;
            Db.Get().MiscStatusItems.UnderwaterVentBlocked.tooltipText = STRINGS.MISC.STATUSITEMS.UNDERWATERVENTBLOCKED.TOOLTIP;
            Strings.Add("STRINGS.UI.BUILDINGEFFECTS.UNDERWATERVENT_SHEARING", STRINGS.UI.BUILDINGEFFECTS.UNDERWATERVENT_SHEARING);
        }

        public static void ModifyVanillaDynamicStrings(MiscStatusItems miscStatusItems)
        {
            miscStatusItems.UnderwaterVentBuildUpProgress.resolveStringCallback = (str, data) =>
            {
                UnderwaterVent.Instance instance = (UnderwaterVent.Instance)data;
                str = str
                    .Replace("{ELEMENT}",
                        ElementLoader.FindElementByHash(instance.def.data.SolidElement).name)
                    .Replace("{PERCENTAGE}",
                        GameUtil.GetFormattedPercent(instance.BuildUpProgress * 100f));
                return str;
            };
            miscStatusItems.UnderwaterVentBlocked.resolveStringCallback = (str, data) =>
            {
                UnderwaterVent.Instance instance = (UnderwaterVent.Instance)data;
                str = str.Replace("{ELEMENT}",
                    ElementLoader.FindElementByHash(instance.def.data.SolidElement).name);
                return str;
            };
        }

        private void SchedualedRetry()
        {
            if (isRolled || retryHandler.IsValid || GameScheduler.Instance == null)
            {
                return;
            }

            retryHandler = GameScheduler.Instance.ScheduleNextFrame(
                nameof(ApplyRandomize),
                _ =>
                {
                    retryHandler = default;
                    if (this != null) ApplyRandomize();
                }
            );
        }

        private static class RandomElementSets
        {
            public static readonly UnderwaterVent.Data Uranium = new UnderwaterVent.Data(
                new Vector3(1f, 2.5f, 0f),
                new Vector3(1f, 1.5f, 0f), 
                SimHashes.Fallout, 
                403.15f, 
                0.02f, 
                SimHashes.UraniumOre, 
                10f, 
                403.15f,
                2400f);
            public static readonly UnderwaterVent.Data DepletedUranium = new UnderwaterVent.Data(
                new Vector3(1f, 2.5f, 0f),
                new Vector3(1f, 1.5f, 0f), 
                SimHashes.Fallout, 
                403.15f, 
                0.02f, 
                SimHashes.DepletedUranium, 
                10f, 
                403.15f,
                2400f);
            public static readonly UnderwaterVent.Data EnrichedUranium = new UnderwaterVent.Data(
                new Vector3(1f, 2.5f, 0f),
                new Vector3(1f, 1.5f, 0f), 
                SimHashes.Fallout, 
                1123.15f, 
                0.02f, 
                SimHashes.EnrichedUranium, 
                10f, 
                1123.15f,
                2400f);
            public static readonly UnderwaterVent.Data RockGas = new UnderwaterVent.Data(
                new Vector3(1f, 2.5f, 0f),
                new Vector3(1f, 1.5f, 0f),
                SimHashes.RockGas,
                3945.15f,
                0.02f,
                SimHashes.Basalt,
                10f,
                1523.15f,
                600f);
            public static readonly UnderwaterVent.Data LeadGas = new UnderwaterVent.Data(
                new Vector3(1f, 2.5f, 0f),
                new Vector3(1f, 1.5f, 0f),
                SimHashes.LeadGas,
                3033.15f,
                0.02f,
                isAIOLoaded?
                    ElementLoader.FindElementByName("STRINGS.ELEMENTS.SOLIDSILVER.NAME")?.id ?? SimHashes.Zinc:
                    SimHashes.Zinc,
                10f,
                isAIOLoaded?1223.15f:673.15f,
                2400f);
            public static readonly UnderwaterVent.Data SourGas = new UnderwaterVent.Data(
                new Vector3(1f, 2.5f, 0f),
                new Vector3(1f, 1.5f, 0f),
                SimHashes.SourGas,
                373.15f,
                0.083333336f,
                SimHashes.Sulfur,
                2000f,
                373.15f,
                600f);

            public static readonly UnderwaterVent.Data MercuryGas = new UnderwaterVent.Data(
                new Vector3(1f, 2.5f, 0f),
                new Vector3(1f, 1.5f, 0f),
                SimHashes.MercuryGas,
                945.15f,
                0.083333336f,
                SimHashes.Cinnabar,
                1000f,
                945.15f,
                1200f);

            public static readonly UnderwaterVent.Data RawGas = new UnderwaterVent.Data(
                new Vector3(1f, 2.5f, 0f),
                new Vector3(1f, 1.5f, 0f),
                isAIOLoaded? 
                    ElementLoader.FindElementByName("STRINGS.ELEMENTS.RAWNATURALGAS.NAME")?.id ?? SimHashes.Methane:
                    SimHashes.Methane,
                373.15f,
                0.083333336f,
                SimHashes.Sulfur,
                isAIOLoaded?2000f:1000f,
                373.15f,
                isAIOLoaded?600f:1200f);
        }
    }
}