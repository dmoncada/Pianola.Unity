using Pianola.Input;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pianola
{
    [RequireComponent(typeof(UIDocument))]
    public class PlaybackController : MonoBehaviour
    {
        private const TrickleDown UseTrickleDown = TrickleDown.TrickleDown;

        [SerializeField]
        private MidiPlayer _midiPlayer = null;

        [SerializeField]
        private InputActionButton _onScreenBackButton = default;

        [SerializeField]
        private InputActionButton _onScreenStopButton = default;

        [SerializeField]
        private InputActionButton _onScreenPlayButton = default;

        [SerializeField]
        private InputActionButton _onScreenForwardButton = default;

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

            _onScreenBackButton.UIToolkitButton = _backButton;
            _onScreenStopButton.UIToolkitButton = _stopButton;
            _onScreenPlayButton.UIToolkitButton = _playButton;
            _onScreenForwardButton.UIToolkitButton = _forwardButton;
        }

        private void OnEnable()
        {
            _onScreenBackButton.Bind(OnBackActionPerformed);
            _onScreenStopButton.Bind(OnStopActionPerformed);
            _onScreenPlayButton.Bind(OnPlayActionPerformed);
            _onScreenForwardButton.Bind(OnForwardActionPerformed);

            _positionSlider.RegisterValueChangedCallback(OnSliderDragged);
            var dragContainer = _positionSlider.Q<VisualElement>("unity-drag-container");
            dragContainer.RegisterCallback<PointerUpEvent>(OnPointerUp, UseTrickleDown);
            dragContainer.RegisterCallback<PointerDownEvent>(OnPointerDown, UseTrickleDown);
        }

        private void OnDisable()
        {
            _onScreenBackButton.Unbind();
            _onScreenStopButton.Unbind();
            _onScreenPlayButton.Unbind();
            _onScreenForwardButton.Unbind();

            _positionSlider.UnregisterValueChangedCallback(OnSliderDragged);
            var dragContainer = _positionSlider.Q<VisualElement>("unity-drag-container");
            dragContainer.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            dragContainer.UnregisterCallback<PointerDownEvent>(OnPointerDown);
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
            _currentTimeLabel.text = Utils.FormatTime(currentTime);

            _remainingTimeLabel.text = Utils.FormatTime(currentTime - duration);
        }

        private void RefreshPlaybackPosition(float currentTime, float duration)
        {
            _positionSlider.SetValueWithoutNotify(Mathf.InverseLerp(0f, duration, currentTime));
        }

        private void OnPlayActionPerformed()
        {
            _midiPlayer.Toggle();
        }

        private void OnStopActionPerformed()
        {
            _midiPlayer.Stop();
        }

        private void OnBackActionPerformed()
        {
            _midiPlayer.Seek(_midiPlayer.CurrentTime - 10);

            _isDirty = true;
        }

        private void OnForwardActionPerformed()
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
