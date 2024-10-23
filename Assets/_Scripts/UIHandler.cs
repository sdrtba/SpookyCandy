using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Text scoreText;
    private int _score = 0;
    private int _hp = 3;

    private void OnEnable()
    {
        Candy.OnPickup += ChangeScore;
        Candy.OnHPChange += ChangeHP;

    }

    private void OnDisable()
    {
        Candy.OnPickup -= ChangeScore;
        Candy.OnHPChange -= ChangeHP;
    }

    private void ChangeScore(int value)
    {
        _score += value;
        scoreText.text = "Score: " + _score;
    }

    private void ChangeHP(int value)
    {
        _hp += value;
        if (_hp <= 0)
        {
            SceneManager.LoadScene(0);
            return;
        }
        else if (_hp > 3)
        {
            _hp = 3;
        }
        hpSlider.value = _hp;
    }
}
