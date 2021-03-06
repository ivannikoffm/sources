using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using Windows.UI.Core;
using Windows.Web.Http;

namespace DatingBoard.Resourses
{
    // Абстрактное определение интерфейса и класса IncrementalLoadingCollection
    public interface IIncrementalSource<T>
    {
        Task<IEnumerable<T>> GetPagedItems(int pageIndex, int pageSize);
    }

    public class IncrementalLoadingCollection<T, I> : ObservableCollection<I>,
         ISupportIncrementalLoading
         where T : IIncrementalSource<I>, new()
    {
        private T source;
        private int itemsPerPage;
        private bool hasMoreItems;
        private int currentPage;

        public IncrementalLoadingCollection(int itemsPerPage = 20)
        {
            this.source = new T();
            this.itemsPerPage = itemsPerPage;
            this.hasMoreItems = true;
        }

        public bool HasMoreItems
        {
            get { return hasMoreItems; }
        }

        // Метод асинхронно добавляет новые элементы в коллекцию при скролле до тех пор, пока они не закончатся
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            var dispatcher = Window.Current.Dispatcher;

            return Task.Run<LoadMoreItemsResult>(
                async () =>
                {
                    uint resultCount = 0;
                    var result = await source.GetPagedItems(currentPage++, itemsPerPage);

                    if (result == null || result.Count() == 0)
                    {
                        hasMoreItems = false;
                    }
                    else
                    {
                        resultCount = (uint)result.Count();

                        await dispatcher.RunAsync(
                            CoreDispatcherPriority.Normal,
                            () =>
                            {
                                foreach (I item in result)
                                    this.Add(item);
                            });
                    }

                    return new LoadMoreItemsResult() { Count = resultCount };

                }).AsAsyncOperation<LoadMoreItemsResult>();
        }
    }


    public class PersonGroup
    {
        public string GroupName { get; set; }     
        public ObservableCollection<Person> Persons { get; set; }

        public PersonGroup() 
        { 
            Persons = new ObservableCollection<Person>(); 
        }
    }


    public class Person
    {
        public string ID { get; set; }
        public string ShowingID { get; set; }
        public string Sex { get; set; }
        public string Nic { get; set; }
        public string Type { get; set; }
        public string Age { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string Messenger { get; set; }
        public string Title { get; set; }
        public string About { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string Marital { get; set; }
        public string Children { get; set; }
        public string Smoke { get; set; }
        public string Pubdate { get; set; }
        public string Photo1 { get; set; }
        public string Photo2 { get; set; }
        public string Photo3 { get; set; }
        public string IsPremium { get; set; }

        private string _template;
        public string Template
        {
            get { return _template; }
            set { _template = value; }
        }

        private string _background;
        public string Background
        {
            get { return _background; }
            set
            {
                if (_template == "premium")
                    _background = "Gold";
            }
        }

        public Person()
        {
            _template = "default";
            if (_template == "premium")
                _background = "Gold";
        }

        private int _horizontalSize = 1; 
        public int HorizontalSize { get { return _horizontalSize; } set { _horizontalSize = value; } }

        private int _verticalSize = 1; 
        public int VerticalSize { get { return _verticalSize; } set { _verticalSize = value; } }

        public Person CreatePerson(string person)
        {
            person = MysqlWrapper.PreviewData();

            string[] per = person.Split('&');
            Person newPerson = new Person();

            for (int i = 0; i < per.Length; i++)
            {
                string[] temp = per[i].Split('=');

                switch (temp[0])
                {
                    case "sex": newPerson.Sex = temp[1]; break;
                    case "nic": newPerson.Nic = temp[1]; break;
                    case "age": newPerson.Age = string.Format("{0} {1}", Localization.GetLocalString("PersAge"), temp[1]); break;
                    case "country": newPerson.Country = string.Format("{0} {1}", Localization.GetLocalString("From"), temp[1]); break;
                    case "region": newPerson.Region = string.Format("{0} {1}", Localization.GetLocalString("PersMes"), temp[1]); break;
                    case "city": newPerson.City = temp[1]; break;
                    case "email": newPerson.Email = temp[1]; break;
                    case "messenger": newPerson.Messenger = temp[1]; break;
                    case "about": newPerson.About = temp[1]; break;
                    case "title": newPerson.Title = temp[1]; break;
                    case "typerel": newPerson.Type = temp[1]; break;
                    case "marital": newPerson.Marital = string.Format("{0} {1}", Localization.GetLocalString("PersMarital"), temp[1]); break;
                    case "children": newPerson.Children = string.Format("{0} {1}", Localization.GetLocalString("PersChildren"), temp[1]); break;
                    case "height": newPerson.Height = string.Format("{0} {1}", Localization.GetLocalString("PersHeight"), temp[1]); break;
                    case "weight": newPerson.Weight = string.Format("{0} {1}", Localization.GetLocalString("PersWeight"), temp[1]); break;
                    case "smoke": newPerson.Smoke = string.Format("{0} {1}", Localization.GetLocalString("PersSmoke"), temp[1]); break;
                    case "photo1": newPerson.Photo1 = temp[1]; break;
                    case "photo2": newPerson.Photo2 = temp[1]; break;
                    case "photo3": newPerson.Photo3 = temp[1]; break;
                }
            }

            if (newPerson.City == "0")
                newPerson.City = "";

            newPerson = MysqlWrapper.DeCodePersonData(newPerson);

            return newPerson;
        }

    }




    //Кастомизация PersonGroupe с использованием IncrementalLoading под юзеров сгруппированных рядом
    public class PersonGroupeNear : IIncrementalSource<Person>
    {
        public string GroupName { get; set; }
        public ObservableCollection<Person> Persons { get; set; }

        public PersonGroupeNear()
        {
            Persons = new ObservableCollection<Person>();
            Persons = MysqlWrapper.DataQuerySync(Services.GetData("", "near", "2000"));
            Persons[0].VerticalSize = 1; Persons[0].HorizontalSize = 1;
        }



        public async Task<IEnumerable<Person>> GetPagedItems(int pageIndex, int pageSize)
        {
            return await Task.Run<IEnumerable<Person>>(() =>
            {
                var result = (from p in Persons
                              select p).Skip(pageIndex * pageSize).Take(pageSize);

                return result;
            });
        }
    }
}
