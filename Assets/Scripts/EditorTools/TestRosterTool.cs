#if UNITY_EDITOR
using Assets.Scripts.Soldiers;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.EditorTools
{
    /// <summary>
    /// Creates a throwaway roster with many soldiers (e.g. to test UI overflow) in its own folder,
    /// so the real roster and soldier assets are never touched. Delete removes the whole folder.
    /// </summary>
    public static class TestRosterTool
    {
        private const string LogPrefix = "[TestRosterTool]";
        private const string ParentFolder = "Assets/Data";
        private const string FolderName = "_Test";
        private const string Folder = ParentFolder + "/" + FolderName;
        private const int TestSoldierCount = 20;


        [MenuItem("Tools/Test Data/Create Test Roster")]
        private static void Create()
        {
            if (AssetDatabase.IsValidFolder(Folder))
            {
                if (!EditorUtility.DisplayDialog("Create Test Roster",
                        $"{Folder} already exists. Delete it and create it again?", "Recreate", "Cancel"))
                    return;

                AssetDatabase.DeleteAsset(Folder);
            }

            RosterSO realRoster = FindRealRoster();
            if (realRoster == null)
                return;

            List<SoldierSO> realSoldiers = realRoster.Soldiers.Where(s => s != null).ToList();
            if (realSoldiers.Count == 0)
            {
                Debug.LogError($"{LogPrefix} {realRoster.name} has no soldiers to copy from", realRoster);
                return;
            }

            AssetDatabase.CreateFolder(ParentFolder, FolderName);

            // Real soldiers first so the default squad looks the same as usual
            List<SoldierSO> soldiers = new List<SoldierSO>(realSoldiers);

            for (int i = 1; i <= TestSoldierCount; i++)
            {
                // Copy keeps unitConfig, traits and defaults valid, only the name changes
                SoldierSO template = realSoldiers[(i - 1) % realSoldiers.Count];
                SoldierSO soldier = Object.Instantiate(template);

                SerializedObject serialized = new SerializedObject(soldier);
                serialized.FindProperty("displayName").stringValue = $"Test Soldier {i:00}";
                serialized.ApplyModifiedPropertiesWithoutUndo();

                AssetDatabase.CreateAsset(soldier, $"{Folder}/TestSoldier{i:00}.asset");
                soldiers.Add(soldier);
            }

            RosterSO testRoster = ScriptableObject.CreateInstance<RosterSO>();
            AssetDatabase.CreateAsset(testRoster, $"{Folder}/TestRoster.asset");

            SerializedObject rosterSerialized = new SerializedObject(testRoster);
            SerializedProperty list = rosterSerialized.FindProperty("soldiers");
            list.arraySize = soldiers.Count;
            for (int i = 0; i < soldiers.Count; i++)
                list.GetArrayElementAtIndex(i).objectReferenceValue = soldiers[i];
            rosterSerialized.ApplyModifiedPropertiesWithoutUndo();

            AssetDatabase.SaveAssets();

            Debug.Log($"{LogPrefix} created {Folder}/TestRoster.asset with {soldiers.Count} soldiers " +
                      $"({realSoldiers.Count} real + {TestSoldierCount} test). Assign it to LoadoutScreen.roster to use it.", testRoster);
            EditorGUIUtility.PingObject(testRoster);
        }

        [MenuItem("Tools/Test Data/Delete Test Roster")]
        private static void Delete()
        {
            if (!AssetDatabase.IsValidFolder(Folder))
            {
                Debug.Log($"{LogPrefix} nothing to delete, {Folder} does not exist");
                return;
            }

            AssetDatabase.DeleteAsset(Folder);
            Debug.Log($"{LogPrefix} deleted {Folder}. Point LoadoutScreen.roster back at the real roster.");
        }

        private static RosterSO FindRealRoster()
        {
            List<RosterSO> rosters = AssetDatabase.FindAssets("t:RosterSO")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Where(path => !path.StartsWith(Folder + "/"))
                .Select(path => AssetDatabase.LoadAssetAtPath<RosterSO>(path))
                .Where(roster => roster != null)
                .ToList();

            if (rosters.Count == 0)
            {
                Debug.LogError($"{LogPrefix} no RosterSO found outside {Folder}");
                return null;
            }

            if (rosters.Count > 1)
                Debug.LogWarning($"{LogPrefix} found {rosters.Count} rosters, copying from {rosters[0].name}", rosters[0]);

            return rosters[0];
        }
    }
}
#endif
