using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	static GameManager instance;

	public float tickRate = 1f;

	float nextTick;
	List<State> states;

	private void Awake()
	{
		instance = this;
		states = new List<State>();
	}

	void Start()
	{
		nextTick = Time.time + tickRate;
	}
	
	void Update()
	{
		if (nextTick < Time.time)
		{
			for (int i = 0; i < states.Count; i++)
				states[i].Tick();
			nextTick += tickRate;
		}
	}

	public static void RegisterState(State state)
	{
		instance.states.Add(state);
	} 
}
