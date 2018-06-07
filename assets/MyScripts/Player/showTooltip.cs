using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class showTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public GameObject MainUI;
	private GameObject tooltipPanel;
	public Vector3 offset;
	private Rect screenRect;
	private bool isObjectOverflowing = false;

	public void Start(){
		screenRect = new Rect(0, 0, Screen.width, Screen.height);
	}

	public void Update(){

	}

	public void OnPointerEnter(PointerEventData pointerEventData){
		tooltipPanel = (GameObject)Instantiate(Resources.Load("Character Creation/tooltipPanel"));
		tooltipPanel.transform.SetParent(MainUI.transform, false);
		tooltipPanel.transform.position = Input.mousePosition + offset;
		tooltipPanel.GetComponentInChildren<Text>().fontSize = 70;
		tooltipPanel.GetComponentInChildren<Text>().text = pointerEventData.pointerEnter.GetComponent<Text>().text;
		Vector3[] objectCorners = new Vector3[4];
		tooltipPanel.GetComponent<RectTransform>().GetWorldCorners(objectCorners);
		foreach (Vector3 corner in objectCorners){
             if (!screenRect.Contains(corner)){
                 isObjectOverflowing = true;
                 break;
             }
        }
		if(isObjectOverflowing){
			tooltipPanel.transform.position = Input.mousePosition - offset;
		}
	}

	public void OnPointerExit(PointerEventData pointerEventData){
		Destroy(tooltipPanel);
	}
	
}
