using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace UltimateSurvival.Editor
{
	public class ItemManagementWindow : EditorWindow
	{
		public enum Tab { ItemEditor, PropertyEditor }

		private const float DESCRIPTION_HEIGHT = 54f;
		private const float PROPERTY_HEIGHT = 40f;
		private const float RAW_LIST_HEIGHT = 38f;

		private Tab m_SelectedTab;
		private SerializedObject m_ItemDatabase;
		private ReorderableList m_CategoryList;
		private ReorderableList m_PropertyList;

		private Vector2 m_CategoriesScrollPos;
		private Vector2 m_TypesScrollPos;
		private Vector2 m_PropsScrollPos;
		private Vector2 m_ItemsScrollPos;
		private Vector2 m_ItemInspectorScrollPos;

		private ReorderableList m_ItemList;
		private ReorderableList m_CurItemDescriptions;
		private ReorderableList m_CurItemProperties;
		private ReorderableList m_CurItemRequiredItems;

		private string[] m_ItemNamesFull;
		private string[] m_ItemNames;
	

		[MenuItem("Tools/Ultimate Survival/Item Management")]
		public static void Init()
		{
			EditorWindow.GetWindow<ItemManagementWindow>(true, "Item Management");
		}

		public void OnGUI()
		{
			if(m_ItemDatabase == null)
			{
				EditorGUILayout.HelpBox("No ItemDatabase was found in the Resources folder!", MessageType.Error);

				if(GUILayout.Button("Refresh"))
					InitializeWindow();

				if(m_ItemDatabase == null)
					return;
			}

			GUIStyle richTextStyle = new GUIStyle() { richText = true, alignment = TextAnchor.UpperRight };

			// Display the database path.
			EditorGUILayout.LabelField(string.Format("Database path: '{0}'", AssetDatabase.GetAssetPath(m_ItemDatabase.targetObject)));

			// Display the shortcuts
			EditorGUI.LabelField(new Rect(position.width - 262f, 0f, 256f, 16f), "<b>Shift + D</b> to duplicate", richTextStyle);
			EditorGUI.LabelField(new Rect(position.width - 262f, 16f, 256f, 16f), "<b>Delete</b> to delete", richTextStyle);

			Vector2 buttonSize = new Vector2(192f, 32f);
			float topPadding = 32f;

			// Draw the "Item Editor" button.
			Rect itemEditorButtonRect = new Rect(position.width * 0.25f - buttonSize.x / 2f, topPadding, buttonSize.x, buttonSize.y);

			if(m_SelectedTab == Tab.ItemEditor)
				GUI.backgroundColor = Color.grey;
			else
				GUI.backgroundColor = Color.white;
			
			if(GUI.Button(itemEditorButtonRect, "Item Editor"))
				m_SelectedTab = Tab.ItemEditor;

			// Draw the "Property Editor" button.
			Rect propertyEditorButtonRect = new Rect(position.width * 0.75f - buttonSize.x / 2f, topPadding, buttonSize.x, buttonSize.y);

			if(m_SelectedTab == Tab.PropertyEditor)
				GUI.backgroundColor = Color.grey;
			else
				GUI.backgroundColor = Color.white;

			if(GUI.Button(propertyEditorButtonRect, "Property Editor"))
				m_SelectedTab = Tab.PropertyEditor;

			// Reset the bg color.
			GUI.backgroundColor = Color.white;

			// Horizontal line.
			GUI.Box(new Rect(0f, topPadding + buttonSize.y * 1.25f, position.width, 1f), "");

			// Draw the item / recipe editors.
			m_ItemDatabase.Update();

			float innerWindowPadding = 8f;
			Rect innerWindowRect = new Rect(innerWindowPadding, topPadding + buttonSize.y * 1.25f + innerWindowPadding, position.width - innerWindowPadding * 2f, position.height - (topPadding + buttonSize.y * 1.25f + innerWindowPadding * 4.5f));

			// Inner window box.
			GUI.backgroundColor = Color.grey;
			GUI.Box(innerWindowRect, "");
			GUI.backgroundColor = Color.white;

			if(m_SelectedTab == Tab.ItemEditor)
				DrawItemEditor(innerWindowRect);
			else if(m_SelectedTab == Tab.PropertyEditor)
				DrawPropertyEditor(innerWindowRect);

			m_ItemDatabase.ApplyModifiedProperties();
		}

		private void OnEnable()
		{
			InitializeWindow();

			Undo.undoRedoPerformed += Repaint;
		}

		private void InitializeWindow()
		{
			var database = Resources.LoadAll<ItemDatabase>("")[0];

			if(database)
			{
				m_ItemDatabase = new SerializedObject(database);

				m_CategoryList = new ReorderableList(m_ItemDatabase, m_ItemDatabase.FindProperty("m_Categories"), true, true ,true ,true);
				m_CategoryList.drawElementCallback += DrawCategory;
				m_CategoryList.drawHeaderCallback = (Rect rect)=> { EditorGUI.LabelField(rect, ""); };
				m_CategoryList.onSelectCallback += On_SelectedCategory;
				m_CategoryList.onRemoveCallback = (ReorderableList list)=> { m_CategoryList.serializedProperty.DeleteArrayElementAtIndex(m_CategoryList.index); };

				m_PropertyList = new ReorderableList(m_ItemDatabase, m_ItemDatabase.FindProperty("m_ItemProperties"), true, true, true, true);
				m_PropertyList.drawElementCallback += DrawItemPropertyDefinition;
				m_PropertyList.drawHeaderCallback = (Rect rect)=> { EditorGUI.LabelField(rect, ""); };
			}
		}

		private void On_SelectedCategory(ReorderableList list)
		{
			m_ItemList = new ReorderableList(m_ItemDatabase, m_CategoryList.serializedProperty.GetArrayElementAtIndex(m_CategoryList.index).FindPropertyRelative("m_Items"), true, true, true, true);
			m_ItemList.drawElementCallback += DrawItem;
			m_ItemList.drawHeaderCallback = (Rect rect)=> { EditorGUI.LabelField(rect, ""); };
			m_ItemList.onSelectCallback += On_SelectedItem;
			m_ItemList.onRemoveCallback = (ReorderableList l)=> { m_ItemList.serializedProperty.DeleteArrayElementAtIndex(m_ItemList.index); };
			m_ItemList.onChangedCallback += On_SelectedItem;
		}
			
		private void On_SelectedItem(ReorderableList list)
		{
			if(m_ItemList == null || m_ItemList.count == 0 || m_ItemList.index == -1 || m_ItemList.index >= m_ItemList.count)
				return;

			m_ItemNames = ItemManagementUtility.GetItemNames(m_CategoryList.serializedProperty);
			m_ItemNamesFull = ItemManagementUtility.GetItemNamesFull(m_CategoryList.serializedProperty);

			m_CurItemDescriptions = new ReorderableList(m_ItemDatabase, m_ItemList.serializedProperty.GetArrayElementAtIndex(m_ItemList.index).FindPropertyRelative("m_Descriptions"), true, true, true, true);
			m_CurItemDescriptions.drawHeaderCallback = (Rect rect)=> { EditorGUI.LabelField(rect, ""); };
			m_CurItemDescriptions.drawElementCallback += DrawItemDescription;
			m_CurItemDescriptions.elementHeight = DESCRIPTION_HEIGHT;

			m_CurItemProperties = new ReorderableList(m_ItemDatabase, m_ItemList.serializedProperty.GetArrayElementAtIndex(m_ItemList.index).FindPropertyRelative("m_PropertyValues"), true, true, true, true);
			m_CurItemProperties.drawHeaderCallback = (Rect rect)=> { EditorGUI.LabelField(rect, ""); };
			m_CurItemProperties.drawElementCallback += DrawItemPropertyValue;
			m_CurItemProperties.elementHeight = PROPERTY_HEIGHT;

			//Debug.Log(m_ItemList.index + " <-index | count-> " + m_ItemList.count);
			m_CurItemRequiredItems = new ReorderableList(m_ItemDatabase, m_ItemList.serializedProperty.GetArrayElementAtIndex(m_ItemList.index).FindPropertyRelative("m_Recipe").FindPropertyRelative("m_RequiredItems"), true, true, true, true);
			m_CurItemRequiredItems.drawHeaderCallback = (Rect rect)=> { EditorGUI.LabelField(rect, "Required Items"); };
			m_CurItemRequiredItems.drawElementCallback += DrawRequiredItem;
		}
			
		private void DrawItemDescription(Rect rect, int index, bool isActive, bool isFocused)
		{
			var list = m_CurItemDescriptions;

			if(list.serializedProperty.arraySize == index)
				return;

			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			rect.y += 2f;
			rect.height -= 2f;
			element.stringValue = EditorGUI.TextArea(rect, element.stringValue);

			ItemManagementUtility.DoListElementBehaviours(list, index, isFocused, this);
		}
			
		private void DrawItemPropertyValue(Rect rect, int index, bool isActive, bool isFocused)
		{
			var list = m_CurItemProperties;

			if(list.serializedProperty.arraySize == index)
				return;

			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			rect.y += 2f;
			rect.height -= 2f;
			ItemManagementUtility.DrawItemProperty(rect, element, m_PropertyList);

			ItemManagementUtility.DoListElementBehaviours(list, index, isFocused, this);
		}

		private void DrawRequiredItem(Rect rect, int index, bool isActive, bool isFocused)
		{
			var list = m_CurItemRequiredItems;

			if(list.serializedProperty.arraySize == index)
				return;

			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			var name = element.FindPropertyRelative("m_Name");
			var amount = element.FindPropertyRelative("m_Amount");

			rect.y += 2f;
			rect.height -= 2f;

			// Name field.
			rect.width = 128f;
			rect.height = 16f;

			int selectedIndex = ItemManagementUtility.GetItemIndex(m_CategoryList.serializedProperty, name.stringValue);
			selectedIndex = EditorGUI.Popup(rect, selectedIndex, m_ItemNamesFull);

			name.stringValue = m_ItemNames[Mathf.Clamp(selectedIndex, 0, 9999999)];

			// Amount.
			rect.x = rect.xMax + 4f;
			rect.width = 16f;
			GUI.Label(rect, "x");

			rect.x = rect.xMax;
			rect.width = 64f;
			amount.intValue = EditorGUI.IntField(rect, amount.intValue);
			amount.intValue = Mathf.Clamp(amount.intValue, 1, 9999999);

			ItemManagementUtility.DoListElementBehaviours(list, index, isFocused, this);
		}

		private void DrawItemEditor(Rect totalRect)
		{
			// Inner window cross (partitioning in 4 smaller boxes)
			GUI.Box(new Rect(totalRect.x, totalRect.y + totalRect.height * 0.5f, totalRect.width / 2f, 1f), "");
			GUI.Box(new Rect(totalRect.x + totalRect.width * 0.5f, totalRect.y, 1f, totalRect.height), "");

			Vector2 labelSize = new Vector2(192f, 20f);

			// Draw the item list.
			string itemListName = string.Format("Item List ({0})", (m_CategoryList.count == 0 || m_CategoryList.index == -1) ? "None" : m_CategoryList.serializedProperty.GetArrayElementAtIndex(m_CategoryList.index).FindPropertyRelative("m_Name").stringValue);

			GUI.Box(new Rect(totalRect.x + totalRect.width * 0.25f - labelSize.x * 0.5f, totalRect.y, labelSize.x, labelSize.y), itemListName);
			Rect itemListRect = new Rect(totalRect.x, totalRect.y + labelSize.y, totalRect.width * 0.5f - 2f, totalRect.height * 0.5f - labelSize.y - 1f);

			if(m_CategoryList.count != 0 && m_CategoryList.index != -1 && m_CategoryList.index < m_CategoryList.count)
				DrawList(m_ItemList, itemListRect, ref m_ItemsScrollPos);
			else
			{
				itemListRect.x += 8f;
				GUI.Label(itemListRect, "Select a category...", new GUIStyle() { fontStyle = FontStyle.BoldAndItalic });
			}

			// Draw the categories.
			GUI.Box(new Rect(totalRect.x + totalRect.width * 0.25f - labelSize.x * 0.5f, totalRect.y + totalRect.height * 0.5f + 2f, labelSize.x, labelSize.y), "Category List");

			DrawList(m_CategoryList, new Rect(totalRect.x, totalRect.y + totalRect.height * 0.5f + labelSize.y + 2f, totalRect.width * 0.5f - 2f, totalRect.height * 0.5f - labelSize.y - 3f), ref m_CategoriesScrollPos);

			// Inspector label.
			GUI.Box(new Rect(totalRect.x + totalRect.width * 0.75f - labelSize.x * 0.5f, totalRect.y, labelSize.x, labelSize.y), "Item Inspector");

			// Draw the inspector.
			bool itemIsSelected = m_CategoryList.count != 0 && m_ItemList != null && m_ItemList.count != 0 && m_ItemList.index != -1 && m_ItemList.index < m_ItemList.count && m_CurItemDescriptions != null;
			Rect inspectorRect = new Rect(totalRect.x + totalRect.width * 0.5f + 4f, totalRect.y + labelSize.y, totalRect.width * 0.5f - 5f, totalRect.height - labelSize.y - 1f);

			if(itemIsSelected)
				DrawItemInspector(inspectorRect);
			else
			{
				inspectorRect.x += 4f;
				inspectorRect.y += 4f;
				GUI.Label(inspectorRect, "Select an item to inspect...", new GUIStyle() { fontStyle = FontStyle.BoldAndItalic });
			}
		}

		private void DrawList(ReorderableList list, Rect totalRect, ref Vector2 scrollPosition)
		{
			float scrollbarWidth = 16f;

			Rect onlySeenRect = new Rect(totalRect.x, totalRect.y, totalRect.width, totalRect.height);
			Rect allContentRect = new Rect(totalRect.x, totalRect.y, totalRect.width - scrollbarWidth, (list.count + 4) * list.elementHeight);

			scrollPosition = GUI.BeginScrollView(onlySeenRect, scrollPosition, allContentRect, false, true);

			// Draw the clear button.
			Vector2 buttonSize = new Vector2(56f, 16f);

			if(list.count > 0 && GUI.Button(new Rect(allContentRect.x + 2f, allContentRect.yMax - 60f, buttonSize.x, buttonSize.y), "Clear"))
				if(EditorUtility.DisplayDialog("Warning!", "Are you sure you want the list to be cleared? (All elements will be deleted)", "Yes", "Cancel"))
					list.serializedProperty.ClearArray();

			list.DoList(allContentRect);

			GUI.EndScrollView();
		}

		private void DrawListElement(ReorderableList list, Rect rect, int index, bool isActive, bool isFocused)
		{
			if(list.serializedProperty.arraySize == index)
				return;

			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			rect.y += 2;
			EditorGUI.PropertyField(new Rect(rect.x, rect.y, 256f, EditorGUIUtility.singleLineHeight), element, GUIContent.none);

			ItemManagementUtility.DoListElementBehaviours(list, index, isFocused, this);
		}

		private void DrawCategory(Rect rect, int index, bool isActive, bool isFocused) { ItemManagementUtility.DrawListElementByName(m_CategoryList, index, rect, "m_Name", isFocused, this); }

		private void DrawItem(Rect rect, int index, bool isActive, bool isFocused)
		{
			if(m_ItemList.serializedProperty.arraySize > index)
			{
	            SerializedProperty item = m_ItemList.serializedProperty.GetArrayElementAtIndex(index);
	            SerializedProperty displayProp = item.FindPropertyRelative("m_DisplayName");

	            string toUse = (displayProp.stringValue == string.Empty) ? "m_Name" : "m_DisplayName";

				ItemManagementUtility.DrawListElementByName(m_ItemList, index, rect, toUse, isFocused, this);
			}
		}

		private void DrawItemPropertyDefinition(Rect rect, int index, bool isActive, bool isFocused)
		{
			DrawListElement(m_PropertyList, rect, index, isActive, isFocused);
		}

		private void DrawItemInspector(Rect totalRect)
		{
			SerializedProperty item = m_ItemList.serializedProperty.GetArrayElementAtIndex(m_ItemList.index);

			//SerializedProperty category = item.FindPropertyRelative("m_Category");
			SerializedProperty id = item.FindPropertyRelative("m_Id");
			SerializedProperty name = item.FindPropertyRelative("m_Name");
            SerializedProperty displayName = item.FindPropertyRelative("m_DisplayName");
			SerializedProperty icon = item.FindPropertyRelative("m_Icon");
			SerializedProperty worldObject = item.FindPropertyRelative("m_WorldObject");
			SerializedProperty stackSize = item.FindPropertyRelative("m_StackSize");
			SerializedProperty isBuildable = item.FindPropertyRelative("m_IsBuildable");
			SerializedProperty isCraftable = item.FindPropertyRelative("m_IsCraftable");
			SerializedProperty craftDuration = item.FindPropertyRelative("m_Recipe").FindPropertyRelative("m_Duration");

			float scrollbarWidth = 16f;
			float spacing = 4f;
			float stdWidth = 154f;

			Rect onlySeenRect = new Rect(totalRect.x, totalRect.y, totalRect.width, totalRect.height);

			float totalContentHeight = 0f;

			totalContentHeight = 
				9 * 16f + 5 * spacing + 24f +
				(RAW_LIST_HEIGHT + m_CurItemDescriptions.elementHeight) +
				Mathf.Clamp(m_CurItemDescriptions.count - 1, 0, Mathf.Infinity) * m_CurItemDescriptions.elementHeight +
				(RAW_LIST_HEIGHT + m_CurItemProperties.elementHeight) +
				Mathf.Clamp(m_CurItemProperties.count - 1, 0, Mathf.Infinity) * m_CurItemProperties.elementHeight +
				(isCraftable.boolValue ? 48f : 0f) + 
				(isCraftable.boolValue ? (RAW_LIST_HEIGHT + m_CurItemRequiredItems.elementHeight) : 0) + 
				(isCraftable.boolValue ? (Mathf.Clamp(m_CurItemRequiredItems.count - 1, 0, Mathf.Infinity) * m_CurItemRequiredItems.elementHeight) : 0);

			Rect allContentRect = new Rect(totalRect.x, totalRect.y, totalRect.width - scrollbarWidth, totalContentHeight);

			// SCROLL VIEW BEGIN.
			m_ItemInspectorScrollPos = GUI.BeginScrollView(onlySeenRect, m_ItemInspectorScrollPos, allContentRect, false, true);

			if(totalContentHeight > 0f)
				GUI.Box(allContentRect, "");

			Rect rect = new Rect(allContentRect.x + spacing, allContentRect.y + spacing, stdWidth, 16f);

			GUIStyle richTextStyle = new GUIStyle() { richText = true };

			/*// Category.
				GUI.Label(rect, string.Format("<b>Category:</b> {0}", category.stringValue), richTextStyle);*/

			// Id.
			//rect.y = rect.yMax + spacing;
			rect.width = 48f;
			GUI.Label(rect, string.Format("<b>ID:</b> {0}", id.intValue), richTextStyle);

			// Name.
			rect.y = rect.yMax + spacing;
			rect.width = 48f;
			GUI.Label(rect, "<b>Name:</b> ", richTextStyle);

            rect.x = rect.xMax + 54f;
            rect.width = stdWidth;
            EditorGUI.PropertyField(rect, name, GUIContent.none);

            rect.x = allContentRect.x + spacing;
            rect.y = rect.yMax + spacing;
            rect.width = 48f;
            GUI.Label(rect, "<b>Display Name</b> ", richTextStyle);

            rect.x = rect.xMax + 54f;
            rect.width = stdWidth;
            EditorGUI.PropertyField(rect, displayName, GUIContent.none);

            // Icon.
            rect.x = allContentRect.x + spacing;
			rect.y = rect.yMax + spacing;
			rect.width = 48f;
			GUI.Label(rect, "<b>Icon:</b> ", richTextStyle);

			rect.x = rect.xMax + 54f;
			rect.width = stdWidth;
			EditorGUI.PropertyField(rect, icon, GUIContent.none);

			// World object.
			rect.x = allContentRect.x + spacing;
			rect.y = rect.yMax + spacing;
			rect.width = 48f;
			GUI.Label(rect, "<b>World Object:</b> ", richTextStyle);

			rect.x = rect.xMax + 54f;
			rect.width = stdWidth;
			EditorGUI.PropertyField(rect, worldObject, GUIContent.none);

			// Stack size.
			rect.x = allContentRect.x + spacing;
			rect.y = rect.yMax + spacing;
			rect.width = 48f;
			GUI.Label(rect, "<b>Stack Size:</b> ", richTextStyle);

			rect.x = rect.xMax + 54f;
			rect.width = stdWidth * 2f;
			stackSize.intValue = EditorGUI.IntSlider(rect, stackSize.intValue, 1, 999);

			// Descriptions label.
			rect.x = allContentRect.x + spacing;
			rect.y = rect.yMax + spacing;

			rect.width = 48f;
			GUI.Label(rect, "<b>Descriptions:</b> ", richTextStyle);

			// Description list.
			rect.x = rect.xMax + 54f;
			rect.width = stdWidth * 2f + 6f;
			m_CurItemDescriptions.DoList(rect);

			// Properties label.
			rect.x = allContentRect.x + spacing;
			rect.y = rect.yMax + 26f + Mathf.Max(m_CurItemDescriptions.count, 1) * DESCRIPTION_HEIGHT;

			rect.width = 48f;
			GUI.Label(rect, "<b>Properties:</b> ", richTextStyle);

			// Property list.
			rect.x = rect.xMax + 54f;
			rect.width = stdWidth * 2f + 6f;
			m_CurItemProperties.DoList(rect);

			// Is buildable label.
			rect.x = allContentRect.x + spacing;
			rect.y = rect.yMax + 26f + Mathf.Max(m_CurItemProperties.count, 1) * PROPERTY_HEIGHT;

			rect.width = 48f;
			GUI.Label(rect, "<b>Is Placeable?</b>", richTextStyle);

			// Is craftable toggle.
			rect.x = rect.xMax + 54f;
			isBuildable.boolValue = EditorGUI.Toggle(rect, isBuildable.boolValue);

			// Is craftable label.
			rect.x = allContentRect.x + spacing;
			rect.y = rect.yMax + 26f;

			rect.width = 48f;
			GUI.Label(rect, "<b>Is Craftable?</b>", richTextStyle);

			// Is craftable toggle.
			rect.x = rect.xMax + 54f;
			isCraftable.boolValue = EditorGUI.Toggle(rect, isCraftable.boolValue);

			if(isCraftable.boolValue)
			{
				// Recipe label.
				rect.x = allContentRect.x + spacing;
				rect.y = rect.yMax + spacing;
				rect.width = 48f;
				GUI.Label(rect, "<b>Recipe:</b> ", richTextStyle);

				// Duration label.
				rect.x = rect.xMax + 54f;
				rect.width = 128f;
				GUI.Label(rect, "Duration (seconds):");

				// Duration slider.
				rect.x = rect.xMax;
				rect.width = 64f;
				craftDuration.intValue = EditorGUI.IntField(rect, craftDuration.intValue);
				craftDuration.intValue = Mathf.Clamp(craftDuration.intValue, 1, 999);

				// Required items list.
				rect.y += 24f;
				rect.x = allContentRect.x + spacing + 102f;
				rect.width = stdWidth * 2f + 6f;
				m_CurItemRequiredItems.DoList(rect);

				if(m_CurItemRequiredItems.count == 0)
				{
					m_CurItemRequiredItems.serializedProperty.arraySize = 1;
					Repaint();
				}
			}

			GUI.EndScrollView();
		}

		private void DrawPropertyEditor(Rect totalRect)
		{
			Vector2 labelSize = new Vector2(128f, 24f);

			// Properties label.
			GUI.Box(new Rect(totalRect.x + totalRect.width * 0.5f - labelSize.x * 0.5f, totalRect.y, labelSize.x, labelSize.y), "Property List");

			// Draw the properties.
			totalRect.y += 24f;
			totalRect.height -= 25f;
			DrawList(m_PropertyList, totalRect, ref m_PropsScrollPos);
		}
	}

	public static class ItemManagementUtility
	{
		public static void DoListElementBehaviours(ReorderableList list, int index, bool isFocused, EditorWindow window = null)
		{
			var current = Event.current;

			if(current.type == EventType.KeyDown)
			{
				if(list.index == index && isFocused)
				{
					if(current.keyCode == KeyCode.Delete)
					{
						int newIndex = 0;
						if(list.count == 1)
							newIndex = -1;
						else if(index == list.count - 1)
							newIndex = index - 1;
						else if(index > 0)
							newIndex = index - 1;
						
						list.serializedProperty.DeleteArrayElementAtIndex(index);

						if(newIndex != -1)
						{
							list.index = newIndex;
							if(list.onSelectCallback != null)
								list.onSelectCallback(list);
						}

						Event.current.Use();
						if(window)
							window.Repaint();
					}
					else if(current.shift && current.keyCode == KeyCode.D)
					{
						list.serializedProperty.InsertArrayElementAtIndex(list.index);
						list.index ++;
						if(list.onSelectCallback != null)
							list.onSelectCallback(list);

						Event.current.Use();
						if(window)
							window.Repaint();
					}
				}
			}
		}

		public static string[] GetItemNamesFull(SerializedProperty categoryList)
		{
			List<string> names = new List<string>();

			for(int i = 0;i < categoryList.arraySize;i ++)
			{
				var category = categoryList.GetArrayElementAtIndex(i);
				var itemList = category.FindPropertyRelative("m_Items");
				for(int j = 0;j < itemList.arraySize;j ++)
					names.Add(category.FindPropertyRelative("m_Name").stringValue + "/" + itemList.GetArrayElementAtIndex(j).FindPropertyRelative("m_Name").stringValue);
			}
				
			return names.ToArray();
		}

		public static string[] GetItemNames(SerializedProperty categoryList)
		{
			List<string> names = new List<string>();
			for(int i = 0;i < categoryList.arraySize;i ++)
			{
				var category = categoryList.GetArrayElementAtIndex(i);
				var itemList = category.FindPropertyRelative("m_Items");
				for(int j = 0;j < itemList.arraySize;j ++)
					names.Add(itemList.GetArrayElementAtIndex(j).FindPropertyRelative("m_Name").stringValue);
			}

			return names.ToArray();
		}

		public static int GetItemIndex(SerializedProperty categoryList, string itemName)
		{
			int index = 0;
			for(int i = 0;i < categoryList.arraySize;i ++)
			{
				var category = categoryList.GetArrayElementAtIndex(i);
				var itemList = category.FindPropertyRelative("m_Items");
				for(int j = 0;j < itemList.arraySize;j ++)
				{
					var name = itemList.GetArrayElementAtIndex(j).FindPropertyRelative("m_Name").stringValue;
					if(name == itemName)
						return index;

					index ++;
				}
			}

			return -1;
		}

		public static void DrawListElementByName(ReorderableList list, int index, Rect rect, string nameProperty, bool isFocused, EditorWindow window)
		{
			if(list.serializedProperty.arraySize == index)
				return;

			rect.y += 2;
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			var name = element.FindPropertyRelative(nameProperty);

			name.stringValue = EditorGUI.TextField(new Rect(rect.x, rect.y, 256f, 16f), name.stringValue);

			DoListElementBehaviours(list, index, isFocused, window);
		}

		public static void DrawItemProperty(Rect rect, SerializedProperty itemProperty, ReorderableList propertyList)
		{
			var name = itemProperty.FindPropertyRelative("m_Name");
			var type = itemProperty.FindPropertyRelative("m_Type");

			float initialX = rect.x;

			// Source label.
			rect.width = 64f;
			rect.height = 16f;
			GUI.Label(rect, "Property: ");

			// Source popup.
			var allProperties = GetStringNames(propertyList.serializedProperty, "m_Name");

			if(allProperties.Length == 0)
				return;

			rect.x = rect.xMax;
			rect.width = 128f;

			int selectedIndex = GetStringIndex(name.stringValue, allProperties);
			selectedIndex = EditorGUI.Popup(rect, selectedIndex, allProperties);
			name.stringValue = allProperties[selectedIndex];

			type.enumValueIndex = propertyList.serializedProperty.GetArrayElementAtIndex(selectedIndex).FindPropertyRelative("m_Type").enumValueIndex;
			ItemProperty.Type typeToEnum = (ItemProperty.Type)type.enumValueIndex;

			// Value label.
			rect.x = initialX;
			rect.width = 64f;
			rect.y = rect.yMax + 4f;

			GUI.Label(rect, "Value: ");

			// Editing the value based on the type.
			rect.x = rect.xMax;

			if(typeToEnum == ItemProperty.Type.Bool)
				DrawBoolProperty(rect, itemProperty.FindPropertyRelative("m_Bool"));
			if(typeToEnum == ItemProperty.Type.Int)
				DrawIntProperty(rect, itemProperty.FindPropertyRelative("m_Int"));
			else if(typeToEnum == ItemProperty.Type.IntRange)
				DrawIntRangeProperty(rect, itemProperty.FindPropertyRelative("m_IntRange"));
			else if(typeToEnum == ItemProperty.Type.RandomInt)
				DrawRandomIntProperty(rect, itemProperty.FindPropertyRelative("m_RandomInt"));
			else if(typeToEnum == ItemProperty.Type.Float)
				DrawFloatProperty(rect, itemProperty.FindPropertyRelative("m_Float"));
			else if(typeToEnum == ItemProperty.Type.RandomFloat)
				DrawRandomFloatProperty(rect, itemProperty.FindPropertyRelative("m_RandomFloat"));
			else if(typeToEnum == ItemProperty.Type.FloatRange)
				DrawFloatRangeProperty(rect, itemProperty.FindPropertyRelative("m_FloatRange"));
			else if(typeToEnum == ItemProperty.Type.String)
				DrawStringProperty(rect, itemProperty.FindPropertyRelative("m_String"));
			else if(typeToEnum == ItemProperty.Type.Sound)
				DrawSoundProperty(rect, itemProperty.FindPropertyRelative("m_Sound"));
		}

		public static string[] GetStringNames(SerializedProperty property, string subProperty = "")
		{
			List<string> strings = new List<string>();
			for(int i = 0;i < property.arraySize;i ++)
			{
				if(subProperty == "")
					strings.Add(property.GetArrayElementAtIndex(i).stringValue);
				else
					strings.Add(property.GetArrayElementAtIndex(i).FindPropertyRelative(subProperty).stringValue);
			}

			return strings.ToArray();
		}

		public static int GetStringIndex(string str, string[] strings)
		{
			for(int i = 0;i < strings.Length;i ++)
				if(strings[i] == str)
					return i;

			return 0;
		}

		private static void DrawBoolProperty(Rect position, SerializedProperty property)
		{
			property.boolValue = EditorGUI.Toggle(position, property.boolValue);
		}

		private static void DrawIntProperty(Rect position, SerializedProperty property)
		{
			var current = property.FindPropertyRelative("m_Current");
			var defaultVal = property.FindPropertyRelative("m_Default");

			current.intValue = EditorGUI.IntField(position, current.intValue);
			defaultVal.intValue = current.intValue;
		}

		private static void DrawIntRangeProperty(Rect position, SerializedProperty property)
		{
			SerializedProperty current = property.FindPropertyRelative("m_Current");
			SerializedProperty min = property.FindPropertyRelative("m_Min");
			SerializedProperty max = property.FindPropertyRelative("m_Max");

			float fieldWidth = 36f;

			// Current label
			position.width = 54f;
			EditorGUI.PrefixLabel(position, new GUIContent("Current:"));

			// Current field
			position.x = position.xMax;
			position.width = fieldWidth;
			current.intValue = EditorGUI.IntField(position, current.intValue);
			current.intValue = Mathf.Clamp(current.intValue, min.intValue, max.intValue);

			// Min label
			position.x = position.xMax;
			position.width = 32f;
			EditorGUI.PrefixLabel(position, new GUIContent("Min:"));

			// Min field
			position.x = position.xMax;
			position.width = fieldWidth;
			min.intValue = EditorGUI.IntField(position, min.intValue);

			// Max label
			position.x = position.xMax;
			position.width = 32f;
			EditorGUI.PrefixLabel(position, new GUIContent("Max:"));

			// Max field
			position.x = position.xMax;
			position.width = fieldWidth;
			max.intValue = EditorGUI.IntField(position, max.intValue);
		}

		private static void DrawRandomIntProperty(Rect position, SerializedProperty property)
		{
			SerializedProperty min = property.FindPropertyRelative("m_Min");
			SerializedProperty max = property.FindPropertyRelative("m_Max");

			float fieldWidth = 36f;

			// Min label
			position.width = 32f;
			EditorGUI.PrefixLabel(position, new GUIContent("Min:"));

			// Min field
			position.x = position.xMax;
			position.width = fieldWidth;
			min.intValue = EditorGUI.IntField(position, min.intValue);

			// Max label
			position.x = position.xMax;
			position.width = 32f;
			EditorGUI.PrefixLabel(position, new GUIContent("Max:"));

			// Max field
			position.x = position.xMax;
			position.width = fieldWidth;
			max.intValue = EditorGUI.IntField(position, max.intValue);
		}

		private static void DrawFloatProperty(Rect position, SerializedProperty property)
		{
			var current = property.FindPropertyRelative("m_Current");
			var defaultVal = property.FindPropertyRelative("m_Default");

			current.floatValue = EditorGUI.FloatField(position, current.floatValue);
			defaultVal.floatValue = current.floatValue;
		}

		private static void DrawFloatRangeProperty(Rect position, SerializedProperty property)
		{
			SerializedProperty current = property.FindPropertyRelative("m_Current");
			SerializedProperty min = property.FindPropertyRelative("m_Min");
			SerializedProperty max = property.FindPropertyRelative("m_Max");

			float fieldWidth = 36f;

			// Current label
			position.width = 54f;
			EditorGUI.PrefixLabel(position, new GUIContent("Current:"));

			// Current field
			position.x = position.xMax;
			position.width = fieldWidth;
			current.floatValue = EditorGUI.FloatField(position, current.floatValue);
			current.floatValue = Mathf.Clamp(current.floatValue, min.floatValue, max.floatValue);

			// Min label
			position.x = position.xMax;
			position.width = 32f;
			EditorGUI.PrefixLabel(position, new GUIContent("Min:"));

			// Min field
			position.x = position.xMax;
			position.width = fieldWidth;
			min.floatValue = EditorGUI.FloatField(position, min.floatValue);

			// Max label
			position.x = position.xMax;
			position.width = 32f;
			EditorGUI.PrefixLabel(position, new GUIContent("Max:"));

			// Max field
			position.x = position.xMax;
			position.width = fieldWidth;
			max.floatValue = EditorGUI.FloatField(position, max.floatValue);
		}

		private static void DrawRandomFloatProperty(Rect position, SerializedProperty property)
		{
			SerializedProperty min = property.FindPropertyRelative("m_Min");
			SerializedProperty max = property.FindPropertyRelative("m_Max");

			float fieldWidth = 36f;

			// Min label
			position.width = 32f;
			EditorGUI.PrefixLabel(position, new GUIContent("Min:"));

			// Min field
			position.x = position.xMax;
			position.width = fieldWidth;
			min.floatValue = EditorGUI.FloatField(position, min.floatValue);

			// Max label
			position.x = position.xMax;
			position.width = 32f;
			EditorGUI.PrefixLabel(position, new GUIContent("Max:"));

			// Max field
			position.x = position.xMax;
			position.width = fieldWidth;
			max.floatValue = EditorGUI.FloatField(position, max.floatValue);
		}

		private static void DrawStringProperty(Rect position, SerializedProperty property)
		{
			position.width = 128f;
			property.stringValue = EditorGUI.TextField(position, property.stringValue);
		}

		private static void DrawSoundProperty(Rect position, SerializedProperty property)
		{
			position.width = 128f;
			EditorGUI.PropertyField(position, property, GUIContent.none);
		}
	}
}
