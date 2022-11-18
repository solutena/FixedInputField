using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/FixedInputField")]
public class FixedInputField : InputField
{
	public void Remove()
	{
		if (m_TextComponent == null)
			return;
		if (m_TextComponent.text.Equals(m_Text))
			return;
		int min = Mathf.Min(m_CaretSelectPosition, m_CaretPosition);
		int max = Mathf.Max(m_CaretSelectPosition, m_CaretPosition);
		if (min == max)
			return;
		int offsetMax = max + 1;
		if (m_TextComponent.text.Length <= offsetMax)
			offsetMax = m_TextComponent.text.Length;
		m_Text = m_TextComponent.text.Substring(0, min) + m_TextComponent.text.Substring(offsetMax, m_TextComponent.text.Length - offsetMax);
		m_CaretSelectPosition = Mathf.Clamp(m_CaretSelectPosition, 0, min);
		m_CaretPosition = m_CaretSelectPosition;
		UpdateLabel();
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		Remove();
	}

	protected override void Append(char input)
	{
		if (0 < characterLimit && characterLimit <= m_Text.Length)
			return;
		Remove();
		int prev = m_CaretPosition;
		base.Append(input);
		int current = m_CaretPosition;
		int move = 1 - (current - prev);
		if (move == 0)
			return;
		m_CaretSelectPosition = Mathf.Clamp(m_CaretSelectPosition + move, 0, m_Text.Length);
		m_CaretPosition = m_CaretSelectPosition;
	}

#if UNITY_EDITOR
	[MenuItem("GameObject/UI/FixedInputField")]
	static void CreateFixedInputField()
	{
		GameObject go = new GameObject("FixedInputField", typeof(RectTransform));
		go.AddComponent<InputField>();

		if (Selection.activeGameObject == null)
		{
			Canvas canvas = FindObjectOfType<Canvas>();
			if (canvas == null)
			{
				GameObject canvasGo = new GameObject("Canvas");
				canvas = canvasGo.AddComponent<Canvas>();
			}
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			go.transform.SetParent(canvas.transform);
			go.transform.localPosition = Vector3.zero;
		}
		else
			go.transform.SetParent(Selection.activeGameObject.transform);
		Undo.RegisterCreatedObjectUndo(go, "CreateFixedInputField");
		Selection.activeGameObject = go;
	}
#endif
}