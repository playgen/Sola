﻿using System.Linq;

using PlayGen.SUGAR.Unity;
using PlayGen.Unity.Utilities.BestFit;

using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.Localization;
using PlayGen.SUGAR.Common;

public class LeaderboardInterface : BaseLeaderboardInterface
{
	/// <summary>
	/// An array of the LeaderboardPositionInterface on this GameObject, set in the Inspector.
	/// </summary>
	[Tooltip("An array of the LeaderboardPositionInterface on this GameObject, set in the Inspector.")]
	[SerializeField]
	private LeaderboardPositionInterface[] _leaderboardPositions;

	/// <summary>
	/// Button used to go to the previous page of results.
	/// </summary>
	[Tooltip("Button used to go to the previous page of results.")]
	[SerializeField]
	private Button _previousButton;

	/// <summary>
	/// Button used to go to the next page of results.
	/// </summary>
	[Tooltip("Button used to go to the next page of results.")]
	[SerializeField]
	private Button _nextButton;

	/// <summary>
	/// The current page number.
	/// </summary>
	private int _pageNumber;

	/// <summary>
	/// In addition to base onclick adding, sets PositionCount to match number of positions available to show and adds listeners for the previous and next buttons.
	/// </summary>
	protected override void Awake()
	{
		base.Awake();
		SUGARManager.Leaderboard.SetPositionCount(_leaderboardPositions.Length);
		_previousButton.onClick.AddListener(delegate { UpdatePageNumber(-1); });
		_nextButton.onClick.AddListener(delegate { UpdatePageNumber(1); });
	}

	/// <summary>
	/// Trigger DoBestFit method and add event listeners for when resolution and language changes.
	/// </summary>
	private void OnEnable()
	{
		DoBestFit();
		BestFit.ResolutionChange += DoBestFit;
		Localization.LanguageChange += OnLanguageChange;
	}

	/// <summary>
	/// Remove event listeners.
	/// </summary>
	private void OnDisable()
	{
		BestFit.ResolutionChange -= DoBestFit;
		Localization.LanguageChange -= OnLanguageChange;
	}

	/// <summary>
	/// Set the pageNumber to 0 before displaying the UI.
	/// </summary>
	protected override void PreDisplay()
	{
		_pageNumber = 0;
	}

	/// <summary>
	/// Adjust LeaderboardPositionInterface pool to display a page of leaderboard standings.
	/// </summary>
	protected override void Draw()
	{
		if (!SUGARManager.Leaderboard.CurrentStandings.Any() && _pageNumber > 0)
		{
			UpdatePageNumber(-1);
			return;
		}
		if ((!SUGARManager.Leaderboard.CurrentStandings.Any() && _pageNumber < 0))
		{
			UpdatePageNumber(1);
			return;
		}
		for (int i = 0; i < _leaderboardPositions.Length; i++)
		{
			if (i >= SUGARManager.Leaderboard.CurrentStandings.Count)
			{
				_leaderboardPositions[i].Disable();
			}
			else
			{
				_leaderboardPositions[i].SetText(SUGARManager.Leaderboard.CurrentStandings[i]);
			}
		}
		_leaderboardName.text = SUGARManager.Leaderboard.CurrentLeaderboard != null ? SUGARManager.Leaderboard.CurrentLeaderboard.Name : string.Empty;
		_leaderboardType.text = Localization.Get(SUGARManager.Leaderboard.CurrentFilter.ToString());
		_previousButton.interactable = false;
		_nextButton.interactable = false;
		SUGARManager.Leaderboard.GetLeaderboardStandings(_pageNumber - 1, success => { }, resultDown =>
		{
			_previousButton.interactable = resultDown.ToList().Count > 0 && resultDown.First().ActorName != SUGARManager.Leaderboard.CurrentStandings.First().ActorName;
			SUGARManager.Leaderboard.GetLeaderboardStandings(_pageNumber + 1, success => { }, resultUp =>
			{
				_nextButton.interactable = resultUp.ToList().Count > 0;
			});
		});
		_leaderboardPositions.Select(t => t.gameObject).BestFit();
	}

	/// <summary>
	/// If a user signs in via this panel, refresh the current page (which should be page 1).
	/// </summary>
	protected override void OnSignIn()
	{
		UpdatePageNumber(0);
	}

	/// <summary>
	/// Adjust the current page number and redraw the UI.
	/// </summary>
	private void UpdatePageNumber(int changeAmount)
	{
		_pageNumber += changeAmount;
		SUGARManager.Leaderboard.GetLeaderboardStandings(_pageNumber, Show);
	}

	/// <summary>
	/// Set the text of all buttons and all leaderboard positions to be as big as possible and the same size within the same grouping.
	/// </summary>
	private void DoBestFit()
	{
		_leaderboardPositions.Select(t => t.gameObject).BestFit();
		GetComponentsInChildren<Button>(true).Select(t => t.gameObject).BestFit();
	}

	/// <summary>
	/// Refresh the current page to ensure any text set in code is also translated.
	/// </summary>
	private void OnLanguageChange()
	{
		UpdatePageNumber(0);
	}
}