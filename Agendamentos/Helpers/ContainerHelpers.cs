namespace Agendamentos.Helpers;

public static class ContainerHelpers
{
    public static  bool TryGetAppDirectoryFolder(string path, out DirectoryInfo directory)
    {
    
        directory = new DirectoryInfo(path);
        try
        {
            return directory.Exists;

        }
        catch
        {
            return false;
        }

    }
}