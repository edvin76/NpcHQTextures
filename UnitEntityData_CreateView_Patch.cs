using System;
using System.IO;
using Harmony12;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.View;
using UnityEngine;
using UnityModManagerNet;
using Kingmaker.Blueprints.Root;
using Kingmaker.Blueprints;
using Kingmaker;

namespace NpcHQTextures
{
    // Token: 0x02000004 RID: 4
    [HarmonyPatch(typeof(UnitEntityData), "CreateView", MethodType.Normal)]
	public static class UnitEntityData_CreateView_Patch
    {

 
        // Token: 0x06000003 RID: 3 RVA: 0x000027B4 File Offset: 0x000009B4
        private static bool Prefix(UnitEntityData __instance, ref UnitEntityView __result)
		{


            if (!Main.modEnabled || Game.Instance.Player.AllCharacters.Contains(__instance) )
                return true;



            Main.DebugLog("CreateView: " + __instance.CharacterName + " - " + __instance.Blueprint.name);




            Quaternion quaternion = new Quaternion();
            Quaternion quaternion1;
            UnitEntityView unitEntityView3 = new UnitEntityView();
            
            string path = "";
            
            Polymorph activePolymorph = __instance.GetActivePolymorph();
            if (activePolymorph != null)
            {
                UnitEntityView unitEntityView = activePolymorph.Prefab.Load(false);
                if (unitEntityView)
                {
                    quaternion1 = (unitEntityView.ForbidRotation ? Quaternion.identity : Quaternion.Euler(0f, __instance.Orientation, 0f));
                    UnitEntityView unitEntityView1 = UnityEngine.Object.Instantiate<UnitEntityView>(unitEntityView, __instance.Position, quaternion1);
                    activePolymorph.SetReplacementViewOnLoad(unitEntityView1);
                    unitEntityView1.DisableSizeScaling = true;

                    path = Main.randomPool(__instance.Blueprint, unitEntityView1);

                    unitEntityView3 = unitEntityView1;
                    //return unitEntityView1;
                }
            }
            else
            {
                UnitEntityView unitEntityView2 = (string.IsNullOrEmpty(__instance.Descriptor.CustomPrefabGuid) ? __instance.Blueprint.Prefab.Load(false) : ResourcesLibrary.TryGetResource<UnitEntityView>(__instance.Descriptor.CustomPrefabGuid, false));
                if (unitEntityView2 == null)
                {
                    UberDebug.LogError(__instance.Blueprint, "Cannot find prefab for unit", Array.Empty<object>());
                    __result = null;
                    return false;
                }
                quaternion = (unitEntityView2.ForbidRotation ? Quaternion.identity : Quaternion.Euler(0f, __instance.Orientation, 0f));

                path = Main.randomPool(__instance.Blueprint, unitEntityView2);

                unitEntityView3 = UnityEngine.Object.Instantiate<UnitEntityView>(unitEntityView2, __instance.Position, quaternion);

            }


            if (path.Length > 3 && !path.Equals(Main.hqTexPath))
                unitEntityView3 = Main.unitEntityViewTexReplacer(unitEntityView3, path, Path.GetFileNameWithoutExtension(path));


               /*

               foreach (SkinnedMeshRenderer smr in unitEntityView3.GetComponentsInChildren<SkinnedMeshRenderer>())
               {
                   if (smr.material != null)
                   {
                       try
                       {

                           if (path.Length < 3)
                               path = Path.Combine(GetTexturesDir(), smr.material.mainTexture.name + ".png");

                            Main.DebugLog("LOOKING FOR: " + path);

                           if (File.Exists(path))
                           {
                               string textureName = smr.material.mainTexture.name;
                               Main.DebugLog("Found!!!!!!!!!!!!!!!: ");

                               Texture2D baseMap = ReadTexture(path, 1024, 1024);


                               if (baseMap != null && baseMap.GetRawTextureData().Length > 1)
                               {


                                   smr.material.SetTexture("_BaseMap", baseMap);


                                   smr.material.mainTexture = baseMap;
                                   smr.material.mainTexture.name = textureName;
                               }
                           }
                           else
                               Main.DebugLog("NOT found!!!!!");
                       }
                       catch (Exception x) { Main.DebugError(x); }
                   }
               }    

               */
               __result = UnityEngine.Object.Instantiate<UnitEntityView>(unitEntityView3, __instance.Position, quaternion);

            return false;





            /*
            
            if (!Main.modEnabled)
            {
                return true;
            }

            if(Main.customPrefabUnits == null || Main.customPrefabUnits.Count() ==0)
            {

                Main.Init();
            }
           

            Main.DebugLog("NEW CreateView() -  " + __instance.Blueprint.CharacterName + " - " + __instance.Blueprint.name + " - " + __instance.Blueprint.AssetGuid);
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
                        // if (__instance.Blueprint.CustomizationPreset != null || Main.customPrefabUnits.ContainsKey(__instance.Blueprint.name))
                        // {
                    
                             result2 = Main.randomPool(__instance.Blueprint, unitEntityView);
                     //   }

                        unitEntityView = Main.unitEntityViewTexReplacer(unitEntityView, result2.Item1, result2.Item2);

                    }


                    UnitEntityView unitEntityView2 = UnityEngine.Object.Instantiate<UnitEntityView>(unitEntityView, __instance.Position, rotation);



                    if (!isprefab2)
                    {
                        Tuple<string, string> result = new Tuple<string, string>("", "");
      
                        
                      //  if (__instance.Blueprint.CustomizationPreset != null || Main.customPrefabUnits.ContainsKey(__instance.Blueprint.name))
                       // {

                            result = Main.randomPool(__instance.Blueprint, unitEntityView2);
                       // }

                        unitEntityView2 = Main.unitEntityViewTexReplacer(unitEntityView2, result.Item1, result.Item2);
                    }


                    activePolymorph.SetReplacementViewOnLoad(unitEntityView2);
                    unitEntityView2.DisableSizeScaling = true;
  //                  Main.DebugLog("1 " + unitEntityView2.name);
                    __result = unitEntityView2;

                   // Main.OrigTexName = null;
                   // Main.ReadableText = null;


                    Main.preset = null;
                    Main.notRandom = false;
                    return false;
                }
            }
            if (__instance.Descriptor.Doll != null)
            {
                UnitEntityView unitEntityView3 = __instance.Descriptor.Doll.CreateUnitView(false);
                unitEntityView3.transform.position = __instance.Position;
                unitEntityView3.transform.rotation = Quaternion.Euler(0f, __instance.Orientation, 0f);




                Tuple<string, string> result = new Tuple<string, string>("", "");
                //if (!string.IsNullOrEmpty(__instance.Descriptor.CustomPrefabGuid))
               // if (__instance.Blueprint.CustomizationPreset != null || Main.customPrefabUnits.ContainsKey(__instance.Blueprint.name))
               // {

                    result = Main.randomPool(__instance.Blueprint, unitEntityView3);
               // }

                unitEntityView3 = Main.unitEntityViewTexReplacer(unitEntityView3, result.Item1, result.Item2);



                __result = unitEntityView3;
               // Main.OrigTexName = null;
               // Main.ReadableText = null;


                Main.preset = null;
                Main.notRandom = false;
                return false;
            }
            UnitEntityView unitEntityView4 = (!string.IsNullOrEmpty(__instance.Descriptor.CustomPrefabGuid)) ? ResourcesLibrary.TryGetResource<UnitEntityView>(__instance.Descriptor.CustomPrefabGuid, false) : __instance.Blueprint.Prefab.Load(false);


            if (unitEntityView4 == null)
            {
                UberDebug.LogError(__instance.Blueprint, "Cannot find prefab for unit", Array.Empty<object>());

                __result = null;
               // Main.OrigTexName = null;
              //  Main.ReadableText = null;


                Main.preset = null;
                Main.notRandom = false;
                return false;

            }

  

            bool isprefab = false;
            if (unitEntityView4.GetComponentsInChildren<Component>().Count() > 0)
            {
                isprefab = true;

                Tuple<string, string> result2 = new Tuple<string, string>("", "");
               // if (__instance.Blueprint.CustomizationPreset != null || Main.customPrefabUnits.ContainsKey(__instance.Blueprint.name))
               // {

                    result2 = Main.randomPool(__instance.Blueprint, unitEntityView4);
               // }

                unitEntityView4 = Main.unitEntityViewTexReplacer(unitEntityView4, result2.Item1, result2.Item2);

            }





            Quaternion rotation2 = (!unitEntityView4.ForbidRotation) ? Quaternion.Euler(0f, __instance.Orientation, 0f) : Quaternion.identity;


            UnitEntityView resultView = UnityEngine.Object.Instantiate<UnitEntityView>(unitEntityView4, __instance.Position, rotation2);


            if (!isprefab)
            {
                Tuple<string, string> result = new Tuple<string, string>("", "");


               // if (__instance.Blueprint.CustomizationPreset != null || Main.customPrefabUnits.ContainsKey(__instance.Blueprint.name))
               // {

                    result = Main.randomPool(__instance.Blueprint, resultView);
               // }

                resultView = Main.unitEntityViewTexReplacer(resultView, result.Item1, result.Item2);
            }


            __result = resultView;
         //   Main.OrigTexName = null;
           // Main.ReadableText = null;


            Main.preset = null;
            Main.notRandom = false;

            return false;
            */

        }





    }

}
