namespace Empire.IO
{
    /// <summary>
    /// EmpireCloudFolders - navigate the cloud folders for Empire
    /// </summary>
    public static class EmpireCloudFolders
	{
		#region Old code
		/*public static string EmpireFileShareCloudStorageConnectionString
		{
			get {
				string storageAccountName = "empirefileshare";
				string storageAccountKey = "v1eJbZDvBQAETzbttxEHlws7W2HK84oxSdpzRlzYysHb2P23hqzmfqWuP1Oz5I3xofJydUBKcxjzFjBcuZEfvA==";
				return string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", storageAccountName, storageAccountKey);
			}
		}*/
		#endregion

		public const string ShareName = "apps";
		public const string PartsImages = "PartsImages";
		public const string UploadImages = "Uploaded_Images";
		public const string Thumbnails = "Thumbnails";
		public const string SignatureImages = "Signature_Images";
	}
}
