using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IUsable
{
	Transform t;
	bool isOpen;
	bool isLocked;

	private void Awake()
	{
		t = GetComponent<Transform>();
	}

	public void Use()
	{
		if (!isLocked)
		{
			if (isOpen)
			{
				t.DORotate(new Vector3(0f, 0f, 90f), .5f, RotateMode.LocalAxisAdd);
			}
			else
			{
				t.DORotate(new Vector3(0f, 0f, -90f), .5f, RotateMode.LocalAxisAdd);
			}
			isOpen = !isOpen;
		}
	}
}
