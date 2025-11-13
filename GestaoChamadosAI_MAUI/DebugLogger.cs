using System.Diagnostics;
using System.Text;

namespace GestaoChamadosAI_MAUI
{
    public static class DebugLogger
    {
        private static readonly string LogFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "GestaoChamadosAI_Debug.log"
        );

        private static readonly object lockObject = new object();

        public static void Log(string message)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var logMessage = $"[{timestamp}] {message}";
            
            // Escrever no Console (aparece no logcat)
            Console.WriteLine(logMessage);
            
            // Escrever no Debug Output
            Debug.WriteLine(logMessage);
            
            // Escrever no arquivo
            try
            {
                lock (lockObject)
                {
                    File.AppendAllText(LogFilePath, logMessage + Environment.NewLine);
                }
            }
            catch
            {
                // Ignora erros ao escrever no arquivo
            }
        }

        public static void Clear()
        {
            try
            {
                lock (lockObject)
                {
                    if (File.Exists(LogFilePath))
                    {
                        File.Delete(LogFilePath);
                    }
                }
            }
            catch
            {
                // Ignora erros ao limpar o arquivo
            }
        }

        public static string GetLogPath()
        {
            return LogFilePath;
        }

        public static string ReadLogs()
        {
            try
            {
                lock (lockObject)
                {
                    if (File.Exists(LogFilePath))
                    {
                        return File.ReadAllText(LogFilePath);
                    }
                }
            }
            catch
            {
                // Ignora erros ao ler o arquivo
            }
            
            return "Nenhum log encontrado.";
        }
    }
}
