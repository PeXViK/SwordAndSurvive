using UnityEngine;
using TMPro;
using System.Collections;

public class AnnouncementController : MonoBehaviour
{
    [SerializeField] private GameObject obj;
    [SerializeField] private CanvasGroup announcement;
    [SerializeField] private TextMeshProUGUI announcementText;
    [SerializeField] private GameObject[] images;

    void Start()
    {
        announcement = obj.GetComponentInChildren<CanvasGroup>();
        announcementText = obj.GetComponentInChildren<TextMeshProUGUI>();
        announcement.alpha = 0;
        announcement.interactable = false;
    }

    public void DisplayInfo(string text, float fontSize, Color color, float time)
    {
        StartCoroutine(Info(text, fontSize, color, time));
    }


    public void Announce(string text, Color color)
    {
        obj.SetActive(true);
        announcementText.text = text;
        announcementText.color = color;
        Transform pos = announcementText.GetComponent<Transform>();
        StartCoroutine(Fade(announcement, 0, 1, 1f));
        StartCoroutine(Slide(images[0], 0.6f));
        StartCoroutine(Slide(images[1], 0.6f));
        StartCoroutine(Move(pos, 2));
        
    }

    public IEnumerator Fade(CanvasGroup fade, float from, float to, float duration)
    {
        fade.alpha = from;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            fade.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);
        elapsed = 0f;
        duration /= 2;

        while (elapsed < duration)
        {
            fade.alpha = Mathf.Lerp(to, from, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        fade.alpha = from;

        obj.SetActive(false);
    }

    public IEnumerator Slide(GameObject img, float duration)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        float elapsed = 0f;
        while (elapsed < duration)
        {
            img.transform.localScale = new Vector3(Mathf.Lerp(0f, 1f, elapsed / duration), 1, 1);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator Move(Transform obj, float duration)
    {
        float elapsed = 0f;

        Vector3 to = new Vector3(obj.position.x + 50, obj.position.y + 50, obj.position.z);

        Vector3 from = obj.position;
        while (elapsed < duration)
        {
            obj.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.position = from;
    }

    private IEnumerator Info(string text, float fontSize, Color color, float time)
    {
        Vector3 pos = announcement.transform.position;
        announcement.transform.position = new Vector3(announcementText.transform.position.x, -240, announcementText.transform.position.z);
        announcementText.text = text;
        announcementText.fontSize = fontSize;
        announcementText.color = color;
        announcement.alpha = 1;
        obj.SetActive(true);
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
        announcement.alpha = 0;
        announcement.transform.position = pos;
    }
}
