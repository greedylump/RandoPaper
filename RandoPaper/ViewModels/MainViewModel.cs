using RandoPaper.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WallPaperModel;
using static WallPaperModel.Wallpaper;

namespace RandoPaper.ViewModels
{
    public class MainViewModel:INotifyPropertyChanged
    {
        private BingSearch bSearch = new BingSearch();
        private JsonStrToClass jStrToClass = new JsonStrToClass();
        private IList<SearchResult> searchResults;
        private int respawn;
        private Style wpStyle;


        public ICommand SetRandomWPCommand { get; set; }


        /// <summary>
        /// Number of desired search results
        /// </summary>
        public string Count
        {
            get
            {
                return bSearch.Count;
            }

            set
            {
                bSearch.Count = value;
                RaisePropertyChanged("Count");
            }
        }
        /// <summary>
        /// Time till next random wallpaper
        /// To be implemented last
        /// </summary>
        public int Respawn
        {
            get
            {
                return respawn;
            }

            set
            {
                respawn = value;
                RaisePropertyChanged("Respawn");
            }
        }
        /// <summary>
        /// Search string query.
        /// </summary>
        public string Query
        {
            get
            {
                return bSearch.Query;
            }

            set
            {
                bSearch.Query = value;
                RaisePropertyChanged("Query");
            }
        }

        /// <summary>
        /// Style the wallpaper is layed out. Tiled,
        ///    Centered,
        ///    Stretched,
        ///    Fill,
        ///    Fit,
        ///    Span
        /// </summary>
        public Style WPStyle
        {
            get
            {
                return wpStyle;
            }
            set
            {
                wpStyle = value;
                RaisePropertyChanged("WPStyle");
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            LoadCommands();
        }

        private void LoadCommands()
        {
            SetRandomWPCommand = new CustomCommand(SetWallpaper, CanSet);

        }

        private bool CanSet(object obj)
        {
            throw new NotImplementedException();
        }

        private void SetWallpaper(object obj)
        {
            Random rando = new Random(DateTime.Now.Second);
            Uri randPic = new Uri(searchResults[rando.Next(searchResults.Count)].ContentUrl);
            Wallpaper.Set(randPic, WPStyle);
        }

        public async void PopulateResultList()
        {
           
            var result = await bSearch.MakeRequest();

            jStrToClass.ParseToList(result);

            searchResults = jStrToClass.TokenToResult();
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }

    }
}
