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
            Polymorph activePolymorph = __instance.GetActivePolymorph();
            if (activePolymorph != null)
            {
                UnitEntityView unitEntityView = activePolymorph.Prefab.Load(false);
                if (unitEntityView)
                {

                    Quaternion rotation = (!unitEntityView.ForbidRotation) ? Quaternion.Euler(0f, __instance.Orientation, 0f) : Quaternion.identity;
                    UnitEntityView unitEntityView2 = UnityEngine.Object.Instantiate<UnitEntityView>(unitEntityView, __instance.Position, rotation);

                    string texfullpath = "";
                    if (!string.IsNullOrEmpty(__instance.Descriptor.CustomPrefabGuid))
                    {
                        texfullpath = Main.randomPool(__instance.Blueprint);
                    }

                    unitEntityView2 = Main.unitEntityViewTexReplacer(unitEntityView2, texfullpath);

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

                string texfullpath = "";
                if (!string.IsNullOrEmpty(__instance.Descriptor.CustomPrefabGuid))
                {
                    texfullpath = Main.randomPool(__instance.Blueprint);
                }

                unitEntityView3 = Main.unitEntityViewTexReplacer(unitEntityView3, texfullpath);
                //                Main.DebugLog("2 " + unitEntityView3.name);
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
            else
            {

                string texfullpath = "";
                if (!string.IsNullOrEmpty(__instance.Descriptor.CustomPrefabGuid))
                {
                    texfullpath = Main.randomPool(__instance.Blueprint);
                }

                unitEntityView4 = Main.unitEntityViewTexReplacer(unitEntityView4, texfullpath);

            }






            Quaternion rotation2 = (!unitEntityView4.ForbidRotation) ? Quaternion.Euler(0f, __instance.Orientation, 0f) : Quaternion.identity;


            __result = UnityEngine.Object.Instantiate<UnitEntityView>(unitEntityView4, __instance.Position, rotation2);
            return false;
		}

     

    

    }

}
