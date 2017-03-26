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
        private SearchResult nextResult;
        private int respawn;
        private Style wpStyle;


        public ICommand SetRandomWPCommand { get; set; }
        public ICommand SetNextWPCommand { get; set; }


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

        public SearchResult NextResult
        {
            get
            {
                return nextResult;
            }
            set
            {
                nextResult = value;
                RaisePropertyChanged("NextResult");
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

        public IList<Style> StyleTypes
        {
            get
            {
                // Will result in a list like {"Tester", "Engineer"}
                return Enum.GetValues(typeof(Style)).Cast<Style>().ToList<Style>();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            LoadCommands();
        }

        private void LoadCommands()
        {
            SetRandomWPCommand = new CustomCommand(SetRandomWallpaper, CanSet);
            SetNextWPCommand = new CustomCommand(SetNextWallpaperCom, CanSet);

        }

        private void SetNextWallpaperCom(object obj)
        {
            SetNextWallpaper();
        }

        private bool CanSet(object obj)
        {
            if (searchResults == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private void SetRandomWallpaper(object obj)
        {
            PopulateResultList();
            SetNextWallpaper();
        }

        private void SetNextWallpaper()
        {
            Uri randomUri;
            Random rando = new Random(DateTime.Now.Second);
            if (nextResult == null)
            {
                GetNextResult();
                randomUri = new Uri(NextResult.ContentUrl);
                Wallpaper.Set(randomUri, WPStyle);
                GetNextResult();
            }
            else
            {
                randomUri = new Uri(NextResult.ContentUrl);
                Wallpaper.Set(randomUri, WPStyle);
                GetNextResult();
            }
        }
       

        private void GetNextResult()
        {
            Random rando = new Random(DateTime.Now.Second);
            NextResult = searchResults[rando.Next(searchResults.Count)];
        }

        private async void PopulateResultList()
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
