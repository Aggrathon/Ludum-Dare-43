using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(LineRenderer))]
public class Army : MonoBehaviour
{
	[NonSerialized] public State state;
	[NonSerialized] public int size;
	[NonSerialized] public State target;
	public float speed = 2f;

	LineRenderer lr;

	private void Awake()
	{
		lr = GetComponent<LineRenderer>();
		lr.positionCount = 2;
	}

	private void Update()
	{
		Vector3 dir = target.transform.position - transform.position;
		float len = Vector3.Magnitude(dir);
		float speed = this.speed * Time.deltaTime;
		if (len <= speed)
		{
			if (target == state)
			{
				state.people += size;
				size = 0;
				Destroy(gameObject);
			}
			else
			{
				// Start Combat
				target = state;
			}
			speed = len;
		}
		speed /= len;
		transform.position += dir * speed;
		lr.SetPosition(1, transform.position);
		lr.SetPosition(0, target.transform.position);
	}

	public void Setup(State state, int size, State target)
	{
		transform.position = state.transform.position;
		lr.SetPosition(0, transform.position);
		lr.SetPosition(1, target.transform.position);
		this.state = state;
		this.size = size;
		this.target = target;
	}
}
