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
using Kingmaker.View;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace NpcHQTextures
{
    [HarmonyPatch(typeof(EntityCreationController), "SpawnUnit", new Type[] { typeof(BlueprintUnit), typeof(UnitEntityView), typeof(Vector3), typeof(Quaternion), typeof(SceneEntitiesState) })]
    public static class EntityCreationController_SpawnUnit_Patch
    {
        // Token: 0x06000038 RID: 56 RVA: 0x0000227B File Offset: 0x0000047B
        private static bool Prefix(EntityCreationController __instance, BlueprintUnit unit, UnitEntityView prefab, Vector3 position, Quaternion rotation, SceneEntitiesState state, ref UnitEntityData __result)
        {
            if (!Main.modEnabled)
            {
                return false;
            }
            if (unit == null)
            {

                Main.DebugLog("Trying to spawn null unit");
                return false;
            }
            if (prefab == null)
            {
                Main.DebugLog("Trying to spawn unit without prefab");
                return false;
            }


            Main.DebugLog("SpawnUnit() - " + unit.CharacterName + " - " + unit.name + " - " + unit.AssetGuid);


            prefab.UniqueId = Guid.NewGuid().ToString();








            UnitEntityView unitEntityView = UnityEngine.Object.Instantiate<UnitEntityView>(prefab, position, rotation);


            

            string texfullpath = "";
            //if (!string.IsNullOrEmpty(__instance.Descriptor.CustomPrefabGuid))
            if(unit.CustomizationPreset != null)
            {
                texfullpath = Main.randomPool(unit);
            }

            unitEntityView = Main.unitEntityViewTexReplacer(unitEntityView, texfullpath);


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

