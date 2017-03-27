using RandoPaper.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
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
        //public Timer respawnTimer;
        //private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        



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
                if(Int32.Parse(value)<1)
                {
                    bSearch.Count = "1";
                    RaisePropertyChanged("Count");
                }
                else
                {
                    bSearch.Count = value;
                    RaisePropertyChanged("Count");
                }
               
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
                if(value<1)
                {
                    respawn = 1;
                    RaisePropertyChanged("Respawn");
                }
                else
                {
                    respawn = value;
                    RaisePropertyChanged("Respawn");
                }
               
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
        /// Style the style wallpaper is layed out. Tiled,
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
                return Enum.GetValues(typeof(Style)).Cast<Style>().ToList<Style>();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            LoadCommands();
        }

        /* Dispatch Timer majorly screwed up performance
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            SetNextWallpaper();
        }
        */

        private void LoadCommands()
        {
            SetRandomWPCommand = new CustomCommand(SetRandomWallpaper, CanGet);
            SetNextWPCommand = new CustomCommand(SetNextWallpaperCom, CanSet);

        }

        private void SetNextWallpaperCom(object obj)
        {
            SetNextWallpaper();
        }

        private bool CanGet(object obj)
        {
                return true;
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
        
        private async void SetRandomWallpaper(object obj)
        {
            await PopulateResultList();
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

        private async Task PopulateResultList()
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
