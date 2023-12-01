using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrameUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TxtFrame = null;
    
    private void Update()
    {
        m_TxtFrame.text = (Time.frameCount % 100).ToString("D2");
    }
}
