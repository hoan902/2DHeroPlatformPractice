using System;
using System.Collections.Generic;
using UnityEngine;

public class Trigger2DHelper : MonoBehaviour
{
    [SerializeField] private LayerMask m_layerMask;
    [SerializeField] private List<TriggerHandler> m_onTriggerEnter2D;
    [SerializeField] private List<TriggerHandler> m_onTriggerExit2D;
    [SerializeField] private List<TriggerHandler> m_onTriggerStay2D;
    [SerializeField] private List<TriggerHandler> m_onCollisionEnter2D;
    [SerializeField] private List<TriggerHandler> m_onCollisionExit2D;
    [SerializeField] private List<TriggerHandler> m_onCollisionStay2D;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & m_layerMask) != 0 || m_layerMask == 0)
        {
            foreach (TriggerHandler h in m_onTriggerEnter2D)
            {
                if (h == null)
                    continue;
                Send(h, collision);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & m_layerMask) != 0 || m_layerMask == 0)
        {
            foreach (TriggerHandler h in m_onTriggerExit2D)
            {
                if (h == null)
                    continue;
                Send(h, collision);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & m_layerMask) != 0 || m_layerMask == 0)
        {
            foreach (TriggerHandler h in m_onTriggerStay2D)
            {
                if (h == null)
                    continue;
                Send(h, collision);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & m_layerMask) != 0 || m_layerMask == 0)
        {
            foreach (TriggerHandler h in m_onCollisionEnter2D)
            {
                if (h == null)
                    continue;
                Send(h, collision);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & m_layerMask) != 0 || m_layerMask == 0)
        {
            foreach (TriggerHandler h in m_onCollisionExit2D)
            {
                if (h == null)
                    continue;
                Send(h, collision);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & m_layerMask) != 0 || m_layerMask == 0)
        {
            foreach (TriggerHandler h in m_onCollisionStay2D)
            {
                if (h == null)
                    continue;
                Send(h, collision);
            }
        }
    }

    void Send(TriggerHandler h, object data)
    {
        if (h.sender == null)
            h.gameObject.SendMessage(h.method, data);
        else
            h.gameObject.SendMessage(h.method, new CollisionData() { sender = h.sender, data = data });
    }

    [Serializable]
    private class TriggerHandler
    {
        public GameObject sender;
        public GameObject gameObject;
        public string method;
    }
}

public class CollisionData
{
    public GameObject sender;
    public object data;
}
