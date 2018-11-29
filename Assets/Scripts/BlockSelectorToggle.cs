using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockSelectorToggle : MonoBehaviour
{
	public Toggle toggle;
	public Animator animator;

	public void OnToggle()
	{
		if (toggle.isOn)
		{
			animator.Play("PopupAnimation");
		}
		else
		{
			animator.Play("PopdownAnimation");
		}
	}
}
