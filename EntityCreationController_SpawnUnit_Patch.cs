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
using UnityModManagerNet;
using Vector3 = UnityEngine.Vector3;

namespace NpcHQTextures
{



    [HarmonyPatch(typeof(EntityCreationController), "SpawnUnit", new Type[] { typeof(BlueprintUnit), typeof(Vector3), typeof(Quaternion), typeof(SceneEntitiesState) })]
    public static class EntityCreationController_SpawnUnit0_Patch
    {
        // Token: 0x06000038 RID: 56 RVA: 0x0000227B File Offset: 0x0000047B
        private static bool Prefix(EntityCreationController __instance, BlueprintUnit unit, Vector3 position, Quaternion rotation, SceneEntitiesState state, ref UnitEntityData __result)
        {


            Main.DebugLog("Spawning0: " + unit.name);

            return true;
        }
    }



            [HarmonyPatch(typeof(EntityCreationController), "SpawnUnit", new Type[] { typeof(BlueprintUnit), typeof(UnitEntityView), typeof(Vector3), typeof(Quaternion), typeof(SceneEntitiesState) })]
    public static class EntityCreationController_SpawnUnit_Patch
    {
        // Token: 0x06000038 RID: 56 RVA: 0x0000227B File Offset: 0x0000047B
        private static bool Prefix(EntityCreationController __instance, BlueprintUnit unit, ref UnitEntityView prefab, Vector3 position, Quaternion rotation, SceneEntitiesState state, ref UnitEntityData __result)
        {

            Main.DebugLog("Spawning: " + unit.name);

            string path = Main.randomPool(unit, prefab);

            if(path.Length > 3 && !path.Equals(Main.hqTexPath))
            prefab = Main.unitEntityViewTexReplacer(prefab, path, Path.GetFileNameWithoutExtension(path));

            /*
            foreach (SkinnedMeshRenderer smr in prefab.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (smr.material != null)
                {
                    try
                    {
                        if (path.Length < 3)
                            path = Path.Combine(GetTexturesDir(), smr.material.mainTexture.name + ".png");

                        Main.DebugLog("Spawner looking for: " + path);

                        string textureName = smr.material.mainTexture.name;

                        if (File.Exists(path))
                        {

                            Main.DebugLog("Spawner: texture Found!!!");

                            Texture2D baseMap = ReadTexture(path, 1024, 1024);


                            if (baseMap != null && baseMap.GetRawTextureData().Length > 1)
                            {
                                smr.material.SetTexture("_BaseMap", baseMap);
                                
                                smr.material.mainTexture = baseMap;
                                smr.material.mainTexture.name = textureName;
                            }
                        }
                        else
                            Main.DebugLog("Spawner: texture NOT Found!!!");

                    }
                    catch (Exception x) { Main.DebugError(x); }
                }
            }
            */


            return true;

        }


        public static string GetTexturesDir()
        {
            return Path.Combine(UnityModManager.modsPath, Main.harmonyInstance.Id, "HQTex");

        }


        public static Texture2D ReadTexture(string path, int x, int y)
        {
            byte[] array = File.ReadAllBytes(path);


            Texture2D texture2D = new Texture2D(x, y, TextureFormat.ARGB32, true);

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

            return readableText;

        }

    }


}

