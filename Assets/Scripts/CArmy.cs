using UnityEngine;
using System.Collections;
using System;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class CArmy : Army
{

	public override void EndCombat(bool won)
	{
		if (won)
		{
			GameManager.EndGame();
		}
		else
		{
			target.score += 10000;
			base.EndCombat(won);
		}
	}
}
