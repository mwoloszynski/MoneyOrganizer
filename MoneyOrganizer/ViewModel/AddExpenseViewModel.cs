using MoneyOrganizer.Model;
using MoneyOrganizer.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MoneyOrganizer.ViewModel
{
    public class AddExpenseViewModel : ViewModelBase, ICloseWindows
    {

        #region Fields

        private MainViewModel _mvm;
        private ICommand _closeCommand;
        private ICommand _saveCommand;
        private List<Categories> _categories;
        private Categories _selectedCategory;
        private string _description;
        private string _price;
        private DateTime _data;

        #endregion

        #region Binds

        public ICommand CloseCommand { get { return _closeCommand; } }
        public ICommand SaveCommand { get { return _saveCommand; } }
        public List<Categories> Category
        {
            get
            {
                return _categories;
            }
            set
            {
                _categories = value;
                OnPropertyChanged("Category");
            }
        }
        public Categories SelectedCategory
        {
            get
            {
                return _selectedCategory;
            }
            set
            {
                _selectedCategory = value;
                OnPropertyChanged("SelectedCategory");
            }
        }
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }
        public string Price
        {
            get
            {
                return _price;
            }
            set
            {
                _price = value;
                OnPropertyChanged("Price");
            }
        }
        public DateTime Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                OnPropertyChanged("Data");
            }
        }

        #endregion


        #region Constructor

        public AddExpenseViewModel(MainViewModel mvm)
        {
            _mvm = mvm;
            _closeCommand = new RelayCommand(_closeCommand => CloseWindow());
            _saveCommand = new RelayCommand(_saveCommand => Save());
            _categories = _mvm.AppSettings.categories;
            _selectedCategory = _categories.Where(x => x.Id == 17).FirstOrDefault();
            _data = DateTime.Today;
        }

        #endregion

        #region Functions

        private void Save()
        {
            if (!string.IsNullOrEmpty(Price))
            {
                if (string.IsNullOrEmpty(Description))
                    Description = "Inne";
                float amountTemp = float.Parse(Price, System.Globalization.CultureInfo.InvariantCulture);
                float Amount = (float)Math.Ceiling(amountTemp * 100f) / 100f;

                if (_mvm.AppSettings.userInfo.Balance - Amount < 0)
                {
                    MessageBox.Show("Wydatek przekracza budżet");
                    // Obsługa błędów - przekroczono portfel
                }
                else
                {
                    // SelectedCategory
                    // Description
                    // Amount
                    // Data
                    string[] newExpensesCol = { "CategoryId", "Name", "Cost", "Date" };
                    string[] newExpensesValues = { SelectedCategory.Id.ToString(), Description, Amount.ToString(), Data.ToString("yyyy-MM-dd") };

                    string tableName = "UserInfo";
                    string[] columns = { "Balance" };
                    string[] values = { ((float)Math.Ceiling((_mvm.AppSettings.userInfo.Balance - Amount)*100f)/100f).ToString() };
                    string[] colCond = { "Id" };
                    string[] valCond = { _mvm.AppSettings.userInfo.Id.ToString() };
                    string[] eqCond = { "=" };
                    string[] andor = { "" };

                    _mvm.DBC.InsertIntoTable("Expenses", newExpensesCol, newExpensesValues);
                    _mvm.DBC.UpdateTable(tableName, columns, values, colCond, valCond, eqCond, andor);
                    float newAmount = (float)Math.Ceiling((_mvm.AppSettings.userInfo.Balance - Amount) * 100f) / 100f;
                    _mvm.AppSettings.userInfo.Balance = newAmount;

                    var id = _mvm.DBC.SelectSingleColumnOneResult("Expenses", "ORDER BY Id DESC LIMIT 1", "Id");

                    _mvm.AppSettings.expenses.Add(new Model.DBModel.Expenses
                    {
                        Id = Int32.Parse(id),
                        CategoryId = SelectedCategory.Id,
                        Name = Description,
                        Cost = Amount,
                        Date = Data.ToString("yyyy-MM-dd")
                    });

                    MessageBox.Show("Dodano wydatek");

                }
                CloseWindow();
           
            }
            else
            {
                // Obsługa błędów - brak ceny
            }
        }

        private void CloseWindow()
        {
            Close?.Invoke();
        }

        public Action Close { get; set; }

        public bool CanClose()
        {
            return true;
        }

        #endregion

    }
}
