using UnityEngine;

[CreateAssetMenu(fileName = "AudioCueLibrary", menuName = "WeaponMaster/Audio/Audio Cue Library")]
public class AudioCueLibrary : ScriptableObject
{
    [Header("Music")]
    [SerializeField] private AudioCue mainMenuMusic;
    [SerializeField] private AudioCue gameplayMusic;
    [SerializeField] private AudioCue bossMusic;

    [Header("Door")]
    [SerializeField] private AudioCue doorOpen;
    [SerializeField] private AudioCue doorClose;

    [Header("Player")]
    [SerializeField] private AudioCue swordAttack;
    [SerializeField] private AudioCue gunAttack;
    [SerializeField] private AudioCue playerHurt;
    [SerializeField] private AudioCue playerDeath;

    [Header("Enemy")]
    [SerializeField] private AudioCue enemyAttack;
    [SerializeField] private AudioCue enemyHurt;
    [SerializeField] private AudioCue enemyDeath;

    [Header("Item Pickup")]
    [SerializeField] private AudioCue goldPickup;
    [SerializeField] private AudioCue healthPickup;
    [SerializeField] private AudioCue buffPickup;

    [Header("UI")]
    [SerializeField] private AudioCue buttonClick;
    // [SerializeField] private AudioCue buttonHover;
    // [SerializeField] private AudioCue menuOpen;
    // [SerializeField] private AudioCue menuClose;

    [Header("Gacha & Quiz")]
    [SerializeField] private AudioCue gachaSpin;
    [SerializeField] private AudioCue gachaReward;
    [SerializeField] private AudioCue quizCorrect;
    [SerializeField] private AudioCue quizWrong;

    public AudioCue MainMenuMusic => mainMenuMusic;
    public AudioCue GameplayMusic => gameplayMusic;
    public AudioCue BossMusic => bossMusic;
    public AudioCue DoorOpen => doorOpen;
    public AudioCue DoorClose => doorClose;
    public AudioCue SwordAttack => swordAttack;
    public AudioCue GunAttack => gunAttack;
    public AudioCue PlayerHurt => playerHurt;
    public AudioCue PlayerDeath => playerDeath;
    public AudioCue EnemyAttack => enemyAttack;
    public AudioCue EnemyHurt => enemyHurt;
    public AudioCue EnemyDeath => enemyDeath;
    public AudioCue GoldPickup => goldPickup;
    public AudioCue HealthPickup => healthPickup;
    public AudioCue BuffPickup => buffPickup;
    public AudioCue ButtonClick => buttonClick;
    // public AudioCue ButtonHover => buttonHover;
    // public AudioCue MenuOpen => menuOpen;
    // public AudioCue MenuClose => menuClose;
    
    public AudioCue GachaSpin => gachaSpin;
    public AudioCue GachaReward => gachaReward;
    public AudioCue QuizCorrect => quizCorrect;
    public AudioCue QuizWrong => quizWrong;
}
