﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class State : MonoBehaviour {

	[Header("Resources")]
	public int people = 100;
	public int prisoners = 0;
	public float food = 100;
	public float gold = 100;
	public int farmers = 10;
	public int miners = 0;
	public int warriors = 0;
	public float piety = 0.5f;

	[Header("Growth")]
	[SerializeField] float baseFoodMultiplier = 3f;
	[SerializeField] float baseGoldMultiplier = 0.1f;
	[SerializeField] float baseFertility = 10f;
	[SerializeField] float foodFertility = 0.05f;
	[SerializeField] float basePietyDecay = 0.03f;
	[SerializeField] float baseFoodCost = 1f;
	[SerializeField] float baseGoldCost = 1f;
	[NonSerialized] public float foodMultiplier;
	[NonSerialized] public float goldMultiplier;
	[NonSerialized] public float fertility;
	[NonSerialized] public float pietyDecay;
	[NonSerialized] public float foodCost;
	[NonSerialized] public float goldCost;

	[Header("Status")]
	public List<AStateModifier> modifiers;
	[Range(0f,0.1f)] public float autoSacrifice = 0.05f;
	public bool aI = true;
	[Range(0f, 0.5f)] public float aiFree = 0.2f;
	[Range(0, 500)] public int aiMinMilitary = 100;
	[Range(0, 1000)] public int aiMilitarySize = 200;

	[Header("Abilities")]
	public List<AStateModifier> blessings;
	public List<AStateModifier> curses;


	void Start () {
		GameManager.RegisterState(this);
	}

	public void Tick()
	{
		foodMultiplier = baseFoodMultiplier;
		goldMultiplier = baseGoldMultiplier;
		fertility = baseFertility + food * foodFertility;
		pietyDecay = basePietyDecay;
		goldCost = baseGoldCost;
		foodCost = baseFoodCost;
		for (int i = 0; i < modifiers.Count; i++)
			modifiers[i].Apply(this);
		food += farmers * foodMultiplier;
		gold += miners * goldMultiplier;
		piety -= pietyDecay;
		int humans = (prisoners + farmers + miners + warriors + people);
		if (food > humans)
		{
			food -= humans;
			people += (int)fertility;
		}
		if (autoSacrifice > 0)
			AutoSacrifice();
		if (aI)
		{
			AiAllocate();
			AiAct();
		}
	}

	void AutoSacrifice()
	{
		int humans = (prisoners + farmers + miners + warriors + people);
		int sacrifice = (int)(humans * Mathf.Min(autoSacrifice, 1f - piety));
		int delta = Mathf.Min(sacrifice, prisoners);
		int total = delta;
		sacrifice -= delta;
		prisoners -= delta;
		if (sacrifice > 0)
		{
			delta = Mathf.Min(sacrifice, people);
			total += delta;
			sacrifice -= delta;
			people -= delta;
		}
		if (sacrifice > 0)
		{
			delta = Mathf.Min(sacrifice, miners);
			total += delta;
			sacrifice -= delta;
			miners -= delta;
		}
		piety += (float)total / (float)humans;
	}

	void AiAllocate()
	{
		int humans = (prisoners + farmers + miners + warriors + people);
		int delta = Mathf.CeilToInt((humans + fertility - farmers * foodMultiplier) / foodMultiplier);
		if (delta > 0 && food * 4 < humans)
		{
			delta = Mathf.Min(delta, people);
			people -= delta;
			farmers += delta;
		}
		if (people > 0)
		{
			delta = Mathf.Max(warriors-aiMinMilitary, people - (int)(humans * aiFree));
			delta = Mathf.Min(people, delta);
			if (delta > 0)
			{
				delta = Mathf.Min(delta, Mathf.FloorToInt(gold / goldCost));
				delta = Mathf.Min(delta, Mathf.FloorToInt(food / foodCost));
				people -= delta;
				warriors += delta;
				food -= delta * foodCost;
				gold -= delta * goldCost;
			}
		}
		if (people > 0)
		{
			delta = people - (int)(humans * aiFree);
			if (delta > 0)
			{
				delta = Mathf.Min(delta, people);
				people -= delta;
				miners += delta;
			}
		}
	}

	void AiAct()
	{
		State target = GameManager.GetRandomEnemyState(this);
		if (warriors > aiMilitarySize)
		{
			Attack(target);
			if (piety > 0.75 && curses.Count > 0)
			{
				Curse(curses.GetRandom(), target);
			}
		}
		else if (piety > 0.95)
		{
			if (curses.Count != 0 || blessings.Count != 0)
			{
				if (curses.Count == 0)
					Bless(blessings.GetRandom());
				else if (blessings.Count == 0)
					Curse(curses.GetRandom(), target);
				else if (UnityEngine.Random.value > 0.5f)
					Curse(curses.GetRandom(), target);
				else
					Bless(blessings.GetRandom());
			}
		}
	}

	public void Attack(State target)
	{
		warriors = 0;
	}

	public void Bless(AStateModifier mod)
	{
		StartCoroutine(AddBlesssing(mod, this));
	}

	IEnumerator AddBlesssing(AStateModifier mod, State target)
	{
		if (blessings.Remove(mod))
		{
			piety -= mod.cost;
			target.modifiers.Add(mod);
			yield return new WaitForSeconds(mod.time);
			target.modifiers.Remove(mod);
			if (mod.cooldown > mod.time)
				yield return new WaitForSeconds(mod.cooldown - mod.time);
			blessings.Add(mod);
		}
	}

	public void Curse(AStateModifier mod, State target)
	{
		StartCoroutine(AddCurse(mod, target));
	}

	IEnumerator AddCurse(AStateModifier mod, State target)
	{
		if (curses.Remove(mod))
		{
			piety -= mod.cost;
			target.modifiers.Add(mod);
			yield return new WaitForSeconds(mod.time);
			target.modifiers.Remove(mod);
			if (mod.cooldown > mod.time)
				yield return new WaitForSeconds(mod.cooldown - mod.time);
			curses.Add(mod);
		}
	}
}
