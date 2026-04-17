using System.Collections.Generic;
using UnityEngine;

public class PuzzleSystem : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Player player;

    [Header("Question Configuration")]
    [SerializeField] private int goldCostPerBatch = 10;
    [SerializeField] private int questionsPerBatch = 5;
    [SerializeField] private List<QuestionData> questionPool;

    private void Awake()
    {
        if (player == null)
            player = FindFirstObjectByType<Player>();
    }

    /// <summary>
    /// Try to buy a batch of questions using player's gold.
    /// </summary>
    public bool BuyQuestions()
    {
        if (player == null) return false;

        if (player.gold >= goldCostPerBatch)
        {
            player.gold -= goldCostPerBatch;
            player.remainingQuestions += questionsPerBatch;
            
            GachaEvents.OnRemainingQuestionsChanged?.Invoke(player.remainingQuestions);
            
            // Optionally play buy sound
            if (AudioManager.Instance != null && AudioManager.Instance.CueLibrary != null)
            {
                // Replace with specific buy sound if you have one
                AudioManager.Instance.PlaySFX(AudioManager.Instance.CueLibrary.ButtonClick);
            }
            
            return true;
        }

        return false;
    }

    /// <summary>
    /// Returns a random question from the pool. Returns null if pool is empty.
    /// </summary>
    public QuestionData GetRandomQuestion()
    {
        if (questionPool == null || questionPool.Count == 0) return null;
        return questionPool[Random.Range(0, questionPool.Count)];
    }

    /// <summary>
    /// Processes the player's answer.
    /// </summary>
    public void SubmitAnswer(QuestionData question, int selectedIndex)
    {
        if (player == null || player.remainingQuestions <= 0) return;

        // Deduct 1 question
        player.remainingQuestions--;
        GachaEvents.OnRemainingQuestionsChanged?.Invoke(player.remainingQuestions);

        bool isCorrect = (selectedIndex == question.correctIndex);
        
        if (isCorrect)
        {
            // Give reward (Gacha Roll) via event instead of strict coupling
            GachaEvents.OnPuzzleSolved?.Invoke();
        }

        // Fire event so UI/Audio can react
        GachaEvents.OnQuestionAnswered?.Invoke(isCorrect);
    }
}
