using System.Collections.Generic;
using UnityEngine;
using KModkit;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;
using Rnd = UnityEngine.Random;
using System.Collections;

public class SimpleModuleScript : MonoBehaviour {

	public KMAudio audio;
	public KMBombInfo info;
	public KMBombModule module;
	public KMRuleSeedable ruleSeed;
	public KMSelectable[] movingButton;
	public KMSelectable[] pressableModule;
	public GameObject movingButtonObject;
	static int ModuleIdCounter = 1;
	int ModuleId;

	private float xpos;
	private float zpos;
	private float ypos;

	public AudioSource correct;

	bool _isSolved = false;
	bool incorrect = false;
	int ruleSelected;

	void Awake()
	{
		ModuleId = ModuleIdCounter++;

		foreach (KMSelectable button in movingButton)
		{
			KMSelectable pressedButton = button;
			button.OnInteract += delegate () { movingButtonStuff(pressedButton); return false; };
		}

		foreach (KMSelectable button in pressableModule)
		{
			KMSelectable pressedButton = button;
			button.OnInteract += delegate () { ModulePressing(pressedButton); return false; };
		}
	}

	void Start()
	{
		xpos = Rnd.Range (5f / 10f, -5f / 10f);
		zpos = Rnd.Range (2f / 10f, -2f / 10f);
		ypos = Rnd.Range (0f, 2f / 10f);

		movingButtonObject.transform.localPosition = new Vector3 (xpos, ypos, zpos);

		var rng = ruleSeed.GetRNG();
		Log("Using rule seed: " + rng.Seed);
		if (rng.Seed != 1)
			ruleSelected = rng.Next(3);
	}

	void movingButtonStuff(KMSelectable pressedButton)
	{
		audio.PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.ButtonPress, transform);
		int buttonPosition = new int();
		for(int i = 0; i < movingButton.Length; i++)
		{
			if (pressedButton == movingButton[i])
			{
				buttonPosition = i;
				break;
			}
		}

		if (_isSolved == false) 
		{
			if (ruleSelected == 1)
				incorrect = true;
			switch (buttonPosition) 
			{
			case 0:
				Log ("You did it!");
				break;
			}
			if (incorrect) 
			{
				Log("However, I did not want you to press the button. Strike!");
				module.HandleStrike ();
				incorrect = false;
			}
			else
			{
				correct.Play ();
				module.HandlePass();
				_isSolved = true;
			}
		}
	}

	void ModulePressing(KMSelectable pressedButton)
	{
		audio.PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.ButtonPress, transform);
		int buttonPosition = new int();
		for(int i = 0; i < movingButton.Length; i++)
		{
			if (pressedButton == movingButton[i])
			{
				buttonPosition = i;
				break;
			}
		}

		if (_isSolved == false) 
		{
			if (ruleSelected == 2)
				incorrect = true;
			switch (buttonPosition) 
			{
			case 0:
				Log ("Ok cool.");
				break;
			}
			if (incorrect) 
			{
				Log("However, I did not want you to press the question mark. Strike!");
				module.HandleStrike ();
				incorrect = false;
			}
			else
			{
				correct.Play ();
				module.HandlePass();
				_isSolved = true;
			}
		}
	}

	void Log(string message)
	{
		Debug.LogFormat("[Moved #{0}] {1}", ModuleId, message);
	}

	//Twitch Plays support

	#pragma warning disable 414
	private readonly string TwitchHelpMessage = @"!{0} button/b [Presses the button] | !{0} ? [Presses the question mark]";
	#pragma warning restore 414

	IEnumerator ProcessTwitchCommand(string command)
	{
		if (command.EqualsIgnoreCase("button") || command.EqualsIgnoreCase("b"))
		{
			yield return null;
			movingButton[0].OnInteract();
		}
		else if (command.EqualsIgnoreCase("?"))
		{
			yield return null;
			pressableModule[0].OnInteract();
		}
	}

	IEnumerator TwitchHandleForcedSolve()
	{
		movingButton[0].OnInteract();
		yield return new WaitForSeconds(.1f);
	}
}
