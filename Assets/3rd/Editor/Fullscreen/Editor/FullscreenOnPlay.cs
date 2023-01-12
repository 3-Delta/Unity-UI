using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FullscreenEditor {
    /// <summary>Toggle fullscreen upon playmode change if <see cref="FullscreenPreferences.FullscreenOnPlayEnabled"/> is set to true.</summary>
    [InitializeOnLoad]
    internal static class FullscreenOnPlay {

        static FullscreenOnPlay() {

            #if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged += state => {
                switch (state) {
                    case PlayModeStateChange.ExitingEditMode:
                        SetIsPlaying(true);
                        break;

                    case PlayModeStateChange.ExitingPlayMode:
                        SetIsPlaying(false);
                        break;
                }
            };

            EditorApplication.pauseStateChanged += state => SetIsPlaying(EditorApplication.isPlayingOrWillChangePlaymode && state == PauseState.Unpaused);
            #else 
            EditorApplication.playmodeStateChanged += () => SetIsPlaying(EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPaused);
            #endif

        }

        private static void SetIsPlaying(bool playing) {

            var fullscreens = Fullscreen.GetAllFullscreen()
                .Select(fullscreen => fullscreen as FullscreenWindow)
                .Where(fullscreen => fullscreen);

            // We close all the game views created on play, even if the option was disabled in the middle of the play mode
            // This is done to best reproduce the default behaviour of the maximize on play
            if (!playing) {
                foreach (var fs in fullscreens)
                    if (fs && fs.CreatedByFullscreenOnPlay) // fs might have been destroyed
                        fs.Close();
                return;
            }

            if (!FullscreenPreferences.FullscreenOnPlayEnabled)
                return; // Nothing to do here

            var gameView = FullscreenUtility
                .GetGameViews()
                .FirstOrDefault(gv => gv && gv.GetPropertyValue<bool>("maximizeOnPlay"));

            if (!gameView && FullscreenUtility.GetGameViews().Length > 0)
                return;

            foreach (var fs in fullscreens)
                if (fs && fs.Rect.Overlaps(gameView.position)) // fs might have been destroyed
                    return; // We have an open fullscreen where the new one would be, so let it there

            if (gameView && Fullscreen.GetFullscreenFromView(gameView))
                return; // The gameview is already in fullscreen

            var gvfs = Fullscreen.MakeFullscreen(Types.GameView, gameView);
            gvfs.CreatedByFullscreenOnPlay = true;
        }

        [InitializeOnLoadMethod]
        private static void OverrideMaximizeOnPlay() {
            After.Frames(1, () => { // Call after one frame, so we don't acess the styles class before it's created

                var stylesClass = Types.GameView.GetNestedType("Styles", ReflectionUtility.FULL_BINDING);
                var currentContent = stylesClass.GetFieldValue<GUIContent>("maximizeOnPlayContent");

                var newContent = new GUIContent("Fullscreen on Play", FullscreenUtility.FullscreenOnPlayIcon);
                var originalContent = new GUIContent(currentContent);

                var overrideEnabled = FullscreenPreferences.FullscreenOnPlayEnabled;

                currentContent.text = overrideEnabled ? newContent.text : originalContent.text;
                currentContent.image = overrideEnabled ? newContent.image : originalContent.image;
                currentContent.tooltip = overrideEnabled ? newContent.tooltip : originalContent.tooltip;

                FullscreenPreferences.FullscreenOnPlayEnabled.OnValueSaved += v => {
                    currentContent.text = v ? newContent.text : originalContent.text;
                    currentContent.image = v ? newContent.image : originalContent.image;
                    currentContent.tooltip = v ? newContent.tooltip : originalContent.tooltip;

                    if (FullscreenUtility.GetMainGameView())
                        FullscreenUtility.GetMainGameView().SetPropertyValue("maximizeOnPlay", v);
                };

            });
        }

    }
}
