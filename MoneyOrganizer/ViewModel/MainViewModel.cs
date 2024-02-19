using MoneyOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MoneyOrganizer.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        #region Fields

        private ViewModelBase _currentViewModel;
        private SettingsViewModel _settingsViewModel;
        private IncomesViewModel _incomesViewModel;
        private OutcomesViewModel _outcomesViewModel;
        private HomeViewModel _homeViewModel;
        private ICommand _navigateHomeCommand;
        private ICommand _navigateIncomesCommand;
        private ICommand _navigateOutcomesCommand;
        private ICommand _navigateSettingsCommand;
        private Settings _settings;
        private DBController _dbc;

        #endregion

        #region Binds

        public ICommand NavigateHomeCommand { get { return _navigateHomeCommand; } }
        public ICommand NavigateIncomesCommand { get { return _navigateIncomesCommand; } }
        public ICommand NavigateOutcomesCommand { get { return _navigateOutcomesCommand; } }
        public ICommand NavigateSettingsCommand { get { return _navigateSettingsCommand; } }
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel?.Dispose();
                _currentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }
        public Settings AppSettings
        {
            get
            {
                return _settings;
            }
            set
            {
                _settings = value;
                OnPropertyChanged("AppSettings");
            }
        }
        public DBController DBC
        {
            get
            {
                return _dbc;
            }
            set
            {
                _dbc = value;
                OnPropertyChanged("DBC");
            }
        }


        #endregion

        #region Constructor

        public MainViewModel(Model.Settings settings, Model.DBController dbc)
        {
            AppSettings = settings;
            DBC = dbc;
            _navigateHomeCommand = new RelayCommand(_navigateHomeCommand => NagivateHome());
            _navigateIncomesCommand = new RelayCommand(_navigateHomeCommand => NavigateIncomes());
            _navigateOutcomesCommand = new RelayCommand(_navigateHomeCommand => NavigateOutcomes());
            _navigateSettingsCommand = new RelayCommand(_navigateHomeCommand => NavigateSettings());
            _homeViewModel = new HomeViewModel(this);
            _incomesViewModel = new IncomesViewModel(this);
            _outcomesViewModel = new OutcomesViewModel(this);
            _settingsViewModel = new SettingsViewModel(this);

            CurrentViewModel = _homeViewModel;

        }

        #endregion

        #region Functions

        public void NagivateHome()
        {
            _homeViewModel.Balance = _settings.userInfo.Balance;
            _homeViewModel.UpdateTransactions();
            _homeViewModel.UpdateChart();
            CurrentViewModel = _homeViewModel;
        }

        public void NavigateIncomes()
        {
            _incomesViewModel.UpdateTransactions();
            CurrentViewModel = _incomesViewModel;
        }

        public void NavigateOutcomes()
        {
            _outcomesViewModel.UpdateTransactions();
            CurrentViewModel = _outcomesViewModel;
        }

        public void NavigateSettings()
        {
            CurrentViewModel = _settingsViewModel;
        }


        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        #endregion

    }
}
