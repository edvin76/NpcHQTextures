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


 



    }


}

