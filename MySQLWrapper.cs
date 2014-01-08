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
        CloudBlobContainer container = blobCl.GetContainerReference("MyContainer");
        await container.CreateIfNotExistsAsync();

        ICloudBlob bl = container.GetBlockBlobReference(filename);
        await bl.UploadFromStreamAsync(stream);

        return true;
    }
    catch (Exception ex) { Services.SendDialog(ex.Message); return false; }
}

    /// <summary>
    /// Allows to make a query to DataBase for get a User Data
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>new object of Persons class</returns>
public static async Task<Person> GetSinglePerson(string id)
        {
            HttpClient client = new HttpClient();
            StringContent query = new StringContent(string.Format("query=single&id={0}", id), System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");

            var resp = await client.PostAsync(new Uri("http://xxxxxxxxxx.com/wrapper/queryDB.php"), query);
            string cont = await resp.Content.ReadAsStringAsync();

            Dictionary<string, string> persDic = new Dictionary<string, string>();
            string[] temp = cont.Split(new string[] { "<s>" }, StringSplitOptions.None);

            for (int k = 0; k < temp.Length; k++)
            {
                string[] keyValue = temp[k].Split(new string[] { "=" }, StringSplitOptions.None);
                persDic.Add(keyValue[0], keyValue[1]);
            }

            Person person = new Person
            {
                ID = persDic["ID"],
                ShowingID = string.Format("ID: {0}", persDic["ID"]),
                Sex = persDic["Sex"],
                Nic = persDic["Nic"],
                Type = persDic["Type"],
                Age = string.Format("{0} {1}", Localization.GetLocalString("PersAge"), persDic["Age"]),
                Country = string.Format("{0} {1}", Localization.GetLocalString("From"), persDic["Country"]),
                City = persDic["City"],
                Email = persDic["Email"],
                Messenger = string.Format("{0} {1}", Localization.GetLocalString("PersMes"), persDic["Messenger"]),
                Title = persDic["Title"],
                About = persDic["About"],
                Height = string.Format("{0} {1}", Localization.GetLocalString("PersHeight"), persDic["Height"]),
                Weight = string.Format("{0} {1}", Localization.GetLocalString("PersWeight"), persDic["Weight"]),
                Marital = string.Format("{0} {1}", Localization.GetLocalString("PersMarital"), persDic["Marital"]),
                Children = string.Format("{0} {1}", Localization.GetLocalString("PersChildren"), persDic["Children"]),
                Smoke = string.Format("{0} {1}", Localization.GetLocalString("PersSmoke"), persDic["Smoke"]),
                Pubdate = string.Format("{0} {1}", Localization.GetLocalString("PersPubdate"), persDic["Pubdate"]),
                Photo1 = persDic["Photo1"],
                Photo2 = persDic["Photo2"],
                Photo3 = persDic["Photo3"],
                IsPremium = persDic["IsPremium"],
                Template = "default"
            };
            return person;
        }
        
        
        /// <summary>
        /// Allows to get an ObservableCollection of users
        /// </summary>
        /// <param name="data">DataQuery of needed users</param>
        /// <returns>ObservableCollection of users</returns>
public static async Task<ObservableCollection<Person>> DataQueryAsync(string data)
        {
            HttpClient client = new HttpClient();
            StringContent query = new StringContent(data, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");

            var resp = await client.PostAsync(new Uri("http://datingpoint2.com/wrapper/queryDB.php"), query);
            string cont = await resp.Content.ReadAsStringAsync();


            List<string> rawPersons = new List<string>();
            if (cont.Contains("Connected successfully<r>"))
            {
                cont = cont.Replace("Connected successfully<r>", "");
                rawPersons = cont.Split(new string[] { "<r>" }, StringSplitOptions.None).ToList<string>();
            }

            ObservableCollection<Person> persons = new ObservableCollection<Person>();
            for (int i = 0; i < rawPersons.Count - 1; i++)
            {
                Dictionary<string, string> persDic = new Dictionary<string, string>();
                string[] temp = rawPersons[i].Split(new string[] { "<s>" }, StringSplitOptions.None);

                for (int k = 0; k < temp.Length; k++)
                {
                    string[] keyValue = temp[k].Split(new string[] { "=" }, StringSplitOptions.None);
                    persDic.Add(keyValue[0], keyValue[1]);

                }
                persons.Add(new Person
                {
                    ID = persDic["ID"],
                    Sex = persDic["Sex"],
                    Nic = persDic["Nic"],
                    Type = persDic["Type"],
                    Age = persDic["Age"],
                    Country = persDic["Country"],
                    City = persDic["City"],
                    Email = persDic["Email"],
                    Messenger = persDic["Messenger"],
                    Title = persDic["Title"],
                    About = persDic["About"],
                    Height = persDic["Height"],
                    Weight = persDic["Weight"],
                    Marital = persDic["Marital"],
                    Children = persDic["Children"],
                    Smoke = persDic["Smoke"],
                    Pubdate = persDic["Pubdate"],
                    Photo1 = persDic["Photo1"],
                    Photo2 = persDic["Photo2"],
                    Photo3 = persDic["Photo3"],
                    IsPremium = persDic["IsPremium"],
                    Template = "premium",
                    Background = "Gold",
                    HorizontalSize = 2,
                    VerticalSize = 2
                });
            }
            return persons;
        }
