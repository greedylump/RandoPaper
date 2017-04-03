using RandoPaper.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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
        private PaperStyle wpStyle;
        private TimeSpan span;
        private DispatcherTimer respawnTimer;
        private SearchResult defaultResult = new SearchResult
        {
            Name = "Error Default",
            ContentUrl = "https://www.smashingmagazine.com/images/404-error-pages/gog.jpg",
            ThumbnailUrl = "https://www.smashingmagazine.com/images/404-error-pages/gog.jpg"
        };






        public ICommand SetRandomWPCommand { get; set; }
        public ICommand SetNextWPCommand { get; set; }
        public ICommand SkipThisWPCommand { get; set; }


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
                if(Int32.Parse(value)<1 || value =="" || value ==null)
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
                if(value<1 )
                {
                    respawn = 1;
                    span = new TimeSpan(0, respawn,0);
                    RaisePropertyChanged("Respawn");
                }
                else
                {
                    respawn = value;
                    span = new TimeSpan(0, respawn, 0);
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
        public PaperStyle WPStyle
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

        public IList<PaperStyle> StyleTypes
        {
            get
            {
                return Enum.GetValues(typeof(PaperStyle)).Cast<PaperStyle>().ToList<PaperStyle>();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            LoadCommands();
            respawnTimer = new DispatcherTimer();
            respawnTimer.Interval = new TimeSpan(0, 10, 0);
            respawnTimer.Tick += dispatcherTimer_Tick;
        }

        
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            SetNextWallpaper();
        }
        

        private void LoadCommands()
        {
            SetRandomWPCommand = new CustomCommand(SetRandomWallpaper, CanGet);
            SetNextWPCommand = new CustomCommand(SetNextWallpaperCom, CanSet);
            SkipThisWPCommand = new CustomCommand(SkipThisWP, CanSet);

        }

        private void SkipThisWP(object obj)
        {
            GetNextResult();
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
            if(respawnTimer.IsEnabled)
            {
                respawnTimer.Stop();
            }
            respawnTimer.Interval = span;
            respawnTimer.Start();
            if (nextResult == null)
            {
                GetNextResult();
                SetAndGetNext();

            }
            else
            {
                SetAndGetNext();
            }
        }

        private void SetAndGetNext()
        {
            Uri randomUri = new Uri(NextResult.ContentUrl);
            try
            {
                Wallpaper.Set(randomUri, WPStyle);
            }
            catch(WebException we)
            {
                GetNextResult();
                randomUri = new Uri(NextResult.ContentUrl);
                Wallpaper.Set(randomUri, WPStyle);
            }
           
            GetNextResult();
           
        }

        private void GetNextResult()
        {
            Random rando = new Random(DateTime.Now.Second);
            NextResult = searchResults[rando.Next(searchResults.Count)];
           




        }

        private async Task PopulateResultList()
        {
            if(Query=="" || Query==null)
            {
                Query = "OctoCat";
            }
            string result;
            try
            {
                result = await bSearch.MakeRequest();
                jStrToClass.ParseToList(result);
                searchResults = jStrToClass.TokenToResult();
            }
            catch(WebException e)
            {
                NextResult = defaultResult;
                searchResults = new List<SearchResult>();
                searchResults.Add(NextResult);
                /*Trying a different approach for now
                MessageBoxResult message = MessageBox.Show(e.Message, "Shut Down?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (message == MessageBoxResult.Yes)
                {
                    Application.Current.Shutdown();
                }
                */
            }

            
            
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }

    }
}
