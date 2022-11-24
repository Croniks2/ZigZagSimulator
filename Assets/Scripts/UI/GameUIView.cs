using UnityEngine;

using TMPro;


public class GameUIView : UIView
{
    [SerializeField] private TextMeshProUGUI _scoresText;
    private int _scores;

    
    public void AddScores(int scores)
    {
        _scores += scores;
        _scoresText.text = _scores.ToString();
    }

    public void ResetScores()
    {
        _scores = 0;
        _scoresText.text = _scores.ToString();
    }
}
