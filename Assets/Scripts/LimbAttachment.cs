using System;
using System.ComponentModel;
using Unity.Mathematics;
using UnityEngine;

public class LimbAttachment : MonoBehaviour
{
    private AttachmentPoint _attachmentTarget;

    private float _attachmentStartTime;

    [Description("Time to travel from positon to being attached in seconds")]
    public float totalAttachmentTime = 1;
    public float accelerationRate = 2;

    public void Attach(AttachmentPoint attachmentTarget)
    {
        _attachmentTarget = attachmentTarget;
        _attachmentStartTime = Time.time;
    }

    public void Detach()
    {
        _attachmentTarget = null;
        transform.SetParent(null);
    }

    private void Update()
    {
        if (!_attachmentTarget || transform.parent == _attachmentTarget.transform) return;

        var progress = math.pow(
            (Time.time - _attachmentStartTime) / totalAttachmentTime,
            accelerationRate
            );
        
        if (totalAttachmentTime <= (Time.time - _attachmentStartTime))
        {
            transform.SetParent(_attachmentTarget.transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            return;
        }

        transform.position = math.lerp(transform.position, _attachmentTarget.transform.position, progress);
        transform.rotation = math.slerp(transform.rotation, _attachmentTarget.transform.rotation, progress);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 1, .6f);
        Gizmos.DrawSphere(transform.position, .6f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, .2f);
        Gizmos.DrawSphere(transform.position, .6f);
    }
}