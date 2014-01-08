




/// <summary>
/// Allows to upload an image to Win Azure Service using Azure API
/// </summary>
/// <param name="stream">The Stream containing image for upload</param>
/// <param name="filename">Generated filename for image on server</param>
/// <returns>True if upload was success</returns>
public async static Task<bool> UploadWinAzure(IRandomAccessStreamWithContentType stream, string filename)
{
    try
    {
        StorageCredentials credintials = new StorageCredentials("XXXXXXXXX", "XXXXXXXXXXXXXXXX");

        CloudStorageAccount acc = new CloudStorageAccount(credintials, false);

        CloudBlobClient blobCl = acc.CreateCloudBlobClient();
        CloudBlobContainer container = blobCl.GetContainerReference("datingpoint");
        await container.CreateIfNotExistsAsync();

        ICloudBlob bl = container.GetBlockBlobReference(filename);
        await bl.UploadFromStreamAsync(stream);

        return true;
    }
    catch (Exception ex) { Services.SendDialog(ex.Message); return false; }
}
