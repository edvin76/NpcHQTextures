using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Harmony12;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Persistence.Scenes;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Customization;
using Kingmaker.View;
using UnityEngine;
//
namespace NpcHQTextures                                                                                                               
{
	// Token: 0x02000004 RID: 4
	[HarmonyPatch(typeof(UnitEntityData), "CreateView", MethodType.Normal)]
	public static class UnitEntityData_CreateView_Patch
    {
		// Token: 0x06000003 RID: 3 RVA: 0x000027B4 File Offset: 0x000009B4
		private static bool Prefix(UnitEntityData __instance, ref UnitEntityView __result)
		{
            if (!Main.modEnabled)
            {
                return true;
            }

            if(Main.customPrefabUnits == null || Main.customPrefabUnits.Count() ==0)
            {

                Main.Init();
            }
           

            Main.DebugLog("CreateView() -  " + __instance.Blueprint.CharacterName + " - " + __instance.Blueprint.name + " - " + __instance.Blueprint.AssetGuid);
            Polymorph activePolymorph = __instance.GetActivePolymorph();
            if (activePolymorph != null)
            {
                UnitEntityView unitEntityView = activePolymorph.Prefab.Load(false);
                if (unitEntityView)
                {

                    Quaternion rotation = (!unitEntityView.ForbidRotation) ? Quaternion.Euler(0f, __instance.Orientation, 0f) : Quaternion.identity;



                    bool isprefab2 = false;
                    if (unitEntityView.GetComponentsInChildren<Component>().Count() > 0)
                    {
                        isprefab2 = true;

                        Tuple<string, string> result2 = new Tuple<string, string>("", "");
                               if (__instance.Blueprint.CustomizationPreset != null || Main.customPrefabUnits.ContainsKey(__instance.Blueprint.name))
                        {

                            result2 = Main.randomPool(__instance.Blueprint, unitEntityView);
                        }

                        unitEntityView = Main.unitEntityViewTexReplacer(unitEntityView, result2.Item1, result2.Item2);

                    }


                    UnitEntityView unitEntityView2 = UnityEngine.Object.Instantiate<UnitEntityView>(unitEntityView, __instance.Position, rotation);



                    if (!isprefab2)
                    {
                        Tuple<string, string> result = new Tuple<string, string>("", "");
      
                        
                        if (__instance.Blueprint.CustomizationPreset != null || Main.customPrefabUnits.ContainsKey(__instance.Blueprint.name))
                        {

                            result = Main.randomPool(__instance.Blueprint, unitEntityView2);
                        }

                        unitEntityView2 = Main.unitEntityViewTexReplacer(unitEntityView2, result.Item1, result.Item2);
                    }

                    /*

                    string texfullpath = "";
                    if (!string.IsNullOrEmpty(__instance.Descriptor.CustomPrefabGuid)&& __instance.Blueprint != null)
                    {
                        //texfullpath = Main.randomPool(__instance.Blueprint, __instance.Descriptor.CustomPrefabGuid);

                        unitEntityView2
                    }

                    unitEntityView2 = Main.unitEntityViewTexReplacer(unitEntityView2, texfullpath);
                    */
                    activePolymorph.SetReplacementViewOnLoad(unitEntityView2);
                    unitEntityView2.DisableSizeScaling = true;
  //                  Main.DebugLog("1 " + unitEntityView2.name);
                    __result = unitEntityView2;
                    return false;
                }
            }
            if (__instance.Descriptor.Doll != null)
            {
                UnitEntityView unitEntityView3 = __instance.Descriptor.Doll.CreateUnitView(false);
                unitEntityView3.transform.position = __instance.Position;
                unitEntityView3.transform.rotation = Quaternion.Euler(0f, __instance.Orientation, 0f);

                /*
                string texfullpath = "";
                if (!string.IsNullOrEmpty(__instance.Descriptor.CustomPrefabGuid) && __instance.Blueprint != null)
                {
                    texfullpath = Main.randomPool(__instance.Blueprint, __instance.Descriptor.CustomPrefabGuid);
                }

                unitEntityView3 = Main.unitEntityViewTexReplacer(unitEntityView3, texfullpath);
                //                Main.DebugLog("2 " + unitEntityView3.name);
                */


                Tuple<string, string> result = new Tuple<string, string>("", "");
                //if (!string.IsNullOrEmpty(__instance.Descriptor.CustomPrefabGuid))
                if (__instance.Blueprint.CustomizationPreset != null || Main.customPrefabUnits.ContainsKey(__instance.Blueprint.name))
                {

                    result = Main.randomPool(__instance.Blueprint, unitEntityView3);
                }

                unitEntityView3 = Main.unitEntityViewTexReplacer(unitEntityView3, result.Item1, result.Item2);



                __result = unitEntityView3;
                return false;
            }
            UnitEntityView unitEntityView4 = (!string.IsNullOrEmpty(__instance.Descriptor.CustomPrefabGuid)) ? ResourcesLibrary.TryGetResource<UnitEntityView>(__instance.Descriptor.CustomPrefabGuid, false) : __instance.Blueprint.Prefab.Load(false);


            if (unitEntityView4 == null)
            {
                UberDebug.LogError(__instance.Blueprint, "Cannot find prefab for unit", Array.Empty<object>());

                __result = null;
                return false;

            }

  

            bool isprefab = false;
            if (unitEntityView4.GetComponentsInChildren<Component>().Count() > 0)
            {
                isprefab = true;

                Tuple<string, string> result2 = new Tuple<string, string>("", "");
                if (__instance.Blueprint.CustomizationPreset != null || Main.customPrefabUnits.ContainsKey(__instance.Blueprint.name))
                {

                    result2 = Main.randomPool(__instance.Blueprint, unitEntityView4);
                }

                unitEntityView4 = Main.unitEntityViewTexReplacer(unitEntityView4, result2.Item1, result2.Item2);

            }





            Quaternion rotation2 = (!unitEntityView4.ForbidRotation) ? Quaternion.Euler(0f, __instance.Orientation, 0f) : Quaternion.identity;


            UnitEntityView resultView = UnityEngine.Object.Instantiate<UnitEntityView>(unitEntityView4, __instance.Position, rotation2);


            if (!isprefab)
            {
                Tuple<string, string> result = new Tuple<string, string>("", "");


                if (__instance.Blueprint.CustomizationPreset != null || Main.customPrefabUnits.ContainsKey(__instance.Blueprint.name))
                {

                    result = Main.randomPool(__instance.Blueprint, resultView);
                }

                resultView = Main.unitEntityViewTexReplacer(resultView, result.Item1, result.Item2);
            }


            __result = resultView;


            return false;
		}

     

    

    }

}
