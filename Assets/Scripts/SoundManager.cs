using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    private AudioSource _soundsSource;

    [SerializeField] private AudioClip _explosionClip;
    [SerializeField] private AudioClip _loseClip;
    [SerializeField] private AudioClip _ballClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        _soundsSource = gameObject.GetComponent<AudioSource>();
    }

    public void PlayExplosionSound()
    {
        _soundsSource.PlayOneShot(_explosionClip);
    }
    public void PlayLoseSound()
    {
        _soundsSource.PlayOneShot(_loseClip);
    }
    public void PlayBallSound()
    {
        _soundsSource.PlayOneShot(_ballClip);
    }

}