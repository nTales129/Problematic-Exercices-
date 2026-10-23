using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ProblematicExercise.Runtime
{
    public sealed class ExerciseCardView : MonoBehaviour, IPointerDownHandler, IBeginDragHandler,
        IDragHandler, IEndDragHandler, IPointerUpHandler
    {
        private RectTransform rect;
        private RectTransform canvas;
        private Vector2 home;
        private Vector3 pointerOffset;
        private bool dragging;
        private string exerciseId;
        private Action<string> select;
        private Action<string> confirm;
        private Func<bool> canInteract;

        public void Initialize(RectTransform canvas, string id, Action<string> select,
            Action<string> confirm, Func<bool> canInteract)
        {
            rect = (RectTransform)transform;
            this.canvas = canvas;
            exerciseId = id;
            this.select = select;
            this.confirm = confirm;
            this.canInteract = canInteract;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Trace("press", eventData);
            if (eventData.button == PointerEventData.InputButton.Left && canInteract()) select(exerciseId);
        }

        public void OnPointerUp(PointerEventData eventData) => Trace("up", eventData);

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left || !canInteract()) return;
            home = rect.anchoredPosition;
            // The first drag event can already be far from the press (coalesced OS
            // motion). Anchor the offset at the press, not at that first moved point.
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas, eventData.pressPosition, null, out var point);
            pointerOffset = rect.position - point;
            dragging = true;
            Trace("begin", eventData);
            rect.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!dragging || !canInteract()) return;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas, eventData.position, null, out var point);
            rect.position = point + pointerOffset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!dragging) return;
            bool chosen = canInteract() && rect.position.y >= Screen.height / 3f;
            Trace(chosen ? "release-confirm" : "release-return", eventData);
            dragging = false;
            rect.anchoredPosition = home;
            if (chosen) confirm(exerciseId);
        }

        private void OnDisable()
        {
            if (dragging && rect != null) rect.anchoredPosition = home;
            dragging = false;
        }

        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void Trace(string stage, PointerEventData data)
        {
            Debug.Log("PE_CARD " + stage + " id=" + exerciseId + " pointer=" + data.position
                + " press=" + data.pressPosition + " center=" + rect.position
                + " screen=" + Screen.width + "x" + Screen.height + " frame=" + Time.frameCount);
        }
    }
}
