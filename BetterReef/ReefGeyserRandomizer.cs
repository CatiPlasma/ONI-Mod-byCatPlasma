using System;
using System.Collections.Generic;
using CatUtilLib;
using KSerialization;
using UnityEngine;

namespace BetterReef
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class ReefGeyserRandomizer: KMonoBehaviour, IGameObjectEffectDescriptor
    {
        [Serialize] private float inhaleMultiplier = -1f;
        [Serialize] private float exhaleMultiplier = -1f;

        private bool isRolled;
        
        public float GetInhaleMultiplier() => inhaleMultiplier;
        public float GetExhaleMultiplier() => exhaleMultiplier;
        public float GetInhaleRate() => inhaleMultiplier * 500f;
        public float GetExhaleRate() => exhaleMultiplier * 166.66667f;
        public float GetPeakPower() => 300f * (float)Math.Sqrt(exhaleMultiplier);
        public float GetAveragePower() => GetExhaleRate() * (GetInhaleRate() / (GetInhaleRate() + GetExhaleRate()));
        public float GetStorage() => Math.Max(GetInhaleMultiplier(), GetExhaleMultiplier()) * 15000f;
        
        private const float minMultiplier = 0.25f;
        private const float maxMultiplier = 4f;
        
        private SchedulerHandle retryHandle;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            LogUtil.Debug($"Rate Before: {GetComponent<StateMachineController>()?.GetSMI<BreathingGeyser.Instance>().def.inhaleRate}, {GetComponent<StateMachineController>()?.GetSMI<BreathingGeyser.Instance>().def.exhaleRate}");
            ApplyMultiplier();
            LogUtil.Debug($"Rate After: {GetComponent<StateMachineController>()?.GetSMI<BreathingGeyser.Instance>().def.inhaleRate}, {GetComponent<StateMachineController>()?.GetSMI<BreathingGeyser.Instance>().def.exhaleRate}");
        }

        protected override void OnCleanUp()
        {
            retryHandle.ClearScheduler();
            base.OnCleanUp();
        }
        
        private void InitMultiplier()
        {
            (inhaleMultiplier, exhaleMultiplier) = CatUtils.Roll(gameObject, minMultiplier, maxMultiplier, 2);
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