using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using KSerialization;
using UnityEngine;

namespace BetterReef
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class MarineDrillModifier: KMonoBehaviour, IGameObjectEffectDescriptor
    {
        [Serialize] private float durationMultiplier = -1;
        [Serialize] private float diamondConsumptionMultiplier = -1;
        
        private UnderwaterVentDrill.Instance smi;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            smi = gameObject.GetSMI<UnderwaterVentDrill.Instance>();
            ApplyModifiers();
        }

        private void SetupMultiplier()
        {
            int cell = Grid.PosToCell(smi.transform.GetPosition());
            GameObject geyser = Grid.Objects[cell, (int)ObjectLayer.Building];
            UnderwaterVent.Instance ventSMI = geyser.GetSMI<UnderwaterVent.Instance>();
            Element buildupElement = ElementLoader.FindElementByHash(ventSMI.def.data.SolidElement);
            float buildupElementTemp = ventSMI.def.data.SolidTemp;
            Element standardElement = ElementLoader.FindElementByHash(SimHashes.Sulfur);
            float harnessFactor = Mathf.Log(buildupElement.hardness, 2f)/Mathf.Log(standardElement.hardness, 2f);
            float massFactor = ventSMI.def.data.SolidMass / 1000;
            SimHashes primaryElement = gameObject.GetComponent<PrimaryElement>().ElementID;
            durationMultiplier = buildupElementTemp > turnTempDic.GetValueOrDefault(primaryElement, 1000.15f)
                ? 2 * massFactor * Mathf.Exp(harnessFactor - 1)
                : massFactor * Mathf.Exp(harnessFactor - 1);
            diamondConsumptionMultiplier = massFactor * Mathf.Exp(harnessFactor - 1);
        }

        private void ApplyModifiers()
        {
            if (durationMultiplier < 0 || diamondConsumptionMultiplier < 0) SetupMultiplier();
            
            UnderwaterVentDrill.Def oldDef = smi.def;
            UnderwaterVentDrill.Def newDef = new UnderwaterVentDrill.Def
            {
                DiamondConsumptionRate = oldDef.DiamondConsumptionRate * (diamondConsumptionMultiplier / durationMultiplier),
                DiamondTag = oldDef.DiamondTag,
                WorkDuration = oldDef.WorkDuration * durationMultiplier,
                ProgressBarOffset = oldDef.ProgressBarOffset
            };
            
            smi.def = newDef;
            smi.GetComponent<Storage>().capacityKg = 2 * diamondConsumptionMultiplier * 100;
        }

        public List<Descriptor> GetDescriptors(GameObject go)
        {
            return new List<Descriptor>
            {
                new(
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.UNDERWATERVENTDRILLDURATION,
                        durationMultiplier > 12
                            ? GameUtil.GetFormattedCycles(100 * durationMultiplier)
                            : GameUtil.GetFormattedTime(100 * durationMultiplier)),
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.UNDERWATERVENTDRILLDURATION,
                        durationMultiplier > 12
                            ? GameUtil.GetFormattedCycles(100 * durationMultiplier)
                            : GameUtil.GetFormattedTime(100 * durationMultiplier))),
                new(
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.UNDERWATERVENTDIAMONDCONSUMTION,
                        GameUtil.GetFormattedMass(100 * diamondConsumptionMultiplier)),
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.UNDERWATERVENTDIAMONDCONSUMTION,
                        GameUtil.GetFormattedMass(100 * diamondConsumptionMultiplier))),
                new(
                    string.Format(STRINGS.UI.BUILDINGEFFECTS.UNDERWATERVENTDRILLTEMP,
                        GameUtil.GetFormattedTemperature(turnTempDic.GetValueOrDefault(gameObject.GetComponent<PrimaryElement>().ElementID, 1000.15f))),
                    STRINGS.UI.BUILDINGEFFECTS.UNDERWATERVENTDRILLTEMPTOOLTIP)
            };
        }

        Dictionary<SimHashes, float> turnTempDic = new()
        {
            { SimHashes.Gold, 973.15f },
            { SimHashes.Copper, 1023.15f },
            { SimHashes.Iron, 1073.15f },
            { SimHashes.Cobalt, 1123.15f },
            { SimHashes.Tungsten, 1473.15f },
            { SimHashes.Steel, 1173.15f },
            { SimHashes.Niobium, 1273.15f },
            { SimHashes.TempConductorSolid, 1473.15f }
        };
    }
}