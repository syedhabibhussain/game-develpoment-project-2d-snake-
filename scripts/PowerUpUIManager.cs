using UnityEngine;
using TMPro;
using System.Collections;

public class PowerUpUIManager : MonoBehaviour
{
    public GameObject popUpPrefab;   // Drag your PowerUpPopUp prefab here
    public Canvas canvas;           // Drag your PopUpCanvas (or main Canvas) here

    public void ShowPowerUpMessage(string message, Color color) {
        GameObject popUp = Instantiate(popUpPrefab, canvas.transform);
        TextMeshProUGUI text = popUp.GetComponent<TextMeshProUGUI>();
        text.text = message;
        text.color = color;

        StartCoroutine(AnimatePopUp(popUp));
    }

    private IEnumerator AnimatePopUp(GameObject popUp) {
        TextMeshProUGUI text = popUp.GetComponent<TextMeshProUGUI>();
        CanvasGroup cg = popUp.GetComponent<CanvasGroup>();
        
        float duration = 1.5f;
        float elapsed = 0f;
        Vector3 startPos = popUp.transform.localPosition;
        Vector3 endPos = startPos + Vector3.up * 80f;

        while (elapsed < duration) {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            popUp.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            cg.alpha = Mathf.Lerp(1, 0, t);
            yield return null;
        }
        Destroy(popUp);
    }
}