using System;
using System.Text;
using System.Windows;

namespace ReservesOfRussia.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Displays a detailed error message, including inner exceptions.
        /// </summary>
        /// <param name="messagePrefix">A user-friendly message to display first.</param>
        /// <param name="ex">The exception that occurred.</param>
        public static void ShowError(string messagePrefix, Exception ex)
        {
            var detailedMessage = new StringBuilder();
            detailedMessage.AppendLine(messagePrefix);
            detailedMessage.AppendLine();

            Exception current = ex;
            while (current != null)
            {
                detailedMessage.AppendLine($"---\n{current.Message}");
                current = current.InnerException;
            }

            MessageBox.Show(detailedMessage.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
