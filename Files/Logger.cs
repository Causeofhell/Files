using Files.Interface;
using System;
using System.IO;

public class Logger : ILog
{
    private static readonly string logFilePath = "E:\\FilesTest\\log.txt";

    public void Log(string message)
    {
        // Construye el mensaje completo con la fecha y hora.
        string logMessage = $"{DateTime.Now}: {message}";

        // Escribe el mensaje en la consola.
        Console.WriteLine(logMessage);

        // Asegura que el archivo de log se escriba de forma segura incluso en entornos multi-thread.
        lock (logFilePath)
        {
            // Escribe el mensaje en el archivo 'log.txt'.
            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine(logMessage);
            }
        }
    }
}
