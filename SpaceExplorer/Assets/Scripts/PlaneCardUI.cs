using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaneCardUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI planeNameText;

    [SerializeField]
    private Image planeImage;

    [SerializeField]
    private Slider planeSpeed;

    [SerializeField]
    private Slider fireRate;

    [SerializeField]
    private Transform colorsPanel;

    [SerializeField]
    private Button buyBtn;

    [SerializeField]
    private TextMeshProUGUI priceText;

    [SerializeField]
    private Button equipBtn;

    public GameObject colorBtnPrefab;
    private PlaneData planeData;
    private UserData userData;

    public void Setup(PlaneData data, UserData userData, System.Action onUpdateUI)
    {
        planeData = data;
        this.userData = userData;

        planeImage.sprite = data.selectedSprite;
        planeNameText.text = data.planeName;
        planeSpeed.value = data.speed;
        fireRate.value = 1f - data.fireRate;
        priceText.text = data.price.ToString();

        foreach (Transform child in colorsPanel)
            Destroy(child.gameObject);

        foreach (var color in data.colorOptions)
        {
            GameObject btnObj = Instantiate(colorBtnPrefab, colorsPanel);
            Button btn = btnObj.GetComponent<Button>();
            Image btnImage = btnObj.GetComponent<Image>();
            if (ColorUtility.TryParseHtmlString(color.hexCode, out Color colorVal))
                btnImage.color = colorVal;

            btn.onClick.AddListener(() =>
            {
                data.selectedSprite = color.spriteName;
                Setup(data, userData, onUpdateUI); // Refresh
                onUpdateUI?.Invoke();
            });
        }

        buyBtn.gameObject.SetActive(!data.isUnlocked);
        equipBtn.gameObject.SetActive(data.isUnlocked);

        buyBtn.onClick.RemoveAllListeners();
        buyBtn.onClick.AddListener(() =>
        {
            if (userData.coins >= data.price)
            {
                userData.coins -= data.price;
                data.isUnlocked = true;
                buyBtn.gameObject.SetActive(false);
                equipBtn.gameObject.SetActive(true);
                userData.ownedPlanes.Add(data);
                SaveGame(userData);
                Setup(data, userData, onUpdateUI);
                onUpdateUI?.Invoke();
            }
            else
            {
                ShopManager.Instance.ShowNotEnoughCoinsDialog();
            }
        });

        equipBtn.onClick.RemoveAllListeners();

        equipBtn.GetComponentInChildren<TMP_Text>().text =
            userData.equippedPlaneName == data.planeName ? "Equipped" : "Equip";

        if (data.planeName != userData.equippedPlaneName)
        {
            equipBtn.onClick.AddListener(() =>
            {
                userData.equippedPlaneName = data.planeName;
                userData.equippedPlane = data;
                equipBtn.GetComponentInChildren<TMP_Text>().text = "Equipped";
                SaveGame(userData);
                Setup(data, userData, onUpdateUI);
                onUpdateUI?.Invoke();
            });
        }
    }

    public void SaveGame(UserData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("SpaceShooter_UserData", json);
        PlayerPrefs.Save();
    }
}
