using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Harmony12;
using Kingmaker.Blueprints;
using Kingmaker.Controllers;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic.Customization;
using Kingmaker.View;
using Kingmaker.Visual.Sound;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace NpcHQTextures
{


    [HarmonyPatch(typeof(EntityCreationController), "SpawnUnit", new Type[] { typeof(BlueprintUnit), typeof(Vector3), typeof(Quaternion), typeof(SceneEntitiesState) })]
    public static class EntityCreationController_SpawnUnit0_Patch
    {
        // Token: 0x06000038 RID: 56 RVA: 0x0000227B File Offset: 0x0000047B
        private static bool Prefix(EntityCreationController __instance, BlueprintUnit unit, Vector3 position, Quaternion rotation, SceneEntitiesState state, ref UnitEntityData __result)
        {
            if (unit == null)
            {

                Main.DebugLog("Trying to spawn null unit");
                return true;
            }
            if (unit.CustomizationPreset != null)
            {
                // Main.DebugLog("a");
                     
                UnitCustomizationVariation unitCustomizationVariation;
#if DEBUG
                if (!first)
                {
                unitCustomizationVariation = unit.CustomizationPreset.SelectVariation(unit, null);


                }
                else
                {
                    unitCustomizationVariation = Main.preset;
                }
#endif
                unitCustomizationVariation = unit.CustomizationPreset.SelectVariation(unit, null);

                if (unitCustomizationVariation == null)
                {

                    //

                    Main.DebugLog($"Failed to select customization variation for unit {unit.name} (preset = {unit.CustomizationPreset.name})");

                    if(Main.allVariations == null || Main.allVariations.Count() == 0)
                    {
                        Main.Init();
                    }


                        if (Main.allVariations.ContainsKey(unit.Prefab))
                    {
                        //ucv.Prefab == blueprintUnit.Prefab
                        Main.preset = Main.allVariations[unit.Prefab];
//                        Main.DebugLog(Main.preset.Prefab.AssetId);


                    }
                    else
                    {
                        Main.notRandom = true;
                    }


                    UnitEntityView prefab2 = unit.Prefab.Load(false);
                    //  Main.DebugLog("g");
                    __result = __instance.SpawnUnit(unit, prefab2, position, rotation, state);



                    

              
                    return false;


                }

                /*
                if (unitCustomizationVariation == null)
                {

                    List<UnitCustomizationPreset> presets = ResourcesLibrary.GetBlueprints<UnitCustomizationPreset>().ToList();

                    foreach (UnitCustomizationPreset preset in presets.Where(x => x.name != unit.CustomizationPreset.name))
                    {

                        unitCustomizationVariation = preset.SelectVariation(unit, null);

                        if (unitCustomizationVariation != null)
                        {
                            break;
                        }

                   }



                }
                */
                Main.preset = unitCustomizationVariation;


                BlueprintUnitAsksList voice = unit.CustomizationPreset.SelectVoice(unitCustomizationVariation.Gender);
                //  Main.DebugLog("c");
                bool leftHanded = unit.CustomizationPreset.SelectLeftHanded();
                //  Main.DebugLog("d");
                using (unitCustomizationVariation.CreateSpawningData(voice, leftHanded))
                {
                    // Main.DebugLog("e");
                    UnitEntityView prefab = unitCustomizationVariation.Prefab.Load(false);
                    // Main.DebugLog("Spawn1: "+prefab.name);
                    //   Main.DebugLog("f");
                    __result = __instance.SpawnUnit(unit, prefab, position, rotation, state);
                    return false;
                }

            }
            else
            {
                Main.notRandom = true;
                //  Main.DebugLog("NO CUSTOMPRESET!!!");
            }


            UnitEntityView prefab3 = unit.Prefab.Load(false);
            //  Main.DebugLog("g");
            __result = __instance.SpawnUnit(unit, prefab3, position, rotation, state);


            return false;

        }
    }


    [HarmonyPatch(typeof(EntityCreationController), "SpawnUnit", new Type[] { typeof(BlueprintUnit), typeof(UnitEntityView), typeof(Vector3), typeof(Quaternion), typeof(SceneEntitiesState) })]
    public static class EntityCreationController_SpawnUnit_Patch
    {
        // Token: 0x06000038 RID: 56 RVA: 0x0000227B File Offset: 0x0000047B
        private static bool Prefix(EntityCreationController __instance, BlueprintUnit unit, UnitEntityView prefab, Vector3 position, Quaternion rotation, SceneEntitiesState state, ref UnitEntityData __result)
        {

            /*  if (!Main.first)
              {
                  return true;
              }*/

            if (unit == null)
            {

                Main.DebugLog("Trying to spawn null unit.");
               // Main.OrigTexName = null;
             //   Main.ReadableText = null;


                Main.preset = null;
                Main.notRandom = false;
                return true;
            }
            if (prefab == null)
            {
                Main.DebugLog("Trying to spawn unit without prefab");
               // Main.OrigTexName = null;
               // Main.ReadableText = null;


                Main.preset = null;
                Main.notRandom = false;
                return true;
            }


            Main.DebugLog("SpawnUnit() -  " + unit.CharacterName + " - " + unit.name + " - " + unit.AssetGuid);


            prefab.UniqueId = Guid.NewGuid().ToString();

            bool isprefab = false;
            //if (prefab.Renderers !=null && prefab.Renderers.Count() > 0)
            if (prefab.GetComponentsInChildren<Component>().Count() > 0)
            {
                //prefab.Renderers.ForEach(x => Main.DebugLog("tex? "+x.material?.mainTexture?.name));
                isprefab = true;

                //prefab.GetComponentsInChildren<SkinnedMeshRenderer>().ToList().ForEach(x => Main.DebugLog("tex? " + x.material?.mainTexture?.name));

                Tuple<string, string> result2 = new Tuple<string, string>("", "");
                //if (!string.IsNullOrEmpty(__instance.Descriptor.CustomPrefabGuid))
                if (unit.CustomizationPreset != null || Main.customPrefabUnits.ContainsKey(unit.name))
                {

                    result2 = Main.randomPool(unit, prefab);
                }

                prefab = Main.unitEntityViewTexReplacer(prefab, result2.Item1, result2.Item2);


            }
            else
            {
                Main.DebugLog("noren");

            }

            //var named = (first: "one", second: "two");




            UnitEntityView unitEntityView = UnityEngine.Object.Instantiate<UnitEntityView>(prefab, position, rotation);

            if (!isprefab)
            {
                Tuple<string, string> result = new Tuple<string, string>("", "");
                //if (!string.IsNullOrEmpty(__instance.Descriptor.CustomPrefabGuid))
                if (unit.CustomizationPreset != null || Main.customPrefabUnits.ContainsKey(unit.name))
                {

                    result = Main.randomPool(unit, unitEntityView);
                }

                unitEntityView = Main.unitEntityViewTexReplacer(unitEntityView, result.Item1, result.Item2);
            }


            //  unitEntityView.Destroy();



#if DEBUG
    
            Main.DebugLog("SpawnUnit(): Main.OrigTexName: " + Main.OrigTexName);
            if (unit.AddFacts.Length > 0)
            {
                Array.Clear(unit.AddFacts, 0, unit.AddFacts.Length);

            }
#endif



            unitEntityView.Blueprint = unit;

            __result = (UnitEntityData)__instance.SpawnEntityWithView(unitEntityView, state);

            /*
            if (__result.View.CharacterAvatar != null)
            {

                if (__result.View.CharacterAvatar != null && __result.View.CharacterAvatar.BakedCharacter != null && __result.View.CharacterAvatar.BakedCharacter.RendererDescriptions.Count() > 0 && __result.View.CharacterAvatar.BakedCharacter.RendererDescriptions.Any(x => (x.Material?.mainTexture?.width < 500 || x.Material?.mainTexture?.height < 500) && x.Material?.mainTexture?.name == Main.OrigTexName))
                {
                    Main.DebugLog("SpawnUnit/ after SpawnEntityView CHARAVATAR TRIGGERED");
                    foreach (RendererDescription rd in __result.View.CharacterAvatar.BakedCharacter.RendererDescriptions)
                    {
                        if (rd.Material?.mainTexture?.name == Main.OrigTexName)
                        {
                            rd.Material.mainTexture = Main.ReadableText;
                            rd.Material.mainTexture.name = Main.OrigTexName;
                        }

                    }

                }

            }
          Main.OrigTexName = null;
             */



           // Main.ReadableText = null;


            Main.preset = null;
            Main.notRandom = false;

            return false;

        }
    }

    [HarmonyPatch(typeof(EntityCreationController), "SpawnUnit", new Type[] { typeof(BlueprintUnit), typeof(UnitEntityView), typeof(Vector3), typeof(Quaternion), typeof(SceneEntitiesState) })]
    public static class EntityCreationController_SpawnUnit_Patch_old
    {
        // Token: 0x06000038 RID: 56 RVA: 0x0000227B File Offset: 0x0000047B
        private static bool Prefix(EntityCreationController __instance, BlueprintUnit unit, UnitEntityView prefab, Vector3 position, Quaternion rotation, SceneEntitiesState state, ref UnitEntityData __result)
        {
            if (!Main.modEnabled)
            {
                return true;
            }
            if (unit == null)
            {

                Main.DebugLog("Trying to spawn null unit.");
                return false;
            }
            if (prefab == null)
            {
                Main.DebugLog("Trying to spawn unit without prefab");
                return false;
            }


           // Main.DebugLog("SpawnUnit() -  " + unit.CharacterName + " - " + unit.name + " - " + unit.AssetGuid);


            prefab.UniqueId = Guid.NewGuid().ToString();








            UnitEntityView unitEntityView = UnityEngine.Object.Instantiate<UnitEntityView>(prefab, position, rotation);


            

            string texfullpath = "";
            //if (!string.IsNullOrEmpty(__instance.Descriptor.CustomPrefabGuid))
            if(unit.CustomizationPreset != null)
            {
                
               // texfullpath = Main.randomPool(unit, "");
            }

            //unitEntityView = Main.unitEntityViewTexReplacer(unitEntityView, texfullpath);


            /*
            string origtexname = "";
            string texfullpath = "";
     

            if (unitEntityView.GetComponentsInChildren<SkinnedMeshRenderer>().Count() > 0)
            {
                foreach (SkinnedMeshRenderer smr in unitEntityView.GetComponentsInChildren<SkinnedMeshRenderer>())
                {

                    if (smr.material != null && !string.IsNullOrEmpty(smr.material?.mainTexture?.name))
                    {

                        if (Main.texOnDiskInfoList == null || Main.texOnDiskInfoList.Count() == 0)
                        {
                            Main.Init();
                        }

                        if (Main.texOnDiskInfoList.Keys.Any(key => key.Contains(!string.IsNullOrEmpty(smr.material?.mainTexture?.name) ? smr.material?.mainTexture?.name : "noname")))
                        {

                            texfullpath = Path.Combine(Main.hqTexPath, smr.material.mainTexture.name + ".png");

                            origtexname = smr.material.mainTexture.name;


                            break;
                        }

                    }

                }


                if (!string.IsNullOrEmpty(texfullpath) && !string.IsNullOrEmpty(origtexname))
                {

                    try
                    {
                        byte[] array = File.ReadAllBytes(texfullpath);
                        Vector2Int v2 = Main.texOnDiskInfoList[texfullpath];
  
                        Texture2D texture2D = new Texture2D(v2.x, v2.y, TextureFormat.ARGB32, false);
                        texture2D.filterMode = FilterMode.Point;

                        texture2D.anisoLevel = 9;
                        ImageConversion.LoadImage(texture2D, array);

                        RenderTexture renderTex = RenderTexture.GetTemporary(
                                             texture2D.width,
                                             texture2D.height,
                                             32,
                                             RenderTextureFormat.ARGB32,
                                             RenderTextureReadWrite.sRGB);

                        renderTex.antiAliasing = 8;
                        renderTex.anisoLevel = 9;
 
                        renderTex.filterMode = FilterMode.Trilinear;
                        Graphics.Blit(texture2D, renderTex);
                        RenderTexture previous = RenderTexture.active;
                        RenderTexture.active = renderTex;

                        Texture2D readableText = new Texture2D(texture2D.width, texture2D.height);
                        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
                        readableText.Apply();
                        RenderTexture.active = previous;
                        RenderTexture.ReleaseTemporary(renderTex);

 
                        if (texture2D != null)
                        {

                            string tname = "";

                            unitEntityView.GetComponentsInChildren<SkinnedMeshRenderer>().First(x => ((tname = x.material.mainTexture.name) == origtexname)).material.mainTexture = readableText;
                        }
                    }
                    catch (Exception x) { Main.DebugLog(x.ToString()); }
                }
            }
            else { Main.DebugLog("no renderers in unitentityview"); }
            */



            unitEntityView.Blueprint = unit;
  
            __result = (UnitEntityData)__instance.SpawnEntityWithView(unitEntityView, state);

            return false;

        }
    }

}

