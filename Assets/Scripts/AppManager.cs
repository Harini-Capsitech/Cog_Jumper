using System.Collections;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject GameLogicPrefab;
    private GameObject GameLogic;

    public static AppManager instance;
    void Start()
    {
        instance = this;
        StartCoroutine(ShowLoading());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ShowLoading()
    {
        AppStateManager.Instance.SetLoading();
        yield return new WaitForSeconds(2f);
        AppStateManager.Instance.SetHome();
    }

    public void StartGame()
    {
        if (GameLogic == null)
        {
            GameLogic = Instantiate(GameLogicPrefab);
        }
        AppStateManager.Instance.SetGameplay();
    }
}