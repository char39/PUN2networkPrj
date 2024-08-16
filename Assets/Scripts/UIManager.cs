using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Transform hudUI;
    private Text ammoText;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        ammoText = hudUI.GetChild(0).GetChild(0).GetComponent<Text>();
    }

    void Update()
    {

    }

    public void AmmoTextUpdate(int magAmmo, int ammoRemain)
    {
        ammoText.text = $"{magAmmo}/{ammoRemain}";
    }
}
