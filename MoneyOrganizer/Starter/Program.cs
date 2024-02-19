using MoneyOrganizer.Model;
using MoneyOrganizer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyOrganizer.Starter
{
    public class Program
    {
        public static Loader _loader;
        public static Settings _settings;
        public static DBController _dbc;
        /// <summary>
        /// Main Starter and Controller
        /// </summary>
        public static void Start()
        {
            try
            {
                _loader = new Loader();
                _loader.Show();

                _dbc = new DBController();
                if(_dbc.LoadDB())
                {
                    _settings = new Settings();
                    _settings.LoadSettings(_dbc);


                    View.MainWindowView view = new View.MainWindowView()
                    {
                        DataContext = new MainViewModel(_settings, _dbc)
                    };
                    view.Show();
                    _loader.Close();
                }
                else
                {
                    // Obsługa błędów
                }
                GC.Collect();
            } catch(Exception ex)
            {
                // Obsługa błędów - do zrobienia
                try
                {
                    _loader.Close();
                    GC.Collect();
                }
                catch (Exception) { GC.Collect(); }
            }
        }

        /// <summary>
        /// Start Program
        /// </summary>
        public Program()
        {
            Start();
        }
    }
}
