#if UNITY_EDITOR
using Assets.Scripts.Loadouts.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.EditorTools
{
    /// <summary>
    /// One-shot builder for the RosterSlotUI prefab (phase 4.2.4).
    /// Run once, then tweak the prefab by hand: running it again replaces the prefab.
    /// </summary>
    public static class RosterSlotPrefabBuilder
    {
        private const string LogPrefix = "[RosterSlotPrefabBuilder]";
        private const string PrefabFolder = "Assets/Prefabs/UIs";
        private const string PrefabPath = PrefabFolder + "/RosterSlotUI.prefab";

        private const float SlotHeight = 70f;
        private const float ToggleWidth = 160f;
        private const float Padding = 8f;

        private static readonly Color SlotColor = new Color(0.22f, 0.24f, 0.28f);
        private static readonly Color SelectedColor = new Color(1f, 0.82f, 0.25f, 0.35f);
        private static readonly Color SelectButtonColor = new Color(1f, 1f, 1f, 0.06f);
        private static readonly Color TextColor = new Color(0.92f, 0.92f, 0.92f);


        [MenuItem("Tools/Build Roster Slot Prefab")]
        private static void Build()
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath) != null &&
                !EditorUtility.DisplayDialog("Build Roster Slot Prefab",
                    $"{PrefabPath} already exists.\nReplace it? Hand edits will be lost, " +
                    "and RosterListUI.slotPrefab may need to be dragged in again.",
                    "Replace", "Cancel"))
                return;

            TMP_DefaultControls.Resources uiResources = new TMP_DefaultControls.Resources
            {
                standard = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd"),
                background = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd"),
            };

            // Build in a preview scene so the open scene is not touched or marked dirty
            Scene previewScene = EditorSceneManager.NewPreviewScene();

            try
            {
                GameObject root = CreateSlot(uiResources, previewScene);
                SaveAsPrefab(root);
            }
            finally
            {
                EditorSceneManager.ClosePreviewScene(previewScene);
            }
        }

        private static GameObject CreateSlot(TMP_DefaultControls.Resources uiResources, Scene previewScene)
        {
            // RosterSlotUI            (Image, LayoutElement, RosterSlotUI)
            // ├─ SelectedVisual       (behind the buttons, hidden)
            // ├─ SelectButton
            // │  └─ NameText
            // └─ ToggleButton
            //    └─ ToggleText
            GameObject root = new GameObject("RosterSlotUI", typeof(RectTransform));
            SceneManager.MoveGameObjectToScene(root, previewScene);

            RectTransform rootRect = (RectTransform)root.transform;
            rootRect.sizeDelta = new Vector2(600f, SlotHeight);

            Image background = root.AddComponent<Image>();
            background.sprite = uiResources.background;
            background.type = Image.Type.Sliced;
            background.color = SlotColor;

            // Height comes from here: RosterContainer's VerticalLayoutGroup controls child height
            root.AddComponent<LayoutElement>().preferredHeight = SlotHeight;

            GameObject selectedVisual = CreateSelectedVisual(root.transform, uiResources);
            Button selectButton = CreateSelectButton(root.transform, uiResources, out TextMeshProUGUI nameText);
            Button toggleButton = CreateToggleButton(root.transform, uiResources, out TextMeshProUGUI toggleText);

            RosterSlotUI slot = root.AddComponent<RosterSlotUI>();
            SerializedObject serialized = new SerializedObject(slot);
            SetReference(serialized, "selectButton", selectButton);
            SetReference(serialized, "nameText", nameText);
            SetReference(serialized, "toggleButton", toggleButton);
            SetReference(serialized, "toggleText", toggleText);
            SetReference(serialized, "selectedVisual", selectedVisual);
            serialized.ApplyModifiedPropertiesWithoutUndo();

            SetLayerRecursively(root, LayerMask.NameToLayer("UI"));
            return root;
        }

        // First child -> drawn behind the buttons. No raycast, or it would swallow the clicks.
        private static GameObject CreateSelectedVisual(Transform parent, TMP_DefaultControls.Resources uiResources)
        {
            RectTransform rect = CreateRect("SelectedVisual", parent);
            StretchFull(rect, Vector2.zero, Vector2.zero);

            Image image = rect.gameObject.AddComponent<Image>();
            image.sprite = uiResources.background;
            image.type = Image.Type.Sliced;
            image.color = SelectedColor;
            image.raycastTarget = false;

            rect.gameObject.SetActive(false);
            return rect.gameObject;
        }

        // Fills the slot left of the toggle button
        private static Button CreateSelectButton(Transform parent, TMP_DefaultControls.Resources uiResources, out TextMeshProUGUI nameText)
        {
            GameObject buttonObject = CreateButton("SelectButton", parent, uiResources, "Soldier", out nameText);
            nameText.name = "NameText";
            nameText.alignment = TextAlignmentOptions.MidlineLeft;
            nameText.color = TextColor;
            nameText.margin = new Vector4(16f, 0f, 16f, 0f);

            buttonObject.GetComponent<Image>().color = SelectButtonColor;

            RectTransform rect = (RectTransform)buttonObject.transform;
            StretchFull(rect, new Vector2(Padding, Padding), new Vector2(-(ToggleWidth + Padding * 2f), -Padding));

            return buttonObject.GetComponent<Button>();
        }

        // Pinned to the right edge
        private static Button CreateToggleButton(Transform parent, TMP_DefaultControls.Resources uiResources, out TextMeshProUGUI toggleText)
        {
            GameObject buttonObject = CreateButton("ToggleButton", parent, uiResources, "ADD", out toggleText);
            toggleText.name = "ToggleText";

            RectTransform rect = (RectTransform)buttonObject.transform;
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.sizeDelta = new Vector2(ToggleWidth, -Padding * 2f);
            rect.anchoredPosition = new Vector2(-Padding, 0f);

            return buttonObject.GetComponent<Button>();
        }

        // ---------- Helpers ----------

        private static void SaveAsPrefab(GameObject root)
        {
            if (!AssetDatabase.IsValidFolder(PrefabFolder))
            {
                Debug.LogError($"{LogPrefix} folder {PrefabFolder} not found, nothing was saved");
                return;
            }

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, PrefabPath, out bool saved);
            if (!saved)
            {
                Debug.LogError($"{LogPrefix} failed to save {PrefabPath}");
                return;
            }

            Debug.Log($"{LogPrefix} built {PrefabPath}", prefab);
            EditorGUIUtility.PingObject(prefab);
        }

        private static RectTransform CreateRect(string objectName, Transform parent)
        {
            GameObject uiObject = new GameObject(objectName, typeof(RectTransform));
            uiObject.transform.SetParent(parent, false);
            return (RectTransform)uiObject.transform;
        }

        // Same button as GameObject > UI > Button - TextMeshPro
        private static GameObject CreateButton(string objectName, Transform parent, TMP_DefaultControls.Resources uiResources,
            string label, out TextMeshProUGUI text)
        {
            GameObject button = TMP_DefaultControls.CreateButton(uiResources);
            button.name = objectName;
            button.transform.SetParent(parent, false);

            text = button.GetComponentInChildren<TextMeshProUGUI>();
            text.text = label;
            text.fontSize = 26;
            text.raycastTarget = false;   // the button's Image catches the click

            return button;
        }

        private static void StretchFull(RectTransform rect, Vector2 offsetMin, Vector2 offsetMax)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        private static void SetReference(SerializedObject serialized, string propertyName, Object value)
        {
            SerializedProperty property = serialized.FindProperty(propertyName);
            if (property == null)
            {
                Debug.LogError($"{LogPrefix} {serialized.targetObject.GetType().Name} has no field '{propertyName}' (renamed?)");
                return;
            }

            property.objectReferenceValue = value;
        }

        private static void SetLayerRecursively(GameObject root, int layer)
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
                child.gameObject.layer = layer;
        }
    }
}
#endif
