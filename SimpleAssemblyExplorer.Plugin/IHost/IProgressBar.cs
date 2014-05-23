using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace SimpleAssemblyExplorer.Plugin
{
    public interface IProgressBar
    {
        /// <summary>
        /// Initialize progress bar
        /// </summary>
        /// <param name="min">minimum value, default is 0</param>
        /// <param name="max">maximum value, default is 100</param>
        void InitProgress(int min, int max);

        /// <summary>
        /// Set current progress 
        /// </summary>
        /// <param name="val">progress value</param>
        /// <remarks>
        /// Val will be changed to minimum value if it's less than minimum value,
        /// and to maximum value if it's greater than maximum value.
        /// If val equals to minimum value, progress bar is shown, 
        /// if val equals to maximum value, progress bar is hidden.
        /// </remarks>
        void SetProgress(int val);

        /// <summary>
        /// Set current progress 
        /// </summary>
        /// <param name="val">progress value</param>
        /// <param name="doEvents">whether to call Application.DoEvents</param>
        /// <remarks>
        /// Val will be changed to minimum value if it's less than minimum value,
        /// and to maximum value if it's greater than maximum value.
        /// If val equals to minimum value, progress bar is shown, 
        /// if val equals to maximum value, progress bar is hidden.
        /// </remarks>
        void SetProgress(int val, bool doEvents);

        /// <summary>
        /// Reset progress bar to default state
        /// </summary>
        /// <remarks>
        /// Minimum value will be set to 0 and maximum value will be set to 100.
        /// And Visible will be set to false.
        /// </remarks>
        void ResetProgress();
    }
}
