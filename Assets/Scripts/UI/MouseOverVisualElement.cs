using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Pianola
{
    // See: https://docs.unity3d.com/6000.0/Documentation/Manual/UIE-faq-event-and-input-system.html
    public class MouseOverVisualElement : MonoBehaviour
    {
        private readonly List<VisualElement> _hoveredElements = new();

        [SerializeField]
        private UIDocument _document = null;

        [SerializeField]
        private string[] _trackedElementNames = new string[0];

        public bool Hovered { get; private set; } = false;

        private Mouse _mouse = null;
        private IPanel _panel = null;
        private HashSet<string> _elementNames = null;

        private void Awake()
        {
            _mouse = Mouse.current;
            _panel = _document.rootVisualElement.panel;
            _elementNames = new(_trackedElementNames);
        }

        private void Update()
        {
            var position = _mouse.position.value;
            position.y = Screen.height - position.y;
            position = RuntimePanelUtils.ScreenToPanel(_panel, position);

            _hoveredElements.Clear();
            _panel.PickAll(position, _hoveredElements);

            if (_hoveredElements.Count == 0)
            {
                return;
            }

            var detected = _hoveredElements.Any(e => _elementNames.Contains(e.name));
            if (detected && Hovered == false)
            {
                Hovered = true;
            }
            else if (Hovered && detected == false)
            {
                Hovered = false;
            }
        }
    }
}
