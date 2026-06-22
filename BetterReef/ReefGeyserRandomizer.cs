using System;
using System.Collections.Generic;
using CatUtilLib;
using KSerialization;
using STRINGS;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BetterReef
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class ReefGeyserRandomizer: KMonoBehaviour, IGameObjectEffectDescriptor
    {
        [Serialize] private float inhaleMultiplier = -1f;
        [Serialize] private float exhaleMultiplier = -1f;

        private bool isRolled;
        private const float minMultiplier = 0.25f;
        private const float maxMultiplier = 4f;

        [MyCmpAdd] private UserNameable nameable;
        private SchedulerHandle retryHandle;
        
        public float GetInhaleMultiplier() => inhaleMultiplier;
        public float GetExhaleMultiplier() => exhaleMultiplier;
        public float GetInhaleRate() => inhaleMultiplier * 500f;
        public float GetExhaleRate() => exhaleMultiplier * 166.66667f;
        public float GetPeakPower() => 300f * (float)Math.Sqrt(exhaleMultiplier);
        public float GetAveragePower() => GetExhaleRate() * (GetInhaleRate() / (GetInhaleRate() + GetExhaleRate()));
        public float GetStorage() => Math.Max(GetInhaleMultiplier(), GetExhaleMultiplier()) * 15000f;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            ApplyMultiplier();
            GenerateName();
        }

        protected override void OnCleanUp()
        {
            retryHandle.ClearScheduler();
            base.OnCleanUp();
        }
        
        private void InitMultiplier()
        {
            (inhaleMultiplier, exhaleMultiplier) = CatUtils.RollFloats(gameObject, minMultiplier, maxMultiplier, 2);
        }

        private void ApplyMultiplier()
        {
            isRolled = (inhaleMultiplier > 0 && exhaleMultiplier > 0);
            
            if (!isRolled) InitMultiplier();
            
            if (!TryGetTarget(
                    out Storage storage,
                    out ElementConsumer elementConsumer,
                    out BreathingGeyser.Instance smi))
            {
                SchedualedRetry();
                return;
            }
            
            BreathingGeyser.Def original = smi.def;
            BreathingGeyser.Def target = new BreathingGeyser.Def
            {
                inhaleRate = original.inhaleRate,
                exhaleRate = original.exhaleRate,
                diseaseIdx = original.diseaseIdx,
                germsPerKg = original.germsPerKg
            };
            
            target.inhaleRate = original.inhaleRate * inhaleMultiplier;
            target.exhaleRate = original.exhaleRate * exhaleMultiplier;
            
            smi.def = target;
            
            storage.capacityKg *= Math.Max(inhaleMultiplier, exhaleMultiplier);
            elementConsumer.capacityKG = storage.capacityKg;
            elementConsumer.consumptionRate = target.inhaleRate;
        }

        private void GenerateName()
        {
            if (nameable.savedName !=
                Strings.Get(new StringKey("STRINGS.CREATURES.SPECIES.GEYSER.SMALLREEFGEYSER.NAME"))) return;
            string[] firstTwoAll = NAMEGEN.GEYSER_IDS.IDs.ToString().Split('\n');
            string firstTwo = firstTwoAll[Random.Range(1, firstTwoAll.Length)];
            nameable.SetName(UI.FormatAsLink(string.Concat(
                UI.StripLinkFormatting(gameObject.GetProperName()),
                " ",
                GetAveragePower() < 225f ? "L" : "H",
                firstTwo,
                "-",
                Random.Range(0, 10).ToString()), "SMALLREEFGEYSER"));
        }

        public List<Descriptor> GetDescriptors(GameObject go)
        {
            return new List<Descriptor>
            {
                new Descriptor(
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.TIDALINRATE,
                        GameUtil.GetFormattedMass(GetInhaleRate())),
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.TIDALINRATE,
                        GameUtil.GetFormattedMass(GetInhaleRate()))),
                new Descriptor(
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.TIDALEXRATE,
                        GameUtil.GetFormattedMass(GetExhaleRate())),
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.TIDALINRATE,
                        GameUtil.GetFormattedMass(GetExhaleRate()))),
                new Descriptor(
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.TIDALSTORAGE,
                        GameUtil.GetFormattedMass(GetStorage())),
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.TIDALSTORAGE,
                        GameUtil.GetFormattedMass(GetStorage()))),
                new Descriptor(
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.TIDALPEKGEN,
                        GameUtil.GetFormattedWattage(GetPeakPower())),
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.TIDALPEKGEN,
                        GameUtil.GetFormattedWattage(GetPeakPower()))),
                new Descriptor(
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.TIDALAVGGEN,
                        GameUtil.GetFormattedWattage(GetAveragePower())),
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.TIDALAVGGEN,
                        GameUtil.GetFormattedWattage(GetAveragePower())))
            };
        }

        public static void RegisterReefGeyserString()
        {
            Strings.Add("STRINGS.UI.BUILDINGEFFECTS.TIDALINRATE", STRINGS.UI.BUILDINGEFFECTS.TIDALINRATE);
            Strings.Add("STRINGS.UI.BUILDINGEFFECTS.TIDALEXRATE", STRINGS.UI.BUILDINGEFFECTS.TIDALEXRATE);
            Strings.Add("STRINGS.UI.BUILDINGEFFECTS.TIDALSTORAGE", STRINGS.UI.BUILDINGEFFECTS.TIDALSTORAGE);
            Strings.Add("STRINGS.UI.BUILDINGEFFECTS.TIDALPEKGEN", STRINGS.UI.BUILDINGEFFECTS.TIDALPEKGEN);
            Strings.Add("STRINGS.UI.BUILDINGEFFECTS.TIDALAVGGEN", STRINGS.UI.BUILDINGEFFECTS.TIDALAVGGEN);
        }

        private bool TryGetTarget(
            out Storage storage,
            out ElementConsumer elementConsumer,
            out BreathingGeyser.Instance smi)
        {
            storage = GetComponent<Storage>();
            elementConsumer = GetComponent<ElementConsumer>();
            smi = GetComponent<StateMachineController>()?.GetSMI<BreathingGeyser.Instance>();
            return storage != null && elementConsumer != null && smi != null;
        }
        
        private void SchedualedRetry()
        {
            if (isRolled || retryHandle.IsValid || GameScheduler.Instance == null)
            {
                return;
            }

            retryHandle = GameScheduler.Instance.ScheduleNextFrame(
                nameof(ApplyMultiplier),
                _ =>
                {
                    retryHandle = default;
                    if (this != null) ApplyMultiplier();
                }
            );
        }
    }
}