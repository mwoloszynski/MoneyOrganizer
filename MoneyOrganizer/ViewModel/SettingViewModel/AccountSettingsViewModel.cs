using MoneyOrganizer.Model;
using MoneyOrganizer.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MoneyOrganizer.ViewModel.SettingViewModel
{
    public class AccountSettingsViewModel : ViewModelBase
    {
        #region Fields

        private MainViewModel _mvm;
        private string _firstName;
        private string _lastName;
        private ICommand _saveChangesCommand;

        #endregion

        #region Binds

        public ICommand SaveChangesCommand { get { return _saveChangesCommand; } }
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; OnPropertyChanged("FirstName"); }
        }
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; OnPropertyChanged("LastName"); }
        }

        #endregion

        #region Constructor

        public AccountSettingsViewModel(MainViewModel mvm)
        {
            _mvm = mvm;
            FirstName = _mvm.AppSettings.userInfo.FirstName;
            LastName = _mvm.AppSettings.userInfo.LastName;
            _saveChangesCommand = new RelayCommand(_saveChangesCommand => SaveChanges());
        }

        #endregion

        #region Functions

        private void SaveChanges()
        {
            if(FirstName != _mvm.AppSettings.userInfo.FirstName || LastName != _mvm.AppSettings.userInfo.LastName)
            {
                string tableName = "UserInfo";
                string[] columns = { "FirstName", "LastName" };
                string[] values = { FirstName, LastName };
                string[] colCond = { "Id" };
                string[] valCond = { _mvm.AppSettings.userInfo.Id.ToString() };
                string[] eqCond = { "=" };
                string[] andor = { "" };

                _mvm.DBC.UpdateTable(tableName, columns, values, colCond, valCond, eqCond, andor);
                _mvm.AppSettings.userInfo.FirstName = FirstName;
                _mvm.AppSettings.userInfo.LastName = LastName;

                MessageBox.Show("Zapisano zmiany");
            }   
        }

        #endregion


    }
}
