using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu: MonoBehaviour
{
    public RectTransform mainText;
    
    public void Start()
    {
        mainText.DOPunchPosition(Vector3.up*20f, 2f).SetEase(Ease.OutElastic).SetLoops(-1);
        mainText.DOPunchRotation(Vector3.up*20f, 3.1f).SetEase(Ease.OutElastic).SetLoops(-1);
    }
    
    public void SelectLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}