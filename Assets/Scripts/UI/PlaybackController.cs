using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Pianola
{
    [RequireComponent(typeof(UIDocument))]
    public class PlaybackControlsUI : MonoBehaviour
    {
        [SerializeField]
        private MidiPlayer _midiPlayer = null;

        [SerializeField]
        private InputActionReference _backAction = null;

        [SerializeField]
        private InputActionReference _playAction = null;

        [SerializeField]
        private InputActionReference _forwardAction = null;

        [SerializeField]
        private VectorImage _playImage = null;

        [SerializeField]
        private VectorImage _pauseImage = null;

        [SerializeField]
        private float _refreshRate = 1f;

        private Button _backButton = null;
        private Button _stopButton = null;
        private Button _playButton = null;
        private Button _forwardButton = null;

        private Label _currentTimeLabel = null;
        private Label _remainingTimeLabel = null;
        private Slider _positionSlider = null;

        private float _nextRefresh = 0f;
        private bool _isDragging = false;
        private bool _isDirty = false;

        private TrickleDown _useTrickleDown => TrickleDown.TrickleDown;

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            _backButton = root.Q<Button>("Button_Back");
            _stopButton = root.Q<Button>("Button_Stop");
            _playButton = root.Q<Button>("Button_Play");
            _forwardButton = root.Q<Button>("Button_Forward");
            _currentTimeLabel = root.Q<Label>("Label_CurrentTime");
            _remainingTimeLabel = root.Q<Label>("Label_RemainingTime");
            _positionSlider = root.Q<Slider>("Slider_PlaybackPosition");
        }

        private void OnEnable()
        {
            _backButton.clicked += OnBackClicked;
            _stopButton.clicked += OnStopClicked;
            _playButton.clicked += OnPlayPauseClicked;
            _forwardButton.clicked += OnForwardClicked;

            _positionSlider.RegisterValueChangedCallback(OnSliderDragged);
            var dragContainer = _positionSlider.Q<VisualElement>("unity-drag-container");
            dragContainer.RegisterCallback<PointerUpEvent>(OnPointerUp, _useTrickleDown);
            dragContainer.RegisterCallback<PointerDownEvent>(OnPointerDown, _useTrickleDown);

            _backAction.action.performed += OnBackActionPerformed;
            _playAction.action.performed += OnPlayActionPerformed;
            _forwardAction.action.performed += OnForwardActionPerformed;
        }

        private void OnDisable()
        {
            _backButton.clicked -= OnBackClicked;
            _stopButton.clicked -= OnStopClicked;
            _playButton.clicked -= OnPlayPauseClicked;
            _forwardButton.clicked -= OnForwardClicked;

            _positionSlider.UnregisterValueChangedCallback(OnSliderDragged);
            var dragContainer = _positionSlider.Q<VisualElement>("unity-drag-container");
            dragContainer.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            dragContainer.UnregisterCallback<PointerDownEvent>(OnPointerDown);

            _backAction.action.performed -= OnBackActionPerformed;
            _playAction.action.performed -= OnPlayActionPerformed;
            _forwardAction.action.performed -= OnForwardActionPerformed;
        }

        private void Update()
        {
            if (_isDragging)
            {
                return; // No-op.
            }

            if (_isDirty)
            {
                _isDirty = false;
                _nextRefresh = _refreshRate;
                RefreshImmediately();
                return;
            }

            if (_midiPlayer.IsPlaying)
            {
                _nextRefresh -= Time.deltaTime;
                if (_nextRefresh <= 0f) // Expired?
                {
                    _nextRefresh = _refreshRate;
                    RefreshImmediately();
                }
            }
        }

        private void RefreshImmediately()
        {
            RefreshTimeLabels(_midiPlayer.CurrentTime, _midiPlayer.Duration);

            RefreshPlaybackPosition(_midiPlayer.CurrentTime, _midiPlayer.Duration);
        }

        private void RefreshTimeLabels(float currentTime, float duration)
        {
            _currentTimeLabel.text = FormatTime(currentTime);

            _remainingTimeLabel.text = FormatTime(currentTime - duration);
        }

        private void RefreshPlaybackPosition(float currentTime, float duration)
        {
            _positionSlider.SetValueWithoutNotify(Mathf.InverseLerp(0f, duration, currentTime));
        }

        private string FormatTime(float seconds)
        {
            if (-1f < seconds && seconds < 1f)
            {
                return "0:00";
            }

            var isNegative = seconds < 0;
            seconds = Mathf.Abs(seconds);

            var mins = (int)(seconds / 60);
            var secs = (int)(seconds % 60);

            var formatted = $"{mins}:{secs:D2}";
            return isNegative ? $"-{formatted}" : formatted;
        }

        private void OnPlayActionPerformed(InputAction.CallbackContext _) => OnPlayPauseClicked();

        private void OnBackActionPerformed(InputAction.CallbackContext _) => OnBackClicked();

        private void OnForwardActionPerformed(InputAction.CallbackContext _) => OnForwardClicked();

        private void OnPlayPauseClicked()
        {
            _midiPlayer.Toggle();
        }

        private void OnStopClicked()
        {
            _midiPlayer.Stop();
        }

        private void OnBackClicked()
        {
            _midiPlayer.Seek(_midiPlayer.CurrentTime - 10);

            _isDirty = true;
        }

        private void OnForwardClicked()
        {
            _midiPlayer.Seek(_midiPlayer.CurrentTime + 10);

            _isDirty = true;
        }

        private void OnPointerDown(PointerDownEvent @event)
        {
            _isDragging = true;
        }

        private void OnPointerUp(PointerUpEvent @event)
        {
            _isDragging = false;

            _midiPlayer.Seek(_positionSlider.value * _midiPlayer.Duration);

            _isDirty = true;
        }

        private void OnSliderDragged(ChangeEvent<float> @event)
        {
            var nextTime = @event.newValue * _midiPlayer.Duration;

            RefreshTimeLabels(nextTime, _midiPlayer.Duration);
        }

        public void OnPlaybackSet(float _)
        {
            _isDirty = true;
        }

        public void OnPlayPauseChanged(bool isPlaying)
        {
            _playButton.iconImage = new Background()
            {
                vectorImage = isPlaying ? _pauseImage : _playImage,
            };

            _isDirty = true;
        }
    }
}
