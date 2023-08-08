using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slide : MonoBehaviour
{
    public RectTransform fill;
    public Image fillSlider;
    public TextMeshProUGUI number;
    public int value;
    public float endFill;
    public int maxNumber;
    public bool maxText;
    public bool minText;
    public int rate;
    private int currentNum;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(value - currentNum) > rate)
        {
            if (currentNum < value)
            {
                currentNum += rate;
            }
            else if (currentNum > value)
            {
                currentNum -= rate;
            }
        }
        else
        {
            currentNum = value;
        }

        float x = (float)currentNum / (float)maxNumber * endFill;
        float y = fill.sizeDelta.y;
        fillSlider.fillAmount = (x);
        //Debug.Log("x: " + x +  " - y: " + y);
        //fill.sizeDelta = new Vector2(x, y);
        if (number != null)
        {
            if (maxText && currentNum == maxNumber)
            {
                number.text = "MAX";
            }
            else
            {
                number.text = currentNum.ToString();
            }
        }
    }

    private int IntLerp(int a, int b, float t)
    {
        if (t > 0.9999f)
        {
            return b;
        }
        return a + (int)(((float)b - (float)a) * t);
    }
}