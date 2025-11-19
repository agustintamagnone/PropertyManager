// Clase de ayuda para capturar la salida de la consola (implementa IDisposable para el 'using')
public class ConsoleOutput : IDisposable
{
    private readonly StringWriter _stringWriter;
    private readonly TextWriter _originalOutput;

    public ConsoleOutput()
    {
        // 1. Guardar la salida original de la consola
        _originalOutput = Console.Out;
        
        // 2. Crear un StringWriter para capturar la nueva salida
        _stringWriter = new StringWriter();
        
        // 3. Redirigir la salida de la consola a nuestro StringWriter
        Console.SetOut(_stringWriter);
    }

    public string GetOuput()
    {
        // Obtener el contenido capturado y normalizar los saltos de línea (\r\n)
        return _stringWriter.ToString().TrimEnd(new char[] { '\r', '\n' });
    }

    public void Dispose()
    {
        // 4. Restaurar la salida original de la consola cuando el bloque 'using' termina
        Console.SetOut(_originalOutput);
        _stringWriter.Dispose();
    }
}