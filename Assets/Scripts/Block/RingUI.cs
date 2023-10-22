using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class RingUI : MonoBehaviour
{
    [SerializeField] TMP_Text NumDisplay;
    int Nums;

    // Update is called once per frame
    void Update()
    {
        Nums = RingManager.Instance.GetBlockCount();
        NumDisplay.text = Nums.ToString();
    }
}
