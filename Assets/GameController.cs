using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject endPanel;
    public GameObject endPoints;
    public Camera deathCamera;
    public GameObject dieReason;
    public GameObject pointsPanel;
    public GameObject pointsText;
    public static bool Running;
    private float _measureTime;
    private int _points;
    private bool _holdingBabies;

    public void StartGame()
    {
        Destroy(startPanel);
        pointsPanel.SetActive(true);
        UpdatePoints(0);
        Running = true;
        _measureTime = Time.time;
    }

    public void Reload()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void NotifyBabyDied(Vector2 at, NotSafeStuff.DamageType damageType, string notSafeStuff)
    {
        Running = false;
        deathCamera.transform.position = new Vector3(at.x, at.y, -10);
        dieReason.GetComponent<TextMeshProUGUI>().SetText
        (
            "Die Reason: " + damageType.ToString() + "\n" + notSafeStuff
        );
        endPoints.GetComponent<TextMeshProUGUI>().SetText("GAME OVER - "+_points.ToString()+" Points");
        pointsPanel.SetActive(false);
        endPanel.SetActive(true);
    }

    public void NotifyBabyGrabbed()
    {
        _holdingBabies = true;
    }

    public void NotifyNoBabiesGrabbed()
    {
        _holdingBabies = false;
    }

    void Awake()
    {
        startPanel.SetActive(true);
        pointsPanel.SetActive(false);
    }

    void Update()
    {
        if(Running && !_holdingBabies && Time.time >= _measureTime+1)
        {
            _measureTime = Time.time;
            UpdatePoints(++_points);
        }
    }

    void UpdatePoints(int point)
    {
        pointsText.GetComponent<TextMeshProUGUI>().SetText(point.ToString()+" Points");
    }
}
