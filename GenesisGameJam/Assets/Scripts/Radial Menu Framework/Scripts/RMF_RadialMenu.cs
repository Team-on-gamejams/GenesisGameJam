using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using TMPro;


[AddComponentMenu("Radial Menu Framework/RMF Core Script")]
public class RMF_RadialMenu : MonoBehaviour {

	[HideInInspector]
	public RectTransform rt;
	//public RectTransform baseCircleRT;
	//public Image selectionFollowerImage;

	[Tooltip("With lazy selection, you only have to point your mouse (or joystick) in the direction of an element to select it, rather than be moused over the element entirely.")]
	public bool useLazySelection = true;

	[Tooltip("If set to true, a pointer with a graphic of your choosing will aim in the direction of your mouse. You will need to specify the container for the selection follower.")]
	public bool useSelectionFollower = true;

	[Tooltip("If using the selection follower, this must point to the rect transform of the selection follower's container.")]
	public RectTransform selectionFollowerContainer;
	public Image selectionFollowerImage;

	[Tooltip("This is the text object that will display the labels of the radial elements when they are being hovered over. If you don't want a label, leave this blank.")]
	public TextMeshProUGUI textLabel;
	public TextMeshProUGUI priceSunField;
	public TextMeshProUGUI priceWaterField;
	public GameObject sunParent;
	public GameObject waterParent;

	[Tooltip("This is the list of radial menu elements. This is order-dependent. The first element in the list will be the first element created, and so on.")]
	public List<RMF_RadialMenuElement> elements = new List<RMF_RadialMenuElement>();


	[Tooltip("Controls the total angle offset for all elements. For example, if set to 45, all elements will be shifted +45 degrees. Good values are generally 45, 90, or 180")]
	public float globalOffset = 0f;


	[HideInInspector]
	public float currentAngle = 0f; //Our current angle from the center of the radial menu.


	[HideInInspector]
	public int index = 0; //The current index of the element we're pointing at.

	private int elementCount;

	private float angleOffset; //The base offset. For example, if there are 4 elements, then our offset is 360/4 = 90

	private int previousActiveIndex = 0; //Used to determine which buttons to unhighlight in lazy selection.

	private PointerEventData pointer;

	[System.NonSerialized] public bool isCanSelect = false;

	void Awake() {

		pointer = new PointerEventData(EventSystem.current);

		rt = GetComponent<RectTransform>();

		if (rt == null)
			Debug.LogError("Radial Menu: Rect Transform for radial menu " + gameObject.name + " could not be found. Please ensure this is an object parented to a canvas.");

		if (useSelectionFollower && selectionFollowerContainer == null)
			Debug.LogError("Radial Menu: Selection follower container is unassigned on " + gameObject.name + ", which has the selection follower enabled.");

		elementCount = elements.Count;

		angleOffset = (360f / (float)elementCount);

		//Loop through and set up the elements.
		for (int i = 0; i < elementCount; i++) {
			if (elements[i] == null) {
				Debug.LogError("Radial Menu: element " + i.ToString() + " in the radial menu " + gameObject.name + " is null!");
				continue;
			}
			elements[i].parentRM = this;

			elements[i].setAllAngles((angleOffset * i) + globalOffset, angleOffset);

			elements[i].assignedIndex = i;

		}

	}


	// Update is called once per frame
	void Update() {

		//If your gamepad uses different horizontal and vertical joystick inputs, change them here!
		//==============================================================================================
		bool joystickMoved = Input.GetAxis("Horizontal") != 0.0 || Input.GetAxis("Vertical") != 0.0;
		//==============================================================================================

		const float maxDist = 32.0f;
		Vector3 v = Input.mousePosition - TemplateGameManager.Instance.Camera.WorldToScreenPoint(rt.position);
		float rawAngle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
		float dist = v.magnitude;
		float distPersent = dist / maxDist;

		if (useSelectionFollower) {
			float a = Mathf.Clamp01(distPersent);
			if (a <= 0.6f)
				a /= 2;
			selectionFollowerImage.color = selectionFollowerImage.color.SetA(a);
		}
		
		//If no gamepad, update the angle always. Otherwise, only update it if we've moved the joystick.
		currentAngle = normalizeAngle(-rawAngle + 90 - globalOffset + (angleOffset / 2f));

		//Handles lazy selection. Checks the current angle, matches it to the index of an element, and then highlights that element.
		if (useLazySelection) {
			index = (int)(currentAngle / (angleOffset != 0 ? angleOffset : 1));
			
			if (distPersent >= 1) {

				if (elements[index] != null) {
					selectButton(index);
					if (isCanSelect && (Input.GetMouseButtonUp(0) || Input.GetButtonDown("Submit"))) {
						if ((elements[index].priceSun <= GameManager.Instance.player[ResourceType.Sunlight]) &&
							(elements[index].priceWater <= GameManager.Instance.player[ResourceType.Water])) {

							ExecuteEvents.Execute(elements[index].button.gameObject, pointer, ExecuteEvents.submitHandler);
							GameManager.Instance.player[ResourceType.Sunlight] -= elements[index].priceSun;
							GameManager.Instance.player[ResourceType.Water] -= elements[index].priceWater;
						}


						selectionFollowerImage.color = selectionFollowerImage.color.SetA(0);
						elements[index].unHighlightThisElement(pointer);
						enabled = false;
					}
				}
			}
			else {
				foreach (var element in elements) {
					if (element.active) {
						element.unHighlightThisElement(pointer);
					}
				}
				textLabel.text = "";
				priceSunField.text = "";
				priceWaterField.text = "";

				sunParent.SetActive(false);
				waterParent.SetActive(false);
			}
		}

		//Updates the selection follower if we're using one.
		if (useSelectionFollower && selectionFollowerContainer != null) {
			selectionFollowerContainer.rotation = Quaternion.Euler(0, 0, rawAngle + 270);
		}
	}


	//Selects the button with the specified index.
	private void selectButton(int i) {

		if (elements[i].active == false) {

			elements[i].highlightThisElement(pointer); //Select this one

			if (previousActiveIndex != i)
				elements[previousActiveIndex].unHighlightThisElement(pointer); //Deselect the last one.


		}

		previousActiveIndex = i;

	}

	//Keeps angles between 0 and 360.
	private float normalizeAngle(float angle) {

		angle = angle % 360f;

		if (angle < 0)
			angle += 360;

		return angle;

	}


}
