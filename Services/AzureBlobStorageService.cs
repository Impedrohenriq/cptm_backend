using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace CPTM_Backend.Services;

public class AzureBlobStorageService
{
    private readonly BlobContainerClient? _containerClient;
    private readonly PublicAccessType _publicAccessType;
    private readonly bool _enabled;

    public AzureBlobStorageService(IConfiguration configuration)
    {
        _enabled = !bool.TryParse(configuration["AzureBlob:Enabled"], out var parsedEnabled) || parsedEnabled;

        var connectionString = configuration["AzureBlob:ConnectionString"];
        var containerName = configuration["AzureBlob:ContainerName"];
        var publicAccess = bool.TryParse(configuration["AzureBlob:PublicAccess"], out var parsedPublic) && parsedPublic;

        if (!_enabled)
        {
            _publicAccessType = PublicAccessType.None;
            return;
        }

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("AzureBlob:ConnectionString nao configurado.");

        if (string.IsNullOrWhiteSpace(containerName))
            throw new InvalidOperationException("AzureBlob:ContainerName nao configurado.");

        _publicAccessType = publicAccess ? PublicAccessType.Blob : PublicAccessType.None;
        _containerClient = new BlobContainerClient(connectionString, containerName);
    }

    public async Task<string?> UploadBase64ImageAsync(string chavePrimariaMa, int nrFoto, string? fotoBase64, string? mimeType)
    {
        if (string.IsNullOrWhiteSpace(fotoBase64)) return null;
        if (!_enabled) return null;

        await EnsureContainerAsync();

        var content = fotoBase64;
        var commaIndex = fotoBase64.IndexOf(',');
        if (commaIndex > -1)
        {
            content = fotoBase64[(commaIndex + 1)..];
        }

        var contentType = string.IsNullOrWhiteSpace(mimeType) ? "image/jpeg" : mimeType.Trim();
        var extension = GetExtension(contentType);

        var blobName = $"{SanitizeSegment(chavePrimariaMa)}/foto-{nrFoto}-{Guid.NewGuid():N}{extension}";
        var blob = _containerClient!.GetBlobClient(blobName);

        var bytes = Convert.FromBase64String(content);
        using var stream = new MemoryStream(bytes, writable: false);

        await blob.UploadAsync(stream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType,
                CacheControl = "public, max-age=31536000"
            }
        });

        return blob.Uri.ToString();
    }

    public async Task DeleteBlobByUrlAsync(string? blobUrl)
    {
        if (string.IsNullOrWhiteSpace(blobUrl)) return;
        if (!_enabled || _containerClient is null) return;

        if (!Uri.TryCreate(blobUrl, UriKind.Absolute, out var uri)) return;
        if (!string.Equals(uri.Host, _containerClient.Uri.Host, StringComparison.OrdinalIgnoreCase)) return;

        var marker = $"/{_containerClient.Name}/";
        var path = uri.AbsolutePath;
        var markerIndex = path.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (markerIndex < 0) return;

        var blobName = Uri.UnescapeDataString(path[(markerIndex + marker.Length)..]);
        if (string.IsNullOrWhiteSpace(blobName)) return;

        try
        {
            await EnsureContainerAsync();
            await _containerClient.DeleteBlobIfExistsAsync(blobName, DeleteSnapshotsOption.IncludeSnapshots);
        }
        catch (RequestFailedException)
        {
            // A limpeza de blob nao deve interromper o fluxo principal da API.
        }
        catch (InvalidOperationException)
        {
            // Se o storage estiver indisponivel, a limpeza continua nao bloqueante.
        }
    }

    private async Task EnsureContainerAsync()
    {
        if (!_enabled || _containerClient is null)
            return;

        try
        {
            await _containerClient.CreateIfNotExistsAsync(_publicAccessType);
        }
        catch (Exception ex) when (ex is RequestFailedException || ex is HttpRequestException)
        {
            throw new InvalidOperationException(
                "Azure Blob indisponivel. Verifique AzureBlob:ConnectionString/ContainerName e se o Azurite esta ativo quando usar UseDevelopmentStorage=true.",
                ex);
        }
    }

    private static string SanitizeSegment(string value)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return string.Concat(value.Select(ch => invalid.Contains(ch) ? '-' : ch));
    }

    private static string GetExtension(string mimeType)
    {
        return mimeType.ToLowerInvariant() switch
        {
            "image/png" => ".png",
            "image/webp" => ".webp",
            "image/gif" => ".gif",
            "image/bmp" => ".bmp",
            _ => ".jpg",
        };
    }
}
