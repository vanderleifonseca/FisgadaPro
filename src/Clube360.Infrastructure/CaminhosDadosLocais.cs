namespace Clube360.Infrastructure;

public static class CaminhosDadosLocais
{
    // Compatibility path for installations created before the Clube360 rename.
    public static string Preparar(string raizApi)
    {
        var atual = Path.Combine(raizApi, "App_Data");
        var legado = Path.GetFullPath(Path.Combine(raizApi, "..", "FisgadaPro.Api", "App_Data"));
        if (Directory.Exists(atual) || !Directory.Exists(legado)) return atual;

        var arquivos = new[] { "clubes.json", "contas.json" }
            .Where(nome => File.Exists(Path.Combine(legado, nome))).ToArray();
        if (arquivos.Length == 0) return atual;

        var temporario = atual + ".migracao-" + Guid.NewGuid().ToString("N");
        Directory.CreateDirectory(temporario);
        try
        {
            foreach (var nome in arquivos)
                File.Copy(Path.Combine(legado, nome), Path.Combine(temporario, nome));
            // Publish both files together, retaining the original directory as a backup.
            Directory.Move(temporario, atual);
        }
        finally
        {
            if (Directory.Exists(temporario)) Directory.Delete(temporario, true);
        }
        return atual;
    }
}
