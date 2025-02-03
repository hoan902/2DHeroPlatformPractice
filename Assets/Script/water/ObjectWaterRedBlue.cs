using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading;
using Game2DWaterKit;
using Sirenix.OdinInspector;

public class ObjectWaterRedBlue : MonoBehaviour
{
    [SerializeField] private Vector2 m_size = Vector2.one;
    [SerializeField] private int m_damage = 100;
    [SerializeField] private Color m_color = Color.red;
    [SerializeField] private bool m_muteSound = false;
    [FoldoutGroup("References")]
    [SerializeField] private BoxCollider2D m_collider;
    [FoldoutGroup("References")]
    [SerializeField] private BuoyancyEffector2D m_buoyancyEffector;
    [FoldoutGroup("References")]
    [SerializeField] private Game2DWater m_renderer;
    
    private readonly Dictionary<Transform, (float startTime, CancellationTokenSource canceler)> m_processed = new();//save to fix case player immunity but still inside
    private List<Transform> m_blocked = new();//cache victim to fix case key component disable will ping remove target and ping again when enable but trigger enter call earlier

    private readonly CancellationTokenSource m_asyncCanceler = new();

    void Start()
    {
        m_collider.enabled = false;
        AdjustComponentSizes();
        m_collider.enabled = true;
    }

    void OnDestroy()
    {
        m_asyncCanceler?.Cancel();
        m_asyncCanceler?.Dispose();
        foreach (var e in m_processed)
        {
            e.Value.canceler?.Cancel();
            e.Value.canceler?.Dispose();
        }
    }

    public void AdjustComponentSizes()
    {
        float half = m_size.y / 2;
        float offsetY = 0.2f;
        m_collider.size = m_size;
        m_buoyancyEffector.surfaceLevel = half - offsetY;
        //      
        m_renderer.MainModule.SetSize(m_size + new Vector2(0, 0.5f), true);
        m_renderer.transform.localScale = Vector3.one;
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var size = m_size + new Vector2(0, 0.5f);
        Gizmos.color = m_color;
        Gizmos.DrawCube(transform.position,size);
    }
#endif
}
