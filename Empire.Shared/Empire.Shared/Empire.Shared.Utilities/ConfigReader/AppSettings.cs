using System;

namespace Empire.Shared.Utilities
{
    public class AppSettings : Lazy<AppSettings>
    {
        #region Fields
        protected static readonly Lazy<AppSettings> s_oAppSettings = new Lazy<AppSettings>(() => new AppSettings());
        #endregion

        #region properties
        public string AzureAccountKey
        {
            get;
            protected set;
        }
        public string ShareName
        {
            get;
            protected set;
        }
        public string PartsImages
        {
            get;
            protected set;
        }
        public string UploadImages
        {
            get;
            protected set;
        }
        public string Thumbnails
        {
            get;
            protected set;
        }
        public string SignatureImages
        {
            get;
            protected set;
        }

        public string ApplicationName
        {
            get;
            protected set;
        }
        public string VirtualFolder
        {
            get;
            protected set;
        }

        public string MapUrl
        {
            get;
            protected set;
        }

        public string PostmarkApiToken
        {
            get;
            protected set;
        }

        public string PostmarkBaseUrl
        {
            get;
            protected set;
        }

        public static AppSettings Current
        {
            get
            {
                return s_oAppSettings.Value;
            }
        }

        public MailSettings Mail
        {
            get;
            protected set;
        }
        #endregion

        #region Constructors
        public AppSettings()
        {

            this.ApplicationName = ConfigReader.Default.Read("ApplicationName", "Empire Auto Parts");
            this.AzureAccountKey = ConfigReader.Default.Read("Azure_AccountKey", "v1eJbZDvBQAETzbttxEHlws7W2HK84oxSdpzRlzYysHb2P23hqzmfqWuP1Oz5I3xofJydUBKcxjzFjBcuZEfvA==");
            this.ShareName = ConfigReader.Default.Read("ShareName", "apps");
            this.PartsImages = ConfigReader.Default.Read("PartsImages", "PartsImages");
            this.UploadImages = ConfigReader.Default.Read("UploadImages", "Uploaded_Images");
            this.Thumbnails = ConfigReader.Default.Read("Thumbnails", "Thumbnails");
            this.SignatureImages = ConfigReader.Default.Read("SignatureImages", "Signature_Images");
            this.VirtualFolder = ConfigReader.Default.Read("VirtualFolder", String.Empty);

            this.MapUrl = ConfigReader.Default.Read("GoogleApiUrl", "https://www.google.com/maps/search/?api=1");

            //this.PostmarkApiToken = ConfigReader.Default.Read("PostmarkApiToken", "13cfdf59-d020-4366-9b84-8b05d2147ae6");
            this.PostmarkBaseUrl = ConfigReader.Default.Read("PostmarkBaseUrl", "https://api.postmarkapp.com");

            this.Mail = new MailSettings();
        }
        #endregion

        public class MailSettings
        {

            #region Properties

            public string FromAddress
            {
                get;
                protected set;
            }

            public string ToAddress
            {
                get;
                protected set;
            }

            public string DisplayName
            {
                get;
                protected set;
            }


            public string Subject
            {
                get;
                protected set;
            }
            #endregion

            #region Constructor 
            public MailSettings()
            {
                this.FromAddress = ConfigReader.Default.Read("EmailFromAddress", "custserv_online@empireauto.biz");
                this.ToAddress = ConfigReader.Default.Read("EmailToAddress", "recipient@poyholdings.com");
                this.DisplayName = ConfigReader.Default.Read("EmailFromDisplayName", "Empire Customer Service");
                this.Subject = ConfigReader.Default.Read("EmailSubject", "Application Error");

            }
            #endregion 

        }
    }
}
