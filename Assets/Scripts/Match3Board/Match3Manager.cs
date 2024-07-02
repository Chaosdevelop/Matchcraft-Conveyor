using BaseCore;
using BaseCore.Collections;
using Skills;
using UI;
using UnityEngine;

namespace Match3Game
{
	/// <summary>
	/// Manages the Match3 game, including grid and skills.
	/// </summary>
	public class Match3Manager : MonoBehaviour
	{
		[SerializeField]
		GridController gridController;

		[SerializeField]
		EnumDictionary<SkillSlot, SkillView> abilityViews = new EnumDictionary<SkillSlot, SkillView>();

		EnumDictionary<ItemStat, StatScoreProgress> statScores = new EnumDictionary<ItemStat, StatScoreProgress>();

		int totalScore = 0;
		int turnsMade = 0;
		int turnsLeft = 0;

		GameManager gameManager;
		ShipPartAssemblyResult results;

		void Awake()
		{
			Initialize(GameManager.Instance);
		}

		/// <summary>
		/// Initializes the Match3 manager with the specified game manager.
		/// </summary>
		/// <param name="gameManager">The game manager instance.</param>
		void Initialize(GameManager gameManager)
		{
			this.gameManager = gameManager;
			gridController.OnMoveDone += OnMoveDone;
			gridController.OnMatchProcess += ProcessMatch;
			gridController.CascadeAnimated += WaitCascadeAnimation;

			foreach (var item in abilityViews)
			{
				var skillModel = gameManager.Skills[item.Key];
				skillModel.ResetEnergy();
				item.Value.Initialize(skillModel, gridController);
			}
		}

		void OnEnable()
		{
			gridController.ReinitializeGrid();
			PrepareGame();
		}

		/// <summary>
		/// Handles the event when a move is done.
		/// </summary>
		void OnMoveDone()
		{
			turnsMade++;
			turnsLeft--;
			EventSystem.SendEventToAll(new TurnsChanged { NewValue = turnsLeft, Delta = -1 });
		}

		/// <summary>
		/// Waits for the cascade animation to complete.
		/// </summary>
		void WaitCascadeAnimation()
		{
			if (turnsLeft == 0)
			{
				EndCraftGame();
			}
		}

		/// <summary>
		/// Prepares the game for a new craft.
		/// </summary>
		void PrepareGame()
		{
			totalScore = 0;
			turnsMade = 0;
			turnsLeft = gameManager.TurnsPerCraft;

			statScores[ItemStat.Durability] = new StatScoreProgress(ItemStat.Durability);
			statScores[ItemStat.Quality] = new StatScoreProgress(ItemStat.Quality);
			statScores[ItemStat.Design] = new StatScoreProgress(ItemStat.Design);

			foreach (var item in abilityViews)
			{
				var skillModel = gameManager.Skills[item.Key];
				skillModel.ResetEnergy();
				item.Value.SetAvailability(skillModel.SkillAvailable);
			}

			EventSystem.SendEventToAll(new TurnsChanged { NewValue = turnsLeft });
			results = gameManager.GetCurrentCraftingPart();
		}

		/// <summary>
		/// Ends the current craft.
		/// </summary>
		void EndCraftGame()
		{
			foreach (var item in statScores)
			{
				results.Stats[item.Key] = item.Value.Level;
			}

			results.Scores = totalScore;
			results.Done = true;
			gameManager.NextShipType();
			EventSystem.SendEventToAll(new ScreenChangeEvent { ScreenType = ScreenType.Main });
		}

		/// <summary>
		/// Processes a match and updates scores and stats.
		/// </summary>
		/// <param name="matchInfo">The match information.</param>
		public void ProcessMatch(MatchInfo matchInfo)
		{
			var count = matchInfo.MatchCount;
			var type = matchInfo.MatchType;
			var scores = gameManager.ScoresPerMatch * count;
			totalScore += scores;

			EventSystem.SendEventToAll(new ScoresChanged { NewValue = totalScore, Delta = scores });

			if (type == ChipType.Blue)
			{
				statScores[ItemStat.Durability].AddScore(scores);
				EventSystem.SendEventToAll(new StatScoresChanged { statType = ItemStat.Durability, Delta = scores });
			}
			else if (type == ChipType.Red)
			{
				statScores[ItemStat.Quality].AddScore(scores);
				EventSystem.SendEventToAll(new StatScoresChanged { statType = ItemStat.Quality, Delta = scores });
			}
			else if (type == ChipType.Green)
			{
				statScores[ItemStat.Design].AddScore(scores);
				EventSystem.SendEventToAll(new StatScoresChanged { statType = ItemStat.Design, Delta = scores });
			}
			else if (type == ChipType.Yellow)
			{
				var skillModel = gameManager.Skills[SkillSlot.Yellow];
				skillModel.AddEnergy(scores);
			}
			else if (type == ChipType.Orange)
			{
				var skillModel = gameManager.Skills[SkillSlot.Orange];
				skillModel.AddEnergy(scores);
			}
			else if (type == ChipType.Purple)
			{
				var skillModel = gameManager.Skills[SkillSlot.Purple];
				skillModel.AddEnergy(scores);
			}

		}
	}
}