using UnityEngine;
using UnityEngine.UI;

namespace Volt.Utils.Dev {
    /// <summary>
    /// An invisible button in the bottom-left corner that opens the console after three quick taps: hard to hit by
    /// accident, trivial once known, and it works on phones. Its Button tint flashes it on every tap. The taps start
    /// over once the console closes.
    /// </summary>
    public class ToolsLauncher : MonoBehaviour {
        /// <summary>Taps needed to open the console.</summary>
        private const int K_RequiredTaps = 3;

        /// <summary>How long a run of taps lasts, so stray taps never add up to an opening.</summary>
        private const float K_StreakSeconds = 2f;

        [SerializeField] private Button m_tapArea;

        private int m_taps;
        private float m_firstTapTime;
        private bool m_wasOpen;

        private void Awake() {
            m_tapArea.onClick.AddListener(OnTapped);
        }

        private void OnDestroy() {
            if (m_tapArea != null) m_tapArea.onClick.RemoveListener(OnTapped);
        }

        private void Update() {
            // Closing the console costs the whole gesture again to reopen it.
            bool isOpen = Console.IsOpen();
            if (m_wasOpen && !isOpen) m_taps = 0;
            m_wasOpen = isOpen;
        }

        private void OnTapped() {
            if (Console.IsOpen()) return;

            // Unscaled: a paused or slowed game must still let the console be opened.
            float now = Time.unscaledTime;
            if ((m_taps == 0) || ((now - m_firstTapTime) > K_StreakSeconds)) {
                m_taps = 0;
                m_firstTapTime = now;
            }

            m_taps++;
            if (m_taps < K_RequiredTaps) return;

            m_taps = 0;
            Console.SetOpen(true);
        }
    }
}
