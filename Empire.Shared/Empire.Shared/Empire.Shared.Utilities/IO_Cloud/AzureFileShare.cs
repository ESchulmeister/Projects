using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;

namespace Empire.Shared.Utilities
{
	public class AzureFileShare 
	{
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/// Member Fields and Properties
		protected string azureFileShareConnectionString { get; set; }

		protected string azureFileShareName { get; set; }

      

        /// Construction
        public AzureFileShare(string connString)
		{
			azureFileShareConnectionString = connString;
		}
		public AzureFileShare(string connString, string shareName)
		{
			azureFileShareConnectionString = connString;
			azureFileShareName = shareName;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/// Public members
		/// <summary>
		/// GetCloudFileStreamPartImage
		/// Uses the Parts folder structure to find the appropriate image
		/// </summary>
		/// <param name="partsImagesFolderName"></param>
		/// <param name="imageFileName"></param>
		/// <returns></returns>
		public MemoryStream GetCloudFileStreamPartImage(string imageFileName)
		{
			return this.GetCloudFileStreamPartImage(AppSettings.Current.PartsImages, imageFileName);
		}


		public MemoryStream GetCloudFileStreamPartImage(string partsImagesFolderName, string imageFileName)
		{
			string imageFileDirectory = string.Format("{0}/{1}", partsImagesFolderName, imageFileName.Substring(0, 2));
			return GetCloudFileStream(imageFileDirectory, imageFileName);
		}

		public MemoryStream GetCloudFileStream(string fileDirectory, string fileName)
		{
			MemoryStream memstream = new MemoryStream();

			try
			{
				fileDirectory = CleanRelativeCloudDirectoryName(fileDirectory);

				CloudStorageAccount cloudStorageAccount = GetCloudStorageAccount();
				CloudFileClient fileClient = GetCloudFileClient(cloudStorageAccount);
				CloudFileShare share = GetCloudFileShareReference(fileClient);				
				CloudFileDirectory shareDir = GetCloudFileDirectory(share, fileDirectory);
				if ((shareDir != null) && shareDir.Exists())
				{
					CloudFile file = GetCloudFile(shareDir, fileName);
					if (file != null)
						file.DownloadToStream(memstream);
				}
			}
			catch (Exception oExeption)
			{
				oExeption.Log($"GetCloudFileStream- {fileName}");
			}
			return memstream;
		}

		/// <summary>
		/// Save a Signature Image (overtly simplified)
		/// ensures date path
		/// </summary>
		/// <param name="memStreamFileData"></param>
		/// <param name="fileName"></param>
		public bool SaveCloudFileStreamSignatureImage(MemoryStream memStreamFileData, string fileName)
		{
			// 20170731 - the signature files will be saved in date folders, one for each day
			// if it doesn't exist, we'll create it
			string folderDatePath = $"{DateTime.Now.ToString("yyyyMMdd")}";

			string fileSaveDirectory = String.Format("{0}/{1}", AppSettings.Current.SignatureImages, folderDatePath);

			// ensure the directory exists or create it
			if (CreateDirectory(fileSaveDirectory))
			{
				return SaveCloudFileStream(memStreamFileData, fileSaveDirectory, fileName);
			}
			return false;
		}
		/// <summary>
		/// Save an Upload Image
		/// </summary>
		/// <param name="memStreamFileData"></param>
		/// <param name="fileName"></param>
		/*public bool SaveCloudFileStreamUploadImage(MemoryStream memStreamFileData, string fileName)
		{
			string fileSaveDirectory = "";
			return SaveCloudFileStream(memStreamFileData, fileSaveDirectory, fileName);
		}*/
		/// <summary>
		/// Save a file via MemoryStream to the share
		/// </summary>
		/// <param name="memStreamFileData"></param>
		/// <param name="fileName"></param>
		public bool SaveCloudFileStream(MemoryStream memStreamFileData, string fileSaveDirectory, string saveFileName, bool AllowOverWriteExistingFile = false)
		{
			try
			{
				fileSaveDirectory = this.CleanRelativeCloudDirectoryName(fileSaveDirectory);

				CloudStorageAccount cloudStorageAccount = this.GetCloudStorageAccount();
				CloudFileClient fileClient = this.GetCloudFileClient(cloudStorageAccount);
				CloudFileShare share = this.GetCloudFileShareReference(fileClient);				
				CloudFileDirectory shareDir = this.GetCloudFileDirectory(share, fileSaveDirectory);
				if (!(shareDir != null) && shareDir.Exists())
				{
					return false;
				}

				CloudFile cloudfile = shareDir.GetFileReference(saveFileName);
				if (cloudfile.Exists())
				{
					/// ??? - overwrite?
					if (AllowOverWriteExistingFile)
					{
						return false;		// for now, we can't let this pass!  true;
					}

					throw new DuplicateFileException(saveFileName);

					//ExplodeStaticError(string.Format("{0} [{1}]", "GetCloudFileStream", saveFileName), "File Already Exists!");
					//return false;
				}

				memStreamFileData.Position = 0;
				cloudfile.UploadFromStream(memStreamFileData);
				return true;
			}
			catch (Exception oExeption)
			{
				oExeption.Log($"GetCloudFileStream - [{saveFileName}]");
			}
			return false;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/// Public helpers members
		/// 
		public bool FileExists(string fileDirectory, string fileName)
		{
			try
			{
				fileDirectory = CleanRelativeCloudDirectoryName(fileDirectory);

				CloudStorageAccount cloudStorageAccount = GetCloudStorageAccount();
				CloudFileClient fileClient = GetCloudFileClient(cloudStorageAccount);
				CloudFileShare share = GetCloudFileShareReference(fileClient);
				CloudFileDirectory cloudFileDirectory = GetCloudFileDirectory(share, fileDirectory);
				if ((cloudFileDirectory != null) && cloudFileDirectory.Exists())
				{
					CloudFile cloudFile = cloudFileDirectory.GetFileReference(fileName);
					//CloudFile file = GetCloudFile(shareDir, fileName);
					return cloudFile.Exists();
				}
			}
			catch (Exception oExeption)
			{
				oExeption.Log($"FileExits -  [{fileDirectory}\\{fileName}]");				
			}
			return false;
		}
		public bool DirectoryExists(string directoryName)
		{
			try
			{
				directoryName = CleanRelativeCloudDirectoryName(directoryName);
				
				CloudStorageAccount cloudStorageAccount = GetCloudStorageAccount();
				CloudFileClient fileClient = GetCloudFileClient(cloudStorageAccount);
				CloudFileShare cloudFileShare = GetCloudFileShareReference(fileClient);

				CloudFileDirectory cloudRootShareDirectory = cloudFileShare.GetRootDirectoryReference();
				CloudFileDirectory cloudShareDirectory = cloudRootShareDirectory.GetDirectoryReference(directoryName);
				//CloudFileDirectory shareDir = GetCloudFileDirectory(share, directoryName);
				return cloudShareDirectory.Exists();
			}
			catch (Exception oExeption)
			{
				oExeption.Log( $"DirectoryExists-{directoryName}");
				return false;
			}
		}

		/// <summary>
		/// Create a directory (if it doesn't exist already)
		/// </summary>
		/// <param name="directoryName"></param>
		/// <returns></returns>
		public bool CreateDirectory(string directoryName)
		{
			try
			{
				directoryName = CleanRelativeCloudDirectoryName(directoryName);

				CloudStorageAccount cloudStorageAccount = GetCloudStorageAccount();
				CloudFileClient fileClient = GetCloudFileClient(cloudStorageAccount);
				CloudFileShare cloudFileShare = GetCloudFileShareReference(fileClient);       // share

				/// 20180106 - we need a ref to the directory in order to create it - our create returns null
				/// 20180106 - modified Get/Check to return ref when not found
				// Get a reference to the root directory for the share
				/*CloudFileDirectory cloudRootShareDirectory = cloudFileShare.GetRootDirectoryReference();

				directoryName = CleanRelativeCloudDirectoryName(directoryName);

				// Get a reference to the image directory
				CloudFileDirectory cloudShareDirectory = cloudRootShareDirectory.GetDirectoryReference(directoryName);

				if (!cloudShareDirectory.Exists())
					cloudRootShareDirectory.Create();
				////////  */
				CloudFileDirectory shareDir = GetCloudFileDirectory(cloudFileShare, directoryName);     // share, 
				if ((shareDir != null) && !shareDir.Exists())     // (shareDir == null) || 
					shareDir.Create();
				////////

				return true;
			}
			catch (Exception oExeption)
			{
				oExeption.Log($"CreateDirectory  - [{directoryName}]");
				return false;
			}
		}

		public bool DeleteFile(string fileDirectory, string fileName)
		{
			try
			{
				fileDirectory = CleanRelativeCloudDirectoryName(fileDirectory);

				CloudStorageAccount cloudStorageAccount = GetCloudStorageAccount();
				CloudFileClient fileClient = GetCloudFileClient(cloudStorageAccount);
				CloudFileShare share = GetCloudFileShareReference(fileClient);
				CloudFileDirectory shareDir = GetCloudFileDirectory(share, fileDirectory);
				CloudFile file = GetCloudFile(shareDir, fileName);
				if (file.Exists())
				{
					file.Delete();
					return true;
				}
			}
			catch (Exception oExeption)
			{
				oExeption.Log($"DeleteFile - [{fileDirectory}\\{fileName}]");				
			}
			return false;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/// new 20171207
		///
		/// <summary>
		/// FindFilesInCloudFileDirectory - searches for files using the typical wildcard ('*') IO structure
		/// </summary>
		public FileInfo[] FindFilesInCloudFileDirectory(string fileDirectory, string searchString)
		{
			List<FileInfo> fileInfos = new List<FileInfo>();

			// 20171208 - allow also for: _ ( ) { } [ ]
			string regexString = searchString.Replace("*", @"[A-Za-z\d\[\]\{\},%=_()!@^&$~`+-]{0,}").Replace(".", "\\.");

			IEnumerable<CloudFile> iListFileItems = GetFilesInCloudFileDirectory(fileDirectory).Where(t => Regex.IsMatch(t.Name, regexString, RegexOptions.IgnoreCase));

			if ((iListFileItems != null) && iListFileItems.Count() > 0)
			{
				foreach (CloudFile cfile in iListFileItems)
				{
					FileInfo fileInfo = new FileInfo(cfile.Name);   // 20180104 bug fix!!! iListFileItems.FirstOrDefault().Name);
					fileInfos.Add(fileInfo);
				}
			}
			return fileInfos.ToArray();
		}
		/// <summary>
		/// NEW! 20180109 - returns CloudFiles to support FileLength!
		/// FindFilesInCloudFileDirectory - searches for files using the typical wildcard ('*') IO structure
		/// </summary>
		public IEnumerable<CloudFile> FindCloudFilesInCloudFileDirectory(string fileDirectory, string searchString)
		{
			string regexString = searchString.Replace("*", @"[A-Za-z\d\[\]\{\},%=_()!@^&$~`+-]{0,}").Replace(".", "\\.");

			IEnumerable<CloudFile> iListFileItems = GetFilesInCloudFileDirectory(fileDirectory).Where(t => Regex.IsMatch(t.Name, regexString, RegexOptions.IgnoreCase));

			return iListFileItems;
		}


		public IEnumerable<CloudFile> GetFilesInCloudFileDirectory(string fileDirectory)
		{
			List<CloudFile> iListFileItems = new List<CloudFile>();           // List<IListFileItem> iListFileItems = new List<IListFileItem>();

			fileDirectory = CleanRelativeCloudDirectoryName(fileDirectory);

			CloudStorageAccount cloudStorageAccount = GetCloudStorageAccount();
			CloudFileClient cloudFileClient = GetCloudFileClient(cloudStorageAccount);
			CloudFileShare cloudFileShare = GetCloudFileShareReference(cloudFileClient);
			CloudFileDirectory cloudFileDirectory = GetCloudFileDirectory(cloudFileShare, fileDirectory);
			if (cloudFileDirectory != null)
			{
				iListFileItems = cloudFileDirectory.ListFilesAndDirectories().OfType<CloudFile>().ToList();
			}
			return iListFileItems;
		}

		/// <summary>
		/// GetCloudFile - added 20180110 to return an actual cloud file ref to gain access to properties etc
		/// </summary>
		/// <param name="fileDirectory"></param>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public CloudFile GetCloudFile(string fileDirectory, string fileName)
		{
			fileDirectory = CleanRelativeCloudDirectoryName(fileDirectory);

			CloudStorageAccount cloudStorageAccount = GetCloudStorageAccount();
			CloudFileClient cloudFileClient = GetCloudFileClient(cloudStorageAccount);
			CloudFileShare cloudFileShare = GetCloudFileShareReference(cloudFileClient);
			CloudFileDirectory cloudFileDirectory = GetCloudFileDirectory(cloudFileShare, fileDirectory);
			if (cloudFileDirectory != null)
				return GetCloudFile(cloudFileDirectory, fileName);
			return null;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/// Protected members
		/// 
		protected CloudStorageAccount GetCloudStorageAccount()
		{
			try
			{
				if (string.IsNullOrEmpty(azureFileShareConnectionString))
					throw new Exception($"Invalid Azure Connection String! [refSD00:{azureFileShareConnectionString}]");

				return CloudStorageAccount.Parse(azureFileShareConnectionString);
			}
			catch (Exception oExeption)
			{
				oExeption.Log("GetCloudStorageAccount");
				return null;
			}
		}
		protected CloudFileClient GetCloudFileClient(CloudStorageAccount cloudStorageAccount)
		{
			try
			{
				return cloudStorageAccount.CreateCloudFileClient();
			}
			catch (Exception oExeption)
			{
				oExeption.Log($"GetCloudFileClient [refSD64] - {cloudStorageAccount.ToString()}");
				return null;
			}
		}
		protected CloudFileShare GetCloudFileShareReference(CloudFileClient cloudFileClient)
		{
			try
			{
				if (string.IsNullOrEmpty(azureFileShareName))
					throw new Exception($"Invalid Azure File Share Name! [refSD01:{cloudFileClient.StorageUri.PrimaryUri}/{azureFileShareName}]");

				CloudFileShare cloudFileShare = cloudFileClient.GetShareReference(azureFileShareName);

				if (!cloudFileShare.Exists())
					throw new Exception($"Azure File Share Does Not Exist! [refSD02:{cloudFileClient.StorageUri.PrimaryUri}]");

				return cloudFileShare;
			}
			catch (Exception oExeption)
			{
				oExeption.Log("GetCloudFileShare [refSD03]");
				return null;
			}
		}


		protected CloudFileDirectory GetCloudFileDirectory(CloudFileShare cloudFileShare, string fileDirectory)
		{
			if (cloudFileShare == null)
			{
				throw new ArgumentNullException($"Azure File Share not detected! [refDF16:{fileDirectory}]");
			}
			try
			{

				// Get a reference to the root directory for the share
				CloudFileDirectory cloudRootShareDirectory = cloudFileShare.GetRootDirectoryReference();

				fileDirectory = this.CleanRelativeCloudDirectoryName(fileDirectory);

				// Get a reference to the image directory
				CloudFileDirectory cloudShareDirectory = cloudRootShareDirectory.GetDirectoryReference(fileDirectory);

				//// 20180107 - dont want to return null, we may be looking for a directory that doesn't exist yet in order to create it
				//if (!cloudShareDirectory.Exists())
				//	throw new Exception($"Azure File Share Directory Does Not Exist! [refSD08:{cloudFileShare.Name}/{fileDirectory}]");

				return cloudShareDirectory;
			}
			catch (Exception oExeption)
			{
				oExeption.Log($"GetCloudFileDirectory [refSD12] - {cloudFileShare.Name} /{fileDirectory}");
				return null;
			}
		}


		protected CloudFile GetCloudFile(CloudFileDirectory cloudFileDirectory, string fileName)
		{
			if (cloudFileDirectory == null)
			{
				throw new ArgumentNullException($"Azure File Share Directory not detected! [refDF16:{fileName}]");
			}
			try
			{
				// get a cloud file reference to the image
				CloudFile cloudFile = cloudFileDirectory.GetFileReference(fileName);

				if (!cloudFile.Exists())
				{
					throw new FileNotFoundException(cloudFileDirectory.Name, fileName);
				}

				return cloudFile;
			}
			catch (Exception oExeption)
			{
				oExeption.Log($"GetCloudFileShareDirectory [refSD48]- {cloudFileDirectory.Name}/{fileName}");
				return null;
			}
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/// Protected Helpers
		/// 
		protected string CleanRelativeCloudDirectoryName(string directoryName)
		{
			return directoryName.CleanRelativeCloudDirectoryName();
		}

		/// <summary>
		/// Gets a file reference from the azure cloud file storage if it exists
		/// </summary>
		protected CloudFile GetCloudFileInfo(string fileDirectory, string fileName)
		{
			CloudFile cloudFile = null;
			try
			{
				fileDirectory = CleanRelativeCloudDirectoryName(fileDirectory);

				CloudStorageAccount cloudStorageAccount = GetCloudStorageAccount();
				CloudFileClient fileClient = GetCloudFileClient(cloudStorageAccount);
				CloudFileShare share = GetCloudFileShareReference(fileClient);
				CloudFileDirectory cloudFileDirectory = GetCloudFileDirectory(share, fileDirectory);
				if ((cloudFileDirectory != null) && cloudFileDirectory.Exists())
				{
					cloudFile = cloudFileDirectory.GetFileReference(fileName);
					cloudFile.FetchAttributes();
				}
			}
			catch (Exception oExeption)
			{
				oExeption.Log($"GetCloudFileInfo [{fileDirectory}\\{fileName}]");
			}
			return cloudFile;
		}

		/// <summary>
		/// GetCloudFileStream - ORIG
		/// </summary>
		/// <param name="imageFileDirectory"></param>
		/// <param name="imageFileName"></param>
		/// <returns></returns>
		protected MemoryStream GetCloudFileStreamOrig(string imageFileDirectory, string imageFileName)
		{
			MemoryStream memstream = new MemoryStream();

			try
			{
				CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(azureFileShareConnectionString);

				CloudFileClient fileClient = cloudStorageAccount.CreateCloudFileClient();
				CloudFileShare share = fileClient.GetShareReference(azureFileShareName);
				if (!share.Exists())
				{
					throw new ShareNotFoundException(azureFileShareName);
				}
				// Get a reference to the root directory for the share
				CloudFileDirectory rootDir = share.GetRootDirectoryReference();

				// Get a reference to the image directory
				CloudFileDirectory shareDir = rootDir.GetDirectoryReference(imageFileDirectory);

				if (!shareDir.Exists())
				{
					throw new FolderNotFoundException(imageFileDirectory);
				}
					// get a cloud file reference to the image
				CloudFile file = shareDir.GetFileReference(imageFileName);
				if (!file.Exists())
                {
					throw new FileNotFoundException(imageFileDirectory, imageFileName);
				}

				file.DownloadToStream(memstream);
			}
			catch (Exception oExeption)
			{
				oExeption.Log($"GetCloudFileStreamOrig - {imageFileDirectory}\\{imageFileName}]");
			}
			return memstream;
		}
	}

	public static class AzureCloudIOExtensions
	{
		public static string CleanRelativeCloudDirectoryName(this string directoryName)
		{
			// remove relative marker, if necessary
			directoryName = directoryName.StartsWith("~") ? directoryName.Substring(1) : directoryName;
			// remove any starting dir chars
			do {
				directoryName = directoryName.StartsWith("\\") ? directoryName.Substring(1) : directoryName;
			} while (directoryName.StartsWith("\\"));
			// remove any trailing dir chars
			do {
				directoryName = directoryName.EndsWith("\\") ? directoryName.Substring(0, directoryName.Length - 1) : directoryName;
			} while (directoryName.EndsWith("\\"));
			// replace backslashes with forward slashes
			directoryName = directoryName.Replace("\\", "/").Replace("//", "/").Replace("~", "");

			directoryName = directoryName.StartsWith("/") ? directoryName.Substring(1) : directoryName;

			return directoryName;
		}
	}

}
