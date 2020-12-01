using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineAnimation : MonoBehaviour
{
    public Vector3 axis { get { return m_Axis; } set { m_Axis = value; } }
    [SerializeField]
    private Vector3 m_Axis = Vector3.up;

    public float period { get { return m_Period; } set { m_Period = value; } }
    [SerializeField]
    private float m_Period = 1f / Mathf.PI;

    public float amplitude { get { return m_Amplitude; } set { m_Amplitude = value; } }
    [SerializeField]
    private float m_Amplitude = 1f;

    public float phaseShift { get { return m_PhaseShift; } set { m_PhaseShift = Mathf.Clamp01(value); } }
    [SerializeField, Range(0f, 1f)]
    private float m_PhaseShift;

    void Update()
    {
        transform.localPosition = m_Axis * m_Amplitude * Mathf.Sin((Time.time + m_PhaseShift) / m_Period);
    }

    void OnValidate()
    {
        m_PhaseShift = Mathf.Clamp01(m_PhaseShift);
    }
}
