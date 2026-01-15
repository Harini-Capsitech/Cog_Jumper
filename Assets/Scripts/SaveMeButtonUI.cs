using UnityEngine;

public class SaveMeButtonUI : MonoBehaviour

{

    public void OnSaveMeClicked()

    {

        Debug.Log("save me activated");

        if (GameFlowController.Instance != null)

        {
            Debug.Log("gameflow controller called ");

            GameFlowController.Instance.SaveMe();

        }

    }

}
