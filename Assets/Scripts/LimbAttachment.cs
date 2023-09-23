using System;
using System.ComponentModel;
using Unity.Mathematics;
using UnityEngine;

public class LimbAttachment : MonoBehaviour
{
    private AttachmentPoint _attachmentTarget;
    public bool Attachment => _attachmentTarget != null;

    private float _attachmentStartTime;

    [Description("Time to travel from positon to being attached in seconds")]
    public float totalAttachmentTime = 1;

    public AnimationCurve accelerationRate = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public void Attach(AttachmentPoint attachmentTarget)
    {
        if (attachmentTarget.IsAttached || attachmentTarget.IsTargeted)
        {
            Debug.Log($"${attachmentTarget} is already in use");
            return;
        }

        _attachmentTarget = attachmentTarget;
        _attachmentStartTime = Time.time;

        _attachmentTarget.IsTargeted = true;
    }

    public void Detach()
    {
        _attachmentTarget.IsAttached = false;
        _attachmentTarget.IsTargeted = false;
        _attachmentTarget = null;
        transform.SetParent(null);
    }

    private void Update()
    {
        if (!_attachmentTarget || _attachmentTarget.IsAttached) return;

        var progress = accelerationRate.Evaluate((Time.time - _attachmentStartTime) / totalAttachmentTime);

        if (totalAttachmentTime <= (Time.time - _attachmentStartTime))
        {
            transform.SetParent(_attachmentTarget.transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            _attachmentTarget.IsAttached = true;
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
        var position = transform.position;
        Gizmos.DrawSphere(position, .6f);

        Gizmos.color = new Color(1, .4f, .8f);
        if (_attachmentTarget) Gizmos.DrawLine(position, _attachmentTarget.transform.position);
    }
}