using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;

public class GameLooper : MonoBehaviour 
{
	private bool idle = true;
	private bool pause = false;
	private bool runGame = false;

	public TimerLogic timer;
	public Text inputKeyPrompt;
	public BeaconCounter_Logic beaconCounter;
	public Controller controller;
	public GameObject canvas;
	public GameObject endGameMenuPrefab;
	private GameObject endGameMenu;

	public GameObject pauseMenuPrefab;
	private GameObject pauseMenu;
	private float gameTimeScale;

	private delegate void VoidVoidFunction();

	public void OnResume()
	{
		Destroy(pauseMenu);
		controller.run = true;
		Time.timeScale = gameTimeScale;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		pause = false;
		runGame = true;
	}

	public void OnRestart()
	{
		SceneManager.LoadScene("Arena");
		controller.run = true;
		Time.timeScale = 1.0f;
	}

	public void OnMainMenu()
	{
		SceneManager.LoadScene("Main");
		Time.timeScale = 1.0f;
	}

	public void OnExit()
	{
		// This will be ignored when you are in the editor.
		Application.Quit();
	}



	void AddButtonListener(GameObject parent, string object_name, UnityEngine.Events.UnityAction func)
	{
		GameObject o = parent.transform.Find(object_name).gameObject;
		Button b = o.GetComponent<Button>();
		b.onClick.AddListener(func);
	}

	void Pause()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			OnResume();
		}
	}

	void RunGame()
	{
		if(beaconCounter.AllBeaconsCollected())
		{
			runGame = false;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			Time.timeScale = 0.0f;
			controller.run = false;

			endGameMenu = Instantiate(endGameMenuPrefab);
			endGameMenu.transform.SetParent(canvas.transform, false);
			GameObject final_time = endGameMenu.transform.Find("FinalTime").gameObject;
			Text time_text = final_time.GetComponent<Text>();
			time_text.text = timer.Text();

			AddButtonListener(endGameMenu, "RestartButton", OnRestart);
			AddButtonListener(endGameMenu, "ExitButton", OnExit);
			AddButtonListener(endGameMenu, "MainMenuButton", OnMainMenu);

			timer.StopTimer();
		}

		if(Input.GetKeyDown(KeyCode.Escape))
		{
			runGame = false;
			pause = true;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			gameTimeScale = Time.timeScale;
			Time.timeScale = 0.0f;
			controller.run = false;

			pauseMenu = Instantiate(pauseMenuPrefab);
			pauseMenu.transform.SetParent(canvas.transform, false);

			AddButtonListener(pauseMenu, "ResumeButton", OnResume);
			AddButtonListener(pauseMenu, "RestartButton", OnRestart);
			AddButtonListener(pauseMenu, "ExitButton", OnExit);
			AddButtonListener(pauseMenu, "MainMenuButton", OnMainMenu);
		}
	}

	// Perform actions when game is not being played and game is not paused.
	void Idle()
	{
		if(Input.anyKeyDown)
		{
			idle = false;
			runGame = true;
			Color color = inputKeyPrompt.color;
			color[3] = 0.0f;	
			inputKeyPrompt.color = color;
			timer.StartTimer();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(idle)
		{
			Idle();
			return;
		}
		if(runGame)
		{
			RunGame();
			return;
		}
		if(pause)
		{
			Pause();
		}
	}

	void Start()
	{
		idle = true;
		pause = false;
		runGame = false;
	}
}
