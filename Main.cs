using UnityModManagerNet;
using System;
using System.Linq;
using Kingmaker.Blueprints;
using Kingmaker;
using UnityEngine;
using System.IO;
using Harmony12;
using System.Collections.Generic;
using System.Reflection.Emit;
using Kingmaker.View;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic.Customization;

namespace NpcHQTextures
{
#if DEBUG
    [EnableReloading]
#endif
    public class Main
    {

        public static Dictionary<string, Vector2Int> texOnDiskInfoList;


        
        public static string hqTexPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Mods","NpcHQTextures","HQTex");


   
        public static void Init()
        {

            Main.texOnDiskInfoList = new Dictionary<string, Vector2Int>();

            string hqDir = Main.hqTexPath;

            List<string> texturePathList = Directory.GetFiles(hqDir, "*.png", SearchOption.AllDirectories).ToList();




            texturePathList.ForEach(x => Main.texOnDiskInfoList.Add(x, ImageHeader.GetDimensions(x)));

        }

        public static UnitEntityView unitEntityViewTexReplacer(UnitEntityView unitEntityView, string texfullpath)
        {
            

            string origtexname = "";
      

            if (unitEntityView != null && unitEntityView.GetComponentsInChildren<SkinnedMeshRenderer>().Count() > 0)
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

                            if (string.IsNullOrEmpty(texfullpath))
                            {
                                texfullpath = Main.texOnDiskInfoList.Keys.First(key => key.Contains(smr.material.mainTexture.name));
                            }

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

                            string tname = "none";

                            unitEntityView.GetComponentsInChildren<SkinnedMeshRenderer>().First(x => ((tname = x.material.mainTexture.name) == origtexname)).material.mainTexture = readableText;

                          //  Main.DebugLog(tname);

                        }
                    }
                    catch (Exception x) { Main.DebugLog(x.ToString()); }
                }

            }



            return unitEntityView;
        }
        public static string randomPool(BlueprintUnit blueprintUnit, string customPrefabGuid)
        {

            string path2 = Main.hqTexPath;

           // string texfullpath = "";
            string presetName = blueprintUnit.CustomizationPreset.name;
            UnitCustomizationPreset preset = blueprintUnit.CustomizationPreset;
            BlueprintUnit unit = blueprintUnit;
            BlueprintUnit protoType = blueprintUnit.PrototypeLink as BlueprintUnit;


            int pf = 0;
            string fileName = "";

            if (string.IsNullOrEmpty(customPrefabGuid))
            {
                customPrefabGuid = protoType.Prefab.AssetId;
            }
            


            UnitEntityView unitEntityView = ResourcesLibrary.TryGetResource<UnitEntityView>(customPrefabGuid, false);

            if (unitEntityView != null && unitEntityView.GetComponentsInChildren<SkinnedMeshRenderer>().Count() > 0)
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

                            fileName = smr.material.mainTexture.name;

                            Main.DebugLog("rp: "+fileName);
                        }
                    }
                }
            }

                /*            foreach (UnitEntityData u in Game.Instance.DialogController.InvolvedUnits)
            {
                if (u.Blueprint.name == unit.name)
                {
                    if (!u.View.name.Contains("(Clone)") && u.View.CharacterAvatar != null && u.View.CharacterAvatar.BakedCharacter != null && !string.IsNullOrEmpty(u.View.CharacterAvatar.BakedCharacter.name) && u.View.CharacterAvatar.BakedCharacter.name.StartsWith("0"))
                    {
                        fileName = u.View.CharacterAvatar.BakedCharacter.name;
                    }
                    else if (u.View.name.Contains("(Clone)"))
                    {
                        fileName = u.View.name.Substring(0, u.View.name.Length - "(Clone)".Length);
                    }
                    else
                    {
                        fileName = u.View.name;
                    }
                }
            }*/

            if (preset.Units.Contains(protoType) || preset.Units.Contains(unit))
            {

                foreach (UnitVariations uvs in preset.UnitVariations)
                {
                    pf++;
                    if (uvs.Units.Contains(protoType) || uvs.Units.Contains(unit))
                    {

                        if (preset.AssetGuid == "349505290e20b4441b3c25bf9c58bf3d")
                        {
                            break;
                        }
                        else if (preset.AssetGuid == "a8351a67ec06e9244a318a488ca15151")
                        {
                            bool flag33 = false;
                            foreach (UnitCustomizationVariation ucv in uvs.Variations)
                            {

                                if (ucv.Prefab.Load().name.Contains(fileName))
                                {
                                    flag33 = true;
                                    break;
                                }
                            }
                            if (!flag33)
                            {
                                preset = ResourcesLibrary.TryGetBlueprint<UnitCustomizationPreset>("349505290e20b4441b3c25bf9c58bf3d");
                                presetName = preset.name;
                            }


                            break;
                        }
                        break;
                    }

                }
            }
            else if (preset.AssetGuid == "a8351a67ec06e9244a318a488ca15151")
            {
                preset = ResourcesLibrary.TryGetBlueprint<UnitCustomizationPreset>("349505290e20b4441b3c25bf9c58bf3d");

                presetName = preset.name;

                pf = 1;
                bool flag = false;

                if (unit.PrototypeLink.name == "CR0_BanditWarriorMeleeLevel1")
                {
                    pf = 1;
                    flag = true;

                }
                if (unit.PrototypeLink.name == "CR0_BanditWarriorRangedLevel1")
                {
                    pf = 1;
                    flag = true;

                }


                if (unit.PrototypeLink.name == "CR2_BanditWarriorMeleeLevel4")
                {
                    pf = 1;
                    flag = true;

                }
                if (unit.PrototypeLink.name == "CR3_BanditRogueRangedLevel4")
                {
                    pf = 4;
                    flag = true;

                }
                if (unit.PrototypeLink.name == "CR3_BanditRogueMeleeLevel4")
                {
                    pf = 4;
                    flag = true;

                }
                if (unit.PrototypeLink.name == "CR4_BanditRogueRangedLevel5")
                {
                    pf = 4;
                    flag = true;

                }
                if (unit.PrototypeLink.name == "CR4_BanditRogueMeleeLevel5")
                {
                    pf = 4;
                    flag = true;

                }
                if (unit.PrototypeLink.name == "CR5_BanditRogueRangedLevel6")
                {
                    pf = 4;
                    flag = true;

                }
                if (unit.PrototypeLink.name == "CR5_BanditRogueMeleeLevel6")
                {
                    pf = 4;
                    flag = true;

                }
                if (unit.PrototypeLink.name == "CR6_BanditRogueRangedLevel7")
                {
                    pf = 4;
                    flag = true;

                }
                if (unit.PrototypeLink.name == "CR6_BanditRogueMeleeLevel7")
                {
                    pf = 4;
                    flag = true;

                }
                if (unit.PrototypeLink.name == "CR7_BanditAlchemistLevel8")
                {
                    //preset2 = "Bandit_Alchemist";
                    pf = 4;
                    flag = true;

                }


                if (unit.PrototypeLink.name == "CR3_BanditFighterMeleeLevel4")
                {
                    pf = 3;
                    flag = true;
                }
                if (unit.PrototypeLink.name == "CR3_BanditFighterRangedLevel4")
                {
                    pf = 3;
                    flag = true;
                }
                if (unit.PrototypeLink.name == "CR4_BanditFighterMeleeLevel5")
                {
                    pf = 3;
                    flag = true;
                }
                if (unit.PrototypeLink.name == "CR4_BanditFighterRangedLevel5")
                {
                    pf = 3;
                    flag = true;
                }
                if (unit.PrototypeLink.name == "CR5_BanditFighterMeleeLevel6")
                {
                    pf = 3;
                    flag = true;
                }
                if (unit.PrototypeLink.name == "CR5_BanditFighterRangedLevel6")
                {
                    pf = 3;
                    flag = true;
                }
                if (unit.PrototypeLink.name == "CR6_BanditFighterMeleeLevel7")
                {
                    pf = 3;
                    flag = true;
                }
                if (unit.PrototypeLink.name == "CR6_BanditFighterRangedLevel7")
                {
                    pf = 3;
                    flag = true;
                }
                if (unit.PrototypeLink.name == "CR7_BanditFighterMeleeLevel8")
                {
                    pf = 3;
                    flag = true;
                }
                if (unit.PrototypeLink.name == "CR7_BanditFighterRangedLevel8")
                {
                    pf = 3;
                    flag = true;
                }
                if (!flag) Main.DebugLog("add " + unit.PrototypeLink.name + " to bandit0");

            }
            else if (preset.AssetGuid == "cc09d457355ef3346ae7103a4d40a718")
            {
                if (unit.PrototypeLink.name == "CR2_BanditNecromancerLevel3")
                {
                    pf = 2;
                }
                if (unit.PrototypeLink.name == "CR6_BanditBardLevel7")
                {
                    pf = 1;
                }

            }
            else if (preset.AssetGuid == "9000d6ca929859848903e2721c1a635c")
            {
                pf = 1;
            }
            else if (preset.AssetGuid == "3272ad72ab8720c4eaa778e56ecdd771")
            {
                if (unit.PrototypeLink.name == "HuntingParty_VandalFighterMelee" || unit.PrototypeLink.name == "Vandal_Fighter")
                {
                    pf = 2;

                }
            }
            else
            {
                Main.DebugLog("variation is not in preset");
            }

            string dirInPreset = Path.Combine(presetName, pf.ToString(), fileName);

            string fileFullPath = Path.Combine(path2, dirInPreset + ".png");

            return fileFullPath;

        }
        static bool Load(UnityModManager.ModEntry modEntry)
        {


            try
            {
                Main.logger = modEntry.Logger;
                Main.modEnabled = modEntry.Active;
 
              
                modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(Main.OnToggle);
              //  modEntry.OnGUI = new Action<UnityModManager.ModEntry>(Main.OnGUI);


                Main.harmonyInstance = HarmonyInstance.Create(modEntry.Info.Id);
                modEntry.OnUnload = new Func<UnityModManager.ModEntry, bool>(Main.Unload);

            }
            catch (Exception ex)
            {
                DebugError(ex);
                throw ex;
            }

            if (!Main.ApplyPatch(typeof(EntityCreationController_SpawnUnit_Patch), "EntityCreationController_SpawnUnit_Patch_Patch"))
            {
                DebugLog("Failed to patch EntityCreationController_SpawnUnit");
            }
             if (!Main.ApplyPatch(typeof(UnitEntityData_CreateView_Patch), "UnitEntityData_CreateView_Patch"))
                 {
                     DebugLog("Failed to patch UnitEntityData_CreateView");
                 }

           if (!Main.ApplyPatch(typeof(LibraryScriptableObject_LoadDictionary_Patch), "Run once startup hook for Startup AssetLoader Example1 "))
            {
                DebugLog("Failed to patch LibraryScriptableObject_LoadDictionary");
            }

            
            return true;
        }
        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayoutOption[] noExpandwith = new GUILayoutOption[]
             {
                GUILayout.ExpandWidth(false)
             };




            if (Main.okPatches.Count > 0)
            {
                GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());

                GUILayout.Label("<b>OK: Some patches apply:</b>", noExpandwith);

                foreach (string str in Main.okPatches)
                {
                    GUILayout.Label("  • <b>" + str + "</b>", noExpandwith);
                }
                GUILayout.EndVertical();
            }
            if (Main.failedPatches.Count > 0)
            {
                GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
                GUILayout.Label("<b>Error: Some patches failed to apply. These features may not work:</b>", noExpandwith);
                foreach (string str2 in Main.failedPatches)
                {
                    GUILayout.Label("  • <b>" + str2 + "</b>", noExpandwith);
                }
                GUILayout.EndVertical();
            }
            if (Main.okLoading.Count > 0)
            {
                GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
                GUILayout.Label("<b>OK: Some assets loaded:</b>", noExpandwith);
                foreach (string str3 in Main.okLoading)
                {
                    GUILayout.Label("  • <b>" + str3 + "</b>", noExpandwith);
                }
                GUILayout.EndVertical();
            }
            if (Main.failedLoading.Count > 0)
            {
                GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
                GUILayout.Label("<b>Error: Some assets failed to load. Saves using these features won't work:</b>", noExpandwith);
                foreach (string str4 in Main.failedLoading)
                {
                    GUILayout.Label("  • <b>" + str4 + "</b>", noExpandwith);
                }
                GUILayout.EndVertical();
            }
        } 
        internal static bool ApplyPatch(Type type, string featureName)
        {
            bool result;
            try
            {
                if (Main.typesPatched.ContainsKey(type))
                {
                    result = Main.typesPatched[type];
                }
                else
                {
                    List<HarmonyMethod> harmonyMethods = type.GetHarmonyMethods();
                    if (harmonyMethods == null || harmonyMethods.Count<HarmonyMethod>() == 0)
                    {

                        DebugLog("Failed to apply patch " + featureName + ": could not find Harmony attributes.");
                        Main.failedPatches.Add(featureName);
                        Main.typesPatched.Add(type, false);
                        result = false;
                    }
                    else if (new PatchProcessor(Main.harmonyInstance, type, HarmonyMethod.Merge(harmonyMethods)).Patch().FirstOrDefault<DynamicMethod>() == null)
                    {
                        DebugLog("Failed to apply patch " + featureName + ": no dynamic method generated");

                        Main.failedPatches.Add(featureName);
                        Main.typesPatched.Add(type, false);
                        result = false;
                    }
                    else
                    {
                        Main.okPatches.Add(featureName);
                        Main.typesPatched.Add(type, true);
                        result = true;
                    }
                }
            }
            catch (Exception arg)
            {
                DebugLog("Failed to apply patch " + featureName + ": " + arg + ", type: " + type);
                Main.failedPatches.Add(featureName);
                Main.typesPatched.Add(type, false);
                result = false;
            }
            return result;
        }
        internal static void SafeLoad(Action load, string name)
        {
            try
            {
                load();
                Main.okLoading.Add(name);
            }
            catch (Exception e)
            {
                Main.okLoading.Remove(name);
                Main.failedLoading.Add(name);
                DebugError(e);
            }
        }


        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            Main.modEnabled = value;
           return true;
        }

        // Token: 0x06000026 RID: 38 RVA: 0x000021B3 File Offset: 0x000003B3
        private static bool Unload(UnityModManager.ModEntry modEntry)
        {
              if (Main.okPatches.Count > 0)
            {
                /*if (Main.spawner != null)
                {
                    Main.spawner.Stop();
                    Main.spawner.StopAllCoroutines();
                }*/
            harmonyInstance.UnpatchAll(modEntry.Info.Id);

                //HarmonyInstance.Create(modEntry.Info.Id).UnpatchAll(null);
                return true;
            }
            else { UnityModManager.Logger.Log("couldn't find patches to unload!"); return true; }
        }





        

        public static void DebugLog(string msg)
        {
            if (logger != null) logger.Log(msg);
        }
        public static void DebugError(Exception ex)
        {
            if (logger != null) logger.Log(ex.ToString() + "\n" + ex.StackTrace);
        }


        public static UnityModManagerNet.UnityModManager.ModEntry.ModLogger logger;
        public static bool modEnabled;
        private static HarmonyInstance harmonyInstance;
        private static readonly Dictionary<Type, bool> typesPatched = new Dictionary<Type, bool>();
        private static readonly List<string> failedPatches = new List<string>();
        private static readonly List<string> okPatches = new List<string>();
        private static readonly List<string> okLoading = new List<string>();
        private static readonly List<string> failedLoading = new List<string>();

        [Harmony12.HarmonyPatch(typeof(LibraryScriptableObject), "LoadDictionary")]
        public static class LibraryScriptableObject_LoadDictionary_Patch
        {
            static void Postfix(LibraryScriptableObject __instance)
            {

                Main.SafeLoad(new Action(Main.Init), "Example1 of Startup Asset Load Here");
            }
        }




    }
}
