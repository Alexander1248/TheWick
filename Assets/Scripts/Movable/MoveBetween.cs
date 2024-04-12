﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Movable
{
    public class MoveBetween : MonoBehaviour
    {
        private readonly List<Transform> _onPlatform = new();
        
        [SerializeField] private Vector3 from;
        [SerializeField] private Vector3 to;

        private void Start()
        {
            var position = transform.position;
            from += position;
            to += position;
        }

        public void SetProgress(float progress)
        {
            _onPlatform.ForEach(t => t.position -= transform.position);
            transform.position = Vector3.Lerp(from, to, progress);
            _onPlatform.ForEach(t => t.position += transform.position);
        }

        private void OnDisable()
        {
            var position = transform.position;
            from -= position;
            to -= position;
        }

        private void OnCollisionEnter(Collision other)
        {
            _onPlatform.Add(other.transform);
        }

        private void OnCollisionExit(Collision other)
        {
            _onPlatform.Remove(other.transform);
        }
    }
}