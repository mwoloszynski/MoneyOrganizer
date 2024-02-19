using MoneyOrganizer.Model;
using MoneyOrganizer.Model.DBModel;
using MoneyOrganizer.ViewModel.SettingViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MoneyOrganizer.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {

        #region Fields

        private ViewModelBase _currentSettingsViewModel;
        private AccountSettingsViewModel _accountSettingsViewModel;
        private InterfaceSettingsViewModel _interfaceSettingsViewModel;
        private ICommand _navigateAccountCommand;
        private ICommand _navigateInterfaceCommand;

        #endregion

        #region Binds

        public ICommand NavigateAccountCommand { get { return _navigateAccountCommand; } }
        public ICommand NavigateInterfaceCommand { get { return _navigateInterfaceCommand; } }
        public ViewModelBase CurrentSettingsViewModel
        {
            get => _currentSettingsViewModel;
            set
            {
                _currentSettingsViewModel?.Dispose();
                _currentSettingsViewModel = value;
                OnCurrentViewModelChanged();
            }
        }
        
        #endregion

        #region Constructor

        public SettingsViewModel(MainViewModel mvm)
        {
            _navigateAccountCommand = new RelayCommand(_navigateAccountCommand => NavigateAccount());
            _navigateInterfaceCommand = new RelayCommand(_navigateAccountCommand => NavigateInterface());
            _accountSettingsViewModel = new AccountSettingsViewModel(mvm);
            _interfaceSettingsViewModel = new InterfaceSettingsViewModel(); 

            CurrentSettingsViewModel = _accountSettingsViewModel;
        }

        #endregion

        #region Functions

        public void NavigateAccount()
        {
            CurrentSettingsViewModel = _accountSettingsViewModel;
        }

        public void NavigateInterface()
        {
            CurrentSettingsViewModel = _interfaceSettingsViewModel;
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentSettingsViewModel));
        }


        #endregion
    }
}
